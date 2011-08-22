using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    class HoldemOddsCalculator : OddsCalculator
    {
        const int PRECISION = 2;

        public HoldemOddsCalculator()
        {
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
                result.Add(new Statistic(new StatisticsOddsData("Flop pair to a hole card", 26.939f, PRECISION), "Preflop"));
                result.Add(new Statistic(new StatisticsOddsData("Flop two pair to a hole card", 2.02f, PRECISION), "Preflop"));
                result.Add(new Statistic(new StatisticsOddsData("Flop trips to a hole card", 1.347f, PRECISION), "Preflop"));
                result.Add(new Statistic(new StatisticsOddsData("Flop full house", 0.092f, PRECISION), "Preflop"));
                result.Add(new Statistic(new StatisticsOddsData("Flop quads", 0.01f, PRECISION), "Preflop"));
            }
            else
            {
                // Paired hole cards
                result.Add(new Statistic(new StatisticsOddsData("Flop two pair by pairing the board", 16.163f, PRECISION), "Preflop"));
                result.Add(new Statistic(new StatisticsOddsData("Flop trips for your pair", 10.775f, PRECISION), "Preflop"));
                result.Add(new Statistic(new StatisticsOddsData("Flop full house (set to your pair)", 0.735f, PRECISION), "Preflop"));
                result.Add(new Statistic(new StatisticsOddsData("Flop full house (trips on board)", 0.245f, PRECISION), "Preflop"));
                result.Add(new Statistic(new StatisticsOddsData("Flop quads", 0.245f, PRECISION), "Preflop"));
            }

            // Unsuited
            if (!holdemHand.IsSuited())
            {
                result.Add(new Statistic(new StatisticsOddsData("Flop a four flush", 2.245f, PRECISION), "Preflop"));
            }
            else
            {
                // Suited
                result.Add(new Statistic(new StatisticsOddsData("Flop a flush", 0.842f, PRECISION), "Preflop"));
                result.Add(new Statistic(new StatisticsOddsData("Flop a four flush", 10.449f, PRECISION), "Preflop"));
            }

            // Connectors from 54 to JT
            if (holdemHand.IsConnectorsInRange(
                    new HoldemHand(new Card(CardFace.Five, CardSuit.Clubs), new Card(CardFace.Four, CardSuit.Clubs)),
                    new HoldemHand(new Card(CardFace.Jack, CardSuit.Clubs), new Card(CardFace.Ten, CardSuit.Clubs))))
            {
                result.Add(new Statistic(new StatisticsOddsData("Flop a straight", 1.306f, PRECISION), "Preflop"));
                result.Add(new Statistic(new StatisticsOddsData("Flop an 8 out straight draw", 10.449f, PRECISION), "Preflop"));
            }

            // One gapped connectors from 53 to QT
            if (holdemHand.IsGappedConnectorsInRange(1,
                    new HoldemHand(new Card(CardFace.Five, CardSuit.Clubs), new Card(CardFace.Three, CardSuit.Clubs)),
                    new HoldemHand(new Card(CardFace.Queen, CardSuit.Clubs), new Card(CardFace.Ten, CardSuit.Clubs))))
            {
                result.Add(new Statistic(new StatisticsOddsData("Flop a straight", 0.980f, PRECISION), "Preflop"));
                result.Add(new Statistic(new StatisticsOddsData("Flop an 8 out straight draw", 8.08f, PRECISION), "Preflop"));
            }

            // Two gapped connectors from 52 to KT
            if (holdemHand.IsGappedConnectorsInRange(2,
                    new HoldemHand(new Card(CardFace.Five, CardSuit.Clubs), new Card(CardFace.Two, CardSuit.Clubs)),
                    new HoldemHand(new Card(CardFace.King, CardSuit.Clubs), new Card(CardFace.Ten, CardSuit.Clubs))))
            {
                result.Add(new Statistic(new StatisticsOddsData("Flop a straight", 0.653f, PRECISION), "Preflop"));
                result.Add(new Statistic(new StatisticsOddsData("Flop an 8 out straight draw", 5.224f, PRECISION), "Preflop"));
            }

            // Three gapped connectors from A5 to AT
            if (holdemHand.IsGappedConnectorsInRange(3,
                    new HoldemHand(new Card(CardFace.Ace, CardSuit.Clubs), new Card(CardFace.Five, CardSuit.Clubs)),
                    new HoldemHand(new Card(CardFace.Ace, CardSuit.Clubs), new Card(CardFace.Ten, CardSuit.Clubs))))
            {
                result.Add(new Statistic(new StatisticsOddsData("Flop a straight", 0.327f, PRECISION), "Preflop"));
                result.Add(new Statistic(new StatisticsOddsData("Flop an 8 out straight draw", 2.612f, PRECISION), "Preflop"));
            }

            return result;
        }
    }
}
