using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace PokerMuck
{
    /* Represents a series of cards */
    public class CardList : ICloneable
    {
        public enum SortUsing { AceHigh, AceLow }
        protected List<Card> cards;

        public Card this[object enumIndex]
        {
            get
            {
                int index = (int)enumIndex;
                return cards[index];
            }
        }

        public CardList() : this(5)
        {

        }
        public CardList(int count)
        {
            cards = new List<Card>(count);
        }

        public void AddCard(Card card){
            cards.Add(card);
        }

        public void Sort(bool countAceAsHigh)
        {
            if (countAceAsHigh) Sort(SortUsing.AceHigh);
            else Sort(SortUsing.AceLow);
        }

        public void Sort(SortUsing method)
        {
            bool countAceAsHigh = (method == SortUsing.AceHigh);
            cards.Sort(delegate(Card c1, Card c2)
            {
                return c1.GetFaceValue(countAceAsHigh).CompareTo(c2.GetFaceValue(countAceAsHigh));
            });
        }

        public Card Last
        {
            get
            {
                Trace.Assert(this.Count > 0, "Cannot retrieve last card from an empty list");
                return cards.Last();
            }
        }

        public IEnumerator GetEnumerator()
        {
            return cards.GetEnumerator();
        }

        public int Count{
            get
            {
                return cards.Count;
            }
        }

        public bool HaveIdenticalFaces(int numIdenticalFaces)
        {
            Card matching;
            return HaveIdenticalFaces(numIdenticalFaces, out matching);
        }


        /* @param matching points to a card in the set of identical ones (if any) */
        public bool HaveIdenticalFaces(int numIdenticalFaces, out Card matching)
        {
            matching = null;

            for (int i = 0; i < cards.Count; i++)
            {
                int facesCount = 0;
                for (int j = i + 1; j < cards.Count; j++)
                {
                    if (cards[i].Face == cards[j].Face)
                    {
                        facesCount++;
                    }
                }

                if (facesCount >= numIdenticalFaces - 1)
                {
                    matching = cards[i];
                    return true;
                }
            }

            return false;
        }

        public bool AreConsecutive(bool countAceAsHigh, bool mustBeAllSameSuit, int numCards)
        {
            if (countAceAsHigh) Sort(SortUsing.AceHigh);
            else Sort(SortUsing.AceLow);

            int numConsecutiveCardsInSequence = 0;
            for (int i = 1; i < cards.Count; i++)
            {
                // Skip cards in the sequence that have the same face (ex. 1233456, skip element "3")
                if (cards[i - 1].Face == cards[i].Face) continue;

                if ((cards[i - 1].GetFaceValue(countAceAsHigh) + 1 == cards[i].GetFaceValue(countAceAsHigh)) &&
                    (cards[i - 1].Suit == cards[i].Suit || !mustBeAllSameSuit))
                {
                    numConsecutiveCardsInSequence++;
                    if (numConsecutiveCardsInSequence >= numCards - 1) break;
                }
                else
                {
                    numConsecutiveCardsInSequence = 0;
                }
            }

            return numConsecutiveCardsInSequence >= numCards - 1;
        }

        public override string ToString()
        {
            String res = String.Empty;
            foreach (Card c in cards)
            {
                res += c.ToString() + " ";
            }

            // Remove last space
            if (res == String.Empty) return String.Empty;
            else return res.Substring(0, res.Length - 1);
        }

        /* Perform a deep copy */
        public object Clone()
        {
            CardList cardList = (CardList)this.MemberwiseClone();
            cardList.cards = new List<Card>(5);

            foreach (Card c in cards)
            {
                cardList.AddCard((Card)c.Clone());
            }
            return cardList;
        }


        public override int GetHashCode()
        {
            return cards.GetHashCode();
        }
        
        public override bool Equals(object obj)
        {
            // Null is false
            if (obj == null) return false;
            CardList otherList = (CardList)obj;

            // Different size is false
            if (cards.Count != otherList.cards.Count) return false;

            bool equal = true;
            for (int i = 0; i < cards.Count; i++)
            {
                if (!cards[i].Equals(otherList.cards[i]))
                {
                    equal = false;
                    break;
                }
            }

            return equal;
        }
    }
}
