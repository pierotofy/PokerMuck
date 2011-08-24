using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    /* This class takes care of storing (in memory)
     * information about players */
    public class PlayerDatabase
    {
        List<Player> players;

        public PlayerDatabase()
        {
            players = new List<Player>();
        }

        public bool Contains(String playerName, String gameID)
        {
            Player p = Find(playerName, gameID);
            return p != null;
        }

        public bool Contains(Player p)
        {
            return Contains(p.Name, p.GameID);
        }

        public Player Retrieve(String playerName, String gameID)
        {
            Player p = Find(playerName, gameID);

            Trace.Assert(p != null, "Player " + playerName + " doesn't exist in our database");
            return p.Clone(); // Return a copy
        }


        public void Store(Player player)
        {
            if (Contains(player))
            {
                // Already in our database, delete old copy
                players.RemoveAll(
                            delegate(Player p)
                            {
                                return p.Name == player.Name && p.GameID == player.GameID;
                            }
                );                
            }

            // Add a reference of the player
            players.Add(player);
        }

        /* Finds a player given its player name and game id
         * It could return null */
        private Player Find(String playerName, String gameID)
        {
            // Has this player already been added?
            Player result = players.Find(
                 delegate(Player p)
                 {
                     return p.Name == playerName && p.GameID == gameID;
                 }
            );

            return result;
        }
    }
}
