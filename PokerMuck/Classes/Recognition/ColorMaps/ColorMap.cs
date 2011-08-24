using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Collections;

namespace PokerMuck
{
    /* Every color map object associates a color in a color map file with an action (read it again).
     * For example, if the color map file highlights the first card of the player seating on seat #1 with green
     * getActionColor("player_1_card_1") should return green.
     * 
     * We need a different color map for each game type. For example hold'em has only two cards for each player
     * but omaha has four. hold'em has a flop, but five card draw doesn't. */
    abstract class ColorMap
    {
        /* Contains the associaction action => color, MUST be initialized by initializeMapData */
        protected Hashtable mapData;
        protected abstract void InitializeMapData();

        public ColorMap()
        {
            mapData = new Hashtable();
            InitializeMapData();
        }

        /* Given an action (described as a string), it returns a color
         * that is to be found in the associated color map */
        public Color GetColorFor(String action)
        {
            Trace.Assert(mapData.ContainsKey(action), "Trying to access an action from the color map that has not been assigned to a color: " + action);
            return (Color)mapData[action];
        }

        /* Every game must have player cards in the map, but certain game modes might
         * deal more cards than others (holdem: 2, omaha: 4). This method returns a list
         * of actions that represent the cards of the player at a particular seat */
        public abstract ArrayList GetPlayerCardsActions(int playerSeat);

        /* Certain actions must be of the same size. Subclasses should return a set
         * of actions that are required to be of the same size */
        public abstract ArrayList GetSameSizeActions();

        public bool SupportsCommunityCards { 
            get { return GetCommunityCardsActions().Count > 0; } 
        }

        /* Simply return an empty arraylist if the map does not support community cards */
        public abstract ArrayList GetCommunityCardsActions();

        public ICollection Actions{
            get { return mapData.Keys; }
        }

        public static ColorMap Create(PokerGame game)
        {
            switch (game)
            {
                case PokerGame.Holdem:
                    return new HoldemColorMap();
                default:
                    Trace.Assert(false, "Cannot create color map object for " + game.ToString());
                    return null;
            }
        }
    }
}
