using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PokerMuck
{
    public partial class FrmMain : Form
    {
        /* Instance of the director */
        PokerMuckDirector pmDirector;

        public FrmMain()
        {
            InitializeComponent();

            // Update window title
            this.Text = Application.ProductName + " " + Application.ProductVersion;
        }



        private void FrmMain_Load(object sender, EventArgs e)
        {
            SetStatus("Waiting...");

            pmDirector = new PokerMuckDirector();
            pmDirector.ClearAllPlayerMuckedHands += new PokerMuckDirector.ClearAllPlayerMuckedHandsHandler(pmDirector_ClearAllPlayerMuckedHands);
            pmDirector.DisplayPlayerMuckedHand += new PokerMuckDirector.DisplayPlayerMuckedHandHandler(pmDirector_DisplayPlayerMuckedHand);
            pmDirector.DisplayStatus += new PokerMuckDirector.DisplayStatusHandler(pmDirector_DisplayStatus);
            pmDirector.Test();

            // Adjust size
            this.Size = pmDirector.UserSettings.WindowSize;

            // Adjust window position
            this.Location = pmDirector.UserSettings.WindowPosition;

            // Load configuration
            LoadConfigurationValues();
        }

        void pmDirector_DisplayStatus(string status)
        {
            // Thread safe
            this.BeginInvoke((Action)delegate()
            {
                SetStatus(status);
            });
        }

        void pmDirector_DisplayPlayerMuckedHand(Player player)
        {
            Debug.Print("Displayed!");

            // Thread safe
            this.BeginInvoke((Action)delegate()
            {
                EntityCardListPanel ehp = new EntityCardListPanel();
                ehp.EntityName = player.Name;
                ehp.CardListToDisplay = player.MuckedHand;

                /* We set the initial size of the component to the largest possible, the
                 * addPanel method will take care of setting the proper size */
                ehp.Size = entityHandsContainer.Size;

                entityHandsContainer.AddPanel(ehp, 100);
            });
        }

        void pmDirector_ClearAllPlayerMuckedHands()
        {
            Debug.Print("ClearAllPLayermuckedhands");

            // Thread safe
            this.Invoke((Action)delegate()
            {
                entityHandsContainer.ClearAll();
            });
        }

        /* Helper methods */
        private void SetStatus(String status)
        {
            lblStatus.Text = status;
        }

       
        // Cleanup
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            pmDirector.Terminate();
        }

        // Save new window size
        private void FrmMain_ResizeEnd(object sender, EventArgs e)
        {
            pmDirector.UserSettings.WindowSize = this.Size;
        }

        private void FrmMain_LocationChanged(object sender, EventArgs e)
        {
            // Save window position for the future!
            if (pmDirector != null) pmDirector.UserSettings.WindowPosition = this.Location;
        }

        /* Read the values from the configuration and puts them into the UI */
        private void LoadConfigurationValues()
        {
            txtHandHistoryDirectory.Text = pmDirector.UserSettings.HandHistoryDirectory;
            txtUserId.Text = pmDirector.UserSettings.UserID;
        }

        /* Change hand history directory */
        private void btnChangeHandHistory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();
            DialogResult result = browserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtHandHistoryDirectory.Text = browserDialog.SelectedPath;
                pmDirector.ChangeHandHistoryDirectory(browserDialog.SelectedPath);
            }

        }

        /* User ID has changed, store in config */
        private void txtUserId_TextChanged(object sender, EventArgs e)
        {
            pmDirector.UserSettings.UserID = txtUserId.Text;
        }

    }
}
