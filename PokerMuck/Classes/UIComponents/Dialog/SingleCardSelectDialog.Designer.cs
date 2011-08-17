namespace PokerMuck
{
    partial class SingleCardSelectDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SingleCardSelectDialog));
            this.cardsSelector = new PokerMuck.CardSelector();
            this.SuspendLayout();
            // 
            // cardsSelector
            // 
            this.cardsSelector.Location = new System.Drawing.Point(4, 0);
            this.cardsSelector.Name = "cardsSelector";
            this.cardsSelector.Size = new System.Drawing.Size(121, 392);
            this.cardsSelector.TabIndex = 0;
            this.cardsSelector.CardSelected += new PokerMuck.CardSelector.CardSelectedHandler(this.cardsSelector_CardSelected);
            // 
            // SingleCardSelectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(128, 392);
            this.Controls.Add(this.cardsSelector);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SingleCardSelectDialog";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private CardSelector cardsSelector;
        
    }
}