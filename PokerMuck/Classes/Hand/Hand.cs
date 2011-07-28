using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PokerMuck
{
    /* A hand can be thought of a collection of 1 or more cards */
    public class Hand : CardList
    {
        public Hand()
            : base()
        {
            // No extra init required
        }

        public override int GetHashCode()
        {
            return cards.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            // Null is false
            if (obj == null) return false;
            Hand otherHand = (Hand)obj;

            // Different size is false
            if (cards.Count != otherHand.cards.Count) return false;

            bool equal = true;
            for (int i = 0; i < cards.Count; i++)
            {
                if (!cards[i].Equals(otherHand.cards[i]))
                {
                    equal = false;
                    break;
                }
            }

            return equal;
        }
    }
}
