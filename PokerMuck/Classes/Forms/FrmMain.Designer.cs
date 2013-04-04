namespace PokerMuck
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.consoleTextBox = new ConsoleWidget.ConsoleTextBox();
            this.tabConfigurationPage = new System.Windows.Forms.TabPage();
            this.chkTrainingMode = new System.Windows.Forms.CheckBox();
            this.lblPokerClientTheme = new System.Windows.Forms.Label();
            this.cmbPokerClientTheme = new System.Windows.Forms.ComboBox();
            this.lblPokerClientLanguage = new System.Windows.Forms.Label();
            this.cmbPokerClientLanguage = new System.Windows.Forms.ComboBox();
            this.lblPokerClient = new System.Windows.Forms.Label();
            this.cmbPokerClient = new System.Windows.Forms.ComboBox();
            this.btnChangeHandHistory = new System.Windows.Forms.Button();
            this.txtHandHistoryDirectory = new System.Windows.Forms.TextBox();
            this.tabTools = new System.Windows.Forms.TabPage();
            this.btnTakeWindowScreenshot = new System.Windows.Forms.Button();
            this.tabAboutPage = new System.Windows.Forms.TabPage();
            this.btnSendTweet = new System.Windows.Forms.Button();
            this.pictureEagle = new System.Windows.Forms.PictureBox();
            this.btnJoinFacebookPage = new System.Windows.Forms.Button();
            this.picturePokerMuckIcon = new System.Windows.Forms.PictureBox();
            this.btnDonatePaypal = new System.Windows.Forms.Button();
            this.lblDonate = new System.Windows.Forms.Label();
            this.lblProgramName = new System.Windows.Forms.Label();
            this.txtBitcoinAddr = new System.Windows.Forms.TextBox();
            this.btnDonateBitcoin = new System.Windows.Forms.Button();
            this.lblPokerMuckLink = new System.Windows.Forms.LinkLabel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnCheckForUpdates = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabDebug.SuspendLayout();
            this.tabConfigurationPage.SuspendLayout();
            this.tabTools.SuspendLayout();
            this.tabAboutPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEagle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePokerMuckIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabDebug);
            this.tabControl.Controls.Add(this.tabConfigurationPage);
            this.tabControl.Controls.Add(this.tabTools);
            this.tabControl.Controls.Add(this.tabAboutPage);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(328, 380);
            this.tabControl.TabIndex = 3;
            // 
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.consoleTextBox);
            this.tabDebug.Location = new System.Drawing.Point(4, 22);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Padding = new System.Windows.Forms.Padding(3);
            this.tabDebug.Size = new System.Drawing.Size(320, 354);
            this.tabDebug.TabIndex = 0;
            this.tabDebug.Text = "Debug";
            this.tabDebug.UseVisualStyleBackColor = true;
            // 
            // consoleTextBox
            // 
            this.consoleTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consoleTextBox.Location = new System.Drawing.Point(3, 3);
            this.consoleTextBox.Name = "consoleTextBox";
            this.consoleTextBox.Size = new System.Drawing.Size(314, 348);
            this.consoleTextBox.TabIndex = 6;
            // 
            // tabConfigurationPage
            // 
            this.tabConfigurationPage.Controls.Add(this.chkTrainingMode);
            this.tabConfigurationPage.Controls.Add(this.lblPokerClientTheme);
            this.tabConfigurationPage.Controls.Add(this.cmbPokerClientTheme);
            this.tabConfigurationPage.Controls.Add(this.lblPokerClientLanguage);
            this.tabConfigurationPage.Controls.Add(this.cmbPokerClientLanguage);
            this.tabConfigurationPage.Controls.Add(this.lblPokerClient);
            this.tabConfigurationPage.Controls.Add(this.cmbPokerClient);
            this.tabConfigurationPage.Controls.Add(this.btnChangeHandHistory);
            this.tabConfigurationPage.Controls.Add(this.txtHandHistoryDirectory);
            this.tabConfigurationPage.Location = new System.Drawing.Point(4, 22);
            this.tabConfigurationPage.Name = "tabConfigurationPage";
            this.tabConfigurationPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigurationPage.Size = new System.Drawing.Size(320, 354);
            this.tabConfigurationPage.TabIndex = 1;
            this.tabConfigurationPage.Text = "Configuration";
            this.tabConfigurationPage.UseVisualStyleBackColor = true;
            // 
            // chkTrainingMode
            // 
            this.chkTrainingMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkTrainingMode.Location = new System.Drawing.Point(11, 188);
            this.chkTrainingMode.Name = "chkTrainingMode";
            this.chkTrainingMode.Size = new System.Drawing.Size(301, 24);
            this.chkTrainingMode.TabIndex = 11;
            this.chkTrainingMode.Text = "Image Recognition Training Mode Enabled";
            this.chkTrainingMode.UseVisualStyleBackColor = true;
            this.chkTrainingMode.CheckedChanged += new System.EventHandler(this.chkTrainingMode_CheckedChanged);
            // 
            // lblPokerClientTheme
            // 
            this.lblPokerClientTheme.Location = new System.Drawing.Point(8, 145);
            this.lblPokerClientTheme.Name = "lblPokerClientTheme";
            this.lblPokerClientTheme.Size = new System.Drawing.Size(46, 19);
            this.lblPokerClientTheme.TabIndex = 10;
            this.lblPokerClientTheme.Text = "Theme:";
            this.lblPokerClientTheme.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbPokerClientTheme
            // 
            this.cmbPokerClientTheme.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPokerClientTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPokerClientTheme.FormattingEnabled = true;
            this.cmbPokerClientTheme.Location = new System.Drawing.Point(60, 143);
            this.cmbPokerClientTheme.Name = "cmbPokerClientTheme";
            this.cmbPokerClientTheme.Size = new System.Drawing.Size(252, 21);
            this.cmbPokerClientTheme.TabIndex = 9;
            this.cmbPokerClientTheme.SelectionChangeCommitted += new System.EventHandler(this.cmbPokerClientTheme_SelectionChangeCommitted);
            // 
            // lblPokerClientLanguage
            // 
            this.lblPokerClientLanguage.Location = new System.Drawing.Point(8, 107);
            this.lblPokerClientLanguage.Name = "lblPokerClientLanguage";
            this.lblPokerClientLanguage.Size = new System.Drawing.Size(71, 31);
            this.lblPokerClientLanguage.TabIndex = 8;
            this.lblPokerClientLanguage.Text = "Hand History Language:";
            this.lblPokerClientLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbPokerClientLanguage
            // 
            this.cmbPokerClientLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPokerClientLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPokerClientLanguage.FormattingEnabled = true;
            this.cmbPokerClientLanguage.Location = new System.Drawing.Point(82, 113);
            this.cmbPokerClientLanguage.Name = "cmbPokerClientLanguage";
            this.cmbPokerClientLanguage.Size = new System.Drawing.Size(230, 21);
            this.cmbPokerClientLanguage.TabIndex = 7;
            this.cmbPokerClientLanguage.SelectionChangeCommitted += new System.EventHandler(this.cmbPokerClientLanguage_SelectionChangeCommitted);
            // 
            // lblPokerClient
            // 
            this.lblPokerClient.Location = new System.Drawing.Point(8, 80);
            this.lblPokerClient.Name = "lblPokerClient";
            this.lblPokerClient.Size = new System.Drawing.Size(38, 21);
            this.lblPokerClient.TabIndex = 6;
            this.lblPokerClient.Text = "Client:";
            this.lblPokerClient.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbPokerClient
            // 
            this.cmbPokerClient.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPokerClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPokerClient.FormattingEnabled = true;
            this.cmbPokerClient.Location = new System.Drawing.Point(49, 81);
            this.cmbPokerClient.Name = "cmbPokerClient";
            this.cmbPokerClient.Size = new System.Drawing.Size(263, 21);
            this.cmbPokerClient.TabIndex = 5;
            this.cmbPokerClient.SelectionChangeCommitted += new System.EventHandler(this.cmbPokerClient_SelectionChangeCommitted);
            // 
            // btnChangeHandHistory
            // 
            this.btnChangeHandHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeHandHistory.Location = new System.Drawing.Point(8, 35);
            this.btnChangeHandHistory.Name = "btnChangeHandHistory";
            this.btnChangeHandHistory.Size = new System.Drawing.Size(304, 23);
            this.btnChangeHandHistory.TabIndex = 2;
            this.btnChangeHandHistory.Text = "Change Hand History Directory";
            this.btnChangeHandHistory.UseVisualStyleBackColor = true;
            this.btnChangeHandHistory.Click += new System.EventHandler(this.btnChangeHandHistory_Click);
            // 
            // txtHandHistoryDirectory
            // 
            this.txtHandHistoryDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHandHistoryDirectory.Location = new System.Drawing.Point(8, 9);
            this.txtHandHistoryDirectory.Name = "txtHandHistoryDirectory";
            this.txtHandHistoryDirectory.ReadOnly = true;
            this.txtHandHistoryDirectory.Size = new System.Drawing.Size(304, 20);
            this.txtHandHistoryDirectory.TabIndex = 1;
            // 
            // tabTools
            // 
            this.tabTools.Controls.Add(this.btnCheckForUpdates);
            this.tabTools.Controls.Add(this.btnTakeWindowScreenshot);
            this.tabTools.Location = new System.Drawing.Point(4, 22);
            this.tabTools.Name = "tabTools";
            this.tabTools.Size = new System.Drawing.Size(320, 354);
            this.tabTools.TabIndex = 3;
            this.tabTools.Text = "Tools";
            this.tabTools.UseVisualStyleBackColor = true;
            // 
            // btnTakeWindowScreenshot
            // 
            this.btnTakeWindowScreenshot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTakeWindowScreenshot.Location = new System.Drawing.Point(8, 8);
            this.btnTakeWindowScreenshot.Name = "btnTakeWindowScreenshot";
            this.btnTakeWindowScreenshot.Size = new System.Drawing.Size(304, 26);
            this.btnTakeWindowScreenshot.TabIndex = 0;
            this.btnTakeWindowScreenshot.Text = "Take client window screenshot";
            this.btnTakeWindowScreenshot.UseVisualStyleBackColor = true;
            this.btnTakeWindowScreenshot.Click += new System.EventHandler(this.btnTakeWindowScreenshot_Click);
            // 
            // tabAboutPage
            // 
            this.tabAboutPage.Controls.Add(this.btnSendTweet);
            this.tabAboutPage.Controls.Add(this.pictureEagle);
            this.tabAboutPage.Controls.Add(this.btnJoinFacebookPage);
            this.tabAboutPage.Controls.Add(this.picturePokerMuckIcon);
            this.tabAboutPage.Controls.Add(this.btnDonatePaypal);
            this.tabAboutPage.Controls.Add(this.lblDonate);
            this.tabAboutPage.Controls.Add(this.lblProgramName);
            this.tabAboutPage.Controls.Add(this.txtBitcoinAddr);
            this.tabAboutPage.Controls.Add(this.btnDonateBitcoin);
            this.tabAboutPage.Controls.Add(this.lblPokerMuckLink);
            this.tabAboutPage.Location = new System.Drawing.Point(4, 22);
            this.tabAboutPage.Name = "tabAboutPage";
            this.tabAboutPage.Size = new System.Drawing.Size(320, 354);
            this.tabAboutPage.TabIndex = 2;
            this.tabAboutPage.Text = "About";
            this.tabAboutPage.UseVisualStyleBackColor = true;
            // 
            // btnSendTweet
            // 
            this.btnSendTweet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendTweet.Font = new System.Drawing.Font("Arial Black", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendTweet.Location = new System.Drawing.Point(11, 267);
            this.btnSendTweet.Name = "btnSendTweet";
            this.btnSendTweet.Size = new System.Drawing.Size(301, 30);
            this.btnSendTweet.TabIndex = 7;
            this.btnSendTweet.Text = "4. Send a Tweet about us";
            this.btnSendTweet.UseVisualStyleBackColor = true;
            this.btnSendTweet.Click += new System.EventHandler(this.btnSendTweet_Click);
            // 
            // pictureEagle
            // 
            this.pictureEagle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureEagle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureEagle.Image = global::PokerMuck.Properties.Resources.Eagle;
            this.pictureEagle.Location = new System.Drawing.Point(167, 34);
            this.pictureEagle.Name = "pictureEagle";
            this.pictureEagle.Size = new System.Drawing.Size(53, 48);
            this.pictureEagle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureEagle.TabIndex = 0;
            this.pictureEagle.TabStop = false;
            // 
            // btnJoinFacebookPage
            // 
            this.btnJoinFacebookPage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnJoinFacebookPage.Font = new System.Drawing.Font("Arial Black", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJoinFacebookPage.Location = new System.Drawing.Point(11, 231);
            this.btnJoinFacebookPage.Name = "btnJoinFacebookPage";
            this.btnJoinFacebookPage.Size = new System.Drawing.Size(301, 30);
            this.btnJoinFacebookPage.TabIndex = 6;
            this.btnJoinFacebookPage.Text = "3. Join our page on Facebook";
            this.btnJoinFacebookPage.UseVisualStyleBackColor = true;
            this.btnJoinFacebookPage.Click += new System.EventHandler(this.btnJoinFacebookPage_Click);
            // 
            // picturePokerMuckIcon
            // 
            this.picturePokerMuckIcon.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.picturePokerMuckIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picturePokerMuckIcon.Image = global::PokerMuck.Properties.Resources.PokerMuckLogo;
            this.picturePokerMuckIcon.Location = new System.Drawing.Point(103, 34);
            this.picturePokerMuckIcon.Name = "picturePokerMuckIcon";
            this.picturePokerMuckIcon.Size = new System.Drawing.Size(48, 48);
            this.picturePokerMuckIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picturePokerMuckIcon.TabIndex = 5;
            this.picturePokerMuckIcon.TabStop = false;
            // 
            // btnDonatePaypal
            // 
            this.btnDonatePaypal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDonatePaypal.Font = new System.Drawing.Font("Arial Black", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDonatePaypal.Location = new System.Drawing.Point(11, 159);
            this.btnDonatePaypal.Name = "btnDonatePaypal";
            this.btnDonatePaypal.Size = new System.Drawing.Size(301, 30);
            this.btnDonatePaypal.TabIndex = 4;
            this.btnDonatePaypal.Text = "1. Donate (via Paypal)";
            this.btnDonatePaypal.UseVisualStyleBackColor = true;
            this.btnDonatePaypal.Click += new System.EventHandler(this.btnDonate_Click);
            // 
            // lblDonate
            // 
            this.lblDonate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDonate.Location = new System.Drawing.Point(3, 85);
            this.lblDonate.Name = "lblDonate";
            this.lblDonate.Size = new System.Drawing.Size(309, 71);
            this.lblDonate.TabIndex = 3;
            this.lblDonate.Text = "This program is free software. If you enjoyed using this program, please choose o" +
    "ne of these options and contribute to the growth of the project. Your help is ne" +
    "cessary to keep the program up to date.";
            this.lblDonate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblProgramName
            // 
            this.lblProgramName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgramName.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgramName.Location = new System.Drawing.Point(8, 8);
            this.lblProgramName.Name = "lblProgramName";
            this.lblProgramName.Size = new System.Drawing.Size(304, 20);
            this.lblProgramName.TabIndex = 1;
            this.lblProgramName.Text = "PokerMuck";
            this.lblProgramName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtBitcoinAddr
            // 
            this.txtBitcoinAddr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBitcoinAddr.Location = new System.Drawing.Point(11, 200);
            this.txtBitcoinAddr.Name = "txtBitcoinAddr";
            this.txtBitcoinAddr.ReadOnly = true;
            this.txtBitcoinAddr.Size = new System.Drawing.Size(301, 20);
            this.txtBitcoinAddr.TabIndex = 9;
            this.txtBitcoinAddr.TabStop = false;
            this.txtBitcoinAddr.Text = "17A9QgZzpNwDvB77SMiwULBbLHyecDPXpg";
            this.txtBitcoinAddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtBitcoinAddr.Visible = false;
            // 
            // btnDonateBitcoin
            // 
            this.btnDonateBitcoin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDonateBitcoin.Font = new System.Drawing.Font("Arial Black", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDonateBitcoin.Location = new System.Drawing.Point(11, 195);
            this.btnDonateBitcoin.Name = "btnDonateBitcoin";
            this.btnDonateBitcoin.Size = new System.Drawing.Size(301, 30);
            this.btnDonateBitcoin.TabIndex = 8;
            this.btnDonateBitcoin.Text = "2. Donate (via Bitcoin)";
            this.btnDonateBitcoin.UseVisualStyleBackColor = true;
            this.btnDonateBitcoin.Click += new System.EventHandler(this.btnDonateBitcoin_Click);
            // 
            // lblPokerMuckLink
            // 
            this.lblPokerMuckLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPokerMuckLink.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPokerMuckLink.Location = new System.Drawing.Point(68, 324);
            this.lblPokerMuckLink.Name = "lblPokerMuckLink";
            this.lblPokerMuckLink.Size = new System.Drawing.Size(206, 24);
            this.lblPokerMuckLink.TabIndex = 2;
            this.lblPokerMuckLink.TabStop = true;
            this.lblPokerMuckLink.Text = "http://www.pokermuck.com";
            this.lblPokerMuckLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPokerMuckLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblPokerMuckLink_LinkClicked);
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.White;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(0, 379);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(328, 39);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Status Label";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCheckForUpdates
            // 
            this.btnCheckForUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckForUpdates.Location = new System.Drawing.Point(8, 40);
            this.btnCheckForUpdates.Name = "btnCheckForUpdates";
            this.btnCheckForUpdates.Size = new System.Drawing.Size(304, 26);
            this.btnCheckForUpdates.TabIndex = 1;
            this.btnCheckForUpdates.Text = "Check for updates";
            this.btnCheckForUpdates.UseVisualStyleBackColor = true;
            this.btnCheckForUpdates.Click += new System.EventHandler(this.btnCheckForUpdates_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(328, 418);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PokerMuck";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResizeEnd += new System.EventHandler(this.FrmMain_ResizeEnd);
            this.LocationChanged += new System.EventHandler(this.FrmMain_LocationChanged);
            this.tabControl.ResumeLayout(false);
            this.tabDebug.ResumeLayout(false);
            this.tabConfigurationPage.ResumeLayout(false);
            this.tabConfigurationPage.PerformLayout();
            this.tabTools.ResumeLayout(false);
            this.tabAboutPage.ResumeLayout(false);
            this.tabAboutPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEagle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePokerMuckIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabConfigurationPage;
        private System.Windows.Forms.Button btnChangeHandHistory;
        private System.Windows.Forms.TextBox txtHandHistoryDirectory;
        private System.Windows.Forms.TabPage tabAboutPage;
        private System.Windows.Forms.PictureBox pictureEagle;
        private System.Windows.Forms.Label lblPokerClient;
        private System.Windows.Forms.ComboBox cmbPokerClient;
        private System.Windows.Forms.Label lblPokerClientLanguage;
        private System.Windows.Forms.ComboBox cmbPokerClientLanguage;
        private System.Windows.Forms.Label lblProgramName;
        private System.Windows.Forms.LinkLabel lblPokerMuckLink;
        private System.Windows.Forms.Label lblDonate;
        private System.Windows.Forms.Button btnDonatePaypal;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblPokerClientTheme;
        private System.Windows.Forms.ComboBox cmbPokerClientTheme;
        private System.Windows.Forms.CheckBox chkTrainingMode;
        private System.Windows.Forms.TabPage tabDebug;
        private ConsoleWidget.ConsoleTextBox consoleTextBox;
        private System.Windows.Forms.PictureBox picturePokerMuckIcon;
        private System.Windows.Forms.Button btnJoinFacebookPage;
        private System.Windows.Forms.Button btnSendTweet;
        private System.Windows.Forms.TabPage tabTools;
        private System.Windows.Forms.Button btnTakeWindowScreenshot;
        private System.Windows.Forms.Button btnDonateBitcoin;
        private System.Windows.Forms.TextBox txtBitcoinAddr;
        private System.Windows.Forms.Button btnCheckForUpdates;


    }
}

