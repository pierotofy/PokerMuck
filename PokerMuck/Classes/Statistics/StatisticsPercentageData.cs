using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    /* Represents a statistical percentage data  */
    public class StatisticsPercentageData : StatisticsData
    {
        private int precision;

        public StatisticsPercentageData(String name, float value, String category = "", int precision = 0) 
            : base(name, value, category)
        {
            this.precision = precision;
        }

        public override string GetValue()
        {
            return GetPercentage(precision) + "%";
        }
    }
}
