using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    /* A board can be seen as a list of cards */
    public abstract class Board : CardList
    {
        /* Every board has an associated description */
        private String description;
        public String Description { get { return description; } }


        /* Has this board been displayed yet? */
        public bool Displayed { get; set; }

        public Board(String description)
        {
            this.description = description;
            Displayed = false;
        }
    }
}
