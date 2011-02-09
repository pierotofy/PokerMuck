using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    class HoldemHand : Hand
    {
        public HoldemHand(Card first, Card second) : base()
        {
            AddCard(first);
            AddCard(second);
        }
    }
}
