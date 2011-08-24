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

        // We need a limit for the number of domains for the multiple domain index calculation
        const int MAX_DOMAINS = 256;

        private object[] subdomains; // Can be null if there are no subdomains

        public ValueCounter this[object enumIndex]
        {
            get
            {
                int index = (int)enumIndex;
                Trace.Assert(table.ContainsKey(index), "Invalid multiplevaluecounter category requested.");

                return (ValueCounter)table[index];
            }
        }

        public ValueCounter this[object primaryEnumIndex, object secondaryEnumIndex]
        {
            get
            {
                int index = CalculateIndex((int)primaryEnumIndex, (int)secondaryEnumIndex);
                Trace.Assert(table.ContainsKey(index), "Invalid multiplevaluecounter category requested.");

                return (ValueCounter)table[index];
            }
        }

        private int CalculateIndex(int domain, int subdomain)
        {
            return domain + (subdomain * MAX_DOMAINS);
        }

        public MultipleValueCounter(params object[] domains)
        {
            Trace.Assert(domains.Length <= MAX_DOMAINS, "You cannot initialize a multiplevaluecounter with more than " + MAX_DOMAINS + " domains");

            subdomains = null;
            table = new Hashtable(domains.Length);

            // Initialize
            foreach (int domain in domains)
            {
                table[domain] = new ValueCounter();
            }
        }

        public MultipleValueCounter(object[] domains, object[] subdomains){
            Trace.Assert(domains.Length <= MAX_DOMAINS, "You cannot initialize a multiplevaluecounter with more than " + MAX_DOMAINS + " domains");

            table = new Hashtable(domains.Length * subdomains.Length);
            this.subdomains = subdomains;

            // Initialize
            foreach (int domain in domains)
            {
                foreach (int subdomain in subdomains)
                {
                    table[CalculateIndex(domain, subdomain)] = new ValueCounter();
                }
            }
        }

        public float GetSumOfAllValues()
        {
            float sum = 0;
            foreach (int index in table.Keys)
            {
                sum += ((ValueCounter)table[index]).Value;
            }
            return sum;
        }

        public float GetSumOfAllValuesIn(object domain)
        {
            float sum = 0;
            if (subdomains != null)
            {
                foreach (int subdomain in subdomains)
                {
                    sum += this[domain, subdomain].Value;
                }
            }
            else
            {
                sum = this[domain].Value;
            }

            return sum;
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

            // Copy a reference of the subdomains (if available)
            if (this.subdomains != null) copy.subdomains = this.subdomains; 

            return copy;
        }

    }
}
