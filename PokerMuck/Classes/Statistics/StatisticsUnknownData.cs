using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    /* Represents a statistics data that we don't know */
    class StatisticsUnknownData : StatisticsData
    {
        public StatisticsUnknownData(String name)
            : base(name, 0)
        {
        }

        public override string GetValue()
        {
            return "?";
        }

        public override StatisticsData Average(String name, int precision, params StatisticsData[] stats)
        {
            Trace.Assert(stats.Length > 0, "Cannot compute the average of zero elements.");

            // Make sure that the stats we have at least one non-unknown value (otherwise this method will cause an
            // infinite loop
            bool allUnknown = true;
            for (int j = 0; j < stats.Length; j++)
            {
                if (!(stats[j] is StatisticsUnknownData))
                {
                    allUnknown = false;
                    break;
                }
            }

            if (allUnknown) return new StatisticsUnknownData(name);
            else
            {
                // This statistics does not know its value, so we take the first element in the other statistics
                // And calculate the average through them

                StatisticsData[] newParams = new StatisticsData[stats.Length];
                int i;
                for (i = 0; i < stats.Length - 1; i++)
                {
                    newParams[i] = stats[i + 1];
                }
                newParams[i] = this;

                return stats[0].Average(name, precision, newParams);
            }
        }
    }
}
