namespace DGPLattice.Screens.Testing
{
    partial class ucAutoWorkTestHarness
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucAutoWorkTestHarness));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ctxmnGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoTimer = new System.Windows.Forms.Timer(this.components);
            this.tsForm = new System.Windows.Forms.ToolStrip();
            this.tslblTitle = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsdrpbtnSelectWork = new System.Windows.Forms.ToolStripDropDownButton();
            this.replicaWorkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalWorkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tslblSelWorkType = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tscmbInterval = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tscmbRunMode = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnPolling = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnRefresh = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvAutoWorkLog = new System.Windows.Forms.DataGridView();
            this.ctxmnGrid.SuspendLayout();
            this.tsForm.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAutoWorkLog)).BeginInit();
            this.SuspendLayout();
            // 
            // ctxmnGrid
            // 
            this.ctxmnGrid.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.ctxmnGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewMenuItem});
            this.ctxmnGrid.Name = "ctxmnApiMethod";
            this.ctxmnGrid.Size = new System.Drawing.Size(283, 42);
            // 
            // viewMenuItem
            // 
            this.viewMenuItem.Name = "viewMenuItem";
            this.viewMenuItem.Size = new System.Drawing.Size(282, 38);
            this.viewMenuItem.Text = "View Log Record...";
            this.viewMenuItem.Click += new System.EventHandler(this.viewMenuItem_Click);
            // 
            // autoTimer
            // 
            this.autoTimer.Tick += new System.EventHandler(this.AutoTimer_Tick);
            // 
            // tsForm
            // 
            this.tsForm.AllowMerge = false;
            this.tsForm.BackColor = System.Drawing.Color.DarkGray;
            this.tsForm.CanOverflow = false;
            this.tsForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tsForm.GripMargin = new System.Windows.Forms.Padding(0);
            this.tsForm.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsForm.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.tsForm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslblTitle,
            this.toolStripSeparator1,
            this.tsdrpbtnSelectWork,
            this.tslblSelWorkType,
            this.toolStripLabel1,
            this.tscmbInterval,
            this.toolStripSeparator6,
            this.toolStripLabel2,
            this.tscmbRunMode,
            this.toolStripSeparator7,
            this.tsbtnPolling,
            this.toolStripSeparator3,
            this.tsbtnRefresh});
            this.tsForm.Location = new System.Drawing.Point(0, 0);
            this.tsForm.Name = "tsForm";
            this.tsForm.Padding = new System.Windows.Forms.Padding(0);
            this.tsForm.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsForm.Size = new System.Drawing.Size(2200, 60);
            this.tsForm.TabIndex = 1;
            this.tsForm.Text = "toolStrip1";
            // 
            // tslblTitle
            // 
            this.tslblTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tslblTitle.Margin = new System.Windows.Forms.Padding(1);
            this.tslblTitle.Name = "tslblTitle";
            this.tslblTitle.Padding = new System.Windows.Forms.Padding(1);
            this.tslblTitle.Size = new System.Drawing.Size(223, 58);
            this.tslblTitle.Text = "  AutoWork Tester";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(1);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Padding = new System.Windows.Forms.Padding(1);
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 58);
            // 
            // tsdrpbtnSelectWork
            // 
            this.tsdrpbtnSelectWork.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdrpbtnSelectWork.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replicaWorkMenuItem,
            this.generalWorkMenuItem});
            this.tsdrpbtnSelectWork.Image = ((System.Drawing.Image)(resources.GetObject("tsdrpbtnSelectWork.Image")));
            this.tsdrpbtnSelectWork.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdrpbtnSelectWork.Name = "tsdrpbtnSelectWork";
            this.tsdrpbtnSelectWork.Size = new System.Drawing.Size(221, 54);
            this.tsdrpbtnSelectWork.Text = "Select Work Type";
            // 
            // replicaWorkMenuItem
            // 
            this.replicaWorkMenuItem.Name = "replicaWorkMenuItem";
            this.replicaWorkMenuItem.Size = new System.Drawing.Size(359, 44);
            this.replicaWorkMenuItem.Text = "ReplicaWork";
            this.replicaWorkMenuItem.Click += new System.EventHandler(this.replicaWorkMenuItem_Click);
            // 
            // generalWorkMenuItem
            // 
            this.generalWorkMenuItem.Name = "generalWorkMenuItem";
            this.generalWorkMenuItem.Size = new System.Drawing.Size(359, 44);
            this.generalWorkMenuItem.Text = "GeneralWork";
            this.generalWorkMenuItem.Click += new System.EventHandler(this.generalWorkMenuItem_Click);
            // 
            // tslblSelWorkType
            // 
            this.tslblSelWorkType.AutoSize = false;
            this.tslblSelWorkType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tslblSelWorkType.Name = "tslblSelWorkType";
            this.tslblSelWorkType.Size = new System.Drawing.Size(200, 54);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.toolStripLabel1.Margin = new System.Windows.Forms.Padding(1);
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Padding = new System.Windows.Forms.Padding(1);
            this.toolStripLabel1.Size = new System.Drawing.Size(198, 58);
            this.toolStripLabel1.Text = "Interval (Seconds)";
            // 
            // tscmbInterval
            // 
            this.tscmbInterval.BackColor = System.Drawing.Color.Silver;
            this.tscmbInterval.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.tscmbInterval.Items.AddRange(new object[] {
            "3",
            "5",
            "10",
            "15"});
            this.tscmbInterval.Name = "tscmbInterval";
            this.tscmbInterval.Size = new System.Drawing.Size(120, 60);
            this.tscmbInterval.SelectedIndexChanged += new System.EventHandler(this.tscmbInterval_SelectedIndexChanged);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Margin = new System.Windows.Forms.Padding(1);
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Padding = new System.Windows.Forms.Padding(1);
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 58);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.toolStripLabel2.Margin = new System.Windows.Forms.Padding(1);
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Padding = new System.Windows.Forms.Padding(1);
            this.toolStripLabel2.Size = new System.Drawing.Size(122, 58);
            this.toolStripLabel2.Text = "Run Mode";
            // 
            // tscmbRunMode
            // 
            this.tscmbRunMode.AutoSize = false;
            this.tscmbRunMode.BackColor = System.Drawing.Color.Silver;
            this.tscmbRunMode.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.tscmbRunMode.Items.AddRange(new object[] {
            "ONCE",
            "REPEAT"});
            this.tscmbRunMode.Margin = new System.Windows.Forms.Padding(1);
            this.tscmbRunMode.Name = "tscmbRunMode";
            this.tscmbRunMode.Padding = new System.Windows.Forms.Padding(1);
            this.tscmbRunMode.Size = new System.Drawing.Size(120, 58);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Margin = new System.Windows.Forms.Padding(1);
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Padding = new System.Windows.Forms.Padding(1);
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 58);
            // 
            // tsbtnPolling
            // 
            this.tsbtnPolling.BackColor = System.Drawing.Color.Gainsboro;
            this.tsbtnPolling.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnPolling.ForeColor = System.Drawing.Color.Black;
            this.tsbtnPolling.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnPolling.Image")));
            this.tsbtnPolling.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnPolling.Margin = new System.Windows.Forms.Padding(1);
            this.tsbtnPolling.Name = "tsbtnPolling";
            this.tsbtnPolling.Padding = new System.Windows.Forms.Padding(1);
            this.tsbtnPolling.Size = new System.Drawing.Size(148, 58);
            this.tsbtnPolling.Text = "Start Polling";
            this.tsbtnPolling.Click += new System.EventHandler(this.tsbtnPolling_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Margin = new System.Windows.Forms.Padding(1);
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Padding = new System.Windows.Forms.Padding(1);
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 58);
            // 
            // tsbtnRefresh
            // 
            this.tsbtnRefresh.BackColor = System.Drawing.Color.Gainsboro;
            this.tsbtnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnRefresh.ForeColor = System.Drawing.Color.Black;
            this.tsbtnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnRefresh.Image")));
            this.tsbtnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnRefresh.Margin = new System.Windows.Forms.Padding(1);
            this.tsbtnRefresh.Name = "tsbtnRefresh";
            this.tsbtnRefresh.Padding = new System.Windows.Forms.Padding(1);
            this.tsbtnRefresh.Size = new System.Drawing.Size(151, 58);
            this.tsbtnRefresh.Text = "Refresh Grid";
            this.tsbtnRefresh.Click += new System.EventHandler(this.tsbtnRefresh_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dgvAutoWorkLog, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tsForm, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(2200, 1300);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // dgvAutoWorkLog
            // 
            this.dgvAutoWorkLog.AllowUserToAddRows = false;
            this.dgvAutoWorkLog.AllowUserToDeleteRows = false;
            this.dgvAutoWorkLog.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvAutoWorkLog.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAutoWorkLog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAutoWorkLog.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAutoWorkLog.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvAutoWorkLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAutoWorkLog.ContextMenuStrip = this.ctxmnGrid;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAutoWorkLog.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvAutoWorkLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAutoWorkLog.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvAutoWorkLog.EnableHeadersVisualStyles = false;
            this.dgvAutoWorkLog.Location = new System.Drawing.Point(0, 60);
            this.dgvAutoWorkLog.Margin = new System.Windows.Forms.Padding(0);
            this.dgvAutoWorkLog.Name = "dgvAutoWorkLog";
            this.dgvAutoWorkLog.ReadOnly = true;
            this.dgvAutoWorkLog.RowHeadersWidth = 10;
            this.dgvAutoWorkLog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAutoWorkLog.ShowEditingIcon = false;
            this.dgvAutoWorkLog.Size = new System.Drawing.Size(2200, 1240);
            this.dgvAutoWorkLog.TabIndex = 6;
            this.dgvAutoWorkLog.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvAutoWorkLog_CellMouseUp);
            // 
            // ucAutoWorkTestHarness
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ucAutoWorkTestHarness";
            this.Size = new System.Drawing.Size(2200, 1300);
            this.ctxmnGrid.ResumeLayout(false);
            this.tsForm.ResumeLayout(false);
            this.tsForm.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAutoWorkLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer autoTimer;
        private System.Windows.Forms.ContextMenuStrip ctxmnGrid;
        private System.Windows.Forms.ToolStripMenuItem viewMenuItem;
        private System.Windows.Forms.ToolStrip tsForm;
        private System.Windows.Forms.ToolStripLabel tslblTitle;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton tsdrpbtnSelectWork;
        private System.Windows.Forms.ToolStripMenuItem replicaWorkMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generalWorkMenuItem;
        private System.Windows.Forms.ToolStripLabel tslblSelWorkType;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox tscmbInterval;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox tscmbRunMode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton tsbtnPolling;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tsbtnRefresh;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dgvAutoWorkLog;
    }
}
