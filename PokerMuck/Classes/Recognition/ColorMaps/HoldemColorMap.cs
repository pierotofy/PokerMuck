using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace PokerMuck
{
    class HoldemColorMap : ColorMap
    {
        public HoldemColorMap()
        {
        }

        public override Color getActionColor(String action)
        {
            switch(action){
                case "player_1_card_1":
                    return Color.Red;
                case "player_1_card_2":
                    return Color.Green;

                default:
                    Debug.Assert(false, "No color is defined for action: " + action);
                    return Color.Blue;
            }
        }
    }
}
