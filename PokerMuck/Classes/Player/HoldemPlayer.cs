using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PokerMuck
{
    /* A holdem player has certain statistics that a five card draw player might not have */
    class HoldemPlayer : Player
    {
        /* Each table is set this way:
         key => value
         GamePhase => value
         Ex. calls[flop] == 4 --> player has flat called 4 times during the flop
         */
        private int limps;
        private Hashtable calls;
        private Hashtable bets;
        private Hashtable folds;
        private Hashtable raises;

        public bool IsBigBlind { get; set; }
        public bool IsSmallBlind { get; set; }

        private bool HasLimpedThisRound;


        public HoldemPlayer(String playerName)
            : base(playerName)
        {
            calls = new Hashtable(5);
            bets = new Hashtable(5);
            folds = new Hashtable(5);
            raises = new Hashtable(5);

            ResetAllStatistics();
        }

        /* Returns the % of limps (1.0 to 0) */
        public float GetLimpPercentage()
        {
            return (float)limps / (float)totalHandsPlayed;
        }

        /* How many times has the player raised? */
        public float GetRaisesPreflopPercentage()
        {
            //return (float)raises[HoldemGamePhase.Preflop] / (float)totalHandsPlayed;
        }
        
        /* This player has raised, increment the stats */
        public void HasRaised(HoldemGamePhase gamePhase){
            IncrementStatistics(raises, gamePhase);
        }

        /* Has bet */
        public void HasBet(HoldemGamePhase gamePhase)
        {
            IncrementStatistics(bets, gamePhase);
        }

        /* Has limped */
        public void HasLimped()
        {
            /* If he's not the small or big blind, this is also a limp */
            if (!IsSmallBlind && !IsBigBlind && !HasLimpedThisRound)
            {
                limps += 1;
                HasLimpedThisRound = true;
            }
        }

        /* Has called */
        public void HasCalled(HoldemGamePhase gamePhase)
        {
            IncrementStatistics(calls, gamePhase);
        }

        /* Folded */
        public void HasFolded(HoldemGamePhase gamePhase)
        {
            IncrementStatistics(folds, gamePhase);
        }

        private void IncrementStatistics(Hashtable table, HoldemGamePhase gamePhase)
        {
            table[gamePhase] = (int)table[gamePhase] + 1;
        }

        public override void PrepareStatisticsForNewRound()
        {
            base.PrepareStatisticsForNewRound();

            IsBigBlind = false;
            IsSmallBlind = false;
            HasLimped = false;
        }

        
        /* Resets all statistics counters */
        public override void ResetAllStatistics()
        {
            base.ResetAllStatistics();

            ResetStatistics(calls);
            ResetStatistics(bets);
            ResetStatistics(folds);
            ResetStatistics(raises);
            limps = 0;

            PrepareStatisticsForNewRound();
        }

        /* Reset the stats for a particular set */
        private void ResetStatistics(Hashtable table)
        {
            table[HoldemGamePhase.Preflop] = 0;
            table[HoldemGamePhase.Flop] = 0;
            table[HoldemGamePhase.Turn] = 0;
            table[HoldemGamePhase.River] = 0;
        }
    }
}
