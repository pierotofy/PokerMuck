using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    class HoldemHand : Hand
    {
        public HoldemHand(Card first, Card second) : base()
        {
            AddCard(first);
            AddCard(second);
        }

        /* In hold'em we don't really care whether we have Kh 6h or Kc 6c,
         * they can both be referred to as K6s (king-six suited) */
        public override string ToString()
        {
            Card firstCard = cards[0];
            Card secondCard = cards[1];

            CardSuit firstSuit = firstCard.Suit;
            CardSuit secondSuit = secondCard.Suit;

            CardFace firstFace = firstCard.Face;
            CardFace secondFace = secondCard.Face;

            // Suited?
            string suited = "o"; // default to offsuit
            if (firstSuit == secondSuit) suited = "s";

            // If we have a pair, it's obviously offsuit
            if (firstFace == secondFace) suited = "";

            // Swap them so that the bigger card is echoed first (A6 instead of 6A)
            if (firstCard.GetFaceValue() < secondCard.GetFaceValue())
            {
                CardFace t = firstFace;
                firstFace = secondFace;
                secondFace = t;
            }

            string firstFaceStr = Card.CardFaceToChar(firstFace).ToString();
            string secondFaceStr = Card.CardFaceToChar(secondFace).ToString();            

            return firstFaceStr + secondFaceStr + suited;
        }
    }
}
