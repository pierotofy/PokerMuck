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
            pmDirector.Test();

            // Adjust size
            this.Size = pmDirector.UserSettings.WindowSize;

            // Adjust window position
            this.Location = pmDirector.UserSettings.WindowPosition;

            // Load configuration
            LoadConfigurationValues();

/*
            handPanel.PlayerName = "PieroTofy";
            handPanel.HandToDisplay = new HoldemHand(new Card(CardFace.Ace, CardSuit.Clubs),
                                                    new Card(CardFace.Four, CardSuit.Diamonds));
 */
        }

        void pmDirector_DisplayPlayerMuckedHand(Player player)
        {
            Debug.Print("Displayed!");
            handPanel.PlayerName = player.Name;
            handPanel.HandToDisplay = player.MuckedHand;

        }

        void pmDirector_ClearAllPlayerMuckedHands()
        {
            Debug.Print("ClearAllPLayermuckedhands");
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
    }
}
