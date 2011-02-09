namespace PokerMuck
{
    partial class PlayerHandPanel
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
            this.lblPlayerName = new System.Windows.Forms.Label();
            this.lblline = new System.Windows.Forms.Label();
            this.handPanel = new PokerMuck.HandPanel();
            this.SuspendLayout();
            // 
            // lblPlayerName
            // 
            this.lblPlayerName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayerName.Location = new System.Drawing.Point(3, 3);
            this.lblPlayerName.Name = "lblPlayerName";
            this.lblPlayerName.Size = new System.Drawing.Size(83, 46);
            this.lblPlayerName.TabIndex = 0;
            this.lblPlayerName.Text = "playerName";
            this.lblPlayerName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblline
            // 
            this.lblline.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblline.Location = new System.Drawing.Point(3, 55);
            this.lblline.Name = "lblline";
            this.lblline.Size = new System.Drawing.Size(186, 2);
            this.lblline.TabIndex = 2;
            // 
            // handPanel
            // 
            this.handPanel.BackColor = System.Drawing.Color.Transparent;
            this.handPanel.BorderPadding = 0;
            this.handPanel.CardSpacing = 4;
            this.handPanel.HandToDisplay = null;
            this.handPanel.Location = new System.Drawing.Point(92, 3);
            this.handPanel.Name = "handPanel";
            this.handPanel.Size = new System.Drawing.Size(98, 46);
            this.handPanel.TabIndex = 1;
            // 
            // PlayerHandPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblline);
            this.Controls.Add(this.handPanel);
            this.Controls.Add(this.lblPlayerName);
            this.Name = "PlayerHandPanel";
            this.Size = new System.Drawing.Size(194, 64);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblPlayerName;
        private HandPanel handPanel;
        private System.Windows.Forms.Label lblline;
    }
}
