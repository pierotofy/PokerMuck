using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    /* Represents a statistical number data */
    public class StatisticsNumberData : StatisticsData
    {
        private int precision;

        public StatisticsNumberData(String name, float value, int precision = 0)
            : base(name, value)
        {
            this.precision = precision;
        }

        public override string GetValue()
        {
            return GetFloat(precision).ToString();
        }

        public override StatisticsData Average(String name, int precision, params StatisticsData[] stats)
        {
            float average = ComputeAverage(stats);
            return new StatisticsNumberData(name, average, precision);
        }
    }
}
