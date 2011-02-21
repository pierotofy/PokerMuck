using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    abstract class Player
    {
        /* Every player has a name */
        private String name;
        public String Name { get { return name; } }

        /* Last mucked hand are stored here */
        public Hand MuckedHand { get; set; }

        /* Has this player showed his hands last round? */
        public bool HasShowedLastRound { get; set; }
        
        public Player(String name)
        {
            this.name = name;
            this.HasShowedLastRound = false;
        }
    }
}
