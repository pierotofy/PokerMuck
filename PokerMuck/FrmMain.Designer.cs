﻿namespace PokerMuck
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
            this.tabHandsPage = new System.Windows.Forms.TabPage();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tabConfigurationPage = new System.Windows.Forms.TabPage();
            this.lblPokerClientLanguage = new System.Windows.Forms.Label();
            this.cmbPokerClientLanguage = new System.Windows.Forms.ComboBox();
            this.lblPokerClient = new System.Windows.Forms.Label();
            this.cmbPokerClient = new System.Windows.Forms.ComboBox();
            this.txtUserId = new System.Windows.Forms.TextBox();
            this.lblUserId = new System.Windows.Forms.Label();
            this.btnChangeHandHistory = new System.Windows.Forms.Button();
            this.txtHandHistoryDirectory = new System.Windows.Forms.TextBox();
            this.tabAboutPage = new System.Windows.Forms.TabPage();
            this.pictureEagle = new System.Windows.Forms.PictureBox();
            this.lblProgramName = new System.Windows.Forms.Label();
            this.lblPieroTofyLink = new System.Windows.Forms.LinkLabel();
            this.lblDonate = new System.Windows.Forms.Label();
            this.btnDonate = new System.Windows.Forms.Button();
            this.entityHandsContainer = new PokerMuck.ControlListContainer();
            this.tabControl.SuspendLayout();
            this.tabHandsPage.SuspendLayout();
            this.tabConfigurationPage.SuspendLayout();
            this.tabAboutPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEagle)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabHandsPage);
            this.tabControl.Controls.Add(this.tabConfigurationPage);
            this.tabControl.Controls.Add(this.tabAboutPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(195, 323);
            this.tabControl.TabIndex = 3;
            // 
            // tabHandsPage
            // 
            this.tabHandsPage.Controls.Add(this.entityHandsContainer);
            this.tabHandsPage.Controls.Add(this.lblStatus);
            this.tabHandsPage.Location = new System.Drawing.Point(4, 22);
            this.tabHandsPage.Name = "tabHandsPage";
            this.tabHandsPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabHandsPage.Size = new System.Drawing.Size(187, 297);
            this.tabHandsPage.TabIndex = 0;
            this.tabHandsPage.Text = "Hands";
            this.tabHandsPage.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(3, 257);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(181, 37);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Status Label";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabConfigurationPage
            // 
            this.tabConfigurationPage.Controls.Add(this.lblPokerClientLanguage);
            this.tabConfigurationPage.Controls.Add(this.cmbPokerClientLanguage);
            this.tabConfigurationPage.Controls.Add(this.lblPokerClient);
            this.tabConfigurationPage.Controls.Add(this.cmbPokerClient);
            this.tabConfigurationPage.Controls.Add(this.txtUserId);
            this.tabConfigurationPage.Controls.Add(this.lblUserId);
            this.tabConfigurationPage.Controls.Add(this.btnChangeHandHistory);
            this.tabConfigurationPage.Controls.Add(this.txtHandHistoryDirectory);
            this.tabConfigurationPage.Location = new System.Drawing.Point(4, 22);
            this.tabConfigurationPage.Name = "tabConfigurationPage";
            this.tabConfigurationPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigurationPage.Size = new System.Drawing.Size(187, 297);
            this.tabConfigurationPage.TabIndex = 1;
            this.tabConfigurationPage.Text = "Configuration";
            this.tabConfigurationPage.UseVisualStyleBackColor = true;
            // 
            // lblPokerClientLanguage
            // 
            this.lblPokerClientLanguage.Location = new System.Drawing.Point(8, 150);
            this.lblPokerClientLanguage.Name = "lblPokerClientLanguage";
            this.lblPokerClientLanguage.Size = new System.Drawing.Size(59, 21);
            this.lblPokerClientLanguage.TabIndex = 8;
            this.lblPokerClientLanguage.Text = "Language:";
            this.lblPokerClientLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbPokerClientLanguage
            // 
            this.cmbPokerClientLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPokerClientLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPokerClientLanguage.FormattingEnabled = true;
            this.cmbPokerClientLanguage.Location = new System.Drawing.Point(68, 151);
            this.cmbPokerClientLanguage.Name = "cmbPokerClientLanguage";
            this.cmbPokerClientLanguage.Size = new System.Drawing.Size(109, 21);
            this.cmbPokerClientLanguage.TabIndex = 7;
            this.cmbPokerClientLanguage.SelectionChangeCommitted += new System.EventHandler(this.cmbPokerClientLanguage_SelectionChangeCommitted);
            // 
            // lblPokerClient
            // 
            this.lblPokerClient.Location = new System.Drawing.Point(8, 123);
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
            this.cmbPokerClient.Location = new System.Drawing.Point(49, 124);
            this.cmbPokerClient.Name = "cmbPokerClient";
            this.cmbPokerClient.Size = new System.Drawing.Size(128, 21);
            this.cmbPokerClient.TabIndex = 5;
            this.cmbPokerClient.SelectionChangeCommitted += new System.EventHandler(this.cmbPokerClient_SelectionChangeCommitted);
            // 
            // txtUserId
            // 
            this.txtUserId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserId.Location = new System.Drawing.Point(82, 8);
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new System.Drawing.Size(95, 20);
            this.txtUserId.TabIndex = 4;
            this.txtUserId.TextChanged += new System.EventHandler(this.txtUserId_TextChanged);
            // 
            // lblUserId
            // 
            this.lblUserId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserId.AutoSize = true;
            this.lblUserId.Location = new System.Drawing.Point(8, 10);
            this.lblUserId.Name = "lblUserId";
            this.lblUserId.Size = new System.Drawing.Size(71, 13);
            this.lblUserId.TabIndex = 3;
            this.lblUserId.Text = "Your User ID:";
            // 
            // btnChangeHandHistory
            // 
            this.btnChangeHandHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeHandHistory.Location = new System.Drawing.Point(8, 78);
            this.btnChangeHandHistory.Name = "btnChangeHandHistory";
            this.btnChangeHandHistory.Size = new System.Drawing.Size(169, 23);
            this.btnChangeHandHistory.TabIndex = 2;
            this.btnChangeHandHistory.Text = "Change Hand History Directory";
            this.btnChangeHandHistory.UseVisualStyleBackColor = true;
            this.btnChangeHandHistory.Click += new System.EventHandler(this.btnChangeHandHistory_Click);
            // 
            // txtHandHistoryDirectory
            // 
            this.txtHandHistoryDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHandHistoryDirectory.Location = new System.Drawing.Point(8, 52);
            this.txtHandHistoryDirectory.Name = "txtHandHistoryDirectory";
            this.txtHandHistoryDirectory.ReadOnly = true;
            this.txtHandHistoryDirectory.Size = new System.Drawing.Size(169, 20);
            this.txtHandHistoryDirectory.TabIndex = 1;
            // 
            // tabAboutPage
            // 
            this.tabAboutPage.Controls.Add(this.btnDonate);
            this.tabAboutPage.Controls.Add(this.lblDonate);
            this.tabAboutPage.Controls.Add(this.lblPieroTofyLink);
            this.tabAboutPage.Controls.Add(this.lblProgramName);
            this.tabAboutPage.Controls.Add(this.pictureEagle);
            this.tabAboutPage.Location = new System.Drawing.Point(4, 22);
            this.tabAboutPage.Name = "tabAboutPage";
            this.tabAboutPage.Size = new System.Drawing.Size(187, 297);
            this.tabAboutPage.TabIndex = 2;
            this.tabAboutPage.Text = "About";
            this.tabAboutPage.UseVisualStyleBackColor = true;
            // 
            // pictureEagle
            // 
            this.pictureEagle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureEagle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureEagle.Image = global::PokerMuck.Properties.Resources.Eagle;
            this.pictureEagle.Location = new System.Drawing.Point(48, 191);
            this.pictureEagle.Name = "pictureEagle";
            this.pictureEagle.Size = new System.Drawing.Size(93, 74);
            this.pictureEagle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureEagle.TabIndex = 0;
            this.pictureEagle.TabStop = false;
            // 
            // lblProgramName
            // 
            this.lblProgramName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgramName.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgramName.Location = new System.Drawing.Point(8, 8);
            this.lblProgramName.Name = "lblProgramName";
            this.lblProgramName.Size = new System.Drawing.Size(171, 20);
            this.lblProgramName.TabIndex = 1;
            this.lblProgramName.Text = "PokerMuck";
            this.lblProgramName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPieroTofyLink
            // 
            this.lblPieroTofyLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPieroTofyLink.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPieroTofyLink.Location = new System.Drawing.Point(3, 268);
            this.lblPieroTofyLink.Name = "lblPieroTofyLink";
            this.lblPieroTofyLink.Size = new System.Drawing.Size(181, 17);
            this.lblPieroTofyLink.TabIndex = 2;
            this.lblPieroTofyLink.TabStop = true;
            this.lblPieroTofyLink.Text = "http://www.pierotofy.it";
            this.lblPieroTofyLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPieroTofyLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblPieroTofyLink_LinkClicked);
            // 
            // lblDonate
            // 
            this.lblDonate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDonate.Location = new System.Drawing.Point(3, 28);
            this.lblDonate.Name = "lblDonate";
            this.lblDonate.Size = new System.Drawing.Size(184, 108);
            this.lblDonate.TabIndex = 3;
            this.lblDonate.Text = resources.GetString("lblDonate.Text");
            this.lblDonate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDonate
            // 
            this.btnDonate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDonate.Font = new System.Drawing.Font("Arial Black", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDonate.Location = new System.Drawing.Point(11, 139);
            this.btnDonate.Name = "btnDonate";
            this.btnDonate.Size = new System.Drawing.Size(168, 46);
            this.btnDonate.TabIndex = 4;
            this.btnDonate.Text = "Donate \r\n(any amount)";
            this.btnDonate.UseVisualStyleBackColor = true;
            this.btnDonate.Click += new System.EventHandler(this.btnDonate_Click);
            // 
            // entityHandsContainer
            // 
            this.entityHandsContainer.BackColor = System.Drawing.Color.Transparent;
            this.entityHandsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityHandsContainer.Location = new System.Drawing.Point(3, 3);
            this.entityHandsContainer.Name = "entityHandsContainer";
            this.entityHandsContainer.Size = new System.Drawing.Size(181, 254);
            this.entityHandsContainer.TabIndex = 4;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(195, 323);
            this.Controls.Add(this.tabControl);
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
            this.tabHandsPage.ResumeLayout(false);
            this.tabConfigurationPage.ResumeLayout(false);
            this.tabConfigurationPage.PerformLayout();
            this.tabAboutPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEagle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabHandsPage;
        private ControlListContainer entityHandsContainer;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TabPage tabConfigurationPage;
        private System.Windows.Forms.Button btnChangeHandHistory;
        private System.Windows.Forms.TextBox txtHandHistoryDirectory;
        private System.Windows.Forms.TabPage tabAboutPage;
        private System.Windows.Forms.PictureBox pictureEagle;
        private System.Windows.Forms.Label lblUserId;
        private System.Windows.Forms.TextBox txtUserId;
        private System.Windows.Forms.Label lblPokerClient;
        private System.Windows.Forms.ComboBox cmbPokerClient;
        private System.Windows.Forms.Label lblPokerClientLanguage;
        private System.Windows.Forms.ComboBox cmbPokerClientLanguage;
        private System.Windows.Forms.Label lblProgramName;
        private System.Windows.Forms.LinkLabel lblPieroTofyLink;
        private System.Windows.Forms.Label lblDonate;
        private System.Windows.Forms.Button btnDonate;


    }
}

