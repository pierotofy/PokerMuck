using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace PokerMuck
{
    public partial class StatisticsDisplay : UserControl
    {
        private int statisticsSpacing;
        [Description("Sets the spacing between labels"),
         Category("Values"),
         DefaultValue(3)]
        public int StatisticsSpacing
        {
            get
            {
                return statisticsSpacing;
            }

            set
            {
                statisticsSpacing = value;
            }
        }

        private int topMargin;
        [Description("Sets the margin of statistics from the top"),
         Category("Values"),
         DefaultValue(3)]
        public int TopMargin
        {
            get
            {
                return topMargin;
            }

            set
            {
                topMargin = value;
            }
        }


        /* Who was the last player we displayed? */
        private Player lastPlayerDisplayed;

        public StatisticsDisplay()
        {
            InitializeComponent();
            lastPlayerDisplayed = null;
        }

        /* Displays the statistics for a player */
        public void DisplayStatistics(Player p)
        {
            lastPlayerDisplayed = p;
            lblPlayerName.Text = p.Name;

            PlayerStatistics statistics = p.GetStatistics();

            ClearAllTabPages();

            GenerateTabPages(statistics.GetCategories());

            FillStatistics(statistics);

            AdjustControls();
        }

        /* Simply update the statistics of the last player we displayed */
        public void UpdateStatistics()
        {
            if (lastPlayerDisplayed != null)
            {
                PlayerStatistics statistics = lastPlayerDisplayed.GetStatistics();

                RemoveStatistics(statistics);
                FillStatistics(statistics);
            }
        }

        private void RemoveStatistics(PlayerStatistics statistics)
        {
            // For each statistic category
            foreach (String category in statistics.GetCategories())
            {
                // Find the tab page for this category
                TabPage tp = FindTabPage(category);

                // Clear it
                tp.Controls.Clear();
            }
        }

        private void ClearAllTabPages()
        {
            tabControl.TabPages.Clear();
        }

        private void GenerateTabPages(List<String> categories)
        {
            foreach (String category in categories)
            {
                TabPage page = new TabPage(category);
                page.BackColor = this.BackColor;
                tabControl.TabPages.Add(page);
            }
        }

        private void FillStatistics(PlayerStatistics statistics)
        {
            //this.SuspendLayout();

            // For each statistic category
            foreach (String category in statistics.GetCategories())
            {
                int positionX = topMargin; // Keep track of the position of new labels

                // Find the tab page for this category
                TabPage tp = FindTabPage(category);

                // For each data in that category
                foreach (Statistic stat in statistics.GetStatistics(category))
                {
                    // Create the proper control for it
                    StatisticItem item = new StatisticItem(stat, tp);

                    // Initialize properties FIRST!
                    item.Top = positionX;
                    item.Width = this.ClientSize.Width - 1;
                    item.Anchor = AnchorStyles.Left | AnchorStyles.Right;

                    // Add the label to the tab page
                    tp.Controls.Add(item);

                    // Increment the top position
                    positionX += item.Height + statisticsSpacing;
                }
            }

            //this.ResumeLayout();
        }

        // Helper to get a reference to a particular tab
        private TabPage FindTabPage(String name)
        {
            foreach(TabPage tp in tabControl.TabPages){
                if (tp.Text == name) return tp;
            }

            Debug.Assert(false, "I couldn't find a tab page for " + name);
            return null;
        }

        public void AdjustControls()
        {
            // We need to refresh the size of the statistics items

            foreach (TabPage tp in tabControl.TabPages)
            {
                foreach (Control item in tp.Controls)
                {
                    if (item is StatisticItem) ((StatisticItem)item).AdjustControls();
                }
            }
        }

        private void StatisticsDisplay_SizeChanged(object sender, EventArgs e)
        {
            AdjustControls();
        }
    }
}
