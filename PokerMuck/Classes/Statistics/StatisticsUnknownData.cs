using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    /* Represents a statistics data that we don't know */
    class StatisticsUnknownData : StatisticsData
    {
        public StatisticsUnknownData(String name, String category = "")
            : base(name, -1, category)
        {

        }

        public override string GetValue()
        {
            return "?";
        }
    }
}
