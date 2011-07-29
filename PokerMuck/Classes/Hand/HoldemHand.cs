using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    public class HoldemHand : Hand
    {
        public class Classification {
            public enum HandType { Unknown, HighCard, Pair, TwoPair, ThreeOfAKind, Straight, Flush, FullHouse, FourOfAKind, RoyalFlush }
            public enum KickerType { Unknown, Irrelevant, Low, Middle, High }
            public enum PairType { Irrelevant, Bottom, Middle, Top }

            public HandType Hand { get; set; }
            public KickerType Kicker { get; set; }
            public PairType Pair { get; set; }

            public Classification(HandType hand, KickerType kicker)
            {
                this.Hand = hand;
                this.Kicker = kicker;
                this.Pair = PairType.Irrelevant;
            }

            public Classification(HandType hand, KickerType kicker, PairType pair)
                : this(hand, kicker)
            {
                this.Pair = pair;
            }

            public Rating GetRating(){
                if (Hand == HandType.HighCard)
                {
                    return Rating.Nothing;
                }
                else if (Hand == HandType.Pair)
                {
                    if ((Pair == PairType.Bottom) ||
                       ((Pair == PairType.Middle && Kicker == KickerType.Low)))
                    {
                        return Rating.Weak;
                    }

                    if ((Pair == PairType.Middle && (Kicker == KickerType.Middle || Kicker == KickerType.High)) ||
                         (Pair == PairType.Top && (Kicker == KickerType.Low || Kicker == KickerType.Middle)))
                    {
                        return Rating.Mediocre;
                    }

                    if ((Pair == PairType.Top && (Kicker == KickerType.High)))
                    {
                        return Rating.Strong;
                    }
                }
                else if (Hand == HandType.TwoPair || Hand == HandType.ThreeOfAKind)
                {
                    return Rating.Strong;
                }
                
                return Rating.Monster;
            }

            public override string ToString()
            {
                return Hand.ToString() + ", " + Kicker.ToString() + " kicker, " + Pair.ToString() + " pair - " + GetRating().ToString();
            }
        }

        public enum Rating { Nothing, Weak, Mediocre, Strong, Monster }

        public HoldemHand(Card first, Card second) : base()
        {
            AddCard(first);
            AddCard(second);
        }

        private Card GetFirstCard()
        {
            return cards[0];
        }
        private Card GetSecondCard()
        {
            return cards[1];
        }

        /* In hold'em we don't really care whether we have Kh 6h or Kc 6c,
         * they can both be referred to as K6s (king-six suited) */
        public override string ToString()
        {
            Card firstCard = GetFirstCard();
            Card secondCard = GetSecondCard();

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

        public HoldemHand.Rating GetHandRating(HoldemGamePhase phase, HoldemBoard board)
        {
            HoldemHand.Classification classification = GetClassification(phase, board);
            Debug.Print(String.Format("Rating hand [{0}] for {1} on board [{2}]: {3}", base.ToString(), phase.ToString(), board.ToString(), classification.ToString()));
            return classification.GetRating();
        }

        /* Calculates the type of the hand given a board
         * during a game phase */
        public HoldemHand.Classification GetClassification(HoldemGamePhase phase, HoldemBoard board)
        {
            return CalculateClassification(board.GetBoardAt(phase));
        }

        private HoldemHand.Classification CalculateClassification(CardList communityCards)
        {
            Debug.Assert(communityCards.Count > 0, "Cannot classificate an empty list of community cards.");

            // Create a new list including the board cards and the cards from the hand
            CardList cards = new CardList(communityCards.Count + 2);
            foreach (Card c in communityCards) cards.AddCard(c);
            cards.AddCard(this.GetFirstCard());
            cards.AddCard(this.GetSecondCard());

            // --- Royal flush

            if (IsRoyalFlush(cards))
            {
                return new Classification(Classification.HandType.RoyalFlush, Classification.KickerType.Irrelevant);
            }

            // -- Four of a kind
            if (cards.HaveIdenticalFaces(4))
            {
                return new Classification(Classification.HandType.FourOfAKind, Classification.KickerType.Irrelevant);
            }

            // -- Full House
            // If we have three of a kind and two pair at the same time, we have a full house
            bool isThreeOfAKind = cards.HaveIdenticalFaces(3);
            bool isTwoPair = IsTwoPair(cards);
            if (isThreeOfAKind && isTwoPair)
            {
                return new Classification(Classification.HandType.FullHouse, Classification.KickerType.Irrelevant);
            }

            // -- Flush
            for (int i = 0; i < cards.Count; i++)
            {
                int numCardsSameSuit = 0;
                for (int j = i + 1; j < cards.Count; j++)
                {
                    if (cards[i].Suit == cards[j].Suit)
                    {
                        numCardsSameSuit++;
                    }
                }

                if (numCardsSameSuit >= 4)
                {
                    return new Classification(Classification.HandType.Flush, Classification.KickerType.Irrelevant);
                }
            }

            // -- Straight
            if (IsStraight(cards))
            {
                return new Classification(Classification.HandType.Straight, Classification.KickerType.Irrelevant);
            }

            // -- Trips
            if (isThreeOfAKind)
            {
                return new Classification(Classification.HandType.ThreeOfAKind, Classification.KickerType.Irrelevant);
            }

            // -- Two pair
            if (isTwoPair)
            {
                return new Classification(Classification.HandType.TwoPair, Classification.KickerType.Irrelevant);
            }

            // -- Pair
            Card matching;
            if (cards.HaveIdenticalFaces(2, out matching))
            {
                // Sort list by face value (ace high first)
                cards.Sort(SortUsing.AceHigh);
                
                // Find kicker (check from end of the list where face values are higher)
                Card kicker = cards[0];
                for (int i = cards.Count - 1; i >= 0; i--)
                {
                    if (cards[i].Face != matching.Face)
                    {
                        kicker = cards[i];
                        break;
                    }
                }

                Classification.KickerType kickerType = GetKickerTypeFromCard(kicker);
                Classification.PairType pairType = GetPairType(communityCards, matching);
                return new Classification(Classification.HandType.Pair, kickerType, pairType);
            }

            // -- High card
            cards.Sort(SortUsing.AceHigh);
            Card highCard = cards.Last;
            
            return new Classification(Classification.HandType.HighCard, GetKickerTypeFromCard(highCard));
        }

        /* cards must be sorted before calling this method */
        private Classification.PairType GetPairType(CardList communityCards, Card matching)
        {
            communityCards.Sort(SortUsing.AceHigh);

            Card firstCard = this.GetFirstCard();
            Card secondCard = this.GetSecondCard();

            // Pocket pair
            if (firstCard.Face == secondCard.Face)
            {
                if (firstCard.GetFaceValue() >= communityCards.Last.GetFaceValue()) return Classification.PairType.Top;
                else if (firstCard.GetFaceValue() <= communityCards[0].GetFaceValue()) return Classification.PairType.Bottom;
                else return Classification.PairType.Middle;
            }
            else
            {
                // Matched the board
                if (matching.Face == communityCards.Last.Face) return Classification.PairType.Top;
                else if (matching.Face == communityCards[0].Face) return Classification.PairType.Bottom;
                else return Classification.PairType.Middle;
            }
        }

        private Classification.KickerType GetKickerTypeFromCard(Card c)
        {
            switch (c.Face)
            {
                case CardFace.Ace:
                case CardFace.King:
                case CardFace.Queen:
                case CardFace.Jack:
                     return  Classification.KickerType.High;
                case CardFace.Ten:
                case CardFace.Nine:
                case CardFace.Eight:
                case CardFace.Seven:
                    return Classification.KickerType.Middle;
                case CardFace.Six:
                case CardFace.Five:
                case CardFace.Four:
                case CardFace.Three:
                case CardFace.Two:
                    return Classification.KickerType.Low;
                default:
                    return Classification.KickerType.Unknown;
            }
        }

        private bool IsTwoPair(CardList cards)
        {
            // Keep track of which pair face we have already found
            int firstPairFace = -1;
            bool foundFirstPair = false;
            bool foundSecondPair = false;

            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = i + 1; j < cards.Count; j++)
                {
                    // firstPairFace is always != face the first set of iterations
                    if (cards[i].Face == cards[j].Face && 
                        ((int)cards[i].Face != firstPairFace))
                    {
                        if (foundFirstPair)
                        {
                            foundSecondPair = true;
                        }
                        else
                        {
                            foundFirstPair = true;
                            firstPairFace = (int)cards[i].Face;
                        }
                        break;
                    }
                }
            }

            return foundSecondPair;
        }



        private bool IsStraight(CardList cards)
        {
            if (cards.AreConsecutive(true, false, 5))
            {
                return true;
            }

            if (cards.AreConsecutive(false, false, 5))
            {
                return true;
            }

            return false;
        }

        private bool IsRoyalFlush(CardList cards)
        {
            if (cards.AreConsecutive(true, true, 5))
            {
                return true;
            }

            if (cards.AreConsecutive(false, true, 5))
            {
                return true;
            }

            return false;
        }
    }
}
