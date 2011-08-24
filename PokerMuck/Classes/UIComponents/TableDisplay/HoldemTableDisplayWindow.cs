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
        HoldemBoard lastBoard;

        public HoldemTableDisplayWindow(Table table) :
            base(table)
        {
        }

        public void DisplayBoard(CardList boardCards)
        {
            HoldemBoard board = CreateBoardFromCardList(boardCards);
            if (board != null)
            {
                UpdateBoardOdds(board);
                lastBoard = board;
            }
        }

        public override void ClearHandInformation()
        {
            lastBoard = null;
            base.ClearHandInformation();
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

        protected bool BoardHasChanged(HoldemBoard board)
        {
            return lastBoard == null || !lastBoard.Equals(board);
        }


        /* Note that we are sharing the handcontrollayout between the board and the player cards.
         * This is going to work OK because the DisplayPlayerHand method is going to be called ALWAYS
         * before the DisplayBoard. Subsequent calls to DisplayPlayerHand will not change the UI as the hand
         * is going to be the same */
        protected void UpdateBoardOdds(HoldemBoard board)
        {
            // Update only if the hand is different than the previous one and we have a player hand
            if (BoardHasChanged(board) && lastPlayerHand != null)
            {
                // Clear previous stuff
                handControlLayout.Controls.Clear();

                // Display current hand
                CardListPanel playerCardsPanel = new CardListPanel();
                playerCardsPanel.BackColor = Color.Transparent;
                playerCardsPanel.CardSpacing = CARD_SPACING;
                playerCardsPanel.BorderPadding = CARD_BORDER_PADDING;

                playerCardsPanel.CardListToDisplay = lastPlayerHand;
                playerCardsPanel.Height = 70;
                handControlLayout.Controls.Add(playerCardsPanel);

                // Display board
                CardListPanel boardCardsPanel = new CardListPanel();
                boardCardsPanel.BackColor = Color.Transparent;
                boardCardsPanel.CardSpacing = CARD_SPACING;
                boardCardsPanel.BorderPadding = CARD_BORDER_PADDING;

                boardCardsPanel.CardListToDisplay = board;
                boardCardsPanel.Height = 70;
                handControlLayout.Controls.Add(boardCardsPanel);

                DisplayOdds((HoldemHand)lastPlayerHand, board);
            }
        }

        protected void DisplayOdds(HoldemHand playerHand, HoldemBoard board)
        {
            // Display odds
            List<Statistic> odds = ((HoldemOddsCalculator)oddsCalculator).Calculate(playerHand, board);

            StatisticItemListDisplay statisticListDisplay = CreateStatisticItemListDisplay();
            statisticListDisplay.Add(odds);

            handControlLayout.Controls.Add(statisticListDisplay);
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

        private HoldemBoard CreateBoardFromCardList(CardList cardList)
        {
            if (cardList.Count >= 3 && cardList.Count <= 5)
            {
                HoldemBoard result = new HoldemBoard(cardList);
                return result;
            }

            return null;
        }
    }
}
