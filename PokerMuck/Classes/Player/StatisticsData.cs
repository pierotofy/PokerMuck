using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    /* Represents a statistical data */
    public class StatisticsData
    {
        private String name;
        public String Name { get { return name; } }

        private float value;

        private String category;
        public String Category { get { return category; } }

        public StatisticsData(String name, float value, String category = "")
        {
            this.name = name;
            this.value = value;
            this.category = category;
        }

        /* Returns the float value of the data */
        public float GetFloat()
        {
            return value;
        }

        /* Converts the data in string percentage */
        public String GetPercentage(int precision = 0)
        {
            return Math.Round(value * 100, precision).ToString();
        }

        /* To String override */
        public override String ToString()
        {
            return String.Format("{0}: {1} ({2})", Name, value, category);
        }
    }
}
