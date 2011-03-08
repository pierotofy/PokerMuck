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

        private int limps;
        private bool HasLimpedThisRound;

        /* VPF */
        
        private int voluntaryPutMoneyPreflop;
        private bool HasVoluntaryPutMoneyPreflopThisRound;

        /* Preflop raises */
        
        private int preflopRaises;
        private bool HasPreflopRaisedThisRound;
                
        /* C Bets */ 
        private int opportunitiesToCBet;

        private int cbets;
        private bool HasCBetThisRound;

        private int foldsToACBet;
        private int callsToACBet;
        private int raisesToACBet;

        /* Check-raise */
        
        private bool HasCheckedTheFlopThisRound;
        private bool HasCheckRaisedTheFlopThisRound;
        private int flopCheckRaises;

        private bool HasCheckRaisedTheTurnThisRound;
        private bool HasCheckedTheTurnThisRound;
        private int turnCheckRaises;

        private bool HasCheckedTheRiverThisRound;
        private bool HasCheckRaisedTheRiverThisRound;
        private int riverCheckRaises;


        /* Each table is set this way:
         key => value
         GamePhase => value
         Ex. calls[flop] == 4 --> player has flat called 4 times during the flop
         */
        private Hashtable calls;
        private Hashtable bets;
        private Hashtable folds;
        private Hashtable raises;
        private Hashtable checks;


        public bool IsBigBlind { get; set; }
        public bool IsSmallBlind { get; set; }

        
        public HoldemPlayer(String playerName)
            : base(playerName)
        {
            calls = new Hashtable(5);
            bets = new Hashtable(5);
            folds = new Hashtable(5);
            raises = new Hashtable(5);
            checks = new Hashtable(5);

            ResetAllStatistics();
        }

        /* Resets all statistics counters */
        public override void ResetAllStatistics()
        {
            base.ResetAllStatistics();

            ResetStatistics(calls);
            ResetStatistics(bets);
            ResetStatistics(folds);
            ResetStatistics(raises);
            ResetStatistics(checks);
            limps = 0;
            cbets = 0;

            callsToACBet = 0;
            foldsToACBet = 0;
            raisesToACBet = 0;

            voluntaryPutMoneyPreflop = 0;
            totalHandsPlayed = 0;
            preflopRaises = 0;
            opportunitiesToCBet = 0;

            flopCheckRaises = 0;
            turnCheckRaises = 0;
            riverCheckRaises = 0;

            PrepareStatisticsForNewRound();
        }

        /* Returns the limp statistics */
        public StatisticsData GetLimpStats()
        {
            float limpRatio = 0;

            if (totalHandsPlayed == 0) limpRatio = 0;
            else limpRatio = (float)limps / (float)totalHandsPlayed;
            
            return new StatisticsPercentageData("Limp", limpRatio, "Preflop");
        }

        /* How many times has the player put money in preflop? */
        public StatisticsData GetVPFStats()
        {
            float vpfRatio = 0;

            if (totalHandsPlayed == 0) vpfRatio = 0;
            else vpfRatio = (float)voluntaryPutMoneyPreflop / (float)totalHandsPlayed;
            return new StatisticsPercentageData("Voluntary Put $", vpfRatio, "Preflop");
        }

        /* How many times has the player made a continuation bet following a preflop raise? */
        public StatisticsData GetCBetStats()
        {
            if (opportunitiesToCBet == 0) return new StatisticsUnknownData("Continuation bets", "Flop");

            float cbetsRatio = (float)cbets / (float)opportunitiesToCBet;
            return new StatisticsPercentageData("Continuation bets", cbetsRatio, "Flop");
        }

        /* How many times has a player folded to a continuation bet? */
        public StatisticsData GetFoldToACBetStats()
        {
            float actionsToACbet = foldsToACBet + raisesToACBet + callsToACBet;

            if (actionsToACbet == 0) return new StatisticsUnknownData("Folds to a continuation bet", "Flop");
            else
            {
                return new StatisticsPercentageData("Folds to a continuatino bet",
                    (float)foldsToACBet / (float)(raisesToACBet + callsToACBet + foldsToACBet),
                    "Flop");
            }
        }

        /* How many times has the player raised preflop? */
        public StatisticsData GetPFRStats()
        {
            float pfrRatio = 0;

            if (totalHandsPlayed == 0) pfrRatio = 0;
            else pfrRatio = (float)preflopRaises / (float)totalHandsPlayed;

            return new StatisticsPercentageData("Raises", pfrRatio, "Preflop");
        }

        /* How many times has the player check raised? */
        public StatisticsData GetCheckRaiseStats(HoldemGamePhase phase, String category)
        {
            int totalChecks = (int)checks[phase];

            if (totalChecks == 0) return new StatisticsUnknownData("Check Raise", category);
            else
            {
                float checkRaiseRatio = 0;

                if (phase == HoldemGamePhase.Flop) checkRaiseRatio = (float)flopCheckRaises / (float)totalChecks;
                else if (phase == HoldemGamePhase.Turn) checkRaiseRatio = (float)turnCheckRaises / (float)totalChecks;
                else if (phase == HoldemGamePhase.River) checkRaiseRatio = (float)riverCheckRaises / (float)totalChecks;
                else Debug.Assert(false, "Calculating a check raise for a game phase that doesn't make sense");

                return new StatisticsPercentageData("Check Raise", checkRaiseRatio, category);
            }
        }
        
        /* Has limped */
        public void CheckForLimp()
        {
            /* If he's not the small or big blind, this is also a limp */
            if (!IsSmallBlind && !IsBigBlind && !HasLimpedThisRound)
            {
                limps += 1;
                HasLimpedThisRound = true;
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
            else if (gamePhase == HoldemGamePhase.Flop)
            {
                // Check raise?
                if (HasCheckedTheFlopThisRound && !HasCheckRaisedTheFlopThisRound)
                {
                    flopCheckRaises += 1;
                    HasCheckRaisedTheFlopThisRound = true;
                }
            }
            else if (gamePhase == HoldemGamePhase.Turn)
            {
                // Check raise?
                if (HasCheckedTheTurnThisRound && !HasCheckRaisedTheTurnThisRound)
                {
                    turnCheckRaises += 1;
                    HasCheckRaisedTheTurnThisRound = true;
                }
            }
            else if (gamePhase == HoldemGamePhase.River)
            {
                // Check raise?
                if (HasCheckedTheRiverThisRound && !HasCheckRaisedTheRiverThisRound)
                {
                    riverCheckRaises += 1;
                    HasCheckRaisedTheRiverThisRound = true;
                }
            }

            IncrementStatistics(raises, gamePhase);
        }

        /* Has bet */
        public void HasBet(HoldemGamePhase gamePhase)
        {
            if (gamePhase == HoldemGamePhase.Preflop)
            {
                CheckForVoluntaryPutMoneyPreflop();
            }

            IncrementStatistics(bets, gamePhase);
        }

        /* Has checked */
        public void HasChecked(HoldemGamePhase gamePhase)
        {
            if (gamePhase == HoldemGamePhase.Flop)
            {
                HasCheckedTheFlopThisRound = true;
            }
            else if (gamePhase == HoldemGamePhase.Turn)
            {
                HasCheckedTheTurnThisRound = true;
            }
            else if (gamePhase == HoldemGamePhase.River)
            {
                HasCheckedTheRiverThisRound = true;
            }

            IncrementStatistics(checks, gamePhase);
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
                if (HasPreflopRaisedThisRound)
                {
                    // Somebody has bet before us on the flop and we called
                    // Thus we missed an opportunity to cbet
                    opportunitiesToCBet -= 1;
                }
            }


            IncrementStatistics(calls, gamePhase);
        }

        /* Folded */
        public void HasFolded(HoldemGamePhase gamePhase)
        {
            if (gamePhase == HoldemGamePhase.Preflop)
            {
                if (HasPreflopRaisedThisRound)
                {
                    // We have raised preflop, but then we folded
                    opportunitiesToCBet -= 1;
                }
            }

            IncrementStatistics(folds, gamePhase);
        }


        /* Check for cbets and increments the statistics if it is a valid cbet
         * returns true when this is a cbet */
        public bool CheckForCBet(float amount)
        {
            /* This player has raised preflop and now has bet on the flop when first to act or when everybody
             * checked on him. This is a cbet */
            if (HasPreflopRaisedThisRound && !HasCBetThisRound)
            {
                HasCBetThisRound = true;
                cbets += 1;

                return true;
            }

            return false;
        }

        /* Helper function to increment the VPF stat */
        private void CheckForVoluntaryPutMoneyPreflop()
        {
            if (!HasVoluntaryPutMoneyPreflopThisRound)
            {
                HasVoluntaryPutMoneyPreflopThisRound = true;
                voluntaryPutMoneyPreflop += 1;
            }
        }


        /* Helper function to increment the PFR stat */
        private void CheckForPreflopRaise()
        {
            if (!HasPreflopRaisedThisRound)
            {
                HasPreflopRaisedThisRound = true;
                preflopRaises += 1;
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
            HasLimpedThisRound = false;
            HasVoluntaryPutMoneyPreflopThisRound = false;
            HasPreflopRaisedThisRound = false;
            HasCBetThisRound = false;
            HasCheckedTheFlopThisRound = false;
            HasCheckedTheTurnThisRound = false;
            HasCheckedTheRiverThisRound = false;
            HasCheckRaisedTheFlopThisRound = false;
            HasCheckRaisedTheTurnThisRound = false;
            HasCheckRaisedTheRiverThisRound = false;
        }       


        /* Reset the stats for a particular hash table set */
        private void ResetStatistics(Hashtable table)
        {
            table[HoldemGamePhase.Preflop] = 0;
            table[HoldemGamePhase.Flop] = 0;
            table[HoldemGamePhase.Turn] = 0;
            table[HoldemGamePhase.River] = 0;
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
