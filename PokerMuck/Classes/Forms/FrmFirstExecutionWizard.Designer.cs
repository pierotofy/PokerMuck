namespace PokerMuck
{
    partial class FrmFirstExecutionWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFirstExecutionWizard));
            this.btnNext = new System.Windows.Forms.Button();
            this.lblPageNum = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelPageOne = new System.Windows.Forms.Panel();
            this.lblPageOneText = new System.Windows.Forms.Label();
            this.panelPageThree = new System.Windows.Forms.Panel();
            this.pictureHandHistory = new System.Windows.Forms.PictureBox();
            this.lblHandHistoryLanguageText = new System.Windows.Forms.Label();
            this.lblPageThreeText = new System.Windows.Forms.Label();
            this.panelPageFour = new System.Windows.Forms.Panel();
            this.txtHandHistoriesDirectory = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblPageFourText = new System.Windows.Forms.Label();
            this.panelPageFive = new System.Windows.Forms.Panel();
            this.lblPageFiveText = new System.Windows.Forms.Label();
            this.panelPageTwo = new System.Windows.Forms.Panel();
            this.lblPokerClientTheme = new System.Windows.Forms.Label();
            this.cmbPokerClientTheme = new System.Windows.Forms.ComboBox();
            this.lblPokerClientLanguage = new System.Windows.Forms.Label();
            this.cmbPokerClientLanguage = new System.Windows.Forms.ComboBox();
            this.lblPokerClient = new System.Windows.Forms.Label();
            this.cmbPokerClient = new System.Windows.Forms.ComboBox();
            this.lblPageTwoText = new System.Windows.Forms.Label();
            this.panelPageOne.SuspendLayout();
            this.panelPageThree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureHandHistory)).BeginInit();
            this.panelPageFour.SuspendLayout();
            this.panelPageFive.SuspendLayout();
            this.panelPageTwo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(958, 675);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(82, 32);
            this.btnNext.TabIndex = 0;
            this.btnNext.Text = "&Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // lblPageNum
            // 
            this.lblPageNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPageNum.AutoSize = true;
            this.lblPageNum.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageNum.Location = new System.Drawing.Point(7, 685);
            this.lblPageNum.Name = "lblPageNum";
            this.lblPageNum.Size = new System.Drawing.Size(83, 16);
            this.lblPageNum.TabIndex = 2;
            this.lblPageNum.Text = "Step 1 of 3";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(6, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(147, 22);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "Getting started";
            // 
            // panelPageOne
            // 
            this.panelPageOne.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelPageOne.Controls.Add(this.lblPageOneText);
            this.panelPageOne.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.panelPageOne.Location = new System.Drawing.Point(7, 33);
            this.panelPageOne.Name = "panelPageOne";
            this.panelPageOne.Size = new System.Drawing.Size(415, 191);
            this.panelPageOne.TabIndex = 4;
            // 
            // lblPageOneText
            // 
            this.lblPageOneText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPageOneText.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageOneText.Location = new System.Drawing.Point(0, 0);
            this.lblPageOneText.Margin = new System.Windows.Forms.Padding(0);
            this.lblPageOneText.Name = "lblPageOneText";
            this.lblPageOneText.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.lblPageOneText.Size = new System.Drawing.Size(415, 191);
            this.lblPageOneText.TabIndex = 0;
            this.lblPageOneText.Text = "Welcome to PokerMuck. Please make sure you follow these quick steps in order to g" +
    "et the program to work.";
            // 
            // panelPageThree
            // 
            this.panelPageThree.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelPageThree.Controls.Add(this.pictureHandHistory);
            this.panelPageThree.Controls.Add(this.lblHandHistoryLanguageText);
            this.panelPageThree.Controls.Add(this.lblPageThreeText);
            this.panelPageThree.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.panelPageThree.Location = new System.Drawing.Point(49, 247);
            this.panelPageThree.Name = "panelPageThree";
            this.panelPageThree.Size = new System.Drawing.Size(487, 209);
            this.panelPageThree.TabIndex = 5;
            this.panelPageThree.Visible = false;
            // 
            // pictureHandHistory
            // 
            this.pictureHandHistory.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureHandHistory.Image = global::PokerMuck.Properties.Resources.HandHistorySettings;
            this.pictureHandHistory.Location = new System.Drawing.Point(6, 109);
            this.pictureHandHistory.Name = "pictureHandHistory";
            this.pictureHandHistory.Size = new System.Drawing.Size(468, 326);
            this.pictureHandHistory.TabIndex = 2;
            this.pictureHandHistory.TabStop = false;
            // 
            // lblHandHistoryLanguageText
            // 
            this.lblHandHistoryLanguageText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHandHistoryLanguageText.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHandHistoryLanguageText.Location = new System.Drawing.Point(2, 69);
            this.lblHandHistoryLanguageText.Name = "lblHandHistoryLanguageText";
            this.lblHandHistoryLanguageText.Size = new System.Drawing.Size(481, 37);
            this.lblHandHistoryLanguageText.TabIndex = 1;
            this.lblHandHistoryLanguageText.Text = "Make sure you select <lang> as the language to record the hand histories!";
            // 
            // lblPageThreeText
            // 
            this.lblPageThreeText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPageThreeText.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageThreeText.Location = new System.Drawing.Point(0, 0);
            this.lblPageThreeText.Margin = new System.Windows.Forms.Padding(0);
            this.lblPageThreeText.Name = "lblPageThreeText";
            this.lblPageThreeText.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.lblPageThreeText.Size = new System.Drawing.Size(487, 72);
            this.lblPageThreeText.TabIndex = 0;
            this.lblPageThreeText.Text = resources.GetString("lblPageThreeText.Text");
            // 
            // panelPageFour
            // 
            this.panelPageFour.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelPageFour.Controls.Add(this.txtHandHistoriesDirectory);
            this.panelPageFour.Controls.Add(this.btnBrowse);
            this.panelPageFour.Controls.Add(this.lblPageFourText);
            this.panelPageFour.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.panelPageFour.Location = new System.Drawing.Point(566, 247);
            this.panelPageFour.Name = "panelPageFour";
            this.panelPageFour.Size = new System.Drawing.Size(449, 209);
            this.panelPageFour.TabIndex = 6;
            this.panelPageFour.Visible = false;
            // 
            // txtHandHistoriesDirectory
            // 
            this.txtHandHistoriesDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHandHistoriesDirectory.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txtHandHistoriesDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHandHistoriesDirectory.Location = new System.Drawing.Point(5, 111);
            this.txtHandHistoriesDirectory.Name = "txtHandHistoriesDirectory";
            this.txtHandHistoriesDirectory.ReadOnly = true;
            this.txtHandHistoriesDirectory.Size = new System.Drawing.Size(300, 26);
            this.txtHandHistoriesDirectory.TabIndex = 2;
            this.txtHandHistoriesDirectory.Text = "C:\\";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(309, 108);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(137, 33);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "&Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblPageFourText
            // 
            this.lblPageFourText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPageFourText.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageFourText.Location = new System.Drawing.Point(0, 0);
            this.lblPageFourText.Margin = new System.Windows.Forms.Padding(0);
            this.lblPageFourText.Name = "lblPageFourText";
            this.lblPageFourText.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.lblPageFourText.Size = new System.Drawing.Size(449, 72);
            this.lblPageFourText.TabIndex = 0;
            this.lblPageFourText.Text = resources.GetString("lblPageFourText.Text");
            // 
            // panelPageFive
            // 
            this.panelPageFive.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelPageFive.Controls.Add(this.lblPageFiveText);
            this.panelPageFive.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.panelPageFive.Location = new System.Drawing.Point(52, 501);
            this.panelPageFive.Name = "panelPageFive";
            this.panelPageFive.Size = new System.Drawing.Size(487, 161);
            this.panelPageFive.TabIndex = 7;
            this.panelPageFive.Visible = false;
            // 
            // lblPageFiveText
            // 
            this.lblPageFiveText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPageFiveText.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageFiveText.Location = new System.Drawing.Point(0, 0);
            this.lblPageFiveText.Margin = new System.Windows.Forms.Padding(0);
            this.lblPageFiveText.Name = "lblPageFiveText";
            this.lblPageFiveText.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.lblPageFiveText.Size = new System.Drawing.Size(487, 72);
            this.lblPageFiveText.TabIndex = 0;
            this.lblPageFiveText.Text = "Your configuration has been saved! Enjoy.";
            // 
            // panelPageTwo
            // 
            this.panelPageTwo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelPageTwo.Controls.Add(this.lblPokerClientTheme);
            this.panelPageTwo.Controls.Add(this.cmbPokerClientTheme);
            this.panelPageTwo.Controls.Add(this.lblPokerClientLanguage);
            this.panelPageTwo.Controls.Add(this.cmbPokerClientLanguage);
            this.panelPageTwo.Controls.Add(this.lblPokerClient);
            this.panelPageTwo.Controls.Add(this.cmbPokerClient);
            this.panelPageTwo.Controls.Add(this.lblPageTwoText);
            this.panelPageTwo.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.panelPageTwo.Location = new System.Drawing.Point(528, 16);
            this.panelPageTwo.Name = "panelPageTwo";
            this.panelPageTwo.Size = new System.Drawing.Size(487, 209);
            this.panelPageTwo.TabIndex = 8;
            this.panelPageTwo.Visible = false;
            // 
            // lblPokerClientTheme
            // 
            this.lblPokerClientTheme.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.lblPokerClientTheme.Location = new System.Drawing.Point(79, 180);
            this.lblPokerClientTheme.Name = "lblPokerClientTheme";
            this.lblPokerClientTheme.Size = new System.Drawing.Size(63, 31);
            this.lblPokerClientTheme.TabIndex = 14;
            this.lblPokerClientTheme.Text = "Theme:";
            this.lblPokerClientTheme.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbPokerClientTheme
            // 
            this.cmbPokerClientTheme.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPokerClientTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPokerClientTheme.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.cmbPokerClientTheme.FormattingEnabled = true;
            this.cmbPokerClientTheme.Location = new System.Drawing.Point(142, 185);
            this.cmbPokerClientTheme.Name = "cmbPokerClientTheme";
            this.cmbPokerClientTheme.Size = new System.Drawing.Size(280, 24);
            this.cmbPokerClientTheme.TabIndex = 13;
            this.cmbPokerClientTheme.SelectionChangeCommitted += new System.EventHandler(this.cmbPokerClientTheme_SelectionChangeCommitted);
            // 
            // lblPokerClientLanguage
            // 
            this.lblPokerClientLanguage.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.lblPokerClientLanguage.Location = new System.Drawing.Point(79, 144);
            this.lblPokerClientLanguage.Name = "lblPokerClientLanguage";
            this.lblPokerClientLanguage.Size = new System.Drawing.Size(166, 31);
            this.lblPokerClientLanguage.TabIndex = 12;
            this.lblPokerClientLanguage.Text = "Hand History Language:";
            this.lblPokerClientLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbPokerClientLanguage
            // 
            this.cmbPokerClientLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPokerClientLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPokerClientLanguage.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.cmbPokerClientLanguage.FormattingEnabled = true;
            this.cmbPokerClientLanguage.Location = new System.Drawing.Point(245, 149);
            this.cmbPokerClientLanguage.Name = "cmbPokerClientLanguage";
            this.cmbPokerClientLanguage.Size = new System.Drawing.Size(177, 24);
            this.cmbPokerClientLanguage.TabIndex = 11;
            this.cmbPokerClientLanguage.SelectionChangeCommitted += new System.EventHandler(this.cmbPokerClientLanguage_SelectionChangeCommitted);
            // 
            // lblPokerClient
            // 
            this.lblPokerClient.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.lblPokerClient.Location = new System.Drawing.Point(79, 114);
            this.lblPokerClient.Name = "lblPokerClient";
            this.lblPokerClient.Size = new System.Drawing.Size(52, 21);
            this.lblPokerClient.TabIndex = 10;
            this.lblPokerClient.Text = "Client:";
            this.lblPokerClient.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbPokerClient
            // 
            this.cmbPokerClient.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPokerClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPokerClient.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.cmbPokerClient.FormattingEnabled = true;
            this.cmbPokerClient.Location = new System.Drawing.Point(132, 113);
            this.cmbPokerClient.Name = "cmbPokerClient";
            this.cmbPokerClient.Size = new System.Drawing.Size(290, 24);
            this.cmbPokerClient.TabIndex = 9;
            this.cmbPokerClient.SelectionChangeCommitted += new System.EventHandler(this.cmbPokerClient_SelectionChangeCommitted);
            // 
            // lblPageTwoText
            // 
            this.lblPageTwoText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPageTwoText.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageTwoText.Location = new System.Drawing.Point(0, 0);
            this.lblPageTwoText.Margin = new System.Windows.Forms.Padding(0);
            this.lblPageTwoText.Name = "lblPageTwoText";
            this.lblPageTwoText.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.lblPageTwoText.Size = new System.Drawing.Size(487, 89);
            this.lblPageTwoText.TabIndex = 0;
            this.lblPageTwoText.Text = resources.GetString("lblPageTwoText.Text");
            // 
            // FrmFirstExecutionWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 719);
            this.Controls.Add(this.panelPageTwo);
            this.Controls.Add(this.panelPageOne);
            this.Controls.Add(this.panelPageFour);
            this.Controls.Add(this.panelPageFive);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblPageNum);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.panelPageThree);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmFirstExecutionWizard";
            this.Text = "Welcome to PokerMuck";
            this.panelPageOne.ResumeLayout(false);
            this.panelPageThree.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureHandHistory)).EndInit();
            this.panelPageFour.ResumeLayout(false);
            this.panelPageFour.PerformLayout();
            this.panelPageFive.ResumeLayout(false);
            this.panelPageTwo.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label lblPageNum;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelPageOne;
        private System.Windows.Forms.Label lblPageOneText;
        private System.Windows.Forms.Panel panelPageThree;
        private System.Windows.Forms.Label lblPageThreeText;
        private System.Windows.Forms.Panel panelPageFour;
        private System.Windows.Forms.Label lblPageFourText;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtHandHistoriesDirectory;
        private System.Windows.Forms.Panel panelPageFive;
        private System.Windows.Forms.Label lblPageFiveText;
        private System.Windows.Forms.Panel panelPageTwo;
        private System.Windows.Forms.Label lblPageTwoText;
        private System.Windows.Forms.Label lblPokerClientLanguage;
        private System.Windows.Forms.ComboBox cmbPokerClientLanguage;
        private System.Windows.Forms.Label lblPokerClient;
        private System.Windows.Forms.ComboBox cmbPokerClient;
        private System.Windows.Forms.Label lblHandHistoryLanguageText;
        private System.Windows.Forms.PictureBox pictureHandHistory;
        private System.Windows.Forms.Label lblPokerClientTheme;
        private System.Windows.Forms.ComboBox cmbPokerClientTheme;
    }
}