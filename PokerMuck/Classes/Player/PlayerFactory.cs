using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    /* Creates new instances of the Player class */
    static class PlayerFactory
    {
        /* Factory method for creating new instances of player classes */
        public static Player CreatePlayer(String playerName, PokerGame gameType)
        {
            switch (gameType)
            {
                case PokerGame.Holdem:
                    return new HoldemPlayer(playerName);
            }

            Trace.Assert(false, "CreatePlayer was called for an unhandled game type");
            return null;
        }
    }
}
