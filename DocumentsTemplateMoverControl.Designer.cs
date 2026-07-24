namespace Dynamics365TemplateCompareTransfer
{
    partial class DocumentsTemplateMoverControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle headerStyle =
                new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSelectTarget = new System.Windows.Forms.ToolStripButton();
            this.tsbCompare = new System.Windows.Forms.ToolStripButton();
            this.tsbCopyMissing = new System.Windows.Forms.ToolStripButton();
            this.tsbUpdateExisting = new System.Windows.Forms.ToolStripButton();
            this.tsbDryRun = new System.Windows.Forms.ToolStripButton();
            this.tsbExportCsv = new System.Windows.Forms.ToolStripButton();
            this.tsbAbout = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tslStatus = new System.Windows.Forms.ToolStripLabel();
            this.cboStatusFilter = new System.Windows.Forms.ToolStripComboBox();
            this.tslSearch = new System.Windows.Forms.ToolStripLabel();
            this.txtSearch = new System.Windows.Forms.ToolStripTextBox();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.connectionTable = new System.Windows.Forms.TableLayoutPanel();
            this.sourcePanel = new System.Windows.Forms.Panel();
            this.lblSourceCaption = new System.Windows.Forms.Label();
            this.lblSourceValue = new System.Windows.Forms.Label();
            this.targetPanel = new System.Windows.Forms.Panel();
            this.lblTargetCaption = new System.Windows.Forms.Label();
            this.lblTargetValue = new System.Windows.Forms.Label();
            this.summaryPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTotalCaption = new System.Windows.Forms.Label();
            this.lblTotalValue = new System.Windows.Forms.Label();
            this.lblVisibleCaption = new System.Windows.Forms.Label();
            this.lblVisibleValue = new System.Windows.Forms.Label();
            this.lblCandidateCaption = new System.Windows.Forms.Label();
            this.lblCandidateValue = new System.Windows.Forms.Label();
            this.lblDuplicateCaption = new System.Windows.Forms.Label();
            this.lblDuplicateValue = new System.Windows.Forms.Label();
            this.mainSplit = new System.Windows.Forms.SplitContainer();
            this.dgvTemplates = new System.Windows.Forms.DataGridView();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEntity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSourceStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTargetStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSourceModified = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTargetModified = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSourceSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTargetSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSourceHash = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTargetHash = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.logGroup = new System.Windows.Forms.GroupBox();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.logCommandPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCopyLog = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblActivity = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripMenu.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.connectionTable.SuspendLayout();
            this.sourcePanel.SuspendLayout();
            this.targetPanel.SuspendLayout();
            this.summaryPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            this.mainSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTemplates)).BeginInit();
            this.logGroup.SuspendLayout();
            this.logCommandPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.AutoSize = false;
            this.toolStripMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.tsbClose,
                this.toolStripSeparator1,
                this.tsbSelectTarget,
                this.tsbCompare,
                this.tsbCopyMissing,
                this.tsbUpdateExisting,
                this.tsbDryRun,
                this.tsbExportCsv,
                this.tsbAbout,
                this.toolStripSeparator2,
                this.tslStatus,
                this.cboStatusFilter,
                this.tslSearch,
                this.txtSearch});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Padding = new System.Windows.Forms.Padding(8, 0, 2, 0);
            this.toolStripMenu.Size = new System.Drawing.Size(1360, 40);
            this.toolStripMenu.TabIndex = 0;
            // 
            // toolbar buttons
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Text = "Close";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.tsbSelectTarget.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbSelectTarget.Name = "tsbSelectTarget";
            this.tsbSelectTarget.Text = "Select Target";
            this.tsbSelectTarget.ToolTipText = "Choose a target Dataverse environment";
            this.tsbSelectTarget.Click += new System.EventHandler(this.tsbSelectTarget_Click);
            this.tsbCompare.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbCompare.Name = "tsbCompare";
            this.tsbCompare.Text = "Load && Compare";
            this.tsbCompare.ToolTipText = "Load templates from both environments and compare them";
            this.tsbCompare.Click += new System.EventHandler(this.tsbCompare_Click);
            this.tsbCopyMissing.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbCopyMissing.Name = "tsbCopyMissing";
            this.tsbCopyMissing.Text = "Copy Missing";
            this.tsbCopyMissing.ToolTipText = "Create selected Source Only templates in the target";
            this.tsbCopyMissing.Click += new System.EventHandler(this.tsbCopyMissing_Click);
            this.tsbUpdateExisting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbUpdateExisting.Name = "tsbUpdateExisting";
            this.tsbUpdateExisting.Text = "Update Existing";
            this.tsbUpdateExisting.ToolTipText = "Overwrite selected Different target templates";
            this.tsbUpdateExisting.Click += new System.EventHandler(this.tsbUpdateExisting_Click);
            this.tsbDryRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbDryRun.Name = "tsbDryRun";
            this.tsbDryRun.Text = "Dry Run";
            this.tsbDryRun.ToolTipText = "Preview the selected operations without writing to Dataverse";
            this.tsbDryRun.Click += new System.EventHandler(this.tsbDryRun_Click);
            this.tsbExportCsv.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbExportCsv.Name = "tsbExportCsv";
            this.tsbExportCsv.Text = "Export CSV";
            this.tsbExportCsv.ToolTipText = "Export the currently visible comparison rows";
            this.tsbExportCsv.Click += new System.EventHandler(this.tsbExportCsv_Click);
            this.tsbAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbAbout.Name = "tsbAbout";
            this.tsbAbout.Text = "About";
            this.tsbAbout.Click += new System.EventHandler(this.tsbAbout_Click);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // filter controls
            // 
            this.tslStatus.Name = "tslStatus";
            this.tslStatus.Text = "Status:";
            this.cboStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatusFilter.Items.AddRange(new object[] {
                "All",
                "Different",
                "Source Only",
                "Target Only",
                "Identical",
                "Duplicate"});
            this.cboStatusFilter.Name = "cboStatusFilter";
            this.cboStatusFilter.Size = new System.Drawing.Size(115, 40);
            this.cboStatusFilter.SelectedIndexChanged +=
                new System.EventHandler(this.cboStatusFilter_SelectedIndexChanged);
            this.tslSearch.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.tslSearch.Name = "tslSearch";
            this.tslSearch.Text = "Search:";
            this.txtSearch.AutoSize = false;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(185, 25);
            this.txtSearch.ToolTipText =
                "Filter by status, name, table, type, hash, record ID, or notes";
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(36, 52, 71);
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Controls.Add(this.subtitleLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 40);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1360, 74);
            this.headerPanel.TabIndex = 1;
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font =
                new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Location = new System.Drawing.Point(18, 10);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Text = "Dynamics 365 Template Compare && Transfer";
            this.subtitleLabel.AutoSize = true;
            this.subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(217, 226, 236);
            this.subtitleLabel.Location = new System.Drawing.Point(21, 45);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Text =
                "Compare Word and Excel templates, preview changes, then safely copy or update verified target records.";
            // 
            // connectionTable
            // 
            this.connectionTable.ColumnCount = 2;
            this.connectionTable.ColumnStyles.Add(
                new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.connectionTable.ColumnStyles.Add(
                new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.connectionTable.Controls.Add(this.sourcePanel, 0, 0);
            this.connectionTable.Controls.Add(this.targetPanel, 1, 0);
            this.connectionTable.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionTable.Location = new System.Drawing.Point(0, 114);
            this.connectionTable.Name = "connectionTable";
            this.connectionTable.Padding = new System.Windows.Forms.Padding(12, 9, 12, 3);
            this.connectionTable.RowCount = 1;
            this.connectionTable.RowStyles.Add(
                new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.connectionTable.Size = new System.Drawing.Size(1360, 70);
            this.connectionTable.TabIndex = 2;
            // 
            // sourcePanel
            // 
            this.sourcePanel.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.sourcePanel.Controls.Add(this.lblSourceCaption);
            this.sourcePanel.Controls.Add(this.lblSourceValue);
            this.sourcePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourcePanel.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.lblSourceCaption.AutoSize = true;
            this.lblSourceCaption.Font =
                new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblSourceCaption.Location = new System.Drawing.Point(12, 7);
            this.lblSourceCaption.Text = "SOURCE ENVIRONMENT";
            this.lblSourceValue.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.lblSourceValue.AutoEllipsis = true;
            this.lblSourceValue.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSourceValue.ForeColor = System.Drawing.Color.Firebrick;
            this.lblSourceValue.Location = new System.Drawing.Point(12, 27);
            this.lblSourceValue.Size = new System.Drawing.Size(630, 19);
            this.lblSourceValue.Text = "Not connected";
            // 
            // targetPanel
            // 
            this.targetPanel.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.targetPanel.Controls.Add(this.lblTargetCaption);
            this.targetPanel.Controls.Add(this.lblTargetValue);
            this.targetPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.targetPanel.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.lblTargetCaption.AutoSize = true;
            this.lblTargetCaption.Font =
                new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblTargetCaption.Location = new System.Drawing.Point(12, 7);
            this.lblTargetCaption.Text = "TARGET ENVIRONMENT";
            this.lblTargetValue.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.lblTargetValue.AutoEllipsis = true;
            this.lblTargetValue.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTargetValue.ForeColor = System.Drawing.Color.Firebrick;
            this.lblTargetValue.Location = new System.Drawing.Point(12, 27);
            this.lblTargetValue.Size = new System.Drawing.Size(630, 19);
            this.lblTargetValue.Text = "Not selected";
            // 
            // summaryPanel
            // 
            this.summaryPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblTotalCaption,
                this.lblTotalValue,
                this.lblVisibleCaption,
                this.lblVisibleValue,
                this.lblCandidateCaption,
                this.lblCandidateValue,
                this.lblDuplicateCaption,
                this.lblDuplicateValue});
            this.summaryPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.summaryPanel.Location = new System.Drawing.Point(0, 184);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Padding = new System.Windows.Forms.Padding(15, 7, 0, 0);
            this.summaryPanel.Size = new System.Drawing.Size(1360, 38);
            this.summaryPanel.TabIndex = 3;
            ConfigureSummaryLabel(this.lblTotalCaption, "Total:", false, System.Drawing.Color.Black, 4);
            ConfigureSummaryLabel(this.lblTotalValue, "0", true, System.Drawing.Color.Black, 20);
            ConfigureSummaryLabel(this.lblVisibleCaption, "Visible:", false, System.Drawing.Color.Black, 4);
            ConfigureSummaryLabel(this.lblVisibleValue, "0", true, System.Drawing.Color.Black, 20);
            ConfigureSummaryLabel(
                this.lblCandidateCaption,
                "Transfer candidates:",
                false,
                System.Drawing.Color.Black,
                4);
            ConfigureSummaryLabel(
                this.lblCandidateValue,
                "0",
                true,
                System.Drawing.Color.DarkGoldenrod,
                20);
            ConfigureSummaryLabel(
                this.lblDuplicateCaption,
                "Duplicates:",
                false,
                System.Drawing.Color.Black,
                4);
            ConfigureSummaryLabel(
                this.lblDuplicateValue,
                "0",
                true,
                System.Drawing.Color.Firebrick,
                20);
            // 
            // mainSplit
            // 
            this.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.mainSplit.Location = new System.Drawing.Point(0, 222);
            this.mainSplit.Name = "mainSplit";
            this.mainSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.mainSplit.Panel1.Controls.Add(this.dgvTemplates);
            this.mainSplit.Panel1.Padding = new System.Windows.Forms.Padding(12, 0, 12, 6);
            this.mainSplit.Panel2.Controls.Add(this.logGroup);
            this.mainSplit.Panel2.Padding = new System.Windows.Forms.Padding(12, 0, 12, 8);
            this.mainSplit.Size = new System.Drawing.Size(1360, 538);
            this.mainSplit.Panel1MinSize = 180;
            this.mainSplit.Panel2MinSize = 150;
            this.mainSplit.SplitterDistance = 372;
            this.mainSplit.TabIndex = 4;
            this.mainSplit.SizeChanged += new System.EventHandler(this.mainSplit_SizeChanged);
            // 
            // dgvTemplates
            // 
            this.dgvTemplates.AllowUserToAddRows = false;
            this.dgvTemplates.AllowUserToDeleteRows = false;
            this.dgvTemplates.AllowUserToOrderColumns = true;
            this.dgvTemplates.AutoGenerateColumns = false;
            this.dgvTemplates.BackgroundColor = System.Drawing.Color.White;
            this.dgvTemplates.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvTemplates.CellBorderStyle =
                System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            headerStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            headerStyle.BackColor = System.Drawing.Color.FromArgb(230, 235, 240);
            headerStyle.Font =
                new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            headerStyle.ForeColor = System.Drawing.Color.Black;
            headerStyle.SelectionBackColor = System.Drawing.Color.FromArgb(230, 235, 240);
            headerStyle.SelectionForeColor = System.Drawing.Color.Black;
            headerStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTemplates.ColumnHeadersDefaultCellStyle = headerStyle;
            this.dgvTemplates.ColumnHeadersHeight = 38;
            this.dgvTemplates.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colStatus,
                this.colName,
                this.colEntity,
                this.colType,
                this.colSourceStatus,
                this.colTargetStatus,
                this.colSourceModified,
                this.colTargetModified,
                this.colSourceSize,
                this.colTargetSize,
                this.colSourceHash,
                this.colTargetHash,
                this.colNotes});
            this.dgvTemplates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTemplates.EnableHeadersVisualStyles = false;
            this.dgvTemplates.MultiSelect = true;
            this.dgvTemplates.Name = "dgvTemplates";
            this.dgvTemplates.ReadOnly = true;
            this.dgvTemplates.RowHeadersVisible = false;
            this.dgvTemplates.RowTemplate.Height = 25;
            this.dgvTemplates.SelectionMode =
                System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTemplates.CellDoubleClick +=
                new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTemplates_CellDoubleClick);
            this.dgvTemplates.CellFormatting +=
                new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvTemplates_CellFormatting);
            this.dgvTemplates.SelectionChanged +=
                new System.EventHandler(this.dgvTemplates_SelectionChanged);
            ConfigureColumn(this.colStatus, "Status", "Status", 90, false);
            ConfigureColumn(this.colName, "Name", "Template Name", 190, false);
            ConfigureColumn(this.colEntity, "AssociatedEntity", "Associated Table", 125, false);
            ConfigureColumn(this.colType, "TemplateType", "Type", 55, false);
            ConfigureColumn(this.colSourceStatus, "SourceStatus", "Source Status", 80, false);
            ConfigureColumn(this.colTargetStatus, "TargetStatus", "Target Status", 80, false);
            ConfigureColumn(this.colSourceModified, "SourceModified", "Source Modified", 115, false);
            ConfigureColumn(this.colTargetModified, "TargetModified", "Target Modified", 115, false);
            ConfigureColumn(this.colSourceSize, "SourceSize", "Source Size", 75, false);
            ConfigureColumn(this.colTargetSize, "TargetSize", "Target Size", 75, false);
            ConfigureColumn(this.colSourceHash, "SourceHash", "Source Raw Hash", 105, false);
            ConfigureColumn(this.colTargetHash, "TargetHash", "Target Raw Hash", 105, false);
            this.colSourceHash.ToolTipText =
                "First 12 characters of the source raw package SHA-256 hash";
            this.colTargetHash.ToolTipText =
                "First 12 characters of the target raw package SHA-256 hash";
            ConfigureColumn(this.colNotes, "Notes", "Notes", 240, true);
            // 
            // logGroup
            // 
            this.logGroup.Controls.Add(this.lstLog);
            this.logGroup.Controls.Add(this.logCommandPanel);
            this.logGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logGroup.Font =
                new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.logGroup.Name = "logGroup";
            this.logGroup.Text = "Activity Log";
            this.lstLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLog.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.IntegralHeight = false;
            this.lstLog.Name = "lstLog";
            this.logCommandPanel.AutoSize = false;
            this.logCommandPanel.Controls.Add(this.btnCopyLog);
            this.logCommandPanel.Controls.Add(this.btnClearLog);
            this.logCommandPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.logCommandPanel.FlowDirection =
                System.Windows.Forms.FlowDirection.RightToLeft;
            this.logCommandPanel.Height = 34;
            this.logCommandPanel.Name = "logCommandPanel";
            this.logCommandPanel.Padding = new System.Windows.Forms.Padding(0, 3, 3, 2);
            this.btnCopyLog.AutoSize = true;
            this.btnCopyLog.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnCopyLog.Name = "btnCopyLog";
            this.btnCopyLog.Text = "Copy Log";
            this.btnCopyLog.UseVisualStyleBackColor = true;
            this.btnCopyLog.Click += new System.EventHandler(this.btnCopyLog_Click);
            this.btnClearLog.AutoSize = true;
            this.btnClearLog.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblActivity,
                this.progressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 760);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1360, 22);
            this.statusStrip.TabIndex = 5;
            this.lblActivity.Name = "lblActivity";
            this.lblActivity.Spring = true;
            this.lblActivity.Text = "Ready";
            this.lblActivity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.progressBar.MarqueeAnimationSpeed = 25;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(150, 16);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.Visible = false;
            // 
            // DocumentsTemplateMoverControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainSplit);
            this.Controls.Add(this.summaryPanel);
            this.Controls.Add(this.connectionTable);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.toolStripMenu);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "DocumentsTemplateMoverControl";
            this.Size = new System.Drawing.Size(1360, 782);
            this.Load += new System.EventHandler(this.DocumentsTemplateMoverControl_Load);
            this.OnCloseTool +=
                new System.EventHandler(this.DocumentsTemplateMoverControl_OnCloseTool);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.connectionTable.ResumeLayout(false);
            this.sourcePanel.ResumeLayout(false);
            this.sourcePanel.PerformLayout();
            this.targetPanel.ResumeLayout(false);
            this.targetPanel.PerformLayout();
            this.summaryPanel.ResumeLayout(false);
            this.summaryPanel.PerformLayout();
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
            this.mainSplit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTemplates)).EndInit();
            this.logGroup.ResumeLayout(false);
            this.logCommandPanel.ResumeLayout(false);
            this.logCommandPanel.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private static void ConfigureSummaryLabel(
            System.Windows.Forms.Label label,
            string text,
            bool bold,
            System.Drawing.Color color,
            int rightMargin)
        {
            label.AutoSize = true;
            label.Font = new System.Drawing.Font(
                "Segoe UI Semibold",
                9F,
                bold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular);
            label.ForeColor = color;
            label.Margin = new System.Windows.Forms.Padding(0, 4, rightMargin, 0);
            label.Text = text;
        }

        private static void ConfigureColumn(
            System.Windows.Forms.DataGridViewTextBoxColumn column,
            string propertyName,
            string header,
            int width,
            bool fill)
        {
            column.DataPropertyName = propertyName;
            column.HeaderText = header;
            column.Name = "col" + propertyName;
            column.ReadOnly = true;
            column.Width = width;

            if (fill)
            {
                column.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
                column.MinimumWidth = width;
            }
        }

        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbSelectTarget;
        private System.Windows.Forms.ToolStripButton tsbCompare;
        private System.Windows.Forms.ToolStripButton tsbCopyMissing;
        private System.Windows.Forms.ToolStripButton tsbUpdateExisting;
        private System.Windows.Forms.ToolStripButton tsbDryRun;
        private System.Windows.Forms.ToolStripButton tsbExportCsv;
        private System.Windows.Forms.ToolStripButton tsbAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel tslStatus;
        private System.Windows.Forms.ToolStripComboBox cboStatusFilter;
        private System.Windows.Forms.ToolStripLabel tslSearch;
        private System.Windows.Forms.ToolStripTextBox txtSearch;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.TableLayoutPanel connectionTable;
        private System.Windows.Forms.Panel sourcePanel;
        private System.Windows.Forms.Label lblSourceCaption;
        private System.Windows.Forms.Label lblSourceValue;
        private System.Windows.Forms.Panel targetPanel;
        private System.Windows.Forms.Label lblTargetCaption;
        private System.Windows.Forms.Label lblTargetValue;
        private System.Windows.Forms.FlowLayoutPanel summaryPanel;
        private System.Windows.Forms.Label lblTotalCaption;
        private System.Windows.Forms.Label lblTotalValue;
        private System.Windows.Forms.Label lblVisibleCaption;
        private System.Windows.Forms.Label lblVisibleValue;
        private System.Windows.Forms.Label lblCandidateCaption;
        private System.Windows.Forms.Label lblCandidateValue;
        private System.Windows.Forms.Label lblDuplicateCaption;
        private System.Windows.Forms.Label lblDuplicateValue;
        private System.Windows.Forms.SplitContainer mainSplit;
        private System.Windows.Forms.DataGridView dgvTemplates;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEntity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSourceStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTargetStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSourceModified;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTargetModified;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSourceSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTargetSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSourceHash;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTargetHash;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNotes;
        private System.Windows.Forms.GroupBox logGroup;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.FlowLayoutPanel logCommandPanel;
        private System.Windows.Forms.Button btnCopyLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblActivity;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
    }
}
