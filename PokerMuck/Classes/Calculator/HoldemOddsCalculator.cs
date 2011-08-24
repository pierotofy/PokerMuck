using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace PokerMuck
{
    class HoldemOddsCalculator : OddsCalculator
    {
        const int PRECISION = 2;

        /* Outs odds table
         * Key = num of outs
         * Value[0] = chances by turn
         * Value[1] = chances by river
         * Value[2] = chances by turn or river */
        private static Hashtable outsTable = new Hashtable()
        {
            {2, new float[]{4.3f, 4.3f, 8.4f}},
            {4, new float[]{8.5f, 8.7f, 16.5f}},
            {6, new float[]{12.8f, 13.0f, 24.1f}},
            {8, new float[]{17.0f, 17.4f, 31.5f}},
            {9, new float[]{19.1f, 19.6f, 35.0f}},
            {12, new float[]{25.5f, 26.1f, 45.0f}},
            {15, new float[]{31.9f, 32.6f, 54.1f}}
        };

        private enum OutsByThe{
            Turn, River, TurnOrRiver
        }

        /* Accessor for the outs table */
        private static float GetOutsPercentage(int numOuts, OutsByThe by)
        {
            int index = (int)by;
            return (float)((float[])outsTable[numOuts])[index];
        }

        public HoldemOddsCalculator()
        {
        }

        public List<Statistic> Calculate(HoldemHand hand, HoldemBoard board)
        {
            List<Statistic> result = new List<Statistic>();

            // What game phase are we in (returns empty list if the board is invalid)
            HoldemGamePhase phase = HoldemGamePhase.Flop;
            if (board.Count == 3) phase = HoldemGamePhase.Flop;
            else if (board.Count == 4) phase = HoldemGamePhase.Turn;
            else if (board.Count == 5) phase = HoldemGamePhase.River;
            else
            {
                Trace.WriteLine("We were asked to calculate the odds for " + hand.ToString() + " and " + board.ToString() + " but the board seems to be invalid. Returning empty.");
                return result;
            }

            HoldemHand.ClassificationPostflop classification = (HoldemHand.ClassificationPostflop)hand.GetClassification(phase, board);
            HoldemHand.ClassificationPostflop.HandType handType = classification.GetHand();
            HoldemHand.ClassificationPostflop.DrawType drawType = classification.GetDraw();
            HoldemHand.ClassificationPostflop.StraightDrawType straightDrawType = classification.GetStraightDraw();

            result.Add(new Statistic(new StatisticsDescriptiveData("Your hand", classification.GetHandDescription())));
            
            if (phase == HoldemGamePhase.Flop || phase == HoldemGamePhase.Turn)
            {
                result.Add(new Statistic(new StatisticsDescriptiveData("Draws", classification.GetDrawsDescription())));

                if (handType == HoldemHand.ClassificationPostflop.HandType.HighCard)
                {
                    // Hold high card, hope to make a pair
                    result.Add(CreateOutsStatistic("Improve to a pair", 6, phase));
                }
                else if (handType == HoldemHand.ClassificationPostflop.HandType.Pair)
                {
                    // Hold a pair, hope to make three of a kind
                    result.Add(CreateOutsStatistic("Improve to three of a kind", 2, phase));
                }
                else if (handType == HoldemHand.ClassificationPostflop.HandType.TwoPair)
                {
                    // Hold two pair, hope to make a full house
                    result.Add(CreateOutsStatistic("Improve to a full house", 4, phase));
                }

                if (drawType == HoldemHand.ClassificationPostflop.DrawType.Flush)
                {
                    result.Add(CreateOutsStatistic("Improve to a flush", 9, phase));
                }
                else if (drawType == HoldemHand.ClassificationPostflop.DrawType.Straight)
                {
                    if (straightDrawType == HoldemHand.ClassificationPostflop.StraightDrawType.OpenEndedStraightDraw)
                    {
                        result.Add(CreateOutsStatistic("Improve to a straight", 8, phase));
                    }
                    else if (straightDrawType == HoldemHand.ClassificationPostflop.StraightDrawType.InsideStraightDraw)
                    {
                        result.Add(CreateOutsStatistic("Improve to a straight", 4, phase));
                    }
                    else
                    {
                        Trace.WriteLine("Warning! Straight draw detected when calculating odds but none of the cases was matched?");
                    }
                }
                else if (drawType == HoldemHand.ClassificationPostflop.DrawType.FlushAndStraight)
                {
                    // Open ended straight flush draw
                    if (straightDrawType == HoldemHand.ClassificationPostflop.StraightDrawType.OpenEndedStraightDraw)
                    {
                        // 9 outs for the flush + 6 outs (2 outs for the straight draw are the suit we need to make the flush)
                        result.Add(CreateOutsStatistic("Improve to a flush or straight", 15, phase));
                    }
                    else if (straightDrawType == HoldemHand.ClassificationPostflop.StraightDrawType.InsideStraightDraw)
                    {
                        // inside straight flushd draw, 9 outs + 3 (1 out is the suit we need)
                        result.Add(CreateOutsStatistic("Improve to a flush or straight", 12, phase));
                    }
                    else
                    {
                        Trace.WriteLine("Warning! Flush and straight draw detected when calculating odds but none of the cases was matched?");
                    }
                }
            }

            return result;
        }

        private Statistic CreateOutsStatistic(String name, int numOuts, HoldemGamePhase phase)
        {
            Trace.Assert(phase == HoldemGamePhase.Flop || phase == HoldemGamePhase.Turn, "Cannot create outs statistics for a phase different than flop or turn.");
            float value = 0.0f;
            if (phase == HoldemGamePhase.Flop)
            {
                value = GetOutsPercentage(numOuts, OutsByThe.Turn);
            }
            else if (phase == HoldemGamePhase.Turn)
            {
                value = GetOutsPercentage(numOuts, OutsByThe.River);
            }

            Statistic result = new Statistic(new StatisticsOddsData(name, value, PRECISION));

            // The flop has extra information as we want to see the chances of hitting 
            // something by the turn, or by the turn and river combined
            if (phase == HoldemGamePhase.Flop)
            {
                float valueByTurnOrRiver = GetOutsPercentage(numOuts, OutsByThe.TurnOrRiver);
                result.AddSubStatistic(new StatisticsOddsData("By the turn", value, PRECISION));
                result.AddSubStatistic(new StatisticsOddsData("By the turn or river", valueByTurnOrRiver, PRECISION));
            }

            return result;
        }

        public override List<Statistic> Calculate(Hand hand)
        {
            HoldemHand holdemHand = (HoldemHand)hand;
            List<Statistic> result = new List<Statistic>();

            // Depending on what hand we have, we generate only odds that are consistent with our hand

            /* Odds are taken from http://www.flopturnriver.com/Common-Flop-Odds.html
             * since they are better statisticians than I am */
 
            // Unpaired hole cards
            if (!holdemHand.IsPaired())
            {
                result.Add(new Statistic(new StatisticsOddsData("Flop pair to a hole card", 26.939f, PRECISION)));
                result.Add(new Statistic(new StatisticsOddsData("Flop two pair to a hole card", 2.02f, PRECISION)));
                result.Add(new Statistic(new StatisticsOddsData("Flop trips to a hole card", 1.347f, PRECISION)));
                result.Add(new Statistic(new StatisticsOddsData("Flop full house", 0.092f, PRECISION)));
                result.Add(new Statistic(new StatisticsOddsData("Flop quads", 0.01f, PRECISION)));
            }
            else
            {
                // Paired hole cards
                result.Add(new Statistic(new StatisticsOddsData("Flop two pair by pairing the board", 16.163f, PRECISION)));
                result.Add(new Statistic(new StatisticsOddsData("Flop trips for your pair", 10.775f, PRECISION)));
                result.Add(new Statistic(new StatisticsOddsData("Flop full house (set to your pair)", 0.735f, PRECISION)));
                result.Add(new Statistic(new StatisticsOddsData("Flop full house (trips on board)", 0.245f, PRECISION)));
                result.Add(new Statistic(new StatisticsOddsData("Flop quads", 0.245f, PRECISION)));
            }

            // Unsuited
            if (!holdemHand.IsSuited())
            {
                result.Add(new Statistic(new StatisticsOddsData("Flop a four flush", 2.245f, PRECISION)));
            }
            else
            {
                // Suited
                result.Add(new Statistic(new StatisticsOddsData("Flop a flush", 0.842f, PRECISION)));
                result.Add(new Statistic(new StatisticsOddsData("Flop a four flush", 10.449f, PRECISION)));
            }

            // Connectors from 54 to JT
            if (holdemHand.IsConnectorsInRange(
                    new HoldemHand(new Card(CardFace.Five, CardSuit.Clubs), new Card(CardFace.Four, CardSuit.Clubs)),
                    new HoldemHand(new Card(CardFace.Jack, CardSuit.Clubs), new Card(CardFace.Ten, CardSuit.Clubs))))
            {
                result.Add(new Statistic(new StatisticsOddsData("Flop a straight", 1.306f, PRECISION)));
                result.Add(new Statistic(new StatisticsOddsData("Flop an 8 out straight draw", 10.449f, PRECISION)));
            }

            // One gapped connectors from 53 to QT
            if (holdemHand.IsGappedConnectorsInRange(1,
                    new HoldemHand(new Card(CardFace.Five, CardSuit.Clubs), new Card(CardFace.Three, CardSuit.Clubs)),
                    new HoldemHand(new Card(CardFace.Queen, CardSuit.Clubs), new Card(CardFace.Ten, CardSuit.Clubs))))
            {
                result.Add(new Statistic(new StatisticsOddsData("Flop a straight", 0.980f, PRECISION)));
                result.Add(new Statistic(new StatisticsOddsData("Flop an 8 out straight draw", 8.08f, PRECISION)));
            }

            // Two gapped connectors from 52 to KT
            if (holdemHand.IsGappedConnectorsInRange(2,
                    new HoldemHand(new Card(CardFace.Five, CardSuit.Clubs), new Card(CardFace.Two, CardSuit.Clubs)),
                    new HoldemHand(new Card(CardFace.King, CardSuit.Clubs), new Card(CardFace.Ten, CardSuit.Clubs))))
            {
                result.Add(new Statistic(new StatisticsOddsData("Flop a straight", 0.653f, PRECISION)));
                result.Add(new Statistic(new StatisticsOddsData("Flop an 8 out straight draw", 5.224f, PRECISION)));
            }

            // Three gapped connectors from A5 to AT
            if (holdemHand.IsGappedConnectorsInRange(3,
                    new HoldemHand(new Card(CardFace.Ace, CardSuit.Clubs), new Card(CardFace.Five, CardSuit.Clubs)),
                    new HoldemHand(new Card(CardFace.Ace, CardSuit.Clubs), new Card(CardFace.Ten, CardSuit.Clubs))))
            {
                result.Add(new Statistic(new StatisticsOddsData("Flop a straight", 0.327f, PRECISION)));
                result.Add(new Statistic(new StatisticsOddsData("Flop an 8 out straight draw", 2.612f, PRECISION)));
            }

            return result;
        }
    }
}
