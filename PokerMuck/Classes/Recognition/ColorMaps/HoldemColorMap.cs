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

        protected override void InitializeMapData()
        {
            mapData["player_1_card_1"] = Color.Red;
            mapData["player_1_card_2"] = Color.Blue;
        }
    }
}
