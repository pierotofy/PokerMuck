using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace PokerMuck
{
    /* Stores statistical information */
    public class Statistics
    {
        private Hashtable table;

        public Statistics()
        {
            table = new Hashtable();
        }

        /* Sets a new statistic value */
        public void Set(String name, float value)
        {
            table[name] = value;
        }

        /* Return a particular statistic in float */
        public float GetFloat(String name)
        {
            Debug.Assert(table.ContainsKey(name), "Trying to access a statistic that doesn't exist");

            return (float)table[name];
        }

        /* Converts a particular statistic in string percentage */
        public String GetPercentage(String name, int precision = 0)
        {
            float value = GetFloat(name);
            return Math.Round(value * 100, precision).ToString() + "%";
        }

        /* Returns every statistic recorded, used for debugging */
        public override String ToString()
        {
            String result = String.Empty;

            foreach (string key in table.Keys)
            {
                result += String.Format("{0}: {1} ", key, table[key]);
            }

            return result;
        }
    }
}
