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
using System.Collections;
using System.Deployment.Application;

namespace PokerMuck
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            // Register trace listener
            if (System.IO.File.Exists("debug.txt")) System.IO.File.Delete("debug.txt");
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener("debug.txt"));

            InitializeComponent();

            // Update program title
            lblProgramName.Text = Application.ProductName + " " + Application.ProductVersion;

            // Hide tools tab for non-debug builds
            /*
            #if !(DEBUG)
            {
                tabControl.Controls.Remove(tabTools);
            }
            #endif
             */
        }



        private void FrmMain_Load(object sender, EventArgs e)
        {
            Trace.WriteLine("Starting PokerMuck " + Application.ProductVersion + "... success!");

            SetStatus("Waiting for a game to start...");
            
            Globals.Director = new PokerMuckDirector();
            Globals.Director.RunGUIRoutine += new PokerMuckDirector.RunGUIRoutineHandler(Director_RunGUIRoutine);
            Globals.Director.DisplayStatus += new PokerMuckDirector.DisplayStatusHandler(Director_DisplayStatus);

            /*
            HoldemHand h1 = new HoldemHand(new Card(CardFace.Queen, CardSuit.Diamonds), new Card(CardFace.Ten, CardSuit.Spades));
            HoldemBoard b = new HoldemBoard(new Card(CardFace.Seven, CardSuit.Clubs),
                                        new Card(CardFace.Eight, CardSuit.Clubs),
                                    new Card(CardFace.King, CardSuit.Hearts),
                                    new Card(CardFace.Jack, CardSuit.Spades),
                                    new Card(CardFace.Jack, CardSuit.Spades));
            HoldemHand.Classification c = h1.GetClassification(HoldemGamePhase.Turn, b);
            Trace.WriteLine(c.ToString());


            //pmDirector.Test();
            
            HoldemCardDisplayDialog d = new HoldemCardDisplayDialog();
            d.Show();
            List<String> cards = new List<String>();
            cards.Add("AA");
            cards.Add("87s");
            cards.Add("55");
            cards.Add("KQo");
            d.SelectCards(cards);
       
            Statistic preflop = new Statistic(new StatisticsPercentageData("Raise", 2.5f), "Preflop");
            preflop.AddSubStatistic(new StatisticsPercentageData("For value", 0.2f));
            preflop.AddSubStatistic(new StatisticsPercentageData("Bluff", 0.2f));

            Statistic flop = new Statistic(new StatisticsNumberData("Raise", 0.25f), "Flop");
            flop.AddSubStatistic(new StatisticsPercentageData("For value", 0.5f));
            
            Statistic turn = new Statistic(new StatisticsPercentageData("Raise", 0.25f), "Turn");
            turn.AddSubStatistic(new StatisticsPercentageData("For value", 0.8f));
            turn.AddSubStatistic(new StatisticsPercentageData("Bluff", 0.0f));

            Statistic average = preflop.Average("Summary", 2, flop, turn);

            Trace.WriteLine("Average: " + average.ToString());
        

            HoldemHand h1 = new HoldemHand(new Card(CardFace.Nine, CardSuit.Spades), new Card(CardFace.Six, CardSuit.Diamonds));

            if (h1.IsGappedConnectorsInRange(1, 
        new HoldemHand(new Card(CardFace.Five, CardSuit.Clubs), new Card(CardFace.Four, CardSuit.Clubs)),
        new HoldemHand(new Card(CardFace.Jack, CardSuit.Clubs), new Card(CardFace.Ten, CardSuit.Clubs))))
            {
                Trace.WriteLine("In range");
            }
             */

            /*
            //String res = pmDirector.UserSettings.CurrentPokerClient.GetHandHistoryFilenameRegexPatternFromWindowTitle(".COM Play 736 (6 max) - 1/2 - No Limit Hold'em - Logged In As italystallion89");
            //String res = pmDirector.UserSettings.CurrentPokerClient.GetHandHistoryFilenameRegexPatternFromWindowTitle("$0.95 + $0.05 Heads Up Sit & Go (228858150), Table 1 - 10/20 - No Limit Hold'em - Logged In As italystallion89");
            //Trace.WriteLine("Result: " + res);

            /* TODO remove

            StatisticsNumberData p1 = new StatisticsNumberData("One", 0.5f, "Category", 2);
            StatisticsUnknownData p2 = new StatisticsUnknownData("Two", "Category");
            StatisticsNumberData p3 = new StatisticsNumberData("Three", 0.25f, "Category", 2);
            StatisticsData avg1 = p2.Average("Average1", "Category", 2, p1, p3);

            Trace.WriteLine(p1.Name + ": " + p1.GetValue());
            Trace.WriteLine(p2.Name + ": " + p2.GetValue());
            Trace.WriteLine(p3.Name + ": " + p3.GetValue());
            Trace.WriteLine(avg1.Name + ": " + avg1.GetValue());
             
            
            Regex r = pmDirector.UserSettings.CurrentPokerClient.GetRegex("hand_history_game_token");

            //Match m = r.Match("NL Texas Hold'em €3 EUR Buy-in Trny: 61535376 Level: 1  Blinds(20/40) - Friday, June 17, 23:14:29 CEST 2011");
            Match m = r.Match("€2 EUR NL Texas Hold'em - Monday, August 01, 21:45:12 CEST 2011");
            //(((?<gameType>.+) .[\d\.\,]+ [\w]{3} Buy\-in)|((\$|€)[\d]+ [A-Z]{3} (?<gameType>) - ))
            if (m.Success)
            {
                Trace.WriteLine("OOOK");
                Trace.WriteLine(m.Groups["gameType"].Value);
            }
            */
            //pmDirector.NewForegroundWindow("$0.95 + $0.05 Heads Up Sit & Go (229273428), Table 1 - 10/20 - No Limit Hold'em - Logged In As italystallion89", Rectangle.Empty);


            // Adjust size
            this.Size = Globals.UserSettings.WindowSize;

            // Adjust window position
            this.Location = Globals.UserSettings.WindowPosition;

            // Load configuration
            LoadConfigurationValues();

            // Always start the view on the About tab
            tabControl.SelectedIndex = tabControl.TabCount - 1;
        }

        void Director_RunGUIRoutine(Action d, Boolean asynchronous)
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
                    Trace.WriteLine("Error while running GUI Routine from FrmMain: " + e.Message);
                }
            }
        }

        void Director_DisplayStatus(string status)
        {
            // Thread safe
            this.BeginInvoke((Action)delegate()
            {
                SetStatus(status);
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
            Trace.WriteLine("Quit signal received, cleaning up...");

            Globals.Director.Terminate();             
        }

        // Save new window size
        private void FrmMain_ResizeEnd(object sender, EventArgs e)
        {
            Globals.UserSettings.WindowSize = this.Size;
        }

        private void FrmMain_LocationChanged(object sender, EventArgs e)
        {
            // Save window position for the future!
            if (Globals.Director != null)
            {
                if (this.Location.X > 0 && this.Location.Y > 0) Globals.UserSettings.WindowPosition = this.Location;
            }
        }

        /* Read the values from the configuration and puts them into the UI */
        private void LoadConfigurationValues()
        {
            txtHandHistoryDirectory.Text = Globals.UserSettings.StoredHandHistoryDirectory;

            // Load poker client list
            LoadPokerClientList();

            // Set current poker client
            cmbPokerClient.Text = Globals.UserSettings.CurrentPokerClient.Name;

            // Load languages for the current client
            LoadPokerClientLanguages(Globals.UserSettings.CurrentPokerClient);

            // Load themes
            LoadPokerClientThemes(Globals.UserSettings.CurrentPokerClient);

            // Set current poker client language
            cmbPokerClientLanguage.Text = Globals.UserSettings.CurrentPokerClient.CurrentLanguage;

            chkTrainingMode.Checked = Globals.UserSettings.TrainingModeEnabled;
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

        private void LoadPokerClientThemes(PokerClient client)
        {
            if (client.SupportedVisualRecognitionThemes.Count > 0)
            {
                cmbPokerClientTheme.DataSource = client.SupportedVisualRecognitionThemes;
                cmbPokerClientTheme.Enabled = true;

                // If a colormap became available during the last update, CurrentTheme needs to be changed
                if (client.CurrentTheme == "")
                {
                    client.SetTheme(client.SupportedVisualRecognitionThemes[0].ToString());

                    // Force save settings
                    Globals.UserSettings.CurrentPokerClient = client;

                    // Commit
                    Globals.UserSettings.Save();
                }
            }
            else
            {
                ArrayList list = new ArrayList();
                list.Add("Feature not yet supported");
                cmbPokerClientTheme.DataSource = list;
                cmbPokerClientTheme.Enabled = false;
            }
        }

        /* Change hand history directory */
        private void btnChangeHandHistory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();
            DialogResult result = browserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtHandHistoryDirectory.Text = browserDialog.SelectedPath;
                Globals.Director.ChangeHandHistoryDirectory(browserDialog.SelectedPath);
            }

        }

        /* Pokerclient has changed, store in config and load available languages */
        private void cmbPokerClient_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PokerClient client = PokerClientsList.Find(cmbPokerClient.Text);
            client.InitializeLanguage(client.DefaultLanguage);

            LoadPokerClientLanguages(client);
            LoadPokerClientThemes(client);

            client.SetTheme(cmbPokerClientTheme.Text);
            Globals.Director.ChangePokerClient(client);

            // Refresh hand history directory
            txtHandHistoryDirectory.Text = Globals.UserSettings.StoredHandHistoryDirectory;
        }

        /* Pokerclient language has changed, initialize the new config */
        private void cmbPokerClientLanguage_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PokerClient client = Globals.UserSettings.CurrentPokerClient;
            client.InitializeLanguage(cmbPokerClientLanguage.Text);

            // Tell directory that we have changed the client
            Globals.Director.ChangePokerClient(client);
        }


        private void cmbPokerClientTheme_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PokerClient client = Globals.UserSettings.CurrentPokerClient;
            client.SetTheme(cmbPokerClientTheme.Text);

            // Tell directory that we have changed the client
            Globals.Director.ChangePokerClient(client);
        }

        /* Open www.pierotofy.it */
        private void lblPokerMuckLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo(lblPokerMuckLink.Text));
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

        private void chkTrainingMode_CheckedChanged(object sender, EventArgs e)
        {
            Globals.UserSettings.TrainingModeEnabled = chkTrainingMode.Checked;
        }

        private void btnSendTweet_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://twitter.com/home?status=%23PokerMuck %23Poker open source HUD and muck hand viewer:%20http://www.pokermuck.com/");
        }

        private void btnJoinFacebookPage_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/pages/PokerMuck/210893282298358");
        }

        private void btnTakeWindowScreenshot_Click(object sender, EventArgs e)
        {
            String originalText = btnTakeWindowScreenshot.Text;

            btnTakeWindowScreenshot.Enabled = false;
            btnTakeWindowScreenshot.Text = "Click on the window, screenshot will be taken in 5 seconds";

            Thread t = new Thread(new ThreadStart((Action)delegate()
                {
                    Thread.Sleep(2000);
                    bool success = Globals.Director.TakeActiveWindowScreenshot(true);

                    Director_RunGUIRoutine((Action)delegate()
                    {
                        if (success){
                            MessageBox.Show("Screenshot saved as screenshot.png on Desktop", "Success");
                        }else{
                            MessageBox.Show("Could not take screenshot.", "Error");
                        }
                        btnTakeWindowScreenshot.Enabled = true;
                        btnTakeWindowScreenshot.Text = originalText;
                    }, false);
                }
            ));
            t.Start();


        }

        private void btnDonateBitcoin_Click(object sender, EventArgs e)
        {
            btnDonateBitcoin.Visible = false;
            txtBitcoinAddr.Visible = true;
        }

        // Check for clickonce updates
        // http://msdn.microsoft.com/en-us/library/ms404263.aspx
        private void btnCheckForUpdates_Click(object sender, EventArgs e)
        {
            UpdateCheckInfo info = null;
            btnCheckForUpdates.Enabled = false;
            String originalText = btnCheckForUpdates.Text;
            btnCheckForUpdates.Text = "Checking for updates...";

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                try
                {
                    info = ad.CheckForDetailedUpdate();

                }
                catch (DeploymentDownloadException dde)
                {
                    MessageBox.Show("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                    goto end;
                }
                catch (InvalidDeploymentException ide)
                {
                    MessageBox.Show("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
                    goto end;
                }
                catch (InvalidOperationException ioe)
                {
                    MessageBox.Show("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                    goto end;
                }

                if (info.UpdateAvailable)
                {
                    Boolean doUpdate = true;

                    if (!info.IsUpdateRequired)
                    {
                        DialogResult dr = MessageBox.Show("An update is available. Would you like to update the application now?", "Update Available", MessageBoxButtons.OKCancel);
                        if (!(DialogResult.OK == dr))
                        {
                            doUpdate = false;
                        }
                    }
                    else
                    {
                        // Display a message that the app MUST reboot. Display the minimum required version.
                        MessageBox.Show("This application has detected a mandatory update from your current " +
                            "version to version " + info.MinimumRequiredVersion.ToString() +
                            ". The application will now install the update and restart.",
                            "Update Available", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    if (doUpdate)
                    {
                        try
                        {
                            btnCheckForUpdates.Text = "Updating... please wait";
                            ad.Update();
                            btnCheckForUpdates.Text = "Updated!";
                            MessageBox.Show("The application has been upgraded, and will now restart.");
                            Application.Restart();
                        }
                        catch (DeploymentDownloadException dde)
                        {
                            MessageBox.Show("Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " + dde);
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("There are no updates available at this time.");
                }
            }
            else
            {
                MessageBox.Show("The application is not network deployed and cannot check for updates.");
            }

        end:
            btnCheckForUpdates.Enabled = true;
            btnCheckForUpdates.Text = originalText;
        }
    }
}
