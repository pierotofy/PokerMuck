using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    /* This class queries the PokerProLabs.com website and tries to find the rank of a player */
    abstract class RankScanner
    {
        public RankScanner()
        {

        }

        public abstract Rank FindPlayerRank(String playerName);
    }
}
