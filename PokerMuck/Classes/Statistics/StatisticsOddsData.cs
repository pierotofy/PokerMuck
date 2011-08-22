using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    class StatisticsOddsData : StatisticsPercentageData
    {
        public StatisticsOddsData(String name, float percentageValue, int precision = 0)
            : base(name, (percentageValue / 100.0f), precision)
        {

        }

        public override string GetValue()
        {
            return GetOdds() + " (" + GetPercentage(precision) + "%)";
        }

        protected String GetOdds()
        {
            return Math.Round((1.0f/Value) - 1.0f, precision).ToString() + ":1";
        }
    }
}
