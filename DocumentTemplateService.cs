using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Dynamics365TemplateCompareTransfer
{
    internal sealed class DocumentTemplateService
    {
        private static readonly string[] TemplateColumns =
        {
            "documenttemplateid",
            "name",
            "description",
            "associatedentitytypecode",
            "documenttype",
            "content",
            "clientdata",
            "languagecode",
            "createdon",
            "modifiedon",
            "status",
            "versionnumber"
        };

        public List<DocumentTemplateRecord> RetrieveTemplates(IOrganizationService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            var records = new List<DocumentTemplateRecord>();
            var query = new QueryExpression("documenttemplate")
            {
                ColumnSet = new ColumnSet(TemplateColumns),
                PageInfo = new PagingInfo
                {
                    Count = 5000,
                    PageNumber = 1
                }
            };

            query.Orders.Add(new OrderExpression("name", OrderType.Ascending));

            while (true)
            {
                EntityCollection page = service.RetrieveMultiple(query);

                foreach (Entity entity in page.Entities)
                {
                    records.Add(Map(entity));
                }

                if (!page.MoreRecords)
                {
                    break;
                }

                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = page.PagingCookie;
            }

            return records;
        }

        public List<TemplateComparisonRow> Compare(
            IEnumerable<DocumentTemplateRecord> sourceTemplates,
            IEnumerable<DocumentTemplateRecord> targetTemplates)
        {
            var sourceGroups = (sourceTemplates ?? Enumerable.Empty<DocumentTemplateRecord>())
                .GroupBy(template => template.Key)
                .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.OrdinalIgnoreCase);

            var targetGroups = (targetTemplates ?? Enumerable.Empty<DocumentTemplateRecord>())
                .GroupBy(template => template.Key)
                .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.OrdinalIgnoreCase);

            var keys = new HashSet<string>(sourceGroups.Keys, StringComparer.OrdinalIgnoreCase);
            keys.UnionWith(targetGroups.Keys);

            var rows = new List<TemplateComparisonRow>();

            foreach (string key in keys)
            {
                List<DocumentTemplateRecord> sourceMatches;
                List<DocumentTemplateRecord> targetMatches;
                sourceGroups.TryGetValue(key, out sourceMatches);
                targetGroups.TryGetValue(key, out targetMatches);

                sourceMatches = sourceMatches ?? new List<DocumentTemplateRecord>();
                targetMatches = targetMatches ?? new List<DocumentTemplateRecord>();

                DocumentTemplateRecord source = sourceMatches.FirstOrDefault();
                DocumentTemplateRecord target = targetMatches.FirstOrDefault();
                TemplateComparisonStatus status;
                string notes;

                if (sourceMatches.Count > 1 || targetMatches.Count > 1)
                {
                    status = TemplateComparisonStatus.Duplicate;
                    notes = string.Format(
                        "Ambiguous key: {0} source record(s), {1} target record(s). Transfer is blocked.",
                        sourceMatches.Count,
                        targetMatches.Count);
                }
                else if (source == null)
                {
                    status = TemplateComparisonStatus.TargetOnly;
                    notes = "Template exists only in the target environment.";
                }
                else if (target == null)
                {
                    status = TemplateComparisonStatus.SourceOnly;
                    notes = "Template does not exist in the target environment.";
                }
                else if (string.Equals(source.Fingerprint, target.Fingerprint, StringComparison.Ordinal))
                {
                    status = TemplateComparisonStatus.Identical;
                    notes = "Template content and compared metadata match.";
                }
                else
                {
                    status = TemplateComparisonStatus.Different;
                    notes = BuildDifferenceSummary(source, target);

                    if (target.ModifiedOn.HasValue &&
                        source.ModifiedOn.HasValue &&
                        target.ModifiedOn.Value > source.ModifiedOn.Value)
                    {
                        notes += " Warning: the target record is newer than the source.";
                    }
                }

                DocumentTemplateRecord display = source ?? target;
                rows.Add(new TemplateComparisonRow
                {
                    StatusValue = status,
                    Name = display == null ? string.Empty : display.Name,
                    AssociatedEntity = display == null ? string.Empty : display.AssociatedEntityLogicalName,
                    TemplateType = display == null ? string.Empty : display.DocumentTypeName,
                    SourceStatus = source == null ? string.Empty : source.StatusName,
                    TargetStatus = target == null ? string.Empty : target.StatusName,
                    SourceModified = FormatDate(source == null ? null : source.ModifiedOn),
                    TargetModified = FormatDate(target == null ? null : target.ModifiedOn),
                    SourceSize = FormatSize(source == null ? 0 : source.ContentSizeBytes, source != null),
                    TargetSize = FormatSize(target == null ? 0 : target.ContentSizeBytes, target != null),
                    SourceHash = ShortHash(source == null ? null : source.ContentHash),
                    TargetHash = ShortHash(target == null ? null : target.ContentHash),
                    Notes = notes,
                    Source = source,
                    Target = target
                });
            }

            return rows
                .OrderBy(row => StatusSortOrder(row.StatusValue))
                .ThenBy(row => row.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row.AssociatedEntity, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public TemplateTransferResult Transfer(
            IOrganizationService sourceService,
            IOrganizationService targetService,
            TemplateComparisonRow row,
            TemplateTransferMode mode)
        {
            ValidateTransferRequest(sourceService, targetService, row, mode);

            DocumentTemplateRecord source = row.Source;
            DocumentTemplateRecord target = row.Target;
            bool updated = mode == TemplateTransferMode.UpdateExisting;

            int? sourceTypeCode = GetEntityTypeCode(sourceService, source.AssociatedEntityLogicalName);
            int? targetTypeCode = GetEntityTypeCode(targetService, source.AssociatedEntityLogicalName);

            if (!targetTypeCode.HasValue)
            {
                throw new InvalidOperationException(
                    "The associated table '" + source.AssociatedEntityLogicalName +
                    "' was not found in the target environment.");
            }

            string remappedContent = RemapEntityTypeCodeInPackage(
                source.ContentBase64,
                source.AssociatedEntityLogicalName,
                sourceTypeCode,
                targetTypeCode);

            string remappedClientData = RemapEntityTypeCodeInText(
                source.ClientData,
                source.AssociatedEntityLogicalName,
                sourceTypeCode,
                targetTypeCode);

            Entity entityToWrite = BuildWritableEntity(source, remappedContent, remappedClientData);
            Guid targetId;

            if (updated)
            {
                targetId = target.Id;
                entityToWrite.Id = targetId;
                targetService.Update(entityToWrite);
            }
            else
            {
                targetId = targetService.Create(entityToWrite);
            }

            var result = new TemplateTransferResult
            {
                TemplateName = source.Name,
                WriteCompleted = true,
                UpdatedExisting = updated,
                TargetId = targetId
            };

            try
            {
                // Apply status separately. Some environments keep the default Activated value
                // when status is included only in the initial Create request.
                var statusUpdate = new Entity("documenttemplate", targetId);
                statusUpdate["status"] = source.IsDraft;
                targetService.Update(statusUpdate);

                DocumentTemplateRecord verifiedTarget = RetrieveTemplate(targetService, targetId);
                string verificationMessage = VerifyWrittenTemplate(
                    source,
                    verifiedTarget,
                    remappedContent,
                    remappedClientData);

                result.VerificationSucceeded = string.IsNullOrEmpty(verificationMessage);
                result.Succeeded = result.VerificationSucceeded;
                result.Message = result.VerificationSucceeded
                    ? (updated
                        ? "Updated and verified. Target status: " + verifiedTarget.StatusName + "."
                        : "Created and verified. Target status: " + verifiedTarget.StatusName + ".")
                    : (updated
                        ? "Updated, but verification failed: " + verificationMessage
                        : "Created, but verification failed: " + verificationMessage);
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.VerificationSucceeded = false;
                result.Message = updated
                    ? "Updated, but post-write verification failed: " + ex.Message
                    : "Created, but post-write verification failed: " + ex.Message;
            }

            return result;
        }

        private static void ValidateTransferRequest(
            IOrganizationService sourceService,
            IOrganizationService targetService,
            TemplateComparisonRow row,
            TemplateTransferMode mode)
        {
            if (sourceService == null)
            {
                throw new ArgumentNullException("sourceService");
            }

            if (targetService == null)
            {
                throw new ArgumentNullException("targetService");
            }

            if (row == null || row.Source == null)
            {
                throw new InvalidOperationException("The selected row does not have a source template.");
            }

            if (row.StatusValue == TemplateComparisonStatus.Duplicate)
            {
                throw new InvalidOperationException("Duplicate template keys must be resolved before transfer.");
            }

            if (mode == TemplateTransferMode.CopyMissing && !row.CanCopyMissing)
            {
                throw new InvalidOperationException(
                    "Copy Missing accepts only Source Only templates that do not exist in the target.");
            }

            if (mode == TemplateTransferMode.UpdateExisting && !row.CanUpdateExisting)
            {
                throw new InvalidOperationException(
                    "Update Existing accepts only Different templates with one matching target record.");
            }
        }

        private static Entity BuildWritableEntity(
            DocumentTemplateRecord source,
            string content,
            string clientData)
        {
            var entity = new Entity("documenttemplate");
            entity["name"] = source.Name ?? string.Empty;
            entity["associatedentitytypecode"] = source.AssociatedEntityLogicalName ?? string.Empty;
            entity["documenttype"] = new OptionSetValue(source.DocumentTypeValue);
            entity["content"] = content ?? string.Empty;
            entity["languagecode"] = source.LanguageCode.GetValueOrDefault();
            entity["status"] = source.IsDraft;

            if (source.Description != null)
            {
                entity["description"] = source.Description;
            }

            if (clientData != null)
            {
                entity["clientdata"] = clientData;
            }

            return entity;
        }

        private DocumentTemplateRecord RetrieveTemplate(IOrganizationService service, Guid id)
        {
            Entity entity = service.Retrieve(
                "documenttemplate",
                id,
                new ColumnSet(TemplateColumns));

            return Map(entity);
        }

        private static string VerifyWrittenTemplate(
            DocumentTemplateRecord source,
            DocumentTemplateRecord target,
            string expectedContent,
            string expectedClientData)
        {
            var differences = new List<string>();
            string expectedContentHash = ComputeHash(DecodeBase64(expectedContent));

            AddDifference(
                differences,
                "name",
                !string.Equals(source.Name ?? string.Empty, target.Name ?? string.Empty, StringComparison.Ordinal));

            AddDifference(
                differences,
                "associated table",
                !string.Equals(
                    source.AssociatedEntityLogicalName ?? string.Empty,
                    target.AssociatedEntityLogicalName ?? string.Empty,
                    StringComparison.OrdinalIgnoreCase));

            AddDifference(differences, "document type", source.DocumentTypeValue != target.DocumentTypeValue);
            AddDifference(differences, "language", source.LanguageCode != target.LanguageCode);
            AddDifference(differences, "status", source.IsDraft != target.IsDraft);

            AddDifference(
                differences,
                "description",
                !string.Equals(
                    source.Description ?? string.Empty,
                    target.Description ?? string.Empty,
                    StringComparison.Ordinal));

            AddDifference(
                differences,
                "client metadata",
                !string.Equals(
                    expectedClientData ?? string.Empty,
                    target.ClientData ?? string.Empty,
                    StringComparison.Ordinal));

            AddDifference(
                differences,
                "content hash",
                !string.Equals(expectedContentHash, target.ContentHash, StringComparison.Ordinal));

            return differences.Count == 0
                ? string.Empty
                : "mismatched " + string.Join(", ", differences) + ".";
        }

        private static void AddDifference(List<string> differences, string name, bool isDifferent)
        {
            if (isDifferent)
            {
                differences.Add(name);
            }
        }

        private static DocumentTemplateRecord Map(Entity entity)
        {
            string content = entity.GetAttributeValue<string>("content") ?? string.Empty;
            byte[] contentBytes = DecodeBase64(content);
            string description = entity.GetAttributeValue<string>("description") ?? string.Empty;
            string clientData = entity.GetAttributeValue<string>("clientdata") ?? string.Empty;
            string name = entity.GetAttributeValue<string>("name") ?? string.Empty;
            string associatedEntity = entity.GetAttributeValue<string>("associatedentitytypecode") ?? string.Empty;
            int documentType = GetOptionSetValue(entity, "documenttype");
            int? languageCode = GetNullableInt(entity, "languagecode");
            bool isDraft = entity.GetAttributeValue<bool>("status");
            string contentHash = ComputeHash(contentBytes);
            string comparisonContentHash = ComputeCanonicalContentHash(content, associatedEntity);

            return new DocumentTemplateRecord
            {
                Entity = entity,
                Id = entity.Id,
                Name = name,
                Description = description,
                AssociatedEntityLogicalName = associatedEntity,
                DocumentTypeValue = documentType,
                LanguageCode = languageCode,
                IsDraft = isDraft,
                CreatedOn = GetNullableDateTime(entity, "createdon"),
                ModifiedOn = GetNullableDateTime(entity, "modifiedon"),
                VersionNumber = entity.GetAttributeValue<long?>("versionnumber"),
                ContentBase64 = content,
                ClientData = clientData,
                ContentHash = contentHash,
                ComparisonContentHash = comparisonContentHash,
                Fingerprint = ComputeFingerprint(
                    comparisonContentHash,
                    description,
                    CanonicalizeEntityTypeCodeText(clientData, associatedEntity),
                    languageCode.HasValue
                        ? languageCode.Value.ToString(CultureInfo.InvariantCulture)
                        : string.Empty,
                    isDraft ? "draft" : "activated"),
                ContentSizeBytes = contentBytes.LongLength
            };
        }

        private static int GetOptionSetValue(Entity entity, string attributeName)
        {
            OptionSetValue option = entity.GetAttributeValue<OptionSetValue>(attributeName);
            return option == null ? 0 : option.Value;
        }

        private static int? GetNullableInt(Entity entity, string attributeName)
        {
            if (!entity.Attributes.ContainsKey(attributeName) || entity[attributeName] == null)
            {
                return null;
            }

            if (entity[attributeName] is int)
            {
                return (int)entity[attributeName];
            }

            OptionSetValue option = entity[attributeName] as OptionSetValue;
            return option == null ? (int?)null : option.Value;
        }

        private static DateTime? GetNullableDateTime(Entity entity, string attributeName)
        {
            return entity.Attributes.ContainsKey(attributeName)
                ? (DateTime?)entity.GetAttributeValue<DateTime>(attributeName)
                : null;
        }

        private static byte[] DecodeBase64(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new byte[0];
            }

            try
            {
                return Convert.FromBase64String(value);
            }
            catch (FormatException)
            {
                return Encoding.UTF8.GetBytes(value);
            }
        }

        private static string ComputeHash(byte[] bytes)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return ToHex(sha.ComputeHash(bytes ?? new byte[0]));
            }
        }

        private static string ComputeFingerprint(
            string contentHash,
            string description,
            string clientData,
            string languageCode,
            string status)
        {
            using (var stream = new MemoryStream())
            {
                WriteFingerprintPart(stream, Encoding.UTF8.GetBytes(contentHash ?? string.Empty));
                WriteFingerprintPart(stream, Encoding.UTF8.GetBytes(description ?? string.Empty));
                WriteFingerprintPart(stream, Encoding.UTF8.GetBytes(clientData ?? string.Empty));
                WriteFingerprintPart(stream, Encoding.UTF8.GetBytes(languageCode ?? string.Empty));
                WriteFingerprintPart(stream, Encoding.UTF8.GetBytes(status ?? string.Empty));

                using (SHA256 sha = SHA256.Create())
                {
                    return ToHex(sha.ComputeHash(stream.ToArray()));
                }
            }
        }

        private static void WriteFingerprintPart(Stream stream, byte[] value)
        {
            byte[] length = BitConverter.GetBytes(value.Length);
            stream.Write(length, 0, length.Length);
            stream.Write(value, 0, value.Length);
        }

        private static string ToHex(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Length * 2);
            foreach (byte value in bytes)
            {
                builder.Append(value.ToString("x2", CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }

        private static string BuildDifferenceSummary(
            DocumentTemplateRecord source,
            DocumentTemplateRecord target)
        {
            var differences = new List<string>();

            if (!string.Equals(
                source.ComparisonContentHash,
                target.ComparisonContentHash,
                StringComparison.Ordinal))
            {
                differences.Add("content");
            }

            if (!string.Equals(
                source.Description ?? string.Empty,
                target.Description ?? string.Empty,
                StringComparison.Ordinal))
            {
                differences.Add("description");
            }

            if (!string.Equals(
                CanonicalizeEntityTypeCodeText(source.ClientData, source.AssociatedEntityLogicalName),
                CanonicalizeEntityTypeCodeText(target.ClientData, target.AssociatedEntityLogicalName),
                StringComparison.Ordinal))
            {
                differences.Add("client metadata");
            }

            if (source.LanguageCode != target.LanguageCode)
            {
                differences.Add("language");
            }

            if (source.IsDraft != target.IsDraft)
            {
                differences.Add("status");
            }

            return differences.Count == 0
                ? "Compared metadata differs."
                : "Different " + string.Join(", ", differences) + ".";
        }

        private static int StatusSortOrder(TemplateComparisonStatus status)
        {
            switch (status)
            {
                case TemplateComparisonStatus.Duplicate:
                    return 0;
                case TemplateComparisonStatus.Different:
                    return 1;
                case TemplateComparisonStatus.SourceOnly:
                    return 2;
                case TemplateComparisonStatus.TargetOnly:
                    return 3;
                default:
                    return 4;
            }
        }

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue
                ? value.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                : string.Empty;
        }

        private static string FormatSize(long bytes, bool exists)
        {
            if (!exists)
            {
                return string.Empty;
            }

            if (bytes < 1024)
            {
                return bytes + " B";
            }

            if (bytes < 1024 * 1024)
            {
                return (bytes / 1024d).ToString("0.0", CultureInfo.InvariantCulture) + " KB";
            }

            return (bytes / 1024d / 1024d).ToString("0.0", CultureInfo.InvariantCulture) + " MB";
        }

        private static string ShortHash(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value.Length <= 12 ? value : value.Substring(0, 12);
        }

        private static int? GetEntityTypeCode(IOrganizationService service, string logicalName)
        {
            if (string.IsNullOrWhiteSpace(logicalName))
            {
                return null;
            }

            var request = new RetrieveEntityRequest
            {
                LogicalName = logicalName,
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = true
            };

            var response = (RetrieveEntityResponse)service.Execute(request);
            return response.EntityMetadata.ObjectTypeCode;
        }

        private static string RemapEntityTypeCodeInText(
            string value,
            string logicalName,
            int? sourceTypeCode,
            int? targetTypeCode)
        {
            if (string.IsNullOrEmpty(value) ||
                !sourceTypeCode.HasValue ||
                !targetTypeCode.HasValue ||
                sourceTypeCode.Value == targetTypeCode.Value)
            {
                return value;
            }

            string oldValue = logicalName + "/" +
                              sourceTypeCode.Value.ToString(CultureInfo.InvariantCulture);
            string newValue = logicalName + "/" +
                              targetTypeCode.Value.ToString(CultureInfo.InvariantCulture);
            return value.Replace(oldValue, newValue);
        }

        private static string RemapEntityTypeCodeInPackage(
            string base64Content,
            string logicalName,
            int? sourceTypeCode,
            int? targetTypeCode)
        {
            if (string.IsNullOrWhiteSpace(base64Content) ||
                !sourceTypeCode.HasValue ||
                !targetTypeCode.HasValue ||
                sourceTypeCode.Value == targetTypeCode.Value)
            {
                return base64Content;
            }

            return RewritePackageText(
                base64Content,
                text => RemapEntityTypeCodeInText(
                    text,
                    logicalName,
                    sourceTypeCode,
                    targetTypeCode));
        }

        private static string ComputeCanonicalContentHash(string base64Content, string logicalName)
        {
            if (string.IsNullOrWhiteSpace(base64Content))
            {
                return ComputeHash(new byte[0]);
            }

            try
            {
                byte[] packageBytes = Convert.FromBase64String(base64Content);
                using (var input = new MemoryStream(packageBytes, false))
                using (var archive = new ZipArchive(input, ZipArchiveMode.Read, false))
                using (var canonical = new MemoryStream())
                {
                    foreach (ZipArchiveEntry entry in archive.Entries
                        .OrderBy(item => item.FullName, StringComparer.Ordinal))
                    {
                        WriteFingerprintPart(
                            canonical,
                            Encoding.UTF8.GetBytes(entry.FullName ?? string.Empty));

                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            WriteFingerprintPart(canonical, new byte[0]);
                            continue;
                        }

                        using (Stream entryStream = entry.Open())
                        using (var entryBuffer = new MemoryStream())
                        {
                            entryStream.CopyTo(entryBuffer);
                            byte[] entryBytes = entryBuffer.ToArray();

                            if (IsTextPackagePart(entry.FullName))
                            {
                                string text = DecodeText(entryBytes);
                                entryBytes = Encoding.UTF8.GetBytes(
                                    CanonicalizeEntityTypeCodeText(text, logicalName));
                            }

                            WriteFingerprintPart(canonical, entryBytes);
                        }
                    }

                    return ComputeHash(canonical.ToArray());
                }
            }
            catch (Exception)
            {
                return ComputeHash(DecodeBase64(base64Content));
            }
        }

        private static string DecodeText(byte[] value)
        {
            using (var stream = new MemoryStream(value ?? new byte[0], false))
            using (var reader = new StreamReader(stream, Encoding.UTF8, true))
            {
                return reader.ReadToEnd();
            }
        }

        private static string CanonicalizeEntityTypeCodeText(string value, string logicalName)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(logicalName))
            {
                return value ?? string.Empty;
            }

            string pattern = Regex.Escape(logicalName) + @"/\d+";
            return Regex.Replace(
                value,
                pattern,
                logicalName + "/{TYPECODE}",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        private static string RewritePackageText(string base64Content, Func<string, string> rewrite)
        {
            byte[] originalBytes;
            try
            {
                originalBytes = Convert.FromBase64String(base64Content);
            }
            catch (FormatException)
            {
                return base64Content;
            }

            try
            {
                using (var input = new MemoryStream(originalBytes, false))
                using (var sourceArchive = new ZipArchive(input, ZipArchiveMode.Read, true))
                using (var output = new MemoryStream())
                {
                    using (var targetArchive = new ZipArchive(output, ZipArchiveMode.Create, true))
                    {
                        foreach (ZipArchiveEntry sourceEntry in sourceArchive.Entries)
                        {
                            ZipArchiveEntry targetEntry = targetArchive.CreateEntry(
                                sourceEntry.FullName,
                                CompressionLevel.Optimal);

                            if (string.IsNullOrEmpty(sourceEntry.Name))
                            {
                                continue;
                            }

                            using (Stream sourceStream = sourceEntry.Open())
                            using (Stream targetStream = targetEntry.Open())
                            {
                                if (IsTextPackagePart(sourceEntry.FullName))
                                {
                                    using (var reader = new StreamReader(
                                        sourceStream,
                                        Encoding.UTF8,
                                        true,
                                        4096,
                                        true))
                                    {
                                        string text = reader.ReadToEnd();
                                        string updated = rewrite(text);
                                        byte[] updatedBytes = Encoding.UTF8.GetBytes(updated);
                                        targetStream.Write(updatedBytes, 0, updatedBytes.Length);
                                    }
                                }
                                else
                                {
                                    sourceStream.CopyTo(targetStream);
                                }
                            }
                        }
                    }

                    return Convert.ToBase64String(output.ToArray());
                }
            }
            catch (InvalidDataException)
            {
                return base64Content;
            }
        }

        private static bool IsTextPackagePart(string entryName)
        {
            return entryName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) ||
                   entryName.EndsWith(".rels", StringComparison.OrdinalIgnoreCase);
        }
    }
}
