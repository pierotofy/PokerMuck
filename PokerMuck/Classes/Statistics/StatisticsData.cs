using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    /* Represents a statistical data */
    public abstract class StatisticsData
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
        public float GetFloat(int precision = 0)
        {
            return (float)Math.Round(value, precision);
        }
        
        /* Converts the data in string percentage */
        public String GetPercentage(int precision = 0)
        {
            return Math.Round(value * 100, precision).ToString();
        }

        /* Returns the default format for this data */
        public abstract String GetValue();

        /* To String override */
        public override String ToString()
        {
            return String.Format("{0}: {1} ({2})", Name, value, category);
        }
    }
}
