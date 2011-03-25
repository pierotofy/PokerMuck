using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    /* This class takes care of counting.
     * It has some useful features such as avoiding double counting
     * by setting a flag after the first count, which can be reset in the future
     * to allow for more counts */
    public class ValueCounter : ICloneable
    {
        private int value;
        public int Value { get { return value; } }

        private bool wasIncremented;
        public bool WasIncremented { get { return wasIncremented; } }

        public ValueCounter()
        {
        }

        /* Increments, only if the wasIncremented flag is set to false */
        public void Increment()
        {
            if (!wasIncremented)
            {
                value++;
                wasIncremented = true;
            }
        }

        /* Sets the wasIncremented flag to false to allow for further counting */
        public void AllowIncrement()
        {
            wasIncremented = false;
        }

        /* Reset the value to zero and call the allowincrement method */
        public void Reset()
        {
            value = 0;
            AllowIncrement();
        }

        /* Shallow copy, no objects to take care of */
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
