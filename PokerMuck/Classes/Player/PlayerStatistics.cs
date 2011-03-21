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

        private Comparison<String> categorySortRoutine;

        /* categorySortRoutine decides the order that categories need to be sorted */
        public PlayerStatistics(Comparison<String> categorySortRoutine = null)
        {
            table = new Hashtable();
            this.categorySortRoutine = categorySortRoutine;
        }

        
        /* Sets a new data statistics in our table */
        public void Set(StatisticsData data)
        {
            data.Order = table.Count;
            table[data.Name + "_" + data.Category] = data;
        }

        public StatisticsData Get(String name, String category)
        {
            String key = name + "_" + category;
            StatisticsData result = (StatisticsData)table[key];
            Debug.Assert(result != null, "Trying to access a statistics value that doesn't exist. " + key);

            return result;
        }


        /* Returns all the data pertaining to a specific category
         * Items are sorted by their date of entry (first elements added are
         * displayed first ) */
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

            // Sort
            result.Sort((Comparison<StatisticsData>)CompareEntries);

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

            if (categorySortRoutine != null) result.Sort(categorySortRoutine);

            // Return 
            return result;
        }

        /* Function to sort the statistics data entries */
        private int CompareEntries(StatisticsData entry1, StatisticsData entry2)
        {
            if (entry1.Order == entry2.Order) return 0;
            else if (entry1.Order < entry2.Order) return -1;
            else return 1;
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
