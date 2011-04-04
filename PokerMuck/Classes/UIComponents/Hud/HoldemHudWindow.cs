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
            picCallingStation.Visible = !(calls is StatisticsUnknownData);
            picCallingStation.SetForbiddenSignVisible(calls.Value < 0.40);

            StatisticsData foldSbToRaise = stats.Get("Fold Small Blind to a Raise", "Preflop");
            StatisticsData foldBbToRaise = stats.Get("Fold Big Blind to a Raise", "Preflop");

            // If average of fold big blind to a raise and fold small blind to a raise > 80% then easy steal
            StatisticsData foldBlindAverage = foldSbToRaise.Average("Fold Small/Big Blind to a Raise", "", 2, foldBbToRaise);
            picEasySteal.Visible = !(foldBlindAverage is StatisticsUnknownData);
            picEasySteal.SetForbiddenSignVisible(foldBlindAverage.Value < 0.80);

            // If a person raises more than 50% of his buttons, chances are he might be stealing
            StatisticsData stealRaises = stats.Get("Steal Raises", "Preflop");
            picButtonStealer.Visible = !(stealRaises is StatisticsUnknownData);
            picButtonStealer.SetForbiddenSignVisible(stealRaises.Value < 0.5);

            // If a person wins 80% or more at showdown, he's a solid player (or lucky, but this is what we have)
            StatisticsData wonAtShowdownStats = stats.Get("Won at Showdown", "Summary");
            picSolidPlayer.Visible = !(wonAtShowdownStats is StatisticsUnknownData);
            picSolidPlayer.SetForbiddenSignVisible(wonAtShowdownStats.Value < 0.8);
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
