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
    public partial class HudWindow : Form
    {
        private bool draggingWindow;
        private Point mousePositionOnDrag;

        /* Notifies that the statistics of this hud need to be reset */
        public delegate void OnResetStatisticsButtonPressedHandler(HudWindow sender);
        public event OnResetStatisticsButtonPressedHandler OnResetStatisticsButtonPressed;

        public HudWindow()
        {
            InitializeComponent();
        }

        public void DisplayPlayerName(String playerName)
        {
            lblPlayerName.Text = playerName;
        }

        /* Sets the position of the window relative to windowRect */
        public void SetAbsolutePosition(Point relativePosition, Rectangle windowRect)
        {
            Point absolutePosition = new Point(relativePosition.X + windowRect.X, relativePosition.Y + windowRect.Y);
            this.Location = absolutePosition;
        }

        public Point GetRelativePosition(Rectangle windowRect)
        {
            Point result = new Point(this.Location.X - windowRect.X, this.Location.Y - windowRect.Y);
            return result;
        }

        /* This function is overriden by child classes and takes care of displaying
         * game specific layout */
        public virtual void DisplayStatistics(PlayerStatistics stats)
        {
            
        }

        private void HudWindow_MouseDown(object sender, MouseEventArgs e)
        {
            draggingWindow = true;
            mousePositionOnDrag = e.Location;
        }

        private void HudWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingWindow) this.Location = new Point(Cursor.Position.X - mousePositionOnDrag.X,
                                                            Cursor.Position.Y - mousePositionOnDrag.Y);
        }

        private void HudWindow_MouseUp(object sender, MouseEventArgs e)
        {
            draggingWindow = false;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (OnResetStatisticsButtonPressed != null) OnResetStatisticsButtonPressed(this);
        }
    }
}
