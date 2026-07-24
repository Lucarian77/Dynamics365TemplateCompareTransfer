using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace Dynamics365TemplateCompareTransfer
{
    public partial class DocumentsTemplateMoverControl : MultipleConnectionsPluginControlBase
    {
        private const int MinimumLogHeight = 150;
        private const int DefaultLogHeight = 170;
        private static readonly Color SelectedRowBackColor = Color.FromArgb(0, 90, 158);
        private static readonly Color SelectedRowForeColor = Color.White;

        private readonly DocumentTemplateService templateService;
        private readonly List<TemplateComparisonRow> allRows;
        private Settings settings;
        private ConnectionDetail sourceConnection;
        private ConnectionDetail targetConnection;
        private IOrganizationService sourceService;
        private IOrganizationService targetService;
        private bool workInProgress;
        private bool adjustingSplitter;

        public DocumentsTemplateMoverControl()
        {
            InitializeComponent();
            templateService = new DocumentTemplateService();
            allRows = new List<TemplateComparisonRow>();
        }

        private void DocumentsTemplateMoverControl_Load(object sender, EventArgs e)
        {
            if (!SettingsManager.Instance.TryLoad(GetType(), out settings))
            {
                settings = new Settings();
                LogWarning("Settings were not found. Default settings have been created.");
            }

            cboStatusFilter.SelectedItem = string.IsNullOrWhiteSpace(settings.LastStatusFilter)
                ? "All"
                : settings.LastStatusFilter;

            if (cboStatusFilter.SelectedIndex < 0)
            {
                cboStatusFilter.SelectedIndex = 0;
            }

            txtSearch.Text = settings.LastSearchText ?? string.Empty;
            UpdateConnectionDisplay();
            UpdateCommandState();

            BeginInvoke(new Action(() =>
            {
                EnsureLogExpanded(
                    settings.ActivityLogHeight >= MinimumLogHeight
                        ? settings.ActivityLogHeight
                        : DefaultLogHeight);
            }));

            AddLog(
                "Ready. Version " + GetApplicationVersion() +
                ". Select a target environment, then run Load & Compare.",
                false);
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tsbSelectTarget_Click(object sender, EventArgs e)
        {
            if (!workInProgress)
            {
                AddAdditionalOrganization();
            }
        }

        private void tsbCompare_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadAndCompareTemplates);
        }

        private void tsbCopyMissing_Click(object sender, EventArgs e)
        {
            TransferSelectedTemplates(TemplateTransferMode.CopyMissing);
        }

        private void tsbUpdateExisting_Click(object sender, EventArgs e)
        {
            TransferSelectedTemplates(TemplateTransferMode.UpdateExisting);
        }

        private void tsbDryRun_Click(object sender, EventArgs e)
        {
            ShowDryRunPreview();
        }

        private void tsbExportCsv_Click(object sender, EventArgs e)
        {
            ExportVisibleRowsToCsv();
        }

        private void tsbAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                "Dynamics 365 Template Compare & Transfer\r\n" +
                "Version " + GetApplicationVersion() + "\r\n\r\n" +
                "Author: Adrian Lucaci\r\n\r\n" +
                "Compares Word and Excel document templates between Dataverse environments. " +
                "Copy and update operations use explicit confirmation, post-write retrieval, " +
                "SHA-256 verification, normalized package comparison, and automatic comparison refresh.\r\n\r\n" +
                "No template content, credentials, or connection secrets are written to the Activity Log or CSV export.",
                "About",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void cboStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void dgvTemplates_SelectionChanged(object sender, EventArgs e)
        {
            UpdateCommandState();
        }

        private void dgvTemplates_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            TemplateComparisonRow row =
                dgvTemplates.Rows[e.RowIndex].DataBoundItem as TemplateComparisonRow;

            if (row != null)
            {
                ShowRowDetails(row);
            }
        }

        private void dgvTemplates_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            TemplateComparisonRow row =
                dgvTemplates.Rows[e.RowIndex].DataBoundItem as TemplateComparisonRow;

            if (row == null)
            {
                return;
            }

            Color backColor;
            switch (row.StatusValue)
            {
                case TemplateComparisonStatus.Different:
                    backColor = Color.FromArgb(255, 244, 204);
                    break;
                case TemplateComparisonStatus.SourceOnly:
                    backColor = Color.FromArgb(217, 237, 247);
                    break;
                case TemplateComparisonStatus.TargetOnly:
                    backColor = Color.FromArgb(238, 238, 238);
                    break;
                case TemplateComparisonStatus.Duplicate:
                    backColor = Color.FromArgb(248, 215, 218);
                    break;
                default:
                    backColor = Color.FromArgb(223, 240, 216);
                    break;
            }

            DataGridViewCellStyle style =
                dgvTemplates.Rows[e.RowIndex].DefaultCellStyle;
            style.BackColor = backColor;
            style.ForeColor = Color.Black;
            style.SelectionBackColor = SelectedRowBackColor;
            style.SelectionForeColor = SelectedRowForeColor;
        }

        private void btnCopyLog_Click(object sender, EventArgs e)
        {
            if (lstLog.Items.Count == 0)
            {
                return;
            }

            try
            {
                Clipboard.SetText(string.Join(
                    Environment.NewLine,
                    lstLog.Items.Cast<object>().Select(Convert.ToString)));
                lblActivity.Text = "Activity Log copied to the clipboard.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "The Activity Log could not be copied.\r\n\r\n" + ex.Message,
                    "Copy Log",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
            lblActivity.Text = "Activity Log cleared.";
        }

        private void mainSplit_SizeChanged(object sender, EventArgs e)
        {
            EnsureLogExpanded(GetCurrentLogHeight());
        }

        private void DocumentsTemplateMoverControl_OnCloseTool(object sender, EventArgs e)
        {
            if (settings == null)
            {
                settings = new Settings();
            }

            settings.LastSourceOrganizationUrl = GetConnectionUrl(sourceConnection);
            settings.LastTargetOrganizationUrl = GetConnectionUrl(targetConnection);
            settings.LastStatusFilter = Convert.ToString(cboStatusFilter.SelectedItem);
            settings.LastSearchText = txtSearch.Text;
            settings.ActivityLogHeight = GetCurrentLogHeight();
            SettingsManager.Instance.Save(GetType(), settings);
        }

        public override void UpdateConnection(
            IOrganizationService newService,
            ConnectionDetail detail,
            string actionName,
            object parameter)
        {
            bool isAdditional = string.Equals(
                actionName,
                "AdditionalOrganization",
                StringComparison.OrdinalIgnoreCase);

            if (isAdditional)
            {
                AdditionalConnectionDetails.Clear();
                if (detail != null)
                {
                    AdditionalConnectionDetails.Add(detail);
                }

                targetConnection = detail;
                targetService = newService ??
                                (detail == null ? null : detail.GetCrmServiceClient());

                AddLog(
                    "Target connection changed to: " +
                    GetConnectionName(detail, "(not connected)") + ".",
                    false);
            }
            else
            {
                sourceConnection = detail;
                sourceService = newService;
                AddLog(
                    "Source connection changed to: " +
                    GetConnectionName(detail, "(not connected)") + ".",
                    false);
            }

            base.UpdateConnection(newService, detail, actionName, parameter);

            ClearComparison();
            UpdateConnectionDisplay();
            UpdateCommandState();
        }

        protected override void ConnectionDetailsUpdated(NotifyCollectionChangedEventArgs e)
        {
            if (AdditionalConnectionDetails.Count == 0)
            {
                targetConnection = null;
                targetService = null;
            }
            else
            {
                targetConnection = AdditionalConnectionDetails[0];
                targetService = targetConnection.GetCrmServiceClient();
            }

            ClearComparison();
            UpdateConnectionDisplay();
            UpdateCommandState();
        }

        private void LoadAndCompareTemplates()
        {
            if (!ValidateConnections())
            {
                return;
            }

            SetBusy(true, "Loading source and target document templates...");
            AddLog(
                "Comparison started. Source: " +
                GetConnectionName(sourceConnection, "Source") +
                "; Target: " +
                GetConnectionName(targetConnection, "Target") + ".",
                false);

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading and comparing document templates",
                Work = (worker, args) =>
                {
                    List<DocumentTemplateRecord> sourceTemplates =
                        templateService.RetrieveTemplates(sourceService);
                    List<DocumentTemplateRecord> targetTemplates =
                        templateService.RetrieveTemplates(targetService);
                    args.Result = templateService.Compare(sourceTemplates, targetTemplates);
                },
                PostWorkCallBack = args =>
                {
                    SetBusy(false, string.Empty);

                    if (args.Error != null)
                    {
                        ShowError("Templates could not be loaded.", args.Error);
                        return;
                    }

                    ReplaceRows((List<TemplateComparisonRow>)args.Result);
                    AddLog(
                        "Comparison completed. " + allRows.Count +
                        " template key(s) evaluated; " +
                        allRows.Count(row => row.StatusValue == TemplateComparisonStatus.SourceOnly) +
                        " Source Only; " +
                        allRows.Count(row => row.StatusValue == TemplateComparisonStatus.Different) +
                        " Different; " +
                        allRows.Count(row => row.StatusValue == TemplateComparisonStatus.Duplicate) +
                        " Duplicate.",
                        false);
                }
            });
        }

        private void TransferSelectedTemplates(TemplateTransferMode mode)
        {
            if (!ValidateConnections())
            {
                return;
            }

            List<TemplateComparisonRow> selectedRows = GetSelectedRows();
            if (selectedRows.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Select at least one comparison row first.",
                    "Nothing selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            Func<TemplateComparisonRow, bool> eligibility =
                mode == TemplateTransferMode.CopyMissing
                    ? new Func<TemplateComparisonRow, bool>(row => row.CanCopyMissing)
                    : new Func<TemplateComparisonRow, bool>(row => row.CanUpdateExisting);

            List<TemplateComparisonRow> ineligible = selectedRows
                .Where(row => !eligibility(row))
                .ToList();

            if (ineligible.Count > 0)
            {
                MessageBox.Show(
                    this,
                    mode == TemplateTransferMode.CopyMissing
                        ? "Copy Missing accepts only Source Only rows. The current selection contains " +
                          ineligible.Count + " row(s) with another status."
                        : "Update Existing accepts only Different rows with exactly one target match. " +
                          "The current selection contains " + ineligible.Count +
                          " row(s) that cannot be updated.",
                    "Selection blocked",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!ConfirmTransfer(selectedRows, mode))
            {
                AddLog(
                    (mode == TemplateTransferMode.CopyMissing
                        ? "Copy Missing"
                        : "Update Existing") + " cancelled by the user.",
                    false);
                return;
            }

            EnsureLogExpanded(Math.Max(GetCurrentLogHeight(), DefaultLogHeight));
            AddLog(
                (mode == TemplateTransferMode.CopyMissing
                    ? "COPY START"
                    : "UPDATE START") +
                ": " + selectedRows.Count + " template(s); Source: " +
                GetConnectionName(sourceConnection, "Source") + "; Target: " +
                GetConnectionName(targetConnection, "Target") + ".",
                false);

            SetBusy(
                true,
                mode == TemplateTransferMode.CopyMissing
                    ? "Copying and verifying missing templates..."
                    : "Updating and verifying existing templates...");

            WorkAsync(new WorkAsyncInfo
            {
                Message = mode == TemplateTransferMode.CopyMissing
                    ? "Copying missing document templates"
                    : "Updating existing document templates",
                Work = (worker, args) =>
                {
                    var batch = new TransferBatchResult
                    {
                        Results = new List<TemplateTransferResult>()
                    };

                    foreach (TemplateComparisonRow row in selectedRows)
                    {
                        try
                        {
                            batch.Results.Add(
                                templateService.Transfer(
                                    sourceService,
                                    targetService,
                                    row,
                                    mode));
                        }
                        catch (Exception ex)
                        {
                            batch.Results.Add(new TemplateTransferResult
                            {
                                TemplateName = row.Name,
                                Succeeded = false,
                                Message = ex.Message
                            });
                        }
                    }

                    try
                    {
                        List<DocumentTemplateRecord> sourceTemplates =
                            templateService.RetrieveTemplates(sourceService);
                        List<DocumentTemplateRecord> targetTemplates =
                            templateService.RetrieveTemplates(targetService);
                        batch.RefreshedRows =
                            templateService.Compare(sourceTemplates, targetTemplates);
                    }
                    catch (Exception ex)
                    {
                        batch.RefreshError = ex;
                    }

                    args.Result = batch;
                },
                PostWorkCallBack = args =>
                {
                    SetBusy(false, string.Empty);
                    EnsureLogExpanded(Math.Max(GetCurrentLogHeight(), DefaultLogHeight));

                    if (args.Error != null)
                    {
                        ShowError("The transfer process did not complete.", args.Error);
                        return;
                    }

                    TransferBatchResult batch = (TransferBatchResult)args.Result;
                    foreach (TemplateTransferResult result in batch.Results)
                    {
                        string prefix;
                        if (result.Succeeded)
                        {
                            prefix = "VERIFIED";
                        }
                        else if (result.WriteCompleted)
                        {
                            prefix = "WRITE COMPLETED / VERIFICATION FAILED";
                        }
                        else
                        {
                            prefix = "ERROR";
                        }

                        AddLog(
                            prefix + ": " + result.TemplateName + " - " + result.Message +
                            (result.TargetId == Guid.Empty
                                ? string.Empty
                                : " Target ID: " + result.TargetId),
                            !result.Succeeded);
                    }

                    if (batch.RefreshedRows != null)
                    {
                        ReplaceRows(batch.RefreshedRows);
                        AddLog(
                            "Automatic post-transfer comparison refresh completed.",
                            false);
                    }
                    else if (batch.RefreshError != null)
                    {
                        AddLog(
                            "WARNING: Transfer results were recorded, but the automatic comparison " +
                            "refresh failed: " + batch.RefreshError.Message,
                            true);
                    }

                    int succeeded = batch.Results.Count(result => result.Succeeded);
                    int failed = batch.Results.Count - succeeded;
                    int verificationFailures = batch.Results.Count(
                        result => result.WriteCompleted && !result.VerificationSucceeded);

                    MessageBox.Show(
                        this,
                        "Operation finished.\r\n\r\n" +
                        "Verified: " + succeeded + "\r\n" +
                        "Failed: " + failed + "\r\n" +
                        "Writes requiring review: " + verificationFailures + "\r\n\r\n" +
                        (batch.RefreshedRows != null
                            ? "The comparison grid was refreshed automatically."
                            : "Run Load & Compare after reviewing the Activity Log."),
                        "Transfer complete",
                        MessageBoxButtons.OK,
                        failed == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                }
            });
        }

        private bool ConfirmTransfer(
            IList<TemplateComparisonRow> selectedRows,
            TemplateTransferMode mode)
        {
            string sourceName = GetConnectionName(sourceConnection, "Source");
            string targetName = GetConnectionName(targetConnection, "Target");
            int newerTargets = selectedRows.Count(row => row.TargetIsNewer);
            var message = new StringBuilder();

            if (mode == TemplateTransferMode.CopyMissing)
            {
                message.AppendLine("Create " + selectedRows.Count +
                                   " missing template(s) in the target?");
                message.AppendLine();
                message.AppendLine("Source: " + sourceName);
                message.AppendLine("Target: " + targetName);
                message.AppendLine();
                message.AppendLine(
                    "Only Source Only records will be created. Existing target records will not be changed.");
                message.AppendLine(
                    "Each write will be retrieved and verified before it is reported as successful.");
            }
            else
            {
                message.AppendLine("Overwrite " + selectedRows.Count +
                                   " existing target template(s)?");
                message.AppendLine();
                message.AppendLine("Source: " + sourceName);
                message.AppendLine("Target: " + targetName);
                message.AppendLine();
                message.AppendLine(
                    "This replaces target content and compared metadata with the selected source values.");

                if (newerTargets > 0)
                {
                    message.AppendLine();
                    message.AppendLine(
                        "WARNING: " + newerTargets +
                        " selected target record(s) are newer than their source records.");
                }

                message.AppendLine();
                message.AppendLine(
                    "Each write will be retrieved and verified before it is reported as successful.");
            }

            message.AppendLine();
            message.AppendLine("Nothing will be deleted. Continue?");

            return MessageBox.Show(
                this,
                message.ToString(),
                mode == TemplateTransferMode.CopyMissing
                    ? "Confirm Copy Missing"
                    : "Confirm Update Existing",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        private void ShowDryRunPreview()
        {
            List<TemplateComparisonRow> rows = GetSelectedRows();
            if (rows.Count == 0)
            {
                rows = GetVisibleRows();
            }

            if (rows.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Load a comparison before running a preview.",
                    "Dry Run",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            int creates = rows.Count(row => row.CanCopyMissing);
            int updates = rows.Count(row => row.CanUpdateExisting);
            int newerTargets = rows.Count(row => row.CanUpdateExisting && row.TargetIsNewer);
            int noChange = rows.Count(row => row.StatusValue == TemplateComparisonStatus.Identical);
            int targetOnly = rows.Count(row => row.StatusValue == TemplateComparisonStatus.TargetOnly);
            int duplicates = rows.Count(row => row.StatusValue == TemplateComparisonStatus.Duplicate);

            var preview = new StringBuilder();
            preview.AppendLine("DRY RUN ONLY — no Dataverse writes will occur.");
            preview.AppendLine();
            preview.AppendLine("Source: " + GetConnectionName(sourceConnection, "Source"));
            preview.AppendLine("Target: " + GetConnectionName(targetConnection, "Target"));
            preview.AppendLine("Rows evaluated: " + rows.Count);
            preview.AppendLine();
            preview.AppendLine("Would create (Copy Missing): " + creates);
            preview.AppendLine("Would overwrite (Update Existing): " + updates);
            preview.AppendLine("  Target newer than source: " + newerTargets);
            preview.AppendLine("Would skip as Identical: " + noChange);
            preview.AppendLine("Would skip as Target Only: " + targetOnly);
            preview.AppendLine("Would block as Duplicate: " + duplicates);
            preview.AppendLine();
            preview.AppendLine("Planned records:");

            foreach (TemplateComparisonRow row in rows.Take(25))
            {
                string action = row.CanCopyMissing
                    ? "CREATE"
                    : row.CanUpdateExisting
                        ? (row.TargetIsNewer ? "UPDATE / TARGET NEWER" : "UPDATE")
                        : "SKIP / " + row.Status.ToUpperInvariant();

                preview.AppendLine(
                    "• " + action + " — " + row.Name + " [" +
                    row.AssociatedEntity + ", " + row.TemplateType + "]");
            }

            if (rows.Count > 25)
            {
                preview.AppendLine("• ...and " + (rows.Count - 25) + " more.");
            }

            AddLog(
                "Dry run completed for " + rows.Count + " row(s): " +
                creates + " create; " + updates + " update; " +
                duplicates + " blocked duplicate.",
                false);

            ShowTextDialog("Dry Run Preview", preview.ToString());
        }

        private void ExportVisibleRowsToCsv()
        {
            List<TemplateComparisonRow> rows = GetVisibleRows();
            if (rows.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "There are no visible comparison rows to export.",
                    "Export CSV",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            string sourceDisplayName = GetConnectionName(sourceConnection, "Source");
            string targetDisplayName = GetConnectionName(targetConnection, "Target");
            string sourceFileName = SanitizeFileName(sourceDisplayName);
            string targetFileName = SanitizeFileName(targetDisplayName);

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                dialog.DefaultExt = "csv";
                dialog.AddExtension = true;
                dialog.FileName =
                    "Dynamics365TemplateCompareTransfer_" +
                    sourceFileName + "_vs_" + targetFileName + "_" +
                    DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    WriteCsv(
                        dialog.FileName,
                        rows,
                        sourceDisplayName,
                        targetDisplayName);
                    AddLog(
                        "CSV export completed: " + rows.Count +
                        " visible row(s) written to " + dialog.FileName + ".",
                        false);

                    MessageBox.Show(
                        this,
                        "Export completed successfully.\r\n\r\n" +
                        rows.Count + " visible row(s) were written.",
                        "Export CSV",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    ShowError("The CSV export could not be completed.", ex);
                }
            }
        }

        private static void WriteCsv(
            string path,
            IEnumerable<TemplateComparisonRow> rows,
            string sourceName,
            string targetName)
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Join(",", new[]
            {
                Csv("Source Environment"),
                Csv("Target Environment"),
                Csv("Status"),
                Csv("Template Name"),
                Csv("Associated Table"),
                Csv("Template Type"),
                Csv("Language Code"),
                Csv("Source Status"),
                Csv("Target Status"),
                Csv("Source Modified"),
                Csv("Target Modified"),
                Csv("Target Newer"),
                Csv("Source Size"),
                Csv("Target Size"),
                Csv("Source Raw SHA-256"),
                Csv("Target Raw SHA-256"),
                Csv("Source Normalized Content SHA-256"),
                Csv("Target Normalized Content SHA-256"),
                Csv("Source Record ID"),
                Csv("Target Record ID"),
                Csv("Notes")
            }));

            foreach (TemplateComparisonRow row in rows)
            {
                builder.AppendLine(string.Join(",", new[]
                {
                    Csv(sourceName),
                    Csv(targetName),
                    Csv(row.Status),
                    Csv(row.Name),
                    Csv(row.AssociatedEntity),
                    Csv(row.TemplateType),
                    Csv(row.Source != null && row.Source.LanguageCode.HasValue
                        ? row.Source.LanguageCode.Value.ToString()
                        : row.Target != null && row.Target.LanguageCode.HasValue
                            ? row.Target.LanguageCode.Value.ToString()
                            : string.Empty),
                    Csv(row.SourceStatus),
                    Csv(row.TargetStatus),
                    Csv(row.SourceModified),
                    Csv(row.TargetModified),
                    Csv(row.TargetIsNewer ? "Yes" : "No"),
                    Csv(row.SourceSize),
                    Csv(row.TargetSize),
                    Csv(row.Source == null ? string.Empty : row.Source.ContentHash),
                    Csv(row.Target == null ? string.Empty : row.Target.ContentHash),
                    Csv(row.Source == null ? string.Empty : row.Source.ComparisonContentHash),
                    Csv(row.Target == null ? string.Empty : row.Target.ComparisonContentHash),
                    Csv(row.Source == null ? string.Empty : row.Source.Id.ToString()),
                    Csv(row.Target == null ? string.Empty : row.Target.Id.ToString()),
                    Csv(row.Notes)
                }));
            }

            File.WriteAllText(path, builder.ToString(), new UTF8Encoding(true));
        }

        private void ShowRowDetails(TemplateComparisonRow row)
        {
            var details = new StringBuilder();
            details.AppendLine("COMPARISON");
            details.AppendLine("Status: " + row.Status);
            details.AppendLine("Name: " + row.Name);
            details.AppendLine("Associated table: " + row.AssociatedEntity);
            details.AppendLine("Template type: " + row.TemplateType);
            details.AppendLine("Target newer than source: " + (row.TargetIsNewer ? "Yes" : "No"));
            details.AppendLine("Notes: " + row.Notes);
            details.AppendLine();
            details.AppendLine("HASH INTERPRETATION");
            details.AppendLine(
                "Raw package hashes may differ after Dataverse applies environment-specific " +
                "entity type codes.");
            details.AppendLine(
                "Comparison uses normalized package content plus the displayed compared metadata.");
            details.AppendLine();
            AppendRecordDetails(details, "SOURCE", row.Source);
            details.AppendLine();
            AppendRecordDetails(details, "TARGET", row.Target);

            ShowTextDialog("Template Details — " + row.Name, details.ToString());
        }

        private static void AppendRecordDetails(
            StringBuilder builder,
            string heading,
            DocumentTemplateRecord record)
        {
            builder.AppendLine(heading);
            if (record == null)
            {
                builder.AppendLine("Record does not exist.");
                return;
            }

            builder.AppendLine("Record ID: " + record.Id);
            builder.AppendLine("Name: " + record.Name);
            builder.AppendLine("Description: " + (record.Description ?? string.Empty));
            builder.AppendLine("Associated table: " + record.AssociatedEntityLogicalName);
            builder.AppendLine("Document type: " + record.DocumentTypeName +
                               " (" + record.DocumentTypeValue + ")");
            builder.AppendLine("Language code: " +
                               (record.LanguageCode.HasValue
                                   ? record.LanguageCode.Value.ToString()
                                   : string.Empty));
            builder.AppendLine("Status: " + record.StatusName);
            builder.AppendLine("Created: " + FormatDate(record.CreatedOn));
            builder.AppendLine("Modified: " + FormatDate(record.ModifiedOn));
            builder.AppendLine("Version number: " +
                               (record.VersionNumber.HasValue
                                   ? record.VersionNumber.Value.ToString()
                                   : string.Empty));
            builder.AppendLine("Content size: " + record.ContentSizeBytes + " bytes");
            builder.AppendLine("Raw package SHA-256: " + record.ContentHash);
            builder.AppendLine(
                "Normalized content SHA-256: " + record.ComparisonContentHash);
        }

        private void ShowTextDialog(string title, string text)
        {
            using (var form = new Form())
            using (var textBox = new RichTextBox())
            using (var buttonPanel = new FlowLayoutPanel())
            using (var copyButton = new Button())
            using (var closeButton = new Button())
            {
                form.Text = title;
                form.StartPosition = FormStartPosition.CenterParent;
                form.MinimizeBox = false;
                form.MaximizeBox = true;
                form.ShowIcon = false;
                form.Size = new Size(820, 620);
                form.MinimumSize = new Size(620, 420);

                textBox.Dock = DockStyle.Fill;
                textBox.ReadOnly = true;
                textBox.WordWrap = false;
                textBox.Font = new Font("Consolas", 9F);
                textBox.Text = text;

                buttonPanel.Dock = DockStyle.Bottom;
                buttonPanel.Height = 42;
                buttonPanel.FlowDirection = FlowDirection.RightToLeft;
                buttonPanel.Padding = new Padding(5);

                closeButton.Text = "Close";
                closeButton.AutoSize = true;
                closeButton.DialogResult = DialogResult.OK;

                copyButton.Text = "Copy";
                copyButton.AutoSize = true;
                copyButton.Click += (sender, args) =>
                {
                    try
                    {
                        Clipboard.SetText(textBox.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            form,
                            ex.Message,
                            "Copy",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                };

                buttonPanel.Controls.Add(closeButton);
                buttonPanel.Controls.Add(copyButton);
                form.Controls.Add(textBox);
                form.Controls.Add(buttonPanel);
                form.AcceptButton = closeButton;
                form.ShowDialog(this);
            }
        }

        private bool ValidateConnections()
        {
            if (sourceService == null)
            {
                MessageBox.Show(
                    this,
                    "Connect XrmToolBox to the source environment first.",
                    "Source connection required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }

            if (targetService == null || targetConnection == null)
            {
                MessageBox.Show(
                    this,
                    "Select a target environment first.",
                    "Target connection required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }

            string sourceUrl = NormalizeUrl(GetConnectionUrl(sourceConnection));
            string targetUrl = NormalizeUrl(GetConnectionUrl(targetConnection));
            Guid sourceOrganizationId = TryGetOrganizationId(sourceService);
            Guid targetOrganizationId = TryGetOrganizationId(targetService);

            bool matchingUrl =
                !string.IsNullOrEmpty(sourceUrl) &&
                string.Equals(sourceUrl, targetUrl, StringComparison.OrdinalIgnoreCase);

            bool matchingOrganization =
                sourceOrganizationId != Guid.Empty &&
                targetOrganizationId != Guid.Empty &&
                sourceOrganizationId == targetOrganizationId;

            if (matchingUrl || matchingOrganization)
            {
                MessageBox.Show(
                    this,
                    "The source and target connections point to the same environment. " +
                    "Select a different target.",
                    "Connections must be different",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ApplyFilter()
        {
            if (dgvTemplates == null)
            {
                return;
            }

            string status = Convert.ToString(cboStatusFilter.SelectedItem);
            string search = (txtSearch.Text ?? string.Empty).Trim();
            IEnumerable<TemplateComparisonRow> rows = allRows;

            if (!string.IsNullOrWhiteSpace(status) &&
                !string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
            {
                rows = rows.Where(
                    row => string.Equals(row.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                rows = rows.Where(row =>
                    Contains(row.Status, search) ||
                    Contains(row.Name, search) ||
                    Contains(row.AssociatedEntity, search) ||
                    Contains(row.TemplateType, search) ||
                    Contains(row.SourceStatus, search) ||
                    Contains(row.TargetStatus, search) ||
                    Contains(row.SourceHash, search) ||
                    Contains(row.TargetHash, search) ||
                    Contains(
                        row.Source == null ? string.Empty : row.Source.ContentHash,
                        search) ||
                    Contains(
                        row.Target == null ? string.Empty : row.Target.ContentHash,
                        search) ||
                    Contains(
                        row.Source == null
                            ? string.Empty
                            : row.Source.ComparisonContentHash,
                        search) ||
                    Contains(
                        row.Target == null
                            ? string.Empty
                            : row.Target.ComparisonContentHash,
                        search) ||
                    Contains(
                        row.Source == null ? string.Empty : row.Source.Id.ToString(),
                        search) ||
                    Contains(
                        row.Target == null ? string.Empty : row.Target.Id.ToString(),
                        search) ||
                    Contains(row.Notes, search));
            }

            List<TemplateComparisonRow> visibleRows = rows.ToList();
            dgvTemplates.DataSource = new BindingList<TemplateComparisonRow>(visibleRows);
            UpdateSummary(visibleRows.Count);
            UpdateCommandState();
        }

        private void ReplaceRows(IEnumerable<TemplateComparisonRow> rows)
        {
            allRows.Clear();
            allRows.AddRange(rows ?? Enumerable.Empty<TemplateComparisonRow>());
            ApplyFilter();
        }

        private void UpdateSummary(int visibleCount)
        {
            lblTotalValue.Text = allRows.Count.ToString();
            lblVisibleValue.Text = visibleCount.ToString();
            lblCandidateValue.Text = allRows.Count(
                row => row.CanCopyMissing || row.CanUpdateExisting).ToString();
            lblDuplicateValue.Text = allRows.Count(
                row => row.StatusValue == TemplateComparisonStatus.Duplicate).ToString();
        }

        private void UpdateConnectionDisplay()
        {
            lblSourceValue.Text = GetConnectionName(sourceConnection, "Not connected");
            lblSourceValue.ForeColor = sourceConnection == null
                ? Color.Firebrick
                : Color.FromArgb(0, 112, 60);

            lblTargetValue.Text = GetConnectionName(targetConnection, "Not selected");
            lblTargetValue.ForeColor = targetConnection == null
                ? Color.Firebrick
                : Color.FromArgb(0, 112, 60);
        }

        private void UpdateCommandState()
        {
            if (tsbSelectTarget == null)
            {
                return;
            }

            bool connected = sourceService != null && targetService != null;
            List<TemplateComparisonRow> selected = GetSelectedRows();
            bool canCopy = selected.Count > 0 && selected.All(row => row.CanCopyMissing);
            bool canUpdate = selected.Count > 0 && selected.All(row => row.CanUpdateExisting);
            bool hasRows = allRows.Count > 0;

            tsbSelectTarget.Enabled = !workInProgress;
            tsbCompare.Enabled = !workInProgress && connected;
            tsbCopyMissing.Enabled = !workInProgress && connected && canCopy;
            tsbUpdateExisting.Enabled = !workInProgress && connected && canUpdate;
            tsbDryRun.Enabled = !workInProgress && connected && hasRows;
            tsbExportCsv.Enabled = !workInProgress && hasRows;
            tsbAbout.Enabled = !workInProgress;
            cboStatusFilter.Enabled = !workInProgress;
            txtSearch.Enabled = !workInProgress;
            dgvTemplates.Enabled = !workInProgress;
            btnCopyLog.Enabled = !workInProgress && lstLog.Items.Count > 0;
            btnClearLog.Enabled = !workInProgress && lstLog.Items.Count > 0;
        }

        private void SetBusy(bool busy, string message)
        {
            workInProgress = busy;
            progressBar.Visible = busy;
            lblActivity.Text = string.IsNullOrWhiteSpace(message) ? "Ready" : message;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
            UpdateCommandState();
        }

        private void ClearComparison()
        {
            allRows.Clear();
            if (dgvTemplates != null)
            {
                dgvTemplates.DataSource = null;
            }

            if (lblTotalValue != null)
            {
                UpdateSummary(0);
            }
        }

        private void AddLog(string message, bool isError)
        {
            if (lstLog == null)
            {
                return;
            }

            string entry = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + message;
            int index = lstLog.Items.Add(entry);
            lstLog.SelectedIndex = index;
            lstLog.SelectedIndex = -1;
            lstLog.TopIndex = Math.Max(0, lstLog.Items.Count - 1);

            if (isError)
            {
                LogError(message);
            }
            else
            {
                LogInfo(message);
            }

            if (mainSplit != null && (isError || message.IndexOf(
                "VERIFIED",
                StringComparison.OrdinalIgnoreCase) >= 0))
            {
                EnsureLogExpanded(Math.Max(GetCurrentLogHeight(), DefaultLogHeight));
            }

            UpdateCommandState();
        }

        private void ShowError(string message, Exception error)
        {
            EnsureLogExpanded(Math.Max(GetCurrentLogHeight(), DefaultLogHeight));
            AddLog("ERROR: " + message + " " + error.Message, true);
            MessageBox.Show(
                this,
                message + "\r\n\r\n" + error.Message,
                "Dynamics 365 Template Compare & Transfer",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void EnsureLogExpanded(int requestedHeight)
        {
            if (mainSplit == null ||
                mainSplit.Height <= mainSplit.Panel1MinSize + mainSplit.Panel2MinSize ||
                adjustingSplitter)
            {
                return;
            }

            int desiredHeight = Math.Max(MinimumLogHeight, requestedHeight);
            int maximumHeight = mainSplit.Height - mainSplit.Panel1MinSize -
                                mainSplit.SplitterWidth;
            desiredHeight = Math.Min(desiredHeight, maximumHeight);
            int distance = mainSplit.Height - desiredHeight - mainSplit.SplitterWidth;

            if (distance < mainSplit.Panel1MinSize)
            {
                distance = mainSplit.Panel1MinSize;
            }

            try
            {
                adjustingSplitter = true;
                mainSplit.SplitterDistance = distance;
            }
            finally
            {
                adjustingSplitter = false;
            }
        }

        private int GetCurrentLogHeight()
        {
            if (mainSplit == null || mainSplit.Panel2 == null)
            {
                return DefaultLogHeight;
            }

            return Math.Max(MinimumLogHeight, mainSplit.Panel2.Height);
        }

        private List<TemplateComparisonRow> GetSelectedRows()
        {
            if (dgvTemplates == null)
            {
                return new List<TemplateComparisonRow>();
            }

            return dgvTemplates.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(gridRow => gridRow.DataBoundItem as TemplateComparisonRow)
                .Where(row => row != null)
                .Distinct()
                .ToList();
        }

        private List<TemplateComparisonRow> GetVisibleRows()
        {
            if (dgvTemplates == null)
            {
                return new List<TemplateComparisonRow>();
            }

            return dgvTemplates.Rows
                .Cast<DataGridViewRow>()
                .Select(gridRow => gridRow.DataBoundItem as TemplateComparisonRow)
                .Where(row => row != null)
                .ToList();
        }

        private static string Csv(string value)
        {
            string safeValue = value ?? string.Empty;
            return "\"" + safeValue.Replace("\"", "\"\"") + "\"";
        }

        private static string SanitizeFileName(string value)
        {
            string result = value ?? string.Empty;
            foreach (char invalid in Path.GetInvalidFileNameChars())
            {
                result = result.Replace(invalid, '_');
            }

            result = result.Replace(' ', '_');
            return string.IsNullOrWhiteSpace(result) ? "Environment" : result;
        }

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue
                ? value.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
                : string.Empty;
        }

        private static bool Contains(string value, string search)
        {
            return !string.IsNullOrEmpty(value) &&
                   value.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string GetApplicationVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return version == null ? "1.2026.1.2" : version.ToString();
        }

        private static string GetConnectionName(ConnectionDetail detail, string fallback)
        {
            if (detail == null)
            {
                return fallback;
            }

            if (!string.IsNullOrWhiteSpace(detail.ConnectionName))
            {
                return detail.ConnectionName;
            }

            string url = GetConnectionUrl(detail);
            return string.IsNullOrWhiteSpace(url) ? fallback : url;
        }

        private static string GetConnectionUrl(ConnectionDetail detail)
        {
            return detail == null ? null : detail.WebApplicationUrl;
        }

        private static string NormalizeUrl(string url)
        {
            return string.IsNullOrWhiteSpace(url)
                ? string.Empty
                : url.Trim().TrimEnd('/');
        }

        private static Guid TryGetOrganizationId(IOrganizationService service)
        {
            if (service == null)
            {
                return Guid.Empty;
            }

            try
            {
                var response = (WhoAmIResponse)service.Execute(new WhoAmIRequest());
                return response.OrganizationId;
            }
            catch
            {
                return Guid.Empty;
            }
        }
    }
}
