using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;

namespace PokerMuck
{
    public partial class FrmFirstExecutionWizard : Form
    {
        private int currentPage;
        private const int PAGE_COUNT = 5;
        private PokerMuckUserSettings userSettings;

        public FrmFirstExecutionWizard()
        {
            InitializeComponent();
            currentPage = 1;
            RefreshPageCountLabel();

            userSettings = new PokerMuckUserSettings();
            LoadConfigurationValues();

            AdjustControls();
        }

        /* The designer doesn't show the controls exactly like we want them. 
         * This procedure takes care of adjusting all controls to their correct display position and size */
        private void AdjustControls()
        {
            this.Size = new Size(555, 555);
            
            foreach (Control c in Controls)
            {
                if (c is Panel)
                {
                    c.Location = new Point(7, 33);
                    c.Size = new Size(524, 432);
                    c.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            currentPage += 1;

            if (currentPage == 2)
            {
                panelPageOne.Visible = false;
                panelPageTwo.Visible = true;
                panelPageThree.Visible = false;
                panelPageFour.Visible = false;
                panelPageFive.Visible = false;
                lblTitle.Text = "Choose your poker client";
            }
            else if (currentPage == 3)
            {
                panelPageOne.Visible = false;
                panelPageTwo.Visible = false;
                panelPageThree.Visible = true;
                panelPageFour.Visible = false;
                panelPageFive.Visible = false;
                lblTitle.Text = "Enable recording of hand histories";
                lblHandHistoryLanguageText.Text = lblHandHistoryLanguageText.Text.Replace("<lang>", userSettings.CurrentPokerClient.CurrentLanguage);
            }
            else if (currentPage == 4)
            {
                panelPageOne.Visible = false;
                panelPageTwo.Visible = false;
                panelPageThree.Visible = false;
                panelPageFour.Visible = true;
                panelPageFive.Visible = false;
                lblTitle.Text = "Select hand history directory";
            }
            else if (currentPage == 5)
            {
                panelPageOne.Visible = false;
                panelPageTwo.Visible = false;
                panelPageThree.Visible = false;
                panelPageFour.Visible = false;
                panelPageFive.Visible = true;
                btnNext.Text = "&Finish";
                lblTitle.Text = "Thank you!";

                cmbPokerClient_SelectionChangeCommitted(null, EventArgs.Empty);
                cmbPokerClientLanguage_SelectionChangeCommitted(null, EventArgs.Empty);
                cmbPokerClientTheme_SelectionChangeCommitted(null, EventArgs.Empty);
                userSettings.Save();
            }
            else if (currentPage == 6)
            {
                this.Close();
            }

            RefreshPageCountLabel();
        }

        private void RefreshPageCountLabel()
        {
            lblPageNum.Text = String.Format("Step {0} of {1}", currentPage, PAGE_COUNT);
        }

        /* Read the values from the configuration and puts them into the UI */
        private void LoadConfigurationValues()
        {
            txtHandHistoriesDirectory.Text = userSettings.StoredHandHistoryDirectory;

            // Load poker client list
            LoadPokerClientList();

            // Set current poker client
            cmbPokerClient.Text = userSettings.CurrentPokerClient.Name;

            // Load languages for the current client
            LoadPokerClientLanguages(userSettings.CurrentPokerClient);

            // Load themes
            LoadPokerClientThemes(userSettings.CurrentPokerClient);
            
            cmbPokerClientLanguage.Text = userSettings.CurrentPokerClient.CurrentLanguage;
            cmbPokerClientTheme.Text = userSettings.CurrentPokerClient.CurrentTheme;
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
            }
            else
            {
                ArrayList list = new ArrayList();
                list.Add("Feature not yet supported");
                cmbPokerClientTheme.DataSource = list;
                cmbPokerClientTheme.Enabled = false;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();
            DialogResult result = browserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtHandHistoriesDirectory.Text = browserDialog.SelectedPath;
                userSettings.StoredHandHistoryDirectory = browserDialog.SelectedPath;
            }
        }

        /* Pokerclient has changed, store in config and load available languages */
        private void cmbPokerClient_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PokerClient client = PokerClientsList.Find(cmbPokerClient.Text);
            client.InitializeLanguage(client.DefaultLanguage);

            LoadPokerClientLanguages(client);
            LoadPokerClientThemes(client);

            userSettings.CurrentPokerClient = client;
        }

        /* Pokerclient language has changed, initialize the new config */
        private void cmbPokerClientLanguage_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PokerClient client = userSettings.CurrentPokerClient;
            client.InitializeLanguage(cmbPokerClientLanguage.Text);

            userSettings.CurrentPokerClient = client;
        }

        private void cmbPokerClientTheme_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PokerClient client = userSettings.CurrentPokerClient;
            client.SetTheme(cmbPokerClientTheme.Text);

            userSettings.CurrentPokerClient = client;
        }
    }
}
