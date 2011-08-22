using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PokerMuck
{
    public partial class StatisticItemListDisplay : Panel
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

        public StatisticItemListDisplay()
        {
            InitializeComponent();
        }

        public void Add(List<Statistic> statistics)
        {
            int positionX = topMargin; // Keep track of the position of new labels

            foreach (Statistic stat in statistics)
            {
                // Create the proper control for it
                StatisticItem item = new StatisticItem(stat, this);

                // Initialize properties FIRST!
                item.Top = positionX;
                item.Width = this.ClientSize.Width - 1;
                item.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                // Add the label to the tab page
                this.Controls.Add(item);

                // Increment the top position
                positionX += item.Height + statisticsSpacing;
            }

            AdjustHeightToFit();
        }

        public void AdjustHeightToFit()
        {
            if (this.Controls.Count == 0) this.Height = 10; // If there are no controls, set to fixed height
            else
            {
                int totalHeight = TopMargin;
                foreach (Control c in this.Controls)
                {
                    if (c is StatisticItem) totalHeight += c.Height + StatisticsSpacing;
                }

                this.Height = totalHeight;
            }
        }

        public void Clear()
        {
            this.Controls.Clear();
        }

        public void AdjustControls()
        {
            foreach (Control item in this.Controls)
            {
                if (item is StatisticItem) ((StatisticItem)item).AdjustControls();
            }
        }

        private void StatisticItemListDisplay_SizeChanged(object sender, EventArgs e)
        {
            AdjustControls();
        }
    }
}
