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
        
        /* Flag for disposal 
         * If this variable is set to true,
         * Next time the hud is updated, this window will be destroyed */
        public bool DisposeFlag { get; set; }


        /* Notifies that the statistics of this hud need to be reset */
        private delegate void OnResetStatisticsButtonPressedHandler(HudWindow sender);
        private event OnResetStatisticsButtonPressedHandler OnResetStatisticsButtonPressed;

        /* Notifies that the statistics of everybody need to be reset */
        private delegate void OnResetAllStatisticsButtonPressedHandler(HudWindow sender);
        private event OnResetAllStatisticsButtonPressedHandler OnResetAllStatisticsButtonPressed;

        /* Notifies that the statistics of a our player need to be displayed 
         * in the expanded view */
        private delegate void OnPlayerStatisticsNeedToBeDisplayedHandler(HudWindow sender);
        private event OnPlayerStatisticsNeedToBeDisplayedHandler OnPlayerStatisticsNeedToBeDisplayed;

        public HudWindow()
        {
            InitializeComponent();
            DisposeFlag = false;
        }

        public virtual void RegisterHandlers(Hud hud, Table t, Player p){
            this.OnResetStatisticsButtonPressed += new HudWindow.OnResetStatisticsButtonPressedHandler(p.window_OnResetStatisticsButtonPressed);
            this.OnResetAllStatisticsButtonPressed += new HudWindow.OnResetAllStatisticsButtonPressedHandler(t.window_OnResetAllStatisticsButtonPressed);
            this.OnPlayerStatisticsNeedToBeDisplayed += new HudWindow.OnPlayerStatisticsNeedToBeDisplayedHandler(hud.window_OnPlayerStatisticsNeedToBeDisplayed);
            this.LocationChanged += new EventHandler(hud.window_LocationChanged);
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
            lblTotalHandsPlayed.Text = stats.Get("Total Hands Played", "Summary").MainData.GetFloat().ToString();
        }

        protected void HudWindow_MouseDown(object sender, MouseEventArgs e)
        {
            Control c = (Control)sender;
            
            draggingWindow = true;

            // Is this a hud window we're moving?
            if (c is HudWindow)
            {
                mousePositionOnDrag = e.Location;
            }
            else
            {
                // No, it's a control and we need to compute it's relative location for a smooth move

                mousePositionOnDrag = new Point(e.Location.X + c.Location.X, e.Location.Y + c.Location.Y);
            }
        }

        protected void HudWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingWindow) this.Location = new Point(Cursor.Position.X - mousePositionOnDrag.X,
                                                            Cursor.Position.Y - mousePositionOnDrag.Y);
        }

        protected void HudWindow_MouseUp(object sender, MouseEventArgs e)
        {
            draggingWindow = false;
        }

        private void resetPlayerStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OnResetStatisticsButtonPressed != null) OnResetStatisticsButtonPressed(this);
        }

        private void resetEverybodysStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OnResetAllStatisticsButtonPressed != null) OnResetAllStatisticsButtonPressed(this);
        }

        protected void HudWindow_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (OnPlayerStatisticsNeedToBeDisplayed != null) OnPlayerStatisticsNeedToBeDisplayed(this);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HudWindow)) return false;

            HudWindow otherObj = (HudWindow)obj;

            // Two windows are equal if the playername label is the same (and initialized)
            return (otherObj.lblPlayerName.Text == this.lblPlayerName.Text &&
                this.lblPlayerName.Text != "playerName");
        }

        public override int GetHashCode()
        {
            return lblPlayerName.Text.GetHashCode();
        }

        private void lblPlayerName_MouseUp(object sender, MouseEventArgs e)
        {
            HudWindow_MouseUp(sender, e);
        }

        private void lblPlayerName_MouseMove(object sender, MouseEventArgs e)
        {
            HudWindow_MouseMove(sender, e);
        }

        private void lblPlayerName_MouseDown(object sender, MouseEventArgs e)
        {
            HudWindow_MouseDown(sender, e);
        }

        private void lblTotalHandsPlayed_MouseUp(object sender, MouseEventArgs e)
        {
            HudWindow_MouseUp(sender, e);
        }

        private void lblTotalHandsPlayed_MouseMove(object sender, MouseEventArgs e)
        {
            HudWindow_MouseMove(sender, e);
        }

        private void lblTotalHandsPlayed_MouseDown(object sender, MouseEventArgs e)
        {
            HudWindow_MouseDown(sender, e);
        }

        private void lblTotalHandsPlayed_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            HudWindow_MouseDoubleClick(sender, e);
        }

        private void lblPlayerName_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            HudWindow_MouseDoubleClick(sender, e);
        }

    }
}
