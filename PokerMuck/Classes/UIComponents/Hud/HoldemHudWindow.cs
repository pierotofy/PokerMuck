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
                stats.Get("Voluntary Put $", "Preflop").MainData.GetPercentage(),
                stats.Get("Raises", "Preflop").MainData.GetPercentage(),
                stats.Get("Limp", "Preflop").MainData.GetPercentage(),
                stats.Get("Aggression Frequency", "Summary").MainData.GetValue()
              );

            DisplayIcons(stats);
        }

        public void DisplayIcons(PlayerStatistics stats)
        {
            StatisticsData calls = stats.Get("Calls", "Summary").MainData;
            StatisticsData checkCall = stats.Get("Check Call", "Summary").MainData;
            // If calls are > 40% or check calls are more than 66% then calling station!
            picCallingStation.Visible = (calls.Value >= 0.40 || checkCall.Value >= 0.66) || (calls is StatisticsUnknownData && checkCall is StatisticsUnknownData);
            picCallingStation.SetQuestionSignVisible(calls is StatisticsUnknownData && checkCall is StatisticsUnknownData);

            StatisticsData foldSbToRaise = stats.Get("Fold Small Blind to a Raise", "Preflop").MainData;
            StatisticsData foldBbToRaise = stats.Get("Fold Big Blind to a Raise", "Preflop").MainData;

            // If average of fold big blind to a raise and fold small blind to a raise > 80% then easy steal
            StatisticsData foldBlindAverage = foldSbToRaise.Average("Fold Small/Big Blind to a Raise", 2, foldBbToRaise);
            picEasySteal.Visible = (foldBlindAverage.Value >= 0.80) || (foldBlindAverage is StatisticsUnknownData);
            picEasySteal.SetQuestionSignVisible(foldBlindAverage is StatisticsUnknownData);

            // If a person raises more than 50% of his buttons, chances are he might be stealing
            StatisticsData stealRaises = stats.Get("Steal Raises", "Preflop").MainData;
            picButtonStealer.Visible = (stealRaises.Value >= 0.5) || (stealRaises is StatisticsUnknownData);
            picButtonStealer.SetQuestionSignVisible(stealRaises is StatisticsUnknownData);

            // If a person wins 80% or more at showdown, he's a solid player (or lucky, but this is what we have)
            StatisticsData wonAtShowdownStats = stats.Get("Won at Showdown", "Summary").MainData;
            picSolidPlayer.Visible = (wonAtShowdownStats.Value >= 0.8) || (wonAtShowdownStats is StatisticsUnknownData);
            picSolidPlayer.SetQuestionSignVisible(wonAtShowdownStats is StatisticsUnknownData);
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
