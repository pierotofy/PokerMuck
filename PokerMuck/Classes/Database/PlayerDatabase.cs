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

        public bool Contains(String playerName)
        {
            Player p = Find(playerName);
            return p != null;
        }

        public Player Retrieve(String playerName)
        {
            Player p = Find(playerName);

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

        /* Remove a player from the table */
        private void RemovePlayer(String playerName)
        {
            players.RemoveAll(
                delegate(Player p)
                {
                    return p.Name == playerName;
                }
            );

        }

        /* Finds a player given its player name
         * It could return null */
        private Player Find(String playerName)
        {
            // Has this player already been added?
            Player result = players.Find(
                 delegate(Player p)
                 {
                     return p.Name == playerName;
                 }
            );

            return result;
        }
    }
}
