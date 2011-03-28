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

            /* Create tooltips */
            ToolTip t = new ToolTip();
            t.ShowAlways = true;

            t.InitialDelay = 0;
            t.AutoPopDelay = 0;
            t.ReshowDelay = 0;
            t.IsBalloon = true;

            t.SetToolTip(picEasySteal, "Easy blind steal");
            t.SetToolTip(picButtonStealer, "Frequent blind stealer");
            t.SetToolTip(picCallingStation, "Calling station");
            t.SetToolTip(picSolidPlayer, "Solid Player");
        }

        public override void DisplayStatistics(PlayerStatistics stats)
        {
            base.DisplayStatistics(stats);

            lblImmediateStats.Text = String.Format("VPF:{0} PFR:{1} L:{2} AF:{3}",
                stats.Get("Voluntary Put $", "Preflop").GetPercentage(),
                stats.Get("Raises", "Preflop").GetPercentage(),
                stats.Get("Limp", "Preflop").GetPercentage(),
                stats.Get("Aggression Frequency", "Summary").GetValue()
              );

            DisplayIcons(stats);
        }

        public void DisplayIcons(PlayerStatistics stats)
        {
            StatisticsData calls = stats.Get("Calls", "Summary");

            // If calls are > 40% then calling station!
            float callingStationValue = calls.Value;
            picCallingStation.Visible = (callingStationValue >= 0.40);

            StatisticsData foldSbToSteal = stats.Get("Fold Small Blind to a Steal Raise", "Preflop");
            StatisticsData foldBbToSteal = stats.Get("Fold Big Blind to a Steal Raise", "Preflop");

            // If average of fold big blind to a steal and fold small blind to a steal > 75% then easy steal
            float easyStealValue = foldSbToSteal.Average("Fold Small/Big Blind to a Steal Raise", "", 2, foldBbToSteal).Value;
            picEasySteal.Visible = (easyStealValue >= 0.75);

            // If a person raises more than 50% of his buttons, chances are he might be stealing
            StatisticsData stealRaises = stats.Get("Steal Raises", "Preflop");
            float stealerValue = stealRaises.Value;
            picButtonStealer.Visible = (stealerValue >= 0.5);

            // If a person wins 80% or more at showdown, he's a solid player
            StatisticsData wonAtShowdownStats = stats.Get("Won at Showdown", "Summary");
            float solidPlayerValue = wonAtShowdownStats.Value;
            picSolidPlayer.Visible = (solidPlayerValue >= 0.8);
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
