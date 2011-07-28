using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    public class HoldemBoard : Board
    {
        /* In hold'em there are five cards at the final board */
        public HoldemBoard(Card first, Card second, Card third, Card fourth, Card fifth) : 
            base("Board")
        {
            AddCard(first);
            AddCard(second);
            AddCard(third);
            AddCard(fourth);
            AddCard(fifth);
        }
    }
}
