using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    abstract class Player
    {
        /* Every player has a name */
        private String name;
        public String Name { get { return name; } }

        /* Last mucked hand are stored here */
        public Hand MuckedHand { get; set; }

        /* Has this player showed his hands last round? */
        public bool HasShowedLastRound { get; set; }

        /* Number of hands played */
        protected int totalHandsPlayed;
        
        public Player(String name)
        {
            this.name = name;
            this.HasShowedLastRound = false;
        }

        /* This player received the whole cards */
        public void IsDealtHoleCards()
        {
            totalHandsPlayed += 1;
        }

        public virtual void ResetAllStatistics()
        {
            /* Reset the stats for a particular set */
            totalHandsPlayed = 0;   
        }

        /* Reset the statistics variables that are valid for one round only 
         * ex. We count only one call preflop as a limp, if a player raises and the first player calls again
         * this has to be counted as a single limp, not as two */
        public virtual void PrepareStatisticsForNewRound()
        {

        }
    }
}
