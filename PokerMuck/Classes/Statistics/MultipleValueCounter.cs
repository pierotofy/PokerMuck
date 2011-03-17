using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace PokerMuck
{
    /* This class allows to handle multiple values using a single instance.
     * For example, if we need to keep track of raises, we might be interested into 
     * knowing the raises preflop, on the flop, on the turn, etc.
     * These represent the same value, but on different situations */
    class MultipleValueCounter : ICloneable
    {
        Hashtable table;

        public ValueCounter this[object enumIndex]
        {
            get
            {
                int index = (int)enumIndex;
                Debug.Assert(table.ContainsKey(index), "Invalid multiplevaluecounter category requested.");

                return (ValueCounter)table[index];
            }
        }

        public MultipleValueCounter(params object[] domains)
        {
            table = new Hashtable(domains.Length);

            // Initialize
            foreach (int domain in domains)
            {
                table[domain] = new ValueCounter();
            }
        }

        /* Resets all members */
        public void Reset()
        {
            foreach (ValueCounter c in table.Values)
            {
                c.Reset();
            }
        }

        /* AllowIncrement on all members */
        public void AllowIncrement()
        {
            foreach (ValueCounter c in table.Values)
            {
                c.AllowIncrement();
            }
        }

        /* Deep copy required */
        public object Clone()
        {
            MultipleValueCounter copy = new MultipleValueCounter();
            copy.table = new Hashtable(this.table.Keys.Count);

            // Clone all members of the table
            foreach (int domain in this.table.Keys)
            {
                copy.table[domain] = ((ValueCounter)this.table[domain]).Clone();
            }

            return copy;
        }

    }
}
