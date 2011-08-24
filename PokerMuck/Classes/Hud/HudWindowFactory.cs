using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    /* Takes care of creating the proper type of hud, given a table
     * A holdem hud could be different than an Omaha one */
    static class HudWindowFactory
    {
        public static HudWindow CreateHudWindow(Table t)
        {
            if (t.Game == PokerGame.Holdem)
            {
                return new HoldemHudWindow();
            }

            Trace.Assert(false, "I couldn't create a hud for this table: " + t.GameID);
            return null; 
        }
    }
}
