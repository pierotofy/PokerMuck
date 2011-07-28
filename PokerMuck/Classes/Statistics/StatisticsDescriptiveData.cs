using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    /* Represents a description */
    class StatisticsDescriptiveData : StatisticsData
    {
        private String description;

        public StatisticsDescriptiveData(String name, String description)
            : base(name, 0)
        {
            this.description = description;
        }

        public override string GetValue()
        {
            return description;
        }

        public override StatisticsData Average(String name, int precision, params StatisticsData[] stats)
        {
            throw new NotImplementedException("There's no average for descriptive data.");
        }
    }
}
