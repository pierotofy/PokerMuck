namespace PokerMuck
{
    partial class StatisticItem
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
            this.lblName = new System.Windows.Forms.Label();
            this.lblLine = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.Location = new System.Drawing.Point(0, 3);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(58, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Stat: value";
            this.lblName.Click += new System.EventHandler(this.lblName_Click);
            this.lblName.MouseEnter += new System.EventHandler(this.lblName_MouseEnter);
            this.lblName.MouseLeave += new System.EventHandler(this.lblName_MouseLeave);
            // 
            // lblLine
            // 
            this.lblLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLine.Location = new System.Drawing.Point(3, 10);
            this.lblLine.Name = "lblLine";
            this.lblLine.Size = new System.Drawing.Size(167, 2);
            this.lblLine.TabIndex = 1;
            this.lblLine.Visible = false;
            // 
            // StatisticItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblLine);
            this.Name = "StatisticItem";
            this.Size = new System.Drawing.Size(173, 20);
            this.Load += new System.EventHandler(this.StatisticItem_Load);
            this.SizeChanged += new System.EventHandler(this.StatisticItem_SizeChanged);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.StatisticItem_MouseClick);
            this.MouseEnter += new System.EventHandler(this.StatisticsItem_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.StatisticsItem_MouseLeave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblLine;
    }
}
