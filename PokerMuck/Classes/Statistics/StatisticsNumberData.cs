using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    /* Represents a statistical number data */
    public class StatisticsNumberData : StatisticsData
    {
        private int precision;

        public StatisticsNumberData(String name, float value, String category = "", int precision = 0)
            : base(name, value, category)
        {
            this.precision = precision;
        }

        public override string GetValue()
        {
            return GetFloat(precision).ToString();
        }
    }
}
