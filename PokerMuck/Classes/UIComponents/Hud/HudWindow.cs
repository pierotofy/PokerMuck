using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace PokerMuck
{
    public partial class HudWindow : Form
    {
        private bool draggingWindow;
        private Point mousePositionOnDrag;

        public HudWindow()
        {
            InitializeComponent();
        }

        public void DisplayPlayerName(String playerName)
        {
            lblPlayerName.Text = playerName;
        }

        /* This function is overriden by child classes and takes care of displaying
         * game specific layout */
        public virtual void DisplayStatistics(Statistics stats)
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
    }
}
