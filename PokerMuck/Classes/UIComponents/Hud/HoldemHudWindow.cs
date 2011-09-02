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
        /* Notify that we need to display the player's pushing range preflop */
        private delegate void OnPlayerPreflopPushingRangeNeedToBeDisplayedHandler(HoldemHudWindow sender);
        private event OnPlayerPreflopPushingRangeNeedToBeDisplayedHandler OnPlayerPreflopPushingRangeNeedToBeDisplayed;

        public HoldemHudWindow()
        {
            InitializeComponent();
        }

        public override void RegisterHandlers(Hud hud, Table t, Player p)
        {
            base.RegisterHandlers(hud, t, p);

            this.OnPlayerPreflopPushingRangeNeedToBeDisplayed += new OnPlayerPreflopPushingRangeNeedToBeDisplayedHandler(hud.holdemWindow_OnPlayerPreflopPushingRangeNeedToBeDisplayed);
        }

        void displayRaisingRangeToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (OnPlayerPreflopPushingRangeNeedToBeDisplayed != null) OnPlayerPreflopPushingRangeNeedToBeDisplayed(this);
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

            toggleIsSolidIcon(stats);

            toggleIsDonkIcon(stats);
            /*
            picCallingStation.Visible = true;
            picCallingStation.SetQuestionSignVisible(false);
            picEasySteal.Visible = true;
            picEasySteal.SetQuestionSignVisible(false);
            picButtonStealer.Visible = true;
            picButtonStealer.SetQuestionSignVisible(false);
            picSolidPlayer.Visible = true;
            picSolidPlayer.SetQuestionSignVisible(false);
            picDonkPlayer.Visible = true;
            picDonkPlayer.SetQuestionSignVisible(false);
            */
        }

        private void toggleIsDonkIcon(PlayerStatistics stats)
        {
            // If a person is betting/raising/check raising with nothing more than 10%, he's bluffing more than usual
            // 10% is considered to be a standard "bluffing percentage" you can expect. More than that, is donkish
            StatisticsData nothingBets = stats.Get("Bets", "Summary").FindSubStatistic(HoldemHand.Rating.Nothing.ToString());
            StatisticsData nothingRaises = stats.Get("Raises", "Summary").FindSubStatistic(HoldemHand.Rating.Nothing.ToString());
            StatisticsData nothingCheckRaises = stats.Get("Check Raise", "Summary").FindSubStatistic(HoldemHand.Rating.Nothing.ToString());

            float valueSums = nothingBets.Value +
                              nothingRaises.Value + 
                              nothingCheckRaises.Value;

            // To compute an average, we first have to see if the values are known (or are set to zero just because they are unknown)
            float divideBy = 0;
            if (!(nothingBets is StatisticsUnknownData))
            {
                divideBy += 1.0f;
            }
            if (!(nothingRaises is StatisticsUnknownData))
            {
                divideBy += 1.0f;
            }
            if (!(nothingCheckRaises is StatisticsUnknownData))
            {
                divideBy += 1.0f;
            }
            bool noInformation = (divideBy == 0);

            float valueAverage;
            if (noInformation) valueAverage = 0;
            else valueAverage = valueSums / divideBy;

            picDonkPlayer.Visible = (valueAverage >= 0.1) || noInformation;
            picDonkPlayer.SetQuestionSignVisible(noInformation);
        }

        private void toggleIsSolidIcon(PlayerStatistics stats)
        {
            // If a player raises and bets with value hands, he's solid (80% of bets and raises are value?)
            StatisticsData strongBets = stats.Get("Bets", "Summary").FindSubStatistic(HoldemHand.Rating.Strong.ToString());
            StatisticsData monsterBets = stats.Get("Bets", "Summary").FindSubStatistic(HoldemHand.Rating.Monster.ToString());

            StatisticsData strongRaises = stats.Get("Raises", "Summary").FindSubStatistic(HoldemHand.Rating.Strong.ToString());
            StatisticsData monsterRaises = stats.Get("Raises", "Summary").FindSubStatistic(HoldemHand.Rating.Monster.ToString());

            StatisticsData strongCheckRaises = stats.Get("Check Raise", "Summary").FindSubStatistic(HoldemHand.Rating.Strong.ToString());
            StatisticsData monsterCheckRaises = stats.Get("Check Raise", "Summary").FindSubStatistic(HoldemHand.Rating.Monster.ToString());


            float valueSums = strongBets.Value + monsterBets.Value +
                              strongRaises.Value + monsterRaises.Value +
                              strongCheckRaises.Value + monsterCheckRaises.Value;

            // To compute an average, we first have to see if the values are known (or are set to zero just because they are unknown)
            float divideBy = 0;
            if (!(strongRaises is StatisticsUnknownData && monsterRaises is StatisticsUnknownData))
            {
                divideBy += 1.0f;
            }
            if (!(strongBets is StatisticsUnknownData && monsterBets is StatisticsUnknownData))
            {
                divideBy += 1.0f;
            }
            if (!(strongCheckRaises is StatisticsUnknownData && monsterCheckRaises is StatisticsUnknownData))
            {
                divideBy += 1.0f;
            }
            bool noInformation = (divideBy == 0);

            float valueAverage;
            if (noInformation) valueAverage = 0;
            else valueAverage = valueSums / divideBy;

            picSolidPlayer.Visible = (valueAverage >= 0.8) || noInformation;
            picSolidPlayer.SetQuestionSignVisible(noInformation);
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
