namespace PokerMuck
{
    partial class EntityCardListPanel
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
            this.CardListPanel = new PokerMuck.CardListPanel();
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
            // CardListPanel
            // 
            this.CardListPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CardListPanel.BackColor = System.Drawing.Color.Transparent;
            this.CardListPanel.BorderPadding = 0;
            this.CardListPanel.CardSpacing = 4;
            this.CardListPanel.CardListToDisplay = null;
            this.CardListPanel.Location = new System.Drawing.Point(92, 3);
            this.CardListPanel.Name = "CardListPanel";
            this.CardListPanel.Size = new System.Drawing.Size(96, 46);
            this.CardListPanel.TabIndex = 3;
            // 
            // EntityCardListPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblline);
            this.Controls.Add(this.CardListPanel);
            this.Controls.Add(this.lblEntityName);
            this.Name = "EntityCardListPanel";
            this.Size = new System.Drawing.Size(194, 64);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblEntityName;
        private CardListPanel CardListPanel;
        private System.Windows.Forms.Label lblline;
    }
}
