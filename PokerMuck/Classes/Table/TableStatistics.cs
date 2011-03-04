using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    class TableStatistics
    {
        /* What table owns these statistics? */
        protected Table table;

        public TableStatistics(Table table)
        {
            this.table = table;
        }

        /* This method takes care of registering the handlers specific to the game type */
        public virtual void RegisterParserHandlers(HHParser parser)
        {

        }

        /* We might keep track of certain data (has anybody bet on the flop this round?) that
         * is round specific and needs to be reset at the end of each hand */
        public virtual void PrepareStatisticsForNewRound()
        {

        }
    }
}
