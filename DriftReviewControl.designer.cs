namespace DriftReview.XTB
{
    partial class DriftReviewControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblEnd = new System.Windows.Forms.Label();
            this.lblStart = new System.Windows.Forms.Label();
            this.dtEnd = new System.Windows.Forms.DateTimePicker();
            this.dtStart = new System.Windows.Forms.DateTimePicker();
            this.txtExclNames = new System.Windows.Forms.TextBox();
            this.txtExclUsers = new System.Windows.Forms.TextBox();
            this.txtExclApps = new System.Windows.Forms.TextBox();
            this.lblExclNames = new System.Windows.Forms.Label();
            this.lblExclUsers = new System.Windows.Forms.Label();
            this.lblExclApps = new System.Windows.Forms.Label();
            this.numPage = new System.Windows.Forms.NumericUpDown();
            this.numMax = new System.Windows.Forms.NumericUpDown();
            this.lblPage = new System.Windows.Forms.Label();
            this.lblMax = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.btnOpenLog = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblHint = new System.Windows.Forms.Label();
            this.grid = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCounts = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.btnOpenLog);
            this.panelTop.Controls.Add(this.btnExportCsv);
            this.panelTop.Controls.Add(this.btnRun);
            this.panelTop.Controls.Add(this.btnPreview);
            this.panelTop.Controls.Add(this.btnTest);
            this.panelTop.Controls.Add(this.lblMax);
            this.panelTop.Controls.Add(this.lblPage);
            this.panelTop.Controls.Add(this.numMax);
            this.panelTop.Controls.Add(this.numPage);
            this.panelTop.Controls.Add(this.lblExclApps);
            this.panelTop.Controls.Add(this.lblExclUsers);
            this.panelTop.Controls.Add(this.lblExclNames);
            this.panelTop.Controls.Add(this.txtExclApps);
            this.panelTop.Controls.Add(this.txtExclUsers);
            this.panelTop.Controls.Add(this.txtExclNames);
            this.panelTop.Controls.Add(this.dtStart);
            this.panelTop.Controls.Add(this.dtEnd);
            this.panelTop.Controls.Add(this.lblStart);
            this.panelTop.Controls.Add(this.lblEnd);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1100, 120);
            this.panelTop.TabIndex = 0;
            // 
            // labels & pickers
            // 
            this.lblStart.AutoSize = true; this.lblStart.Location = new System.Drawing.Point(10, 12); this.lblStart.Text = "Start (UTC)";
            this.lblEnd.AutoSize = true; this.lblEnd.Location = new System.Drawing.Point(10, 44); this.lblEnd.Text = "End (UTC)";
            this.dtStart.Location = new System.Drawing.Point(90, 8); this.dtStart.Name = "dtStart"; this.dtStart.Size = new System.Drawing.Size(220, 23);
            this.dtEnd.Location = new System.Drawing.Point(90, 40); this.dtEnd.Name = "dtEnd"; this.dtEnd.Size = new System.Drawing.Size(220, 23);
            // 
            // exclusions
            // 
            this.lblExclNames.AutoSize = true; this.lblExclNames.Location = new System.Drawing.Point(330, 12); this.lblExclNames.Text = "Exclude Names";
            this.txtExclNames.Location = new System.Drawing.Point(430, 8); this.txtExclNames.Width = 280;
            this.lblExclUsers.AutoSize = true; this.lblExclUsers.Location = new System.Drawing.Point(330, 44); this.lblExclUsers.Text = "Exclude User GUIDs";
            this.txtExclUsers.Location = new System.Drawing.Point(430, 40); this.txtExclUsers.Width = 280;
            this.lblExclApps.AutoSize = true; this.lblExclApps.Location = new System.Drawing.Point(330, 76); this.lblExclApps.Text = "Exclude App IDs";
            this.txtExclApps.Location = new System.Drawing.Point(430, 72); this.txtExclApps.Width = 280;
            // 
            // paging
            // 
            this.lblPage.AutoSize = true; this.lblPage.Location = new System.Drawing.Point(730, 12); this.lblPage.Text = "Page size";
            this.numPage.Location = new System.Drawing.Point(790, 8); this.numPage.Minimum = 1; this.numPage.Maximum = 5000; this.numPage.Value = 500;
            this.lblMax.AutoSize = true; this.lblMax.Location = new System.Drawing.Point(730, 44); this.lblMax.Text = "Max rows";
            this.numMax.Location = new System.Drawing.Point(790, 40); this.numMax.Minimum = 1000; this.numMax.Maximum = 1000000; this.numMax.Increment = 1000; this.numMax.Value = 100000;
            // 
            // buttons
            // 
            this.btnTest.Location = new System.Drawing.Point(900, 6); this.btnTest.Text = "Test Conn";
            this.btnPreview.Location = new System.Drawing.Point(900, 36); this.btnPreview.Text = "Preview Top N";
            this.btnRun.Location = new System.Drawing.Point(900, 66); this.btnRun.Text = "Run";
            this.btnExportCsv.Location = new System.Drawing.Point(990, 66); this.btnExportCsv.Text = "Export CSV";
            this.btnOpenLog.Location = new System.Drawing.Point(990, 36); this.btnOpenLog.Text = "Open Log";
            // 
            // splitContainer
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 120);
            this.splitContainer1.Name = "splitContainer1";
            // left
            this.splitContainer1.Panel1.Controls.Add(this.lblHint);
            // right
            this.splitContainer1.Panel2.Controls.Add(this.grid);
            this.splitContainer1.Size = new System.Drawing.Size(1100, 520);
            this.splitContainer1.SplitterDistance = 260;
            // 
            // lblHint
            // 
            this.lblHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHint.Text = "Filters & stats will appear here soon…";
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.ReadOnly = true;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            // 
            // statusStrip
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.lblStatus, this.lblCounts });
            this.statusStrip1.Location = new System.Drawing.Point(0, 640);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1100, 22);
            this.lblStatus.Spring = true; this.lblStatus.Text = "Ready";
            this.lblCounts.Text = "0 rows";
            // 
            // DriftReviewControl
            // 
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStrip1);
            this.Name = "DriftReviewControl";
            this.Size = new System.Drawing.Size(1100, 662);
            this.panelTop.ResumeLayout(false); this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.statusStrip1.ResumeLayout(false); this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.DateTimePicker dtStart;
        private System.Windows.Forms.DateTimePicker dtEnd;
        private System.Windows.Forms.TextBox txtExclNames;
        private System.Windows.Forms.TextBox txtExclUsers;
        private System.Windows.Forms.TextBox txtExclApps;
        private System.Windows.Forms.Label lblExclNames;
        private System.Windows.Forms.Label lblExclUsers;
        private System.Windows.Forms.Label lblExclApps;
        private System.Windows.Forms.NumericUpDown numPage;
        private System.Windows.Forms.NumericUpDown numMax;
        private System.Windows.Forms.Label lblPage;
        private System.Windows.Forms.Label lblMax;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.Button btnOpenLog;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblCounts;
        private System.Windows.Forms.Label lblHint;
    }
}
