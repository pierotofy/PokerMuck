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
    }
}
