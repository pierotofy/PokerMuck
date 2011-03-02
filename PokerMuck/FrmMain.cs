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
using System.Threading;

namespace PokerMuck
{
    public partial class FrmMain : Form
    {
        /* Instance of the director */
        private PokerMuckDirector pmDirector;

        /* Maximum height of card list panels */
        private static int MAXIMUM_CARD_LIST_PANEL_HEIGHT = 100;

        public FrmMain()
        {
            InitializeComponent();

            // Update program title
            lblProgramName.Text = Application.ProductName + " " + Application.ProductVersion;
        }



        private void FrmMain_Load(object sender, EventArgs e)
        {
            // TODO remove
            //RankScanner r = new SharkScopeRankScanner();
            //r.FindPlayerRank("stallion089");

            SetStatus("Waiting for a game to start...");

            pmDirector = new PokerMuckDirector();
            pmDirector.ClearAllPlayerMuckedHands += new PokerMuckDirector.ClearAllPlayerMuckedHandsHandler(pmDirector_ClearAllPlayerMuckedHands);
            pmDirector.DisplayPlayerMuckedHand += new PokerMuckDirector.DisplayPlayerMuckedHandHandler(pmDirector_DisplayPlayerMuckedHand);
            pmDirector.DisplayStatus += new PokerMuckDirector.DisplayStatusHandler(pmDirector_DisplayStatus);
            pmDirector.ClearFinalBoard += new PokerMuckDirector.ClearFinalBoardHandler(pmDirector_ClearFinalBoard);
            pmDirector.DisplayFinalBoard += new PokerMuckDirector.DisplayFinalBoardHandler(pmDirector_DisplayFinalBoard);
            pmDirector.DisplayHud += new PokerMuckDirector.DisplayHudHandler(pmDirector_DisplayHud);
            pmDirector.ShiftHud += new PokerMuckDirector.ShiftHudHandler(pmDirector_ShiftHud);
            pmDirector.RemoveHud += new PokerMuckDirector.RemoveHudHandler(pmDirector_RemoveHud);

            pmDirector.Test();

            /* TODO remove
            Regex r = pmDirector.UserSettings.CurrentPokerClient.GetRegex("hand_history_detect_mucked_hand");
            Match m = r.Match("Seat 1: stallion089 (button) (small blind) mucked [5d 5s]");

            if (m.Success)
            {
                Debug.Print("OOOK");
                Debug.Print(m.Groups["playerName"].Value);
            }*/

            // Adjust size
            this.Size = pmDirector.UserSettings.WindowSize;

            // Adjust window position
            this.Location = pmDirector.UserSettings.WindowPosition;

            // Load configuration
            LoadConfigurationValues();
        }

        void pmDirector_RemoveHud(Table t)
        {
            this.Invoke((Action)delegate()
            {
                t.Hud.RemoveHud();
            });
        }

        /* Shift the position of the hud */
        void pmDirector_ShiftHud(Table t)
        {
            this.BeginInvoke((Action)delegate()
            {
                t.Hud.Shift();
            });
        }

        /* Display the hud
         * This cannot be handled by the director because
         * it has to be thread safe. So we do it. */
        void pmDirector_DisplayHud(Table t)
        {
            this.Invoke((Action)delegate()
            {
                t.Hud.DisplayAndUpdate();
            });
        }

        /* Display the board after all the mucked hands */
        void pmDirector_DisplayFinalBoard(Board board)
        {
            this.BeginInvoke((Action)delegate()
            {
                CardListPanel clp = new CardListPanel();
                clp.CardListToDisplay = board;
                clp.CardSpacing = 4;
                clp.BackColor = Color.Transparent;

                /* We set the initial size of the component to the largest possible, the
                 * addPanel method will take care of setting the proper size */
                clp.Size = entityHandsContainer.Size;

                entityHandsContainer.AddPanel(clp, MAXIMUM_CARD_LIST_PANEL_HEIGHT);
            });
        }

        /* Our board is represented in the same panel as the mucked hands, so we don't need
         * to do anything */
        void pmDirector_ClearFinalBoard()
        {

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

            AddEntityCardListPanelEntry(player.Name, player.MuckedHand);
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

        /* Thread safe insertion in the entity card list panel */
        private void AddEntityCardListPanelEntry(String entityName, CardList list)
        {
            this.BeginInvoke((Action)delegate()
            {
                EntityCardListPanel ehp = new EntityCardListPanel();
                ehp.EntityName = entityName;
                ehp.CardListToDisplay = list;

                /* We set the initial size of the component to the largest possible, the
                 * addPanel method will take care of setting the proper size */
                ehp.Size = entityHandsContainer.Size;

                entityHandsContainer.AddPanel(ehp, MAXIMUM_CARD_LIST_PANEL_HEIGHT);
            });
        }

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

            // Load poker client list
            LoadPokerClientList();

            // Set current poker client
            cmbPokerClient.Text = pmDirector.UserSettings.CurrentPokerClient.Name;

            // Load languages for the current client
            LoadPokerClientLanguages(pmDirector.UserSettings.CurrentPokerClient);

            // Set current poker client language
            cmbPokerClientLanguage.Text = pmDirector.UserSettings.CurrentPokerClient.CurrentLanguage;
        }

        /* Loads the poker clients into the appropriate combobox */
        private void LoadPokerClientList()
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = PokerClientsList.ClientList;

            cmbPokerClient.DataSource = bs;
            cmbPokerClient.DisplayMember = "Key";
        }

        /* Loads the languages available for a specific client */
        private void LoadPokerClientLanguages(PokerClient client)
        {
            cmbPokerClientLanguage.DataSource = client.SupportedLanguages;
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

        /* Pokerclient has changed, store in config and load available languages */
        private void cmbPokerClient_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PokerClient client = PokerClientsList.Find(cmbPokerClient.Text);
            client.InitializeLanguage(client.DefaultLanguage);

            pmDirector.ChangePokerClient(client);
            LoadPokerClientLanguages(client);
        }

        /* Pokerclient language has changed, initialize the new config */
        private void cmbPokerClientLanguage_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PokerClient client = pmDirector.UserSettings.CurrentPokerClient;
            client.InitializeLanguage(cmbPokerClientLanguage.Text);

            // Tell directory that we have changed the client
            pmDirector.ChangePokerClient(client);
        }

        /* Open www.pierotofy.it */
        private void lblPieroTofyLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo(lblPieroTofyLink.Text));
        }


        /* Donate to paypal
         * Thanks to: http://www.gorancic.com/blog/net/c-paypal-donate-button */
        private void btnDonate_Click(object sender, EventArgs e)
        {
            string url = "";

            string business = "pierotofy@gmail.com";  // your paypal email
            string description = "PokerMuck";            // '%20' represents a space. remember HTML!
            string country = "US";                  // AU, US, etc.
            string currency = "USD";                 // AUD, USD, etc.

            url += "https://www.paypal.com/cgi-bin/webscr" +
                "?cmd=" + "_donations" +
                "&business=" + business +
                "&lc=" + country +
                "&item_name=" + description +
                "&currency_code=" + currency +
                "&bn=" + "PP%2dDonationsBF";

            System.Diagnostics.Process.Start(url);
        }

    }
}
