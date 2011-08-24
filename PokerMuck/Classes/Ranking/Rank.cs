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

        public Rank()
        {
            ranks = new Hashtable();
        }

        /* The rank is a value between 0 and 1, with the exception of -1, which indicates that no rank is available */
        public void SetRank(float rank, PokerGameCategory category){
            Trace.Assert((rank >= 0.0f || rank == -1.0f) && rank <= 1.0f, "rank is greather than or less than the allowed range.");

            ranks[category] = rank;
        }

        /* Returns the rank score
         * 1.0 is the strongest, 0 the weakest, -1.0 is N/A */
        public float GetRank(PokerGameCategory category){
            if (ranks.ContainsKey(category))
            {
                return (float)ranks[category];
            }
            else return -1.0f;
        }

        /* Returns an average of all categories */
        public float GetCategoriesAverageRank()
        {
            return 0.0f; //TODO
        }
    }
}
