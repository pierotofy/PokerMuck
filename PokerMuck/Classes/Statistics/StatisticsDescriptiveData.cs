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

        public StatisticsDescriptiveData(String name, String category, String description)
            : base(name, 0, category)
        {
            this.description = description;
        }

        public override string GetValue()
        {
            return description;
        }
    }
}
