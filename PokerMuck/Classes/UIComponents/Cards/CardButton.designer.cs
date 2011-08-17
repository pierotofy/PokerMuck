namespace PokerMuck
{
    partial class CardButton
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
            this.lblText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblText.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblText.Location = new System.Drawing.Point(0, 0);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(30, 30);
            this.lblText.TabIndex = 0;
            this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblText.Click += new System.EventHandler(this.lblText_Click);
            this.lblText.MouseEnter += new System.EventHandler(this.lblText_MouseEnter);
            this.lblText.MouseLeave += new System.EventHandler(this.lblText_MouseLeave);
            // 
            // CardButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblText);
            this.Name = "CardButton";
            this.Size = new System.Drawing.Size(30, 30);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblText;
    }
}
