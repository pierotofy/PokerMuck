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
    public partial class ExtendedStatisticsPanel : Panel
    {
        private List<StatisticsData> subData;
        const int MARGIN_TOP = 2;
        const int SPACING = 2;

        public ExtendedStatisticsPanel(List<StatisticsData> subData)
        {
            this.subData = subData;

            InitializeComponent();

            int positionX = MARGIN_TOP;
            foreach (StatisticsData data in subData)
            {
                StatisticItem item = new StatisticItem(data, this);
                item.HighlightOnMouseOver = false;
                item.Clickable = false;
                item.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                item.AutoSize = true;
                item.Top = positionX;

                this.Controls.Add(item);

                // Increment the top position
                positionX += item.Height + SPACING;
            }
        }
    }
}
