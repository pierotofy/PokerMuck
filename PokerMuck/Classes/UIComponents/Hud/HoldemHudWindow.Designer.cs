namespace PokerMuck
{
    partial class HoldemHudWindow
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
            this.lblImmediateStats = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblImmediateStats
            // 
            this.lblImmediateStats.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblImmediateStats.Location = new System.Drawing.Point(0, 20);
            this.lblImmediateStats.Name = "lblImmediateStats";
            this.lblImmediateStats.Size = new System.Drawing.Size(182, 21);
            this.lblImmediateStats.TabIndex = 1;
            this.lblImmediateStats.Text = "VPF";
            // 
            // HoldemHudWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(183, 40);
            this.Controls.Add(this.lblImmediateStats);
            this.Name = "HoldemHudWindow";
            this.Controls.SetChildIndex(this.lblImmediateStats, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblImmediateStats;
    }
}