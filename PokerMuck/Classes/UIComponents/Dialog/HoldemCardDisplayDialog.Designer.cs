namespace PokerMuck
{
    partial class HoldemCardDisplayDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HoldemCardDisplayDialog));
            this.cardsSelector = new PokerMuck.HoldemCardsSelector();
            this.SuspendLayout();
            // 
            // cardsSelector
            // 
            this.cardsSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardsSelector.Location = new System.Drawing.Point(0, 0);
            this.cardsSelector.Name = "cardsSelector";
            this.cardsSelector.Size = new System.Drawing.Size(391, 391);
            this.cardsSelector.TabIndex = 0;
            // 
            // HoldemCardDisplayDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(391, 391);
            this.Controls.Add(this.cardsSelector);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HoldemCardDisplayDialog";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private HoldemCardsSelector cardsSelector;
        
    }
}