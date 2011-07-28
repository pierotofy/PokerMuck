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
            pmDirector.RunGUIRoutine += new PokerMuckDirector.RunGUIRoutineHandler(pmDirector_RunGUIRoutine);
            pmDirector.ClearAllPlayerMuckedHands += new PokerMuckDirector.ClearAllPlayerMuckedHandsHandler(pmDirector_ClearAllPlayerMuckedHands);
            pmDirector.DisplayPlayerMuckedHand += new PokerMuckDirector.DisplayPlayerMuckedHandHandler(pmDirector_DisplayPlayerMuckedHand);
            pmDirector.DisplayStatus += new PokerMuckDirector.DisplayStatusHandler(pmDirector_DisplayStatus);
            pmDirector.ClearFinalBoard += new PokerMuckDirector.ClearFinalBoardHandler(pmDirector_ClearFinalBoard);
            pmDirector.DisplayFinalBoard += new PokerMuckDirector.DisplayFinalBoardHandler(pmDirector_DisplayFinalBoard);
            pmDirector.DisplayHud += new PokerMuckDirector.DisplayHudHandler(pmDirector_DisplayHud);
            pmDirector.DisplayPlayerStatistics += new PokerMuckDirector.DisplayPlayerStatisticsHandler(pmDirector_DisplayPlayerStatistics);

            //pmDirector.Test();

            HoldemHand h1 = new HoldemHand(new Card(CardFace.Jack, CardSuit.Clubs), new Card(CardFace.Queen, CardSuit.Diamonds));
            HoldemBoard b = new HoldemBoard(new Card(CardFace.Seven, CardSuit.Clubs),
                                            new Card(CardFace.Two, CardSuit.Diamonds),
                                            new Card(CardFace.Ace, CardSuit.Diamonds),
                                            new Card(CardFace.Ten, CardSuit.Clubs),
                                            new Card(CardFace.Three, CardSuit.Diamonds));
            
            HoldemHand.Classification classification = h1.GetClassification(HoldemGamePhase.River, b);
            Debug.Print(classification.Hand.ToString() + " " + classification.Kicker.ToString());
            

            return;

            //String res = pmDirector.UserSettings.CurrentPokerClient.GetHandHistoryFilenameRegexPatternFromWindowTitle(".COM Play 736 (6 max) - 1/2 - No Limit Hold'em - Logged In As italystallion89");
            //String res = pmDirector.UserSettings.CurrentPokerClient.GetHandHistoryFilenameRegexPatternFromWindowTitle("$0.95 + $0.05 Heads Up Sit & Go (228858150), Table 1 - 10/20 - No Limit Hold'em - Logged In As italystallion89");
            //Debug.Print("Result: " + res);

            /* TODO remove

            StatisticsNumberData p1 = new StatisticsNumberData("One", 0.5f, "Category", 2);
            StatisticsUnknownData p2 = new StatisticsUnknownData("Two", "Category");
            StatisticsNumberData p3 = new StatisticsNumberData("Three", 0.25f, "Category", 2);
            StatisticsData avg1 = p2.Average("Average1", "Category", 2, p1, p3);

            Debug.Print(p1.Name + ": " + p1.GetValue());
            Debug.Print(p2.Name + ": " + p2.GetValue());
            Debug.Print(p3.Name + ": " + p3.GetValue());
            Debug.Print(avg1.Name + ": " + avg1.GetValue());
             
            
            Regex r = pmDirector.UserSettings.CurrentPokerClient.GetRegex("hand_history_table_token");
            //Regex r = new Regex(@"FT[0-9]+ Escondido \(shallow\) - \$0\.01-\$0\.02 - No Limit Hold'em");
            //Table (?<tableId>[^-]+) - \$?[\d]+\/\$?[\d]+ - .+ - [\d]{2}:[\d]{2}:[\d]{2} .* - [\d]{4}\/[\d]{2}\/[\d]{2}
            Match m = r.Match("Full Tilt Poker Game #29459258249: Table Valley Of Fire (shallow) - $0.01/$0.02 - No Limit Hold'em - 19:52:33 ET - 2011/03/29");

            if (m.Success)
            {
                Debug.Print("OOOK");
                Debug.Print(m.Groups["gameType"].Value);
            }*/

            //pmDirector.NewForegroundWindow("$0.95 + $0.05 Heads Up Sit & Go (229273428), Table 1 - 10/20 - No Limit Hold'em - Logged In As italystallion89", Rectangle.Empty);


            // Adjust size
            this.Size = pmDirector.UserSettings.WindowSize;

            // Adjust window position
            this.Location = pmDirector.UserSettings.WindowPosition;

            // Load configuration
            LoadConfigurationValues();

            // Always start the view on the About tab
            tabControl.SelectedIndex = 3;
        }

        void pmDirector_RunGUIRoutine(Action d, Boolean asynchronous)
        {
            if (this != null)
            {
                try
                {
                    if (asynchronous) this.BeginInvoke(d);
                    else this.Invoke(d);
                }
                catch (Exception e)
                {
                    Debug.Print("Error while running GUI Routine from FrmMain: " + e.Message);
                }
            }
        }

        void pmDirector_DisplayPlayerStatistics(Player p)
        {
            this.Invoke((Action)delegate()
            {
                statisticsDisplay.Visible = true;
                tabControl.SelectedIndex = 1;
                statisticsDisplay.DisplayStatistics(p);
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

                // We also update the statistics that we might be watching
                statisticsDisplay.UpdateStatistics();
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

                tabControl.SelectedIndex = 0;
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
            txtHandHistoryDirectory.Text = pmDirector.UserSettings.StoredHandHistoryDirectory;
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

            // Refresh hand history directory
            txtHandHistoryDirectory.Text = pmDirector.UserSettings.StoredHandHistoryDirectory;
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
