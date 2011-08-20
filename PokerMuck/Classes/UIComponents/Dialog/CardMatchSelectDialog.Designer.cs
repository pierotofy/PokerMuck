namespace PokerMuck
{
    partial class CardMatchSelectDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardMatchSelectDialog));
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblLine = new System.Windows.Forms.Label();
            this.picCardToMatch = new System.Windows.Forms.PictureBox();
            this.btnViewAll = new System.Windows.Forms.Button();
            this.btnSkip = new System.Windows.Forms.Button();
            this.cardListPanel = new PokerMuck.SelectableCardListPanel();
            ((System.ComponentModel.ISupportInitialize)(this.picCardToMatch)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(8, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(542, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Which card from the list best represents the image on the left?";
            // 
            // lblLine
            // 
            this.lblLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLine.BackColor = System.Drawing.Color.Gray;
            this.lblLine.Location = new System.Drawing.Point(109, 38);
            this.lblLine.Name = "lblLine";
            this.lblLine.Size = new System.Drawing.Size(1, 123);
            this.lblLine.TabIndex = 2;
            // 
            // picCardToMatch
            // 
            this.picCardToMatch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picCardToMatch.Location = new System.Drawing.Point(12, 38);
            this.picCardToMatch.Name = "picCardToMatch";
            this.picCardToMatch.Size = new System.Drawing.Size(91, 123);
            this.picCardToMatch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picCardToMatch.TabIndex = 3;
            this.picCardToMatch.TabStop = false;
            // 
            // btnViewAll
            // 
            this.btnViewAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewAll.Location = new System.Drawing.Point(434, 167);
            this.btnViewAll.Name = "btnViewAll";
            this.btnViewAll.Size = new System.Drawing.Size(55, 23);
            this.btnViewAll.TabIndex = 5;
            this.btnViewAll.Text = "View All";
            this.btnViewAll.UseVisualStyleBackColor = true;
            this.btnViewAll.Click += new System.EventHandler(this.btnViewAll_Click);
            // 
            // btnSkip
            // 
            this.btnSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSkip.Location = new System.Drawing.Point(495, 167);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(55, 23);
            this.btnSkip.TabIndex = 6;
            this.btnSkip.Text = "Skip";
            this.btnSkip.UseVisualStyleBackColor = true;
            this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
            // 
            // cardListPanel
            // 
            this.cardListPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cardListPanel.BackColor = System.Drawing.SystemColors.Control;
            this.cardListPanel.CardListToDisplay = null;
            this.cardListPanel.CardSpacing = 8;
            this.cardListPanel.HighlightColor = System.Drawing.Color.White;
            this.cardListPanel.HighlightTransparency = 150;
            this.cardListPanel.Location = new System.Drawing.Point(116, 38);
            this.cardListPanel.Name = "cardListPanel";
            this.cardListPanel.Size = new System.Drawing.Size(428, 123);
            this.cardListPanel.TabIndex = 4;
            this.cardListPanel.CardSelected += new PokerMuck.SelectableCardListPanel.CardSelectedHandler(this.cardListPanel_CardSelected);
            // 
            // CardMatchSelectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(556, 200);
            this.Controls.Add(this.btnSkip);
            this.Controls.Add(this.btnViewAll);
            this.Controls.Add(this.cardListPanel);
            this.Controls.Add(this.picCardToMatch);
            this.Controls.Add(this.lblLine);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CardMatchSelectDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Training";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.picCardToMatch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblLine;
        private System.Windows.Forms.PictureBox picCardToMatch;
        private SelectableCardListPanel cardListPanel;
        private System.Windows.Forms.Button btnViewAll;
        private System.Windows.Forms.Button btnSkip;
    }
}