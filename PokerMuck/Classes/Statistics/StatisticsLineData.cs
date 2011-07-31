using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    /* Not a real piece of data, just a separator line */
    class StatisticsLineData : StatisticsData
    {

        public StatisticsLineData()
            : base("", 0)
        {
        }

        public override string GetValue()
        {
            return "------";
        }

        public override StatisticsData Average(String name, int precision, params StatisticsData[] stats)
        {
            throw new NotImplementedException("There's no average for line data.");
        }
    }
}
