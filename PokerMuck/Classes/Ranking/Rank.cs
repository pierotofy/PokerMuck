using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace PokerMuck
{
    public enum PokerGameCategory { HeadsUp, SitnNGo, Scheduled };

    /* Represents the rank of a player */
    class Rank
    {
        /* Holds the ranks for different game categories */
        private Hashtable ranks;

        /* The rank value ranges */
        private float maxRankValue, minRankValue;

        public Rank(float minRankValue, float maxRankValue)
        {
            this.minRankValue = minRankValue;
            this.maxRankValue = maxRankValue;
        }

        public void SetRank(float rankValue, PokerGameCategory category){
            Debug.Assert(rankValue >= minRankValue && rankValue <= maxRankValue, "rankValue is greather than or less than range.");

            ranks[category] = rankValue;
        }

        /* Returns the rank score (calculated proportionally to the maxRankValue and minRankValue)
         * 1.0 is the strongest, 0 the weakest */
        public float GetRankScore(PokerGameCategory category){
            return 1.0f;
        }
    }
}
