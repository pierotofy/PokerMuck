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
    public partial class HoldemHudWindow : HudWindow
    {
        public HoldemHudWindow()
        {
            InitializeComponent();

            /*
            ToolTip t = new ToolTip();
            t.ShowAlways = true;

            t.InitialDelay = 0;
            t.AutoPopDelay = 0;
            t.ReshowDelay = 0;
            t.IsBalloon = true;

            t.SetToolTip(lblImmediateStats, "Test");
             * */

        }

        public override void DisplayStatistics(PlayerStatistics stats)
        {
            base.DisplayStatistics(stats);

            lblImmediateStats.Text = String.Format("VPF: {0} PFR: {1} L: {2}",
                stats.Get("Voluntary Put $", "Preflop").GetPercentage(),
                stats.Get("Raises", "Preflop").GetPercentage(),
                stats.Get("Limp", "Preflop").GetPercentage()
              );
        }

        private void lblImmediateStats_MouseUp(object sender, MouseEventArgs e)
        {
            HudWindow_MouseUp(sender, e);
        }
        private void lblImmediateStats_MouseMove(object sender, MouseEventArgs e)
        {
            HudWindow_MouseMove(sender, e);
        }
        private void lblImmediateStats_MouseDown(object sender, MouseEventArgs e)
        {
            HudWindow_MouseDown(sender, e);
        }

        private void HoldemHudWindow_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            HudWindow_MouseDoubleClick(sender, e);
        }
    }
}
