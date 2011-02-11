namespace PokerMuck
{
    partial class EntityHandPanel
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
            this.lblEntityName = new System.Windows.Forms.Label();
            this.lblline = new System.Windows.Forms.Label();
            this.handPanel = new PokerMuck.HandPanel();
            this.SuspendLayout();
            // 
            // lblEntityName
            // 
            this.lblEntityName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblEntityName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEntityName.Location = new System.Drawing.Point(3, 3);
            this.lblEntityName.Name = "lblEntityName";
            this.lblEntityName.Size = new System.Drawing.Size(83, 46);
            this.lblEntityName.TabIndex = 0;
            this.lblEntityName.Text = "playerName";
            this.lblEntityName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblline
            // 
            this.lblline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblline.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblline.Location = new System.Drawing.Point(3, 55);
            this.lblline.Name = "lblline";
            this.lblline.Size = new System.Drawing.Size(186, 2);
            this.lblline.TabIndex = 2;
            // 
            // handPanel
            // 
            this.handPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.handPanel.BackColor = System.Drawing.Color.Transparent;
            this.handPanel.BorderPadding = 0;
            this.handPanel.CardSpacing = 4;
            this.handPanel.HandToDisplay = null;
            this.handPanel.Location = new System.Drawing.Point(92, 3);
            this.handPanel.Name = "handPanel";
            this.handPanel.Size = new System.Drawing.Size(92, 46);
            this.handPanel.TabIndex = 3;
            // 
            // EntityHandPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblline);
            this.Controls.Add(this.handPanel);
            this.Controls.Add(this.lblEntityName);
            this.Name = "EntityHandPanel";
            this.Size = new System.Drawing.Size(194, 64);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblEntityName;
        private HandPanel handPanel;
        private System.Windows.Forms.Label lblline;
    }
}
