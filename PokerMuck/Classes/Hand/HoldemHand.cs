using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace PokerMuck
{
    public class HoldemHand : Hand
    {
        #region Hand Percentiles Hash Table 
        private static Hashtable preflopPercentiles = new Hashtable()
        {
            {"AA", 0.005f},
            {"KK", 0.009f},
            {"QQ", 0.014f},
            {"JJ", 0.018f},
            {"TT", 0.026f},
            {"AKs", 0.026f},
            {"99", 0.03f},
            {"AQs", 0.033f},
            {"AKo", 0.042f},
            
            {"AJs", 0.048f},
            {"KQs", 0.048f},

            {"ATs", 0.056f},
            {"88", 0.056f},

            {"AQo", 0.065f},

            {"KJs", 0.071f},
            {"KTs", 0.071f},
            
            {"QJs", 0.074f},

            {"AJo", 0.083f},

            {"KQo", 0.092f},

            {"QTs", 0.095f},

            {"A9s", 0.098f},

            {"77", 0.103f},
            
            {"ATo", 0.112f},

            {"JTs", 0.115f},

            {"KJo", 0.124f},

            {"A8s", 0.130f},
            {"K9s", 0.130f},

            {"QJo", 0.139f},

            {"A7s", 0.142f},

            {"KTo", 0.151f},

            {"Q9s", 0.154f},

            {"A5s", 0.161f},
            
            {"66", 0.164f},
            {"A6s", 0.164f},

            {"QTo", 0.173f},

            {"J9s", 0.176f},
            
            {"A9o", 0.186f},

            {"T9s", 0.189f},
            
            {"A4s", 0.195f},
            {"K8s", 0.195f},

            {"JTo", 0.204f},

            {"K7s", 0.207f},

            {"A8o", 0.216f},

            {"A3s", 0.219f},

            {"Q8s", 0.222f},

            {"K9o", 0.231f},

            {"A2s", 0.237f},
            {"K6s", 0.237f},

            {"J8s", 0.243f},
            {"T8s", 0.243f},

            {"A7o", 0.252f},

            {"55", 0.256f},

            {"Q9o", 0.265f},

            {"K5s", 0.271f},
            {"98s", 0.271f},

            {"Q7s", 0.275f},

            {"J9o", 0.284f},

            {"A5o", 0.293f},

            {"T9o", 0.302f},
            
            {"A6o", 0.311f},
            
            {"K4s", 0.314f},
            
            {"K8o", 0.323f},

            {"Q6s", 0.326f},

            {"J7s", 0.332f},
            {"T7s", 0.332f},

            {"A4o", 0.341f},

            {"K3s", 0.347f},
            {"97s", 0.347f},

            {"Q5s", 0.353f},
            {"87s", 0.353f},

            {"K7o", 0.362f},

            {"44", 0.367f},

            {"Q8o", 0.376f},

            {"A3o", 0.385f},

            {"K2s", 0.388f},

            {"J8o", 0.397f},

            {"Q4s", 0.400f},

            {"T8o", 0.409f},
            
            {"J6s", 0.412f},

            {"K6o", 0.421f},

            {"A2o", 0.430f},

            {"T6s", 0.433f},

            {"98o", 0.442f},

            {"86s", 0.448f},
            {"76s", 0.448f},

            {"Q3s", 0.454f},
            {"96s", 0.454f},

            {"J5s", 0.457f},

            {"K5o", 0.466f},

            {"Q7o", 0.475f},

            {"Q2s", 0.478f},

            {"J4s", 0.486f},
            {"33", 0.486f},

            {"65s", 0.489f},

            {"J7o", 0.498f},
            
            {"T7o", 0.507f},

            {"K4o", 0.516f},

            {"T5s", 0.522f},
            {"75s", 0.522f},
            
            {"Q6o", 0.531f},
            
            {"J3s", 0.534f},

            {"95s", 0.537f},

            {"87o", 0.546f},

            {"85s", 0.549f},
            
            {"97o", 0.558f},
            
            {"T4s", 0.561f},

            {"K3o", 0.570f},

            {"J2s", 0.576f},
            {"54s", 0.576f},

            {"Q5o", 0.585f},
            
            {"64s", 0.588f},

            {"T3s", 0.596f},
            {"22", 0.596f},
            
            {"K2o", 0.605f},

            {"74s", 0.608f},

            {"76o", 0.617f},

            {"T2s", 0.620f},

            {"Q4o", 0.629f},

            {"J6o", 0.638f},
            
            {"84s", 0.641f},

            {"94s", 0.644f},
            
            {"86o", 0.653f},
            {"T6o", 0.662f},

            {"96o", 0.674f},
            {"53s", 0.674f},

            {"93s", 0.677f},

            {"Q3o", 0.686f},
            
            {"J5o", 0.695f},

            {"63s", 0.698f},
            
            {"92s", 0.704f},
            {"43s", 0.704f},

            {"73s", 0.707f},

            {"65o", 0.716f},

            {"Q2o", 0.725f},
            
            {"J4o", 0.735f},

            {"83s", 0.738f},

            {"75o", 0.747f},
            
            {"52s", 0.750f},

            {"85o", 0.759f},
            
            {"82s", 0.762f},

            {"T5o", 0.771f},

            {"95o", 0.780f},

            {"J3o", 0.789f},

            {"62s", 0.792f},

            {"54o", 0.801f},

            {"42s", 0.804f},
            
            {"T4o", 0.813f},
            
            {"J2o", 0.822f},

            {"72s", 0.825f},
            
            {"64o", 0.834f},
            
            {"T3o", 0.843f},

            {"32s", 0.846f},

            {"74o", 0.855f},

            {"84o", 0.864f},

            {"T2o", 0.873f},

            {"94o", 0.882f},
            
            {"53o", 0.891f},
            
            {"93o", 0.900f},
            
            {"63o", 0.910f},

            {"43o", 0.919f},

            {"92o", 0.928f},
            
            {"73o", 0.937f},
            
            {"83o", 0.946f},

            {"52o", 0.955f},
            {"82o", 0.964f},
            {"42o", 0.973f},
            {"62o", 0.982f},
            {"72o", 0.991f},
            {"32o", 1.000f}
        };

        /* Checks that every possible hand is covered in the table */
        public static void TestPercentiles()
        {
            for (CardFace face1 = CardFace.Ace; face1 <= CardFace.King; face1++)
            {
                for (CardSuit suit1 = CardSuit.Clubs; suit1 <= CardSuit.Spades; suit1++)
                {
                    Card c1 = new Card(face1, suit1);

                    for (CardFace face2 = CardFace.Ace; face2 <= CardFace.King; face2++)
                    {
                        for (CardSuit suit2 = CardSuit.Clubs; suit2 <= CardSuit.Spades; suit2++)
                        {
                            Card c2 = new Card(face2, suit2);

                            HoldemHand hand = new HoldemHand(c1, c2);
                            Debug.Assert(HoldemHand.preflopPercentiles.ContainsKey(hand.ToString()), "Percentile not found for: " + hand.ToString());
                        }
                    }
                }
            }
        }
        #endregion

        public abstract class Classification
        {
            public abstract Rating GetRating();
        }

        public class ClassificationPreflop : Classification
        {
            //public enum HandType { Unknown, PocketPair, SuitedConnectors, Connectors, TwoHighCards, ... }

            /* Between 0 and 1 (0% - 100%) indicates the percentile of the hand preflop */
            private float percentile;

            public ClassificationPreflop(HoldemHand hand)
            {
                this.percentile = (float)HoldemHand.preflopPercentiles[hand.ToString()];
            }

            public override Rating GetRating()
            {
                if (percentile <= 0.10f)
                {
                    return Rating.Strong;
                }
                else if (percentile <= 0.25f)
                {
                    return Rating.Mediocre;
                }
                else if (percentile <= 0.50f)
                {
                    return Rating.Weak;
                }
                else
                {
                    return Rating.Nothing;
                }
            }

            public override string ToString()
            {
                return "In the top " + (percentile * 100) + "% of hands - " + GetRating().ToString();
            }
        }

        public class ClassificationPostflop : Classification {
            public enum HandType { Unknown, HighCard, Pair, TwoPair, ThreeOfAKind, Straight, Flush, FullHouse, FourOfAKind, RoyalFlush }
            public enum KickerType { Unknown, Irrelevant, Low, Middle, High }
            public enum PairType { Irrelevant, Bottom, Middle, Top }
            
            private HandType hand { get; set; }
            private KickerType kicker { get; set; }
            private PairType pair { get; set; }

            public ClassificationPostflop(HandType hand, KickerType kicker)
            {
                this.hand = hand;
                this.kicker = kicker;
                this.pair = PairType.Irrelevant;
            }

            public ClassificationPostflop(HandType hand, KickerType kicker, PairType pair)
                : this(hand, kicker)
            {
                this.pair = pair;
            }

            public override Rating GetRating(){
                if (hand == HandType.HighCard)
                {
                    return Rating.Nothing;
                }
                else if (hand == HandType.Pair)
                {
                    if ((pair == PairType.Bottom) ||
                       ((pair == PairType.Middle && kicker == KickerType.Low)))
                    {
                        return Rating.Weak;
                    }

                    if ((pair == PairType.Middle && (kicker == KickerType.Middle || kicker == KickerType.High)) ||
                         (pair == PairType.Top && (kicker == KickerType.Low || kicker == KickerType.Middle)))
                    {
                        return Rating.Mediocre;
                    }

                    if ((pair == PairType.Top && (kicker == KickerType.High)))
                    {
                        return Rating.Strong;
                    }
                }
                else if (hand == HandType.TwoPair || hand == HandType.ThreeOfAKind)
                {
                    return Rating.Strong;
                }
                
                return Rating.Monster;
            }

            public static PairType GetPairType(CardList communityCards, Card matching, Card firstCard, Card secondCard)
            {
                communityCards.Sort(SortUsing.AceHigh);

                // Pocket pair
                if (firstCard.Face == secondCard.Face)
                {
                    if (firstCard.GetFaceValue() >= communityCards.Last.GetFaceValue()) return PairType.Top;
                    else if (firstCard.GetFaceValue() <= communityCards[0].GetFaceValue()) return PairType.Bottom;
                    else return PairType.Middle;
                }
                else
                {
                    // Matched the board
                    if (matching.Face == communityCards.Last.Face) return PairType.Top;
                    else if (matching.Face == communityCards[0].Face) return PairType.Bottom;
                    else return PairType.Middle;
                }
            }

            public static KickerType GetKickerTypeFromCard(Card c)
            {
                switch (c.Face)
                {
                    case CardFace.Ace:
                    case CardFace.King:
                    case CardFace.Queen:
                    case CardFace.Jack:
                        return KickerType.High;
                    case CardFace.Ten:
                    case CardFace.Nine:
                    case CardFace.Eight:
                    case CardFace.Seven:
                        return KickerType.Middle;
                    case CardFace.Six:
                    case CardFace.Five:
                    case CardFace.Four:
                    case CardFace.Three:
                    case CardFace.Two:
                        return KickerType.Low;
                    default:
                        return KickerType.Unknown;
                }
            }


            public override string ToString()
            {
                return hand.ToString() + ", " + kicker.ToString() + " kicker, " + pair.ToString() + " pair - " + GetRating().ToString();
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
            if (phase == HoldemGamePhase.Preflop)
            {
                return new ClassificationPreflop(this);
            }
            else
            {
                return CalculatePostflopClassification(board.GetBoardAt(phase));
            }
        }

        private HoldemHand.Classification CalculatePostflopClassification(CardList communityCards)
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
                return new ClassificationPostflop(ClassificationPostflop.HandType.RoyalFlush, ClassificationPostflop.KickerType.Irrelevant);
            }

            // -- Four of a kind
            if (cards.HaveIdenticalFaces(4))
            {
                return new ClassificationPostflop(ClassificationPostflop.HandType.FourOfAKind, ClassificationPostflop.KickerType.Irrelevant);
            }

            // -- Full House
            // If we have three of a kind and two pair at the same time, we have a full house
            bool isThreeOfAKind = cards.HaveIdenticalFaces(3);
            bool isTwoPair = IsTwoPair(cards);
            if (isThreeOfAKind && isTwoPair)
            {
                return new ClassificationPostflop(ClassificationPostflop.HandType.FullHouse, ClassificationPostflop.KickerType.Irrelevant);
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
                    return new ClassificationPostflop(ClassificationPostflop.HandType.Flush, ClassificationPostflop.KickerType.Irrelevant);
                }
            }

            // -- Straight
            if (IsStraight(cards))
            {
                return new ClassificationPostflop(ClassificationPostflop.HandType.Straight, ClassificationPostflop.KickerType.Irrelevant);
            }

            // -- Trips
            if (isThreeOfAKind)
            {
                return new ClassificationPostflop(ClassificationPostflop.HandType.ThreeOfAKind, ClassificationPostflop.KickerType.Irrelevant);
            }

            // -- Two pair
            if (isTwoPair)
            {
                return new ClassificationPostflop(ClassificationPostflop.HandType.TwoPair, ClassificationPostflop.KickerType.Irrelevant);
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

                ClassificationPostflop.KickerType kickerType = ClassificationPostflop.GetKickerTypeFromCard(kicker);
                ClassificationPostflop.PairType pairType = ClassificationPostflop.GetPairType(communityCards, matching, this.GetFirstCard(), this.GetSecondCard());
                return new ClassificationPostflop(ClassificationPostflop.HandType.Pair, kickerType, pairType);
            }

            // -- High card
            cards.Sort(SortUsing.AceHigh);
            Card highCard = cards.Last;

            return new ClassificationPostflop(ClassificationPostflop.HandType.HighCard, ClassificationPostflop.GetKickerTypeFromCard(highCard));
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
