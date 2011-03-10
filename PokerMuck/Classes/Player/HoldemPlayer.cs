using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace PokerMuck
{
    /* A holdem player has certain statistics that a five card draw player might not have */
    class HoldemPlayer : Player
    {
        /* Limp */
        private ValueCounter limps;

        /* VPF */
        private ValueCounter voluntaryPutMoneyPreflop;

        /* Raises */
        private MultipleValueCounter raises;
                
        /* C Bets */ 
        private int opportunitiesToCBet;

        private ValueCounter cbets;

        private int foldsToACBet;
        private int callsToACBet;
        private int raisesToACBet;

        /* Checks */
        private MultipleValueCounter checks;

        /* Check-raise */
        private MultipleValueCounter checkRaises;

        
        private int sawFlop;
        private int sawTurn;
        private int sawRiver;
        

        /* Each table is set this way:
         key => value
         GamePhase => value
         Ex. calls[flop] == 4 --> player has flat called 4 times during the flop
         */
        private Hashtable totalCalls;
        private Hashtable totalBets;
        private Hashtable totalFolds;
        private Hashtable totalRaises;
        private Hashtable totalChecks;


        public bool IsBigBlind { get; set; }
        public bool IsSmallBlind { get; set; }

        
        public HoldemPlayer(String playerName)
            : base(playerName)
        {
            totalCalls = new Hashtable(5);
            totalBets = new Hashtable(5);
            totalFolds = new Hashtable(5);
            totalRaises = new Hashtable(5);
            totalChecks = new Hashtable(5);

            limps = new ValueCounter();
            voluntaryPutMoneyPreflop = new ValueCounter();

            raises = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            checkRaises = new MultipleValueCounter(HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            checks = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);

            cbets = new ValueCounter();
            
            ResetAllStatistics();
        }

        /* Resets all statistics counters */
        public override void ResetAllStatistics()
        {
            base.ResetAllStatistics();

            ResetStatistics(totalCalls);
            ResetStatistics(totalBets);
            ResetStatistics(totalFolds);
            ResetStatistics(totalRaises);
            ResetStatistics(totalChecks);

            limps.Reset();
            voluntaryPutMoneyPreflop.Reset();
            raises.Reset();
            cbets.Reset();

            callsToACBet = 0;
            foldsToACBet = 0;
            raisesToACBet = 0;

            sawFlop = 0;
            sawTurn = 0;
            sawRiver = 0;

            totalHandsPlayed = 0;
            opportunitiesToCBet = 0;

            checkRaises.Reset();

            PrepareStatisticsForNewRound();
        }

        /* Returns the limp statistics */
        public StatisticsData GetLimpStats()
        {
            float limpRatio = 0;

            if (totalHandsPlayed == 0) limpRatio = 0;
            else limpRatio = (float)limps.Value / (float)totalHandsPlayed;
            
            return new StatisticsPercentageData("Limp", limpRatio, "Preflop");
        }

        /* How many times has the player put money in preflop? */
        public StatisticsData GetVPFStats()
        {
            float vpfRatio = 0;

            if (totalHandsPlayed == 0) vpfRatio = 0;
            else vpfRatio = (float)voluntaryPutMoneyPreflop.Value / (float)totalHandsPlayed;
            return new StatisticsPercentageData("Voluntary Put $", vpfRatio, "Preflop");
        }

        /* How many times has the player made a continuation bet following a preflop raise? */
        public StatisticsData GetCBetStats()
        {
            if (opportunitiesToCBet == 0) return new StatisticsUnknownData("Continuation bets", "Flop");

            float cbetsRatio = (float)cbets.Value / (float)opportunitiesToCBet;
            return new StatisticsPercentageData("Continuation bets", cbetsRatio, "Flop");
        }

        /* How many times has a player folded to a continuation bet? */
        public StatisticsData GetFoldToACBetStats()
        {
            float actionsToACbet = foldsToACBet + raisesToACBet + callsToACBet;

            if (actionsToACbet == 0) return new StatisticsUnknownData("Folds to a continuation bet", "Flop");
            else
            {
                return new StatisticsPercentageData("Folds to a continuation bet",
                    (float)foldsToACBet / (float)(raisesToACBet + callsToACBet + foldsToACBet),
                    "Flop");
            }
        }

        /* How many times has the player raised preflop? */
        public StatisticsData GetPFRStats()
        {
            float pfrRatio = 0;

            if (totalHandsPlayed == 0) pfrRatio = 0;
            else pfrRatio = (float)raises[HoldemGamePhase.Preflop].Value / (float)totalHandsPlayed;

            return new StatisticsPercentageData("Raises", pfrRatio, "Preflop");
        }

        /* How many times has the player raised? */
        public StatisticsData GetRaiseStats(HoldemGamePhase phase, String category)
        {
            return null; // TODO
        }

        /* How many times has the player check raised? */
        public StatisticsData GetCheckRaiseStats(HoldemGamePhase phase, String category)
        {
            int totalChecksSoFar = (int)totalChecks[phase];

            if (totalChecksSoFar == 0) return new StatisticsUnknownData("Check Raise", category);
            else
            {
                float checkRaiseRatio = (float)checkRaises[phase].Value / (float)totalChecksSoFar;

                return new StatisticsPercentageData("Check Raise", checkRaiseRatio, category);
            }
        }
        
        /* Has limped */
        public void CheckForLimp()
        {
            /* If he's not the small or big blind, this is also a limp */
            if (!IsSmallBlind && !IsBigBlind)
            {
                limps.Increment();
            }
        }

        /* Fold after a cbet */
        public void IncrementFoldToACBet()
        {
            foldsToACBet += 1;
        }

        /* Calls and raises after a cbet */
        public void IncrementCallToACBet()
        {
            callsToACBet += 1;
        }

        public void IncrementRaiseToACBet()
        {
            raisesToACBet += 1;
        }

        /* This player has raised, increment the stats */
        public void HasRaised(HoldemGamePhase gamePhase)
        {
            if (gamePhase == HoldemGamePhase.Preflop)
            {
                CheckForVoluntaryPutMoneyPreflop();
                CheckForPreflopRaise();
            }

            // Check raise?
            if (checks[gamePhase].WasIncremented)
            {
                checkRaises[gamePhase].Increment();
            }    

            raises[gamePhase].Increment();

            IncrementStatistics(totalRaises, gamePhase);
        }

        /* Has bet */
        public void HasBet(HoldemGamePhase gamePhase)
        {
            if (gamePhase == HoldemGamePhase.Preflop)
            {
                CheckForVoluntaryPutMoneyPreflop();
            }

            IncrementStatistics(totalBets, gamePhase);
        }

        /* Has checked */
        public void HasChecked(HoldemGamePhase gamePhase)
        {
            checks[gamePhase].Increment();

            IncrementStatistics(totalChecks, gamePhase);
        }

        /* Has called */
        public void HasCalled(HoldemGamePhase gamePhase)
        {
            if (gamePhase == HoldemGamePhase.Preflop)
            {
                CheckForVoluntaryPutMoneyPreflop();
            }
            else if (gamePhase == HoldemGamePhase.Flop)
            {
                if (raises[HoldemGamePhase.Preflop].WasIncremented)
                {
                    // Somebody has bet before us on the flop and we called
                    // Thus we missed an opportunity to cbet
                    opportunitiesToCBet -= 1;
                }
            }


            IncrementStatistics(totalCalls, gamePhase);
        }

        /* Folded */
        public void HasFolded(HoldemGamePhase gamePhase)
        {
            if (gamePhase == HoldemGamePhase.Preflop)
            {
                if (raises[HoldemGamePhase.Preflop].WasIncremented)
                {
                    // We have raised preflop, but then we folded
                    opportunitiesToCBet -= 1;
                }
            }

            IncrementStatistics(totalFolds, gamePhase);
        }


        /* Check for cbets and increments the statistics if it is a valid cbet
         * returns true when this is a cbet */
        public bool CheckForCBet(float amount)
        {
            /* This player has raised preflop and now has bet on the flop when first to act or when everybody
             * checked on him. This is a cbet */
            if (raises[HoldemGamePhase.Preflop].WasIncremented && !cbets.WasIncremented)
            {
                cbets.Increment();
                return true;
            }

            return false;
        }

        /* Helper function to increment the VPF stat */
        private void CheckForVoluntaryPutMoneyPreflop()
        {
            voluntaryPutMoneyPreflop.Increment();
        }


        /* Helper function to increment the PFR stat */
        private void CheckForPreflopRaise()
        {
            if (!raises[HoldemGamePhase.Preflop].WasIncremented)
            {
                raises[HoldemGamePhase.Preflop].Increment();
                opportunitiesToCBet += 1; // Assume we see the flop and everybody checks on us
            }
        }

        /* Helper function to increment the value in one of the hash tables (calls, raises, folds, etc.) */
        private void IncrementStatistics(Hashtable table, HoldemGamePhase gamePhase)
        {
            if (table != null) table[gamePhase] = (int)table[gamePhase] + 1;
        }

        /* Certain statistics are round specific (for example a person can only limp once per round)
         * This function should get called at the beginning of a new round */
        public override void PrepareStatisticsForNewRound()
        {
            base.PrepareStatisticsForNewRound();

            IsBigBlind = false;
            IsSmallBlind = false;
            limps.AllowIncrement();
            voluntaryPutMoneyPreflop.AllowIncrement();
            raises.AllowIncrement();
            cbets.AllowIncrement();

            checkRaises.AllowIncrement();

            checks.AllowIncrement();
        }       


        /* Reset the stats for a particular hash table set */
        private void ResetStatistics(Hashtable table)
        {
            table[HoldemGamePhase.Preflop] = 0;
            table[HoldemGamePhase.Flop] = 0;
            table[HoldemGamePhase.Turn] = 0;
            table[HoldemGamePhase.River] = 0;
        }

        protected override int GetCategoryOrder(string category)
        {
            switch (category)
            {
                case "Summary": return 0;
                case "Preflop": return 1;
                case "Flop": return 2;
                case "Turn": return 3;
                case "River": return 4;
                default: return 5;
            }
        }

        /* Returns the statistics of the player */
        public override PlayerStatistics GetStatistics()
        {
            PlayerStatistics result =  base.GetStatistics();

            result.Set(GetVPFStats());
            result.Set(GetLimpStats());
            result.Set(GetPFRStats());
            result.Set(GetCBetStats());
            result.Set(GetFoldToACBetStats());

            result.Set(GetCheckRaiseStats(HoldemGamePhase.Flop, "Flop"));
            result.Set(GetCheckRaiseStats(HoldemGamePhase.Turn, "Turn"));
            result.Set(GetCheckRaiseStats(HoldemGamePhase.River, "River"));



            return result;
        }
    }
}
