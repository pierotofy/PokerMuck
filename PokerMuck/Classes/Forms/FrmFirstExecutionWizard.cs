using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PokerMuck
{
    public partial class FrmFirstExecutionWizard : Form
    {
        private int currentPage;
        private const int PAGE_COUNT = 4;
        private PokerMuckUserSettings userSettings;

        public FrmFirstExecutionWizard()
        {
            InitializeComponent();
            currentPage = 1;
            RefreshPageCountLabel();

            userSettings = new PokerMuckUserSettings();
            txtHandHistoriesDirectory.Text = userSettings.HandHistoryDirectory;

            AdjustControls();
        }

        /* The designer doesn't show the controls exactly like we want them. 
         * This procedure takes care of adjusting all controls to their correct display position and size */
        private void AdjustControls()
        {
            this.Size = new Size(525, 333);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            currentPage += 1;

            if (currentPage == 3)
            {
                panelPageOne.Visible = false;
                panelPageThree.Visible = true;
                panelPageFour.Visible = false;
                panelPageFive.Visible = false;
                lblTitle.Text = "Enable recording of hand histories";
            }
            else if (currentPage == 4)
            {
                panelPageOne.Visible = false;
                panelPageThree.Visible = false;
                panelPageFour.Visible = true;
                panelPageFive.Visible = false;
                lblTitle.Text = "Select hand history directory";
            }
            else if (currentPage == 5)
            {
                panelPageOne.Visible = false;
                panelPageThree.Visible = false;
                panelPageFour.Visible = false;
                panelPageFive.Visible = true;
                btnNext.Text = "&Finish";
                lblTitle.Text = "Thank you!";
                userSettings.Save();
            }
            else if (currentPage == 5)
            {
                this.Close();
            }

            RefreshPageCountLabel();
        }

        private void RefreshPageCountLabel()
        {
            lblPageNum.Text = String.Format("Step {0} of {1}", currentPage, PAGE_COUNT);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();
            DialogResult result = browserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtHandHistoriesDirectory.Text = browserDialog.SelectedPath;
                userSettings.HandHistoryDirectory = browserDialog.SelectedPath;
            }
        }
    }
}
