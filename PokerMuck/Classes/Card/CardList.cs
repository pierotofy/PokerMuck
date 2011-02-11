using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    /* Represents a series of cards */
    public class CardList : ICloneable
    {
        protected List<Card> cards;
        public List<Card> Cards { get { return cards; } }

        public CardList()
        {
            cards = new List<Card>(5);
        }

        protected void AddCard(Card card){
            cards.Add(card);
        }

        /* Perform a deep copy */
        public object Clone()
        {
            CardList cardList = (CardList)this.MemberwiseClone();
            cardList.cards = new List<Card>(5);

            foreach (Card c in Cards)
            {
                cardList.AddCard((Card)c.Clone());
            }
            return cardList;
        }
    }
}
