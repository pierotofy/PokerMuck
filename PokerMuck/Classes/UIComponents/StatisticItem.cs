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
    public partial class StatisticItem : UserControl
    {
        private Statistic statisticToDisplay;
        private ExtendedStatisticsPanel extendedPanel = null;
        private Control parent;
        private bool itemIsSelected;
        private bool itemIsALine;

        private Color COLOR_HIGHLIGHT = Color.FromArgb(255, 220, 176, 121);
        private Color COLOR_SELECTED = Color.FromArgb(255, 215, 127, 18);

        private bool highlightOnMouseOver;
        [Description("Turn on/off hightlight on mouse over"),
         Category("Values"),
         DefaultValue(true)]
        public bool HighlightOnMouseOver
        {
            get
            {
                return highlightOnMouseOver;
            }

            set
            {
                highlightOnMouseOver = value;
            }
        }

        private bool clickable;
        [Description("Turn on/off whether this item can be selected"),
         Category("Values"),
         DefaultValue(true)]
        public bool Clickable
        {
            get
            {
                return clickable;
            }

            set
            {
                clickable = value;
            }
        }

        // Wrap around
        public StatisticItem(StatisticsData statisticToDispay, Control parent)
            : this(new Statistic(statisticToDispay, ""), parent)
        {

        }

        public StatisticItem(Statistic statisticToDisplay, Control parent)
        {
            this.statisticToDisplay = statisticToDisplay;
            this.parent = parent;
            this.highlightOnMouseOver = true;
            this.clickable = true;
            this.itemIsSelected = false;
            this.itemIsALine = false;
            InitializeComponent();

            if (statisticToDisplay.MainData is StatisticsLineData)
            {
                this.itemIsALine = true;
                this.lblName.Hide();
                this.lblLine.Show();
            }

            this.lblName.Text = String.Format("{0}: {1}",
                statisticToDisplay.MainData.Name, statisticToDisplay.MainData.GetValue());

        }

        public void AdjustControls()
        {
            if (extendedPanel != null)
            {
                extendedPanel.Top = this.Top;
                extendedPanel.Left = (this.Right - extendedPanel.Width - this.Parent.Margin.Right - extendedPanel.Margin.Right);
            }
        }

        private void StatisticsItem_MouseEnter(object sender, EventArgs e)
        {
            if (HighlightOnMouseOver && !itemIsSelected && !itemIsALine)
            {
                this.BackColor = COLOR_HIGHLIGHT;
            }

            if (statisticToDisplay.HasSubStatistics())
            {
                this.Cursor = Cursors.Hand;
            }

            if (extendedPanel != null) extendedPanel.Show();
        }

        private void StatisticsItem_MouseLeave(object sender, EventArgs e)
        {
            if (MouseIsOverExtendedPanel()) return; 

            if (HighlightOnMouseOver && !itemIsSelected && !itemIsALine)
            {
                this.BackColor = Color.Transparent;
            }
            
            if (statisticToDisplay.HasSubStatistics())
            {
                this.Cursor = Cursors.Default;
            }

            if (extendedPanel != null && !itemIsSelected) extendedPanel.Hide();
        }

        private bool MouseIsOverExtendedPanel()
        {
            if (extendedPanel == null) return false;
            else
            {
                Rectangle panelRect = extendedPanel.RectangleToScreen(extendedPanel.ClientRectangle);
                Point cursorLocation = Cursor.Position;
                const int CURSOR_AREA = 4;

                return panelRect.IntersectsWith(new Rectangle(cursorLocation.X, cursorLocation.Y, CURSOR_AREA, CURSOR_AREA));
            }

        }

        private void lblName_MouseEnter(object sender, EventArgs e)
        {
            StatisticsItem_MouseEnter(sender, e);
        }

        private void lblName_MouseLeave(object sender, EventArgs e)
        {
            StatisticsItem_MouseLeave(sender, e);
        }

        private void StatisticItem_Load(object sender, EventArgs e)
        {
            if (statisticToDisplay.HasSubStatistics())
            {
                // Create the sub statistics panel
                extendedPanel = new ExtendedStatisticsPanel(statisticToDisplay.SubData);
                parent.Controls.Add(extendedPanel);
                extendedPanel.BringToFront();
                extendedPanel.Refresh();

                AdjustControls();
            }
        }

        private void StatisticItem_MouseClick(object sender, EventArgs e)
        {
            if (!clickable || itemIsALine) return;

            itemIsSelected = !itemIsSelected;

            if (itemIsSelected)
            {
                this.BackColor = COLOR_SELECTED;
            }
            else
            {
                this.BackColor = COLOR_HIGHLIGHT;
            }
        }

        private void lblName_Click(object sender, EventArgs e)
        {
            StatisticItem_MouseClick(sender, e);
        }

        private void StatisticItem_SizeChanged(object sender, EventArgs e)
        {
            AdjustControls();
        }
    }
}
