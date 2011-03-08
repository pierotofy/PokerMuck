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
        private int labelSpacing;
        
        [Description("Sets the spacing between labels"),
         Category("Values"),
         DefaultValue(3)]
        public int LabelSpacing
        {
            get
            {
                return labelSpacing;
            }

            set
            {
                labelSpacing = value;
            }
        }

        private int topMargin;

        [Description("Sets the margin of labels from the top"),
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

        public StatisticsDisplay()
        {
            InitializeComponent();
        }

        /* Displays the statistics for a player */
        public void DisplayStatistics(Player p)
        {
            lblPlayerName.Text = p.Name;

            PlayerStatistics statistics = p.GetStatistics();

            ClearAllTabPages();

            GenerateTabPages(statistics.GetCategories());

            FillStatistics(statistics);
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

            // For each statistic category
            foreach (String category in statistics.GetCategories())
            {
                int positionX = topMargin; // Keep track of the position of new labels

                // For each data in that category
                foreach (StatisticsData data in statistics.GetStatistics(category))
                {
                    // Find the tab page for this category
                    TabPage tp = FindTabPage(category);

                    // Create the proper label for it
                    Label label = new Label();
                    
                    // Add the label to the tab page
                    tp.Controls.Add(label);

                    label.BackColor = Color.Transparent;
                    label.AutoSize = true;
                    label.Top = positionX;

                    // Increment the top position
                    positionX += label.Height + labelSpacing;

                    // Set the label text to the value of the data
                    label.Text = String.Format("{0}: {1}",data.Name, data.GetPercentage());


                }
            }

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
    }
}
