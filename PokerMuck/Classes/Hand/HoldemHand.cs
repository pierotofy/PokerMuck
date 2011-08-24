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
        public enum Rating { Nothing, Weak, Mediocre, Strong, Monster }

        #region Hand Percentiles Hash Table and methods
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

        public static List<String> GetCardsWithinPercentile(float percentile)
        {
            List<String> result = new List<String>();

            foreach (String cards in preflopPercentiles.Keys)
            {
                if ((float)preflopPercentiles[cards] <= percentile)
                {
                    result.Add(cards);
                }
            }

            return result;
        }

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
                            Trace.Assert(HoldemHand.preflopPercentiles.ContainsKey(hand.ToString()), "Percentile not found for: " + hand.ToString());
                        }
                    }
                }
            }
        }
        #endregion

        #region Classification classes

        public abstract class Classification
        {
            public abstract Rating GetRating();
        }

        public class ClassificationPreflop : Classification
        {
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
            public enum DrawType { Irrelevant, None, Flush, Straight, FlushAndStraight }
            public enum StraightDrawType { None, InsideStraightDraw, OpenEndedStraightDraw }

            private HandType hand;
            private KickerType kicker;
            private PairType pair;
            private DrawType draw;
            private StraightDrawType straightDraw;

            public ClassificationPostflop(HoldemHand hand, HoldemGamePhase phase, HoldemBoard board)
            {
                CardList communityCards = board.GetBoardAt(phase);

                // Default
                this.hand = HandType.Unknown;
                this.kicker = KickerType.Unknown;
                this.pair = PairType.Irrelevant;
                this.draw = DrawType.Irrelevant;
                this.straightDraw = StraightDrawType.None;

                Trace.Assert(communityCards.Count > 0, "Cannot classificate an empty list of community cards.");

                // Create a new list including the board cards and the cards from the hand
                CardList cards = new CardList(communityCards.Count + 2);
                foreach (Card c in communityCards) cards.AddCard(c);
                cards.AddCard(hand.GetFirstCard());
                cards.AddCard(hand.GetSecondCard());

                // --- Royal flush
                if (IsRoyalFlush(cards))
                {
                    this.hand = HandType.RoyalFlush;
                    this.kicker = KickerType.Irrelevant;
                    return;
                }

                // -- Four of a kind
                if (cards.HaveIdenticalFaces(4))
                {
                    this.hand = HandType.FourOfAKind;
                    this.kicker = KickerType.Irrelevant;
                    return;
                }

                // -- Full House
                // If we have three of a kind and two pair at the same time, we have a full house
                bool isThreeOfAKind = cards.HaveIdenticalFaces(3);
                bool isTwoPair = IsTwoPair(cards);
                if (isThreeOfAKind && isTwoPair)
                {
                    this.hand = HandType.FullHouse;
                    this.kicker = KickerType.Irrelevant;
                    return;
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
                        this.hand = HandType.Flush;
                        this.kicker = KickerType.Irrelevant;
                        return;
                    }
                }

                // -- Straight
                if (IsStraight(cards))
                {
                    this.hand = HandType.Straight;
                    this.kicker = KickerType.Irrelevant;
                    return;
                }

                // Calculate draws (if we got until here, there might be some)
                // Also, no draws are possible at the river
                if (phase == HoldemGamePhase.River)
                {
                    draw = DrawType.None;
                    straightDraw = StraightDrawType.None;
                }
                else
                {
                    draw = GetDrawType(cards);

                    if (IsInsideStraightDraw(cards))
                    {
                        straightDraw = StraightDrawType.InsideStraightDraw;
                    }
                    
                    if (IsOpenEndedStraightDraw(cards))
                    {
                        straightDraw = StraightDrawType.OpenEndedStraightDraw;
                    }
                }

                // -- Trips
                if (isThreeOfAKind)
                {
                    this.hand = HandType.ThreeOfAKind;
                    this.kicker = KickerType.Irrelevant;
                    return;
                }

                // -- Two pair
                if (isTwoPair)
                {
                    this.hand = HandType.TwoPair;
                    this.kicker = KickerType.Irrelevant;
                    return;
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


                    this.hand = HandType.Pair;
                    this.kicker = GetKickerTypeFromCard(kicker);
                    this.pair = GetPairType(communityCards, matching, hand.GetFirstCard(), hand.GetSecondCard());
                    return;
                }

                // -- High card
                cards.Sort(SortUsing.AceHigh);
                Card highCard = cards.Last;

                this.hand = HandType.HighCard;
                this.kicker = GetKickerTypeFromCard(highCard);
            }

            public HandType GetHand()
            {
                return hand;
            }

            public StraightDrawType GetStraightDraw()
            {
                return straightDraw;
            }

            public DrawType GetDraw()
            {
                return draw;
            }

            public String GetHandDescription()
            {
                switch(this.hand){
                    case HandType.HighCard:
                        return "High card, " + kicker.ToString() + " kicker";
                    case HandType.Pair:
                        return pair.ToString() + " pair, " + kicker.ToString() + " kicker";
                    case HandType.TwoPair:
                        return "Two pair";
                    case HandType.ThreeOfAKind:
                        return "Three of a kind";
                    case HandType.Straight:
                        return "Straight";
                    case HandType.Flush:
                        return "Flush";
                    case HandType.FullHouse:
                        return "Full house";
                    case HandType.FourOfAKind:
                        return "Four of a kind";
                    case HandType.RoyalFlush:
                        return "Royal flush";
                    case HandType.Unknown:
                        return "Unknown";
                }
                return "<invalid>";
            }

            public String GetDrawsDescription()
            {
                if (draw == DrawType.Flush)
                {
                    return "Flush draw";
                }
                else if (draw == DrawType.Straight)
                {
                    if (straightDraw == StraightDrawType.InsideStraightDraw)
                    {
                        return "Inside straight draw";
                    }
                    else if (straightDraw == StraightDrawType.OpenEndedStraightDraw)
                    {
                        return "Open ended straight draw";
                    }
                }
                else if (draw == DrawType.FlushAndStraight)
                {
                    if (straightDraw == StraightDrawType.InsideStraightDraw)
                    {
                        return "Flush and inside straight draw";
                    }
                    else if (straightDraw == StraightDrawType.OpenEndedStraightDraw)
                    {
                        return "Flush and open ended straight draw";
                    }
                }

                return "None";
            }

            public bool HasADraw()
            {
                return isADraw(this.draw);
            }

            private bool isADraw(DrawType draw)
            {
                return (draw == DrawType.Flush || draw == DrawType.Straight || draw == DrawType.FlushAndStraight);
            }

            public override Rating GetRating()
            {
                if (hand == HandType.HighCard && draw == DrawType.None)
                {
                    return Rating.Nothing;
                }
                else if (hand == HandType.HighCard && isADraw(draw))
                {
                    return Rating.Weak;
                }
                else if (hand == HandType.Pair)
                {
                    if ((pair == PairType.Bottom) ||
                       ((pair == PairType.Middle && kicker == KickerType.Low)))
                    {
                        if (isADraw(draw)) return Rating.Mediocre;
                        else return Rating.Weak;
                    }

                    if ((pair == PairType.Middle && (kicker == KickerType.Middle || kicker == KickerType.High)) ||
                         (pair == PairType.Top && (kicker == KickerType.Low || kicker == KickerType.Middle)))
                    {
                        if (pair == PairType.Top && isADraw(draw)) return Rating.Strong;
                        else return Rating.Mediocre;
                    }

                    if ((pair == PairType.Top && (kicker == KickerType.High)))
                    {
                        return Rating.Strong;
                    }
                }
                else if (hand == HandType.TwoPair)
                {
                    return Rating.Strong;
                }

                return Rating.Monster;
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


            private DrawType GetDrawType(CardList cardList)
            {
                bool foundStraightDraw = false, foundFlushDraw = false;

                foundFlushDraw = IsFlushDraw(cardList);
                foundStraightDraw = IsStraightDraw(cardList);

                if (foundFlushDraw && !foundStraightDraw) return DrawType.Flush;
                else if (!foundFlushDraw && foundStraightDraw) return DrawType.Straight;
                else if (foundFlushDraw && foundStraightDraw) return DrawType.FlushAndStraight;
                else return DrawType.None;
            }

            private bool IsStraightDraw(CardList cardList)
            {
                if (cardList.Count < 4) return false;

                // Case 1: 4 cards to a straight (open ended straight draw)
                if (IsOpenEndedStraightDraw(cardList))
                {
                    return true;
                }

                // Case 2: inside straight draw
                if (IsInsideStraightDraw(cardList))
                {
                    return true;
                }

                return false;
            }

            private bool IsOpenEndedStraightDraw(CardList cardList)
            {
                if (cardList.Count < 4) return false;
        
                // Except if the ace is the last card, because there's no cards higher than the ace
                if (cardList.AreConsecutive(true, false, 4)){

                    // (list is sorted from lowest to highest)
                    cardList.Sort(SortUsing.AceHigh);

                    // Special case
                    if (cardList[cardList.Count - 1].Face == CardFace.Ace &&
                        cardList[cardList.Count - 2].Face == CardFace.King &&
                        cardList[cardList.Count - 3].Face == CardFace.Queen &&
                        cardList[cardList.Count - 4].Face == CardFace.Jack) return false;
                    else return true;
                }

                // Except if the ace is not the first card, there's no cards lower than the ace

                if (cardList.AreConsecutive(false, false, 4))
                {
                    cardList.Sort(SortUsing.AceLow);

                    // Special case
                    if (cardList[0].Face == CardFace.Ace &&
                        cardList[1].Face == CardFace.Two &&
                        cardList[2].Face == CardFace.Three &&
                        cardList[3].Face == CardFace.Four) return false;
                    else return true;
                }

                return false;
            }

            public bool IsInsideStraightDraw(CardList cardList)
            {
                if (cardList.Count < 4) return false;

                return IsInsideStraightDraw(cardList, true) || IsInsideStraightDraw(cardList, false);
            }

            private bool IsInsideStraightDraw(CardList cardList, bool countAceAsHigh)
            {
                if (cardList.Count < 4) return false;

                cardList.Sort(countAceAsHigh);

                // Case 1 and 2 are special cases

                // Case 1: first card is an ace and we are not counting ace as low, followed by 2, 3, 4
                if (!countAceAsHigh &&
                    cardList[0].Face == CardFace.Ace &&
                    cardList[1].Face == CardFace.Two &&
                    cardList[2].Face == CardFace.Three &&
                    cardList[3].Face == CardFace.Four) return true;

                // Case 2: first card is a J and we are counting ace as high, followed by Q, K, A
                if (countAceAsHigh &&
                    cardList[0].Face == CardFace.Jack &&
                    cardList[1].Face == CardFace.Queen &&
                    cardList[2].Face == CardFace.King &&
                    cardList[3].Face == CardFace.Ace) return true;


                // Case 2: 1 card, missing, 3 straight
                for (int i = 0; i < (cardList.Count - 3); i++)
                {
                    int faceValue = cardList[i].GetFaceValue(countAceAsHigh);
                    if (cardList[i + 1].GetFaceValue(countAceAsHigh) == faceValue + 2 &&
                        cardList[i + 2].GetFaceValue(countAceAsHigh) == faceValue + 3 &&
                        cardList[i + 3].GetFaceValue(countAceAsHigh) == faceValue + 4)
                    {
                        return true;
                    } 
                }

                // Case 3: 2 straight, missing, 2 straight
                for (int i = 1; i < (cardList.Count - 2); i++)
                {
                    int faceValue = cardList[i].GetFaceValue(countAceAsHigh);
                    if (cardList[i - 1].GetFaceValue(countAceAsHigh) == faceValue - 1 &&
                        cardList[i + 1].GetFaceValue(countAceAsHigh) == faceValue + 2 &&
                        cardList[i + 2].GetFaceValue(countAceAsHigh) == faceValue + 3)
                    {
                        return true;
                    }
                }

                // Case 4: 3 straight, missing, 1 card
                for (int i = 2; i < (cardList.Count - 1); i++)
                {
                    int faceValue = cardList[i].GetFaceValue(countAceAsHigh);
                    if (cardList[i - 2].GetFaceValue(countAceAsHigh) == faceValue - 2 &&
                        cardList[i - 1].GetFaceValue(countAceAsHigh) == faceValue - 1 &&
                        cardList[i + 1].GetFaceValue(countAceAsHigh) == faceValue + 2)
                    {
                        return true;
                    }
                }

                return false;
            }

            private bool IsFlushDraw(CardList cardList)
            {
                // Flush draw
                for (int i = 0; i < cardList.Count; i++)
                {
                    int numCardsSameSuit = 1;
                    for (int j = i + 1; j < cardList.Count; j++)
                    {
                        if (cardList[i].Suit == cardList[j].Suit)
                        {
                            numCardsSameSuit++;
                        }
                    }

                    if (numCardsSameSuit == 4)
                    {
                        return true;
                    }
                }

                return false;
            }

            private PairType GetPairType(CardList communityCards, Card matching, Card firstCard, Card secondCard)
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

            private KickerType GetKickerTypeFromCard(Card c)
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
                return hand.ToString() + ", " + kicker.ToString() + " kicker, " + pair.ToString() + " pair, " + draw.ToString() + " draw, " + straightDraw.ToString() + " straightdraw - " + GetRating().ToString();
            }
        }
        #endregion

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

        /* Returns the card with the higher face value, ignoring suit */
        private Card GetHigher()
        {
            if (cards[0].GetFaceValue() > cards[1].GetFaceValue()) return cards[0];
            else return cards[1];
        }

        private Card GetLower()
        {
            if (cards[0].GetFaceValue() < cards[1].GetFaceValue()) return cards[0];
            else return cards[1];
        }

        public bool IsPaired()
        {
            return cards[0].Face == cards[1].Face;
        }

        public bool IsSuited()
        {
            return cards[0].Suit == cards[1].Suit;
        }

        public bool IsConnectors()
        {
            return IsGappedConnectors(0);
        }

        public bool IsGappedConnectors(int gap)
        {
            return (Math.Abs(cards[1].GetFaceValue(true) - cards[0].GetFaceValue(true)) == (gap + 1)) ||
                   (Math.Abs(cards[1].GetFaceValue(false) - cards[0].GetFaceValue(false)) == (gap + 1)) ;
        }


        public bool IsConnectorsInRange(HoldemHand lower, HoldemHand upper)
        {
            return IsGappedConnectorsInRange(0, lower, upper);
        }

        /* Returns true if the hand is a gapped connector and is within the range of hands (inclusive)
         * specified, ex. between 54 and JT (only the upper cards are checked) */
        public bool IsGappedConnectorsInRange(int gap, HoldemHand lower, HoldemHand upper)
        {
            if (this.GetHigher().GetFaceValue() >= lower.GetHigher().GetFaceValue() &&
                this.GetHigher().GetFaceValue() <= upper.GetHigher().GetFaceValue())
            {
                return IsGappedConnectors(gap);
            }
            else return false;
        }

        public float GetPrelopPercentile()
        {
            Trace.Assert(GetFirstCard() != null && GetSecondCard() != null, "Cannot retrieve the preflop percentile if the cards are not known.");

            return (float)preflopPercentiles[this.ToString()];
        }

        public static string ConvertToString(CardFace firstFace, CardSuit firstSuit, CardFace secondFace, CardSuit secondSuit)
        {
            // Suited?
            string suited = "o"; // default to offsuit
            if (firstSuit == secondSuit) suited = "s";

            // If we have a pair, it's obviously offsuit
            if (firstFace == secondFace) suited = "";

            string firstFaceStr = Card.CardFaceToChar(firstFace).ToString();
            string secondFaceStr = Card.CardFaceToChar(secondFace).ToString();

            return firstFaceStr + secondFaceStr + suited;
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

            // Swap them so that the bigger card is echoed first (A6 instead of 6A)
            if (firstCard.GetFaceValue() < secondCard.GetFaceValue())
            {
                CardFace t = firstFace;
                firstFace = secondFace;
                secondFace = t;
            }

            return ConvertToString(firstFace, firstSuit, secondFace, secondSuit);
        }

        public HoldemHand.Classification GetHandClassification(HoldemGamePhase phase, HoldemBoard board)
        {
            HoldemHand.Classification classification = GetClassification(phase, board);
            Trace.WriteLine(String.Format("Rating hand [{0}] for {1} on board [{2}]: {3}", base.ToString(), phase.ToString(), board.ToString(), classification.ToString()));
            return classification;
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
                return new ClassificationPostflop(this, phase, board);
            }
        }

        
    }
}
