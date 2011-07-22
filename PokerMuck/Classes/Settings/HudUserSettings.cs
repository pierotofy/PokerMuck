using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PokerMuck
{
    class HudUserSettings : UserSettings
    {
        public HudUserSettings()
            : base("PokerMuck")
        {
            // Other init stuff?
        }

        protected override void InitializeDefaultValues()
        {

        }

        /* Retrieves the a list of positions for the hud display, based on the poker client
         * and the number of seats available. It will return an empty set if the user
         * has never expressed his preference on the matter. 
         * The positions are stored as follow:
         * x1,y1;x2,y2; ... ;xn,yn 
         * Where x1,y1 is the position of Seat #1
         */
        public List<Point> RetrieveHudWindowPositions(String clientName, int maxSeats, PokerGameType gameType)
        {
            List<Point> hudWindowPositions = new List<Point>();

            // Is the configuration here?
            String settingName = GetSettingNameForHudWindowPositions(clientName, maxSeats, gameType);
            if (HasSetting(settingName))
            {
                // Bingo! Retrieve the positions
                String positions = GetStringSetting(settingName);

                // Split it 
                String[] positionList = positions.Split(';');

                // Convert the resulting pair of "xn,yn" into points and add them to the list
                foreach (String position in positionList)
                {
                    String[] components = position.Split(',');
                    int x = Int32.Parse(components[0]);
                    int y = Int32.Parse(components[1]);

                    Point p = new Point(x, y);
                    hudWindowPositions.Add(p);
                }
            }
            else
            {
                // Nop, no config available at the moment
            }

            return hudWindowPositions;
        }

        public void StoreHudWindowPositions(String clientName, List<Point> positions, PokerGameType gameType)
        {
            // Convert the list of positions into a string format
            // x1,y1;x2,y2;...

            StringBuilder result = new StringBuilder();
            foreach (Point p in positions)
            {
                result.Append(String.Format("{0},{1};", p.X, p.Y));
            }

            // Remove last ;
            result.Remove(result.Length - 1, 1);

            String settingName = GetSettingNameForHudWindowPositions(clientName, positions.Count, gameType);
            SetSetting(settingName, result.ToString());
        }

        /* Retrieves the name of the setting used to represent the window hud position
         * for a particular client and the max seating capacity.
         * Ex. client = "PokerStars.it" and maxSeats = 9
         * Result => "PokerStars.it_9_players_hud_window_positions" */
        private String GetSettingNameForHudWindowPositions(String clientName, int maxSeats, PokerGameType gameType)
        {
            // Replace spaces with underscores
            String escapedClientName = clientName.Replace(" ", "_");

            // Convert enum to string
            String readableGameType = gameType.ToString("g");

            return String.Format("{0}_{1}_{2}_players_hud_window_positions", escapedClientName, maxSeats, readableGameType);
        }

        public override string GetSettingsFilename()
        {
            return "HudSettings.xml";
        }
    }
}
