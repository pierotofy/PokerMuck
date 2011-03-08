using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace PokerMuck
{
    /* Stores statistical information */
    public class PlayerStatistics
    {
        private Hashtable table;

        public PlayerStatistics()
        {
            table = new Hashtable();
        }

        /* Sets a new statistic value */
        public void Set(String name, float value, String category = "Summary")
        {
            table[name] = new StatisticsData(name, value, category);
        }

        /* Return a particular statistic in float */
        public float GetFloat(String name)
        {
            Debug.Assert(table.ContainsKey(name), "Trying to access a statistic that doesn't exist");

            return ((StatisticsData)table[name]).GetFloat();
        }

        /* Converts a particular statistic in string percentage */
        public String GetPercentage(String name, int precision = 0)
        {
            Debug.Assert(table.ContainsKey(name), "Trying to access a statistic that doesn't exist");

            return ((StatisticsData)table[name]).GetPercentage(precision);
        }

        /* Returns all the data pertaining to a specific category */
        public List<StatisticsData> GetStatistics(String category)
        {
            List<StatisticsData> result = new List<StatisticsData>();

            // For every data in our table
            foreach (StatisticsData value in table.Values)
            {
                if (value.Category == category)
                {
                    result.Add(value);
                }
            }

            // TODO sort?

            return result;
        }

        /* Computes all the categories that have been added in the statistics at this point */
        public List<String> GetCategories()
        {
            List<String> result = new List<String>();

            // For every statistical data in our table
            foreach (StatisticsData value in table.Values)
            {
                // Check if we have added the category in our result
                if (!result.Contains(value.Category)){
                    // If not, add it
                    result.Add(value.Category);
                }                
            }

            // TODO sort?

            // Return 
            return result;
        }

        /* Returns every statistic recorded, useful for debugging */
        public override String ToString()
        {
            String result = String.Empty;

            foreach (StatisticsData value in table.Values)
            {
                result += value.ToString();
            }

            return result;
        }
    }
}
