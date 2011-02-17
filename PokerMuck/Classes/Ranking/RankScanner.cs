using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    /* This class queries the PokerProLabs.com website and tries to find the rank of a player */
    abstract class RankScanner
    {
        /* Min and max possible ranks */
        protected abstract float GetMaxRankValue();
        protected abstract float GetMinRankValue();

        public RankScanner()
        {

        }

        public Rank FindPlayerRank(String playerName)
        {
            return new Rank(0.0f, 1.0f); //TODO
        }
    }
}
