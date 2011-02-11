using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PokerMuck
{
    /* A hand can be thought of a collection of 1 or more cards */
    public class Hand : ICloneable
    {
        protected List<Card> cards;
        public List<Card> Cards { get { return cards; } }

        public Hand()
        {
            cards = new List<Card>(5);
        }

        protected void AddCard(Card card){
            cards.Add(card);
        }

        /* Perform a deep copy */
        public object Clone()
        {
            Hand hand = (Hand)this.MemberwiseClone();
            hand.cards = new List<Card>(5);

            foreach (Card c in Cards)
            {
                hand.AddCard((Card)c.Clone());
            }
            return hand;
        }

    }
}
