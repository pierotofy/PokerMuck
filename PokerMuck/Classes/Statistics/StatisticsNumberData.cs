using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    /* Represents a statistical number data */
    public class StatisticsNumberData : StatisticsData
    {
        public StatisticsNumberData(String name, float value, String category = "")
            : base(name, value, category)
        {

        }

        public override string GetValue()
        {
            return GetFloat().ToString();
        }
    }
}
