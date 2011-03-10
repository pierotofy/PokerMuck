using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    /* This class takes care of storing (in memory)
     * information about players */
    class PlayerDatabase
    {
        List<Player> players;

        public PlayerDatabase()
        {
            players = new List<Player>();
        }

        public bool Contains(String playerName, PokerGameType gameType)
        {
            Player p = Find(playerName, gameType);
            return p != null;
        }

        public Player Retrieve(String playerName, PokerGameType gameType)
        {
            Player p = Find(playerName, gameType);

            Debug.Assert(p != null, "Player " + playerName + " doesn't exist in our database");
            return p;
        }


        public void Store(Player player)
        {
            // Are we just updating?
            if (players.Contains(player))
            {
                int index = players.IndexOf(player);
                players[index] = player;
            }
            else
            {
                // No, we need to store a new record
                players.Add(player);
            }
        }

        /* Finds a player given its player name and type of game
         * It could return null */
        private Player Find(String playerName, PokerGameType gameType)
        {
            // Has this player already been added?
            Player result = players.Find(
                 delegate(Player p)
                 {
                     return p.Name == playerName && p.GameType == gameType;
                 }
            );

            return result;
        }
    }
}
