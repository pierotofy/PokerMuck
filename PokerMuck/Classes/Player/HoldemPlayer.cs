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

        private int limps;
        private bool HasLimpedThisRound;

        private int voluntaryPutMoneyPreflop;
        private bool HasVoluntaryPutMoneyPreflopThisRound;

        private int preflopRaises;
        private bool HasPreflopRaisedThisRound;

        private int cbets;
        private bool HasCBetThisRound;

        private int foldsToACBet;

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
            foldsToACBet = 0;
            cbets = 0;
            voluntaryPutMoneyPreflop = 0;
            totalHandsPlayed = 0;
            preflopRaises = 0;

            PrepareStatisticsForNewRound();
        }

        /* Returns the limp ratio (1.0 to 0) */
        public float GetLimpRatio()
        {
            if (totalHandsPlayed == 0) return 0.0f;

            return (float)limps / (float)totalHandsPlayed;
        }

        /* How many times has the player put money in preflop? */
        public float GetVPFRatio()
        {
            if (totalHandsPlayed == 0) return 0.0f;

            return (float)voluntaryPutMoneyPreflop / (float)totalHandsPlayed;
        }

        /* How many times has the player made a continuation bet following a preflop raise? */
        public float GetCBetRatio()
        {
            if (preflopRaises == 0) return 0.0f;

            return (float)cbets / (float)preflopRaises;
        }

        /* How many times has the player raised preflop? */
        public float GetPFRRatio()
        {
            if (totalHandsPlayed == 0) return 0.0f;

            return (float)preflopRaises / (float)totalHandsPlayed;
        }
        
        /* This player has raised, increment the stats */
        public void HasRaised(HoldemGamePhase gamePhase){
            if (gamePhase == HoldemGamePhase.Preflop)
            {
                CheckForVoluntaryPutMoneyPreflop();
                CheckForPreflopRaise();
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

        /* Has checked */
        public void HasChecked(HoldemGamePhase gamePhase)
        {
            Debug.Print(Name + " checks");
            IncrementStatistics(checks, gamePhase);
        }

        /* Has called */
        public void HasCalled(HoldemGamePhase gamePhase)
        {
            if (gamePhase == HoldemGamePhase.Preflop)
            {
                CheckForVoluntaryPutMoneyPreflop();
            }


            IncrementStatistics(calls, gamePhase);
        }

        /* Folded */
        public void HasFolded(HoldemGamePhase gamePhase)
        {
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
            }
        }

        /* Helper function to increment the value in one of the hash tables (calls, raises, folds, etc.) */
        private void IncrementStatistics(Hashtable table, HoldemGamePhase gamePhase)
        {
            table[gamePhase] = (int)table[gamePhase] + 1;
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

            result.Set("VPF", GetVPFRatio());
            result.Set("Limp", GetLimpRatio());
            result.Set("PFR", GetPFRRatio());
            result.Set("CBet", GetCBetRatio());

            return result;
        }
    }
}
