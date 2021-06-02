
namespace DGPLattice.Screens.Testing
{
    partial class frmProcDetail
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProcDetail));
            this.rtbxProcDetail = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtbxProcDetail
            // 
            this.rtbxProcDetail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbxProcDetail.Location = new System.Drawing.Point(22, 24);
            this.rtbxProcDetail.Margin = new System.Windows.Forms.Padding(0);
            this.rtbxProcDetail.Name = "rtbxProcDetail";
            this.rtbxProcDetail.ReadOnly = true;
            this.rtbxProcDetail.Size = new System.Drawing.Size(830, 481);
            this.rtbxProcDetail.TabIndex = 0;
            this.rtbxProcDetail.Text = "";
            // 
            // frmProcDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(874, 529);
            this.Controls.Add(this.rtbxProcDetail);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmProcDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Process Steps";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbxProcDetail;
    }
}