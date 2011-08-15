using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections;

namespace PokerMuck
{
    class HoldemColorMap : ColorMap
    {
        public HoldemColorMap()
        {
        }

       
        protected override void InitializeMapData()
        {
            /* Seat colors for holdem
             * First card: green + (seat) blue
             * Second card: red + (seat) blue */

            mapData["player_card_1_seat_1"] = Color.FromArgb(0, 255, 1);
            mapData["player_card_2_seat_1"] = Color.FromArgb(255, 0, 1);

            mapData["player_card_1_seat_2"] = Color.FromArgb(0, 255, 2);
            mapData["player_card_2_seat_2"] = Color.FromArgb(255, 0, 2);

            mapData["player_card_1_seat_3"] = Color.FromArgb(0, 255, 3);
            mapData["player_card_2_seat_3"] = Color.FromArgb(255, 0, 3);

            mapData["player_card_1_seat_4"] = Color.FromArgb(0, 255, 4);
            mapData["player_card_2_seat_4"] = Color.FromArgb(255, 0, 4);

            mapData["player_card_1_seat_5"] = Color.FromArgb(0, 255, 5);
            mapData["player_card_2_seat_5"] = Color.FromArgb(255, 0, 5);

            mapData["player_card_1_seat_6"] = Color.FromArgb(0, 255, 6);
            mapData["player_card_2_seat_6"] = Color.FromArgb(255, 0, 6);

            mapData["player_card_1_seat_7"] = Color.FromArgb(0, 255, 7);
            mapData["player_card_2_seat_7"] = Color.FromArgb(255, 0, 7);

            mapData["player_card_1_seat_8"] = Color.FromArgb(0, 255, 8);
            mapData["player_card_2_seat_8"] = Color.FromArgb(255, 0, 8);

            mapData["player_card_1_seat_9"] = Color.FromArgb(0, 255, 9);
            mapData["player_card_2_seat_9"] = Color.FromArgb(255, 0, 9);

            mapData["player_card_1_seat_10"] = Color.FromArgb(0, 255, 10);
            mapData["player_card_2_seat_10"] = Color.FromArgb(255, 0, 10);
        }

        public override ArrayList GetPlayerCardsActions(int playerSeat){
            ArrayList result = new ArrayList();
            result.Add("player_card_1_seat_" + playerSeat);
            result.Add("player_card_2_seat_" + playerSeat);
            return result;
        }
    }
}
