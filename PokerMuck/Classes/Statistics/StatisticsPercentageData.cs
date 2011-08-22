using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    /* Represents a statistical percentage data  */
    public class StatisticsPercentageData : StatisticsData
    {
        protected int precision;

        public StatisticsPercentageData(String name, float value, int precision = 0) 
            : base(name, value)
        {
            this.precision = precision;
        }

        public override string GetValue()
        {
            return GetPercentage(precision) + "%";
        }

        public override StatisticsData Average(String name, int precision, params StatisticsData[] stats)
        {
            float average = ComputeAverage(stats);
            return new StatisticsPercentageData(name, average, precision);
        }
    }
}
