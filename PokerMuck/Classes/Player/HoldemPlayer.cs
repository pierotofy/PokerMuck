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
        private Hashtable limps;
        private Hashtable calls;
        private Hashtable bets;
        private Hashtable folds;
        private Hashtable raises;

        public bool IsBigBlind { get; set; }
        public bool IsSmallBlind { get; set; }


        public HoldemPlayer(String playerName)
            : base(playerName)
        {
            limps = new Hashtable(5);
            calls = new Hashtable(5);
            bets = new Hashtable(5);
            folds = new Hashtable(5);
            raises = new Hashtable(5);

            ResetAllStatistics();
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

        /* Has called */
        public void HasCalled(HoldemGamePhase gamePhase)
        {
            /* If he's not the small or big blind, this is also a limp */
            if (!IsSmallBlind && !IsBigBlind && gamePhase == HoldemGamePhase.Preflop) IncrementStatistics(limps, gamePhase);

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

        /* Resets all statistics counters */
        private void ResetAllStatistics()
        {
            ResetStatistics(calls);
            ResetStatistics(bets);
            ResetStatistics(folds);
            ResetStatistics(raises);
        }

        /* Reset the stats for a particular set */
        private void ResetStatistics(Hashtable table)
        {
            table[HoldemGamePhase.Preflop] = 0;
            table[HoldemGamePhase.Flop] = 0;
            table[HoldemGamePhase.Turn] = 0;
            table[HoldemGamePhase.River] = 0;

            IsBigBlind = false;
            IsSmallBlind = false;
        }
    }
}
