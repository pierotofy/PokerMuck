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
            CardSuit firstSuit = cards[0].Suit;
            CardSuit secondSuit = cards[1].Suit;

            CardFace firstFace = cards[0].Face;
            CardFace secondFace = cards[1].Face;

            // Suited?
            string suited = "o"; // default to offsuit
            if (firstSuit == secondSuit) suited = "s";

            // If we have a pair, it's obviously offsuit
            if (firstFace == secondFace) suited = "";

            // Swap them so that the bigger card is echoed first (A6 instead of 6A)
            if ((int)firstFace < (int)secondFace)
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
