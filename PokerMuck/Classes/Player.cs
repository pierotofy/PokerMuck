using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    class Player
    {
        /* Every player has a name */
        private String playerName;
        public String PlayerName { get { return playerName; } }

        /* Last mucked hand are stored here */
        public Hand MuckedHand { get; set; }

        public Player(String playerName)
        {
            this.playerName = playerName;
        }
    }
}
