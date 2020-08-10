namespace MultiFP
{
    partial class LogForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogForm));
            this.NIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lstOffline = new System.Windows.Forms.ListBox();
            this.lstOnline = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // NIcon
            // 
            this.NIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.NIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NIcon.Icon")));
            this.NIcon.Text = "Multi Face Print";
            this.NIcon.Visible = true;
            this.NIcon.DoubleClick += new System.EventHandler(this.NIcon_DoubleClick);
            this.NIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NIcon_MouseDoubleClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lblText);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.lstOffline);
            this.splitContainer1.Panel2.Controls.Add(this.lstOnline);
            this.splitContainer1.Size = new System.Drawing.Size(1171, 734);
            this.splitContainer1.SplitterDistance = 872;
            this.splitContainer1.TabIndex = 2;
            // 
            // lblText
            // 
            this.lblText.BackColor = System.Drawing.Color.White;
            this.lblText.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblText.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblText.Location = new System.Drawing.Point(0, 0);
            this.lblText.Multiline = true;
            this.lblText.Name = "lblText";
            this.lblText.ReadOnly = true;
            this.lblText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lblText.Size = new System.Drawing.Size(872, 316);
            this.lblText.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(111, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Online";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(130, 520);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Offline";
            // 
            // lstOffline
            // 
            this.lstOffline.FormattingEnabled = true;
            this.lstOffline.Location = new System.Drawing.Point(3, 536);
            this.lstOffline.Name = "lstOffline";
            this.lstOffline.Size = new System.Drawing.Size(289, 186);
            this.lstOffline.TabIndex = 1;
            // 
            // lstOnline
            // 
            this.lstOnline.FormattingEnabled = true;
            this.lstOnline.Location = new System.Drawing.Point(2, 40);
            this.lstOnline.Name = "lstOnline";
            this.lstOnline.Size = new System.Drawing.Size(289, 446);
            this.lstOnline.TabIndex = 0;
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1171, 734);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.LogForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon NIcon;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox lblText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstOffline;
        private System.Windows.Forms.ListBox lstOnline;
    }
}

