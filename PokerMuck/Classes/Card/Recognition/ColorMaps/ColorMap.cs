using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;

namespace PokerMuck
{
    /* Every color map associates a color in a card map with an action.
     * For example, if the card map highlights the first card of the player seating on seat #1 with green
     * getActionColor("player_1_card_1") should return green.
     * 
     * We need a different color map for each game type. For example hold'em has only two cards for each player
     * but omaha has four. hold'em has a flop, but five card draw doesn't. */
    abstract class ColorMap
    {
        /* Given an action (described as a string), it returns a color
         * that is to be found in the associated color map */
        public abstract Color getActionColor(String action);

        public static ColorMap Create(PokerGame game)
        {
            switch (game)
            {
                case PokerGame.Holdem:
                    return new HoldemColorMap();
                default:
                    Debug.Assert(false, "Cannot create color map object for " + game.ToString());
                    return null;
            }
        }
    }
}
