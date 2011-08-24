using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    /* Represents a statistical data */
    public abstract class StatisticsData
    {
        private String name;
        public String Name { get { return name; } } // TODO REMOVE SET

        private float value;
        public float Value { get { return value; } }

        public StatisticsData(String name, float value)
        {
            this.name = name;
            this.value = value;
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
            return String.Format("{0}: {1}", Name, value);
        }

        /* Computes the average of the current object with one more statistics data objects
         * They need to be of the same type */
        public abstract StatisticsData Average(String name, int precision, params StatisticsData[] stats);

        /* Shared function to compute the average of multiple statistics data */
        protected float ComputeAverage(params StatisticsData[] stats)
        {
            Trace.Assert(stats.Length > 0, "Cannot compute the average of zero elements.");
            float total = 0;
            float totalStatistics = stats.Length + 1;
            foreach (StatisticsData stat in stats)
            {
                // If it's an unknown value, we skip it
                if (stat is StatisticsUnknownData) totalStatistics -= 1;
                else total += stat.Value;
            }
            total += this.Value;

            float average = total / totalStatistics;
            return average;
        }
    }
}
