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
        }

        public override void DisplayStatistics(PlayerStatistics stats)
        {
            base.DisplayStatistics(stats);

            lblImmediateStats.Text = String.Format("VPF: {0} PFR: {1} L: {2} C: {3}",
                stats.GetPercentage("VPF"),
                stats.GetPercentage("PFR"),
                stats.GetPercentage("Limp"),
                stats.GetPercentage("CBet")
              );
        }
    }
}
