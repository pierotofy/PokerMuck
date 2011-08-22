using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PokerMuck
{
    class HoldemTableDisplayWindow : TableDisplayWindow
    {
        public HoldemTableDisplayWindow(Table table) :
            base(table)
        {
        }

        protected override Hand CreateHandFromCardList(CardList cardList)
        {
            // We need exactly two cards in hold'em to have a hand
            if (cardList.Count == 2)
            {
                HoldemHand hand = new HoldemHand(cardList[0], cardList[1]);
                return hand;
            }
            else return null;
        }

        protected override void DisplayOdds(Hand playerHand)
        {
            // Display how the hand rates in Holdem
            float handPercentile = ((HoldemHand)playerHand).GetPrelopPercentile();
            HoldemHand.Rating rating = new HoldemHand.ClassificationPreflop(((HoldemHand)playerHand)).GetRating();
            Color rateColor = Color.FromArgb(184, 0, 0); // Red
            if (rating == HoldemHand.Rating.Monster || rating == HoldemHand.Rating.Strong)
            {
                rateColor = Color.FromArgb(0, 184, 48); // Green
            }
            else if (rating == HoldemHand.Rating.Mediocre || rating == HoldemHand.Rating.Weak)
            {
                rateColor = Color.FromArgb(255, 150, 0); // Orange
            }

            Label lblPreflopRate = new Label();
            lblPreflopRate.Font = new Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblPreflopRate.ForeColor = rateColor;
            lblPreflopRate.BackColor = Color.Transparent;
            lblPreflopRate.Height = 16;
            lblPreflopRate.Text = "Top " + Math.Round(handPercentile * 100, 2) + "%";
            lblPreflopRate.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);

            handControlLayout.Controls.Add(lblPreflopRate);

            base.DisplayOdds(playerHand);
        }
    }
}
