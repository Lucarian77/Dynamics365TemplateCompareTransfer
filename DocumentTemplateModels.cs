using Microsoft.Xrm.Sdk;
using System;

namespace Dynamics365TemplateCompareTransfer
{
    internal enum TemplateComparisonStatus
    {
        Identical,
        Different,
        SourceOnly,
        TargetOnly,
        Duplicate
    }

    internal enum TemplateTransferMode
    {
        CopyMissing,
        UpdateExisting
    }

    internal sealed class DocumentTemplateRecord
    {
        public Entity Entity { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AssociatedEntityLogicalName { get; set; }

        public int DocumentTypeValue { get; set; }

        public string DocumentTypeName
        {
            get
            {
                switch (DocumentTypeValue)
                {
                    case 1:
                        return "Excel";
                    case 2:
                        return "Word";
                    default:
                        return "Other (" + DocumentTypeValue + ")";
                }
            }
        }

        public int? LanguageCode { get; set; }

        public bool IsDraft { get; set; }

        public string StatusName
        {
            get { return IsDraft ? "Draft" : "Activated"; }
        }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public long? VersionNumber { get; set; }

        public string ContentBase64 { get; set; }

        public string ClientData { get; set; }

        public string ContentHash { get; set; }

        public string ComparisonContentHash { get; set; }

        public string Fingerprint { get; set; }

        public long ContentSizeBytes { get; set; }

        public string Key
        {
            get
            {
                return string.Format(
                    "{0}|{1}|{2}|{3}",
                    Normalize(Name),
                    Normalize(AssociatedEntityLogicalName),
                    DocumentTypeValue,
                    LanguageCode.HasValue ? LanguageCode.Value.ToString() : string.Empty);
            }
        }

        private static string Normalize(string value)
        {
            return (value ?? string.Empty).Trim().ToUpperInvariant();
        }
    }

    internal sealed class TemplateComparisonRow
    {
        public TemplateComparisonStatus StatusValue { get; set; }

        public string Status
        {
            get
            {
                switch (StatusValue)
                {
                    case TemplateComparisonStatus.SourceOnly:
                        return "Source Only";
                    case TemplateComparisonStatus.TargetOnly:
                        return "Target Only";
                    default:
                        return StatusValue.ToString();
                }
            }
        }

        public string Name { get; set; }

        public string AssociatedEntity { get; set; }

        public string TemplateType { get; set; }

        public string SourceStatus { get; set; }

        public string TargetStatus { get; set; }

        public string SourceModified { get; set; }

        public string TargetModified { get; set; }

        public string SourceSize { get; set; }

        public string TargetSize { get; set; }

        public string SourceHash { get; set; }

        public string TargetHash { get; set; }

        public string Notes { get; set; }

        public DocumentTemplateRecord Source { get; set; }

        public DocumentTemplateRecord Target { get; set; }

        public bool CanCopyMissing
        {
            get
            {
                return Source != null &&
                       Target == null &&
                       StatusValue == TemplateComparisonStatus.SourceOnly;
            }
        }

        public bool CanUpdateExisting
        {
            get
            {
                return Source != null &&
                       Target != null &&
                       StatusValue == TemplateComparisonStatus.Different;
            }
        }

        public bool TargetIsNewer
        {
            get
            {
                return Source != null &&
                       Target != null &&
                       Source.ModifiedOn.HasValue &&
                       Target.ModifiedOn.HasValue &&
                       Target.ModifiedOn.Value > Source.ModifiedOn.Value;
            }
        }
    }

    internal sealed class TemplateTransferResult
    {
        public string TemplateName { get; set; }

        public bool Succeeded { get; set; }

        public bool WriteCompleted { get; set; }

        public bool UpdatedExisting { get; set; }

        public bool VerificationSucceeded { get; set; }

        public Guid TargetId { get; set; }

        public string Message { get; set; }
    }

    internal sealed class TransferBatchResult
    {
        public System.Collections.Generic.List<TemplateTransferResult> Results { get; set; }

        public System.Collections.Generic.List<TemplateComparisonRow> RefreshedRows { get; set; }

        public Exception RefreshError { get; set; }
    }
}
