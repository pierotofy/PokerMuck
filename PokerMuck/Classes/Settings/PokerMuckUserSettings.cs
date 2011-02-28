using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace PokerMuck
{
    /* More specialized class that handles program specific characteristics */
    class PokerMuckUserSettings : UserSettings
    {
        public PokerMuckUserSettings()
            : base("PokerMuck")
        {
            // Other init stuff?
        }

        protected override void InitializeDefaultValues()
        {
            HandHistoryDirectory = "";
            CurrentPokerClient = new PokerStarsIT("English");
            WindowPosition = new Point(480, 320); // We assume monitors will be bigger than this resolution
            WindowSize = new Size(209, 331); // Designer size
            UserID = "";
        }

        public String HandHistoryDirectory
        {
            get
            {
                return GetStringSetting("hand_history_directory");
            }

            set{
                SetSetting("hand_history_directory", value);
            }
        }

        public String UserID
        {
            get
            {
                return GetStringSetting("player_user_id");
            }

            set
            {
                SetSetting("player_user_id", value);
            }
        }

        public PokerClient CurrentPokerClient
        {
            get
            {
                String pokerClientName = GetStringSetting("poker_client_name");
                String pokerClientLanguage = GetStringSetting("poker_client_language");
                
                // Find in our list of available poker clients a poker client that reflect the name in the config (if any)
                PokerClient result = PokerClientsList.Find(pokerClientName);
                result.InitializeLanguage(pokerClientLanguage);

                return result;
            }

            set
            {
                SetSetting("poker_client_name", value.Name);
                SetSetting("poker_client_language", value.CurrentLanguage);
            }
        }

        public Size WindowSize
        {
            get
            {
                int width = GetIntSetting("window_width");
                int height = GetIntSetting("window_height");
                return new Size(width, height);
            }

            set
            {
                SetSetting("window_width", value.Width);
                SetSetting("window_height", value.Height);
            }
        }

        public Point WindowPosition
        {
            get
            {
                int posX = GetIntSetting("window_position_x");
                int posY = GetIntSetting("window_position_y");
                return new Point(posX, posY);
            }

            set
            {
                SetSetting("window_position_x", value.X);
                SetSetting("window_position_y", value.Y);
            }
        }

        /* Retrieves the a list of positions for the hud display, based on the poker client
         * and the number of players to display. It will return an empty set if the user
         * has never expressed his preference on the matter. 
         * The positions are stored as follow:
         * x1,y1;x2,y2; ... ;xn,yn */
        public List<Point> GeHudWindowPositions(PokerClient client, int numberOfPlayers)
        {
            List<Point> hudWindowPositions = new List<Point>();

            // Is the configuration here?
            String settingName = GetSettingNameForHudWindowPositions(client, numberOfPlayers);
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

        public void SetHudWindowPositions(PokerClient client, List<Point> positions)
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

            String settingName = GetSettingNameForHudWindowPositions(client, positions.Count);
            SetSetting(settingName, result.ToString());
        }

        /* Retrieves the name of the setting used to represent the window hud position
         * for a particular client and number of players.
         * Ex. client = "PokerStars.it" and numberOfPlayers = 9
         * Result => "PokerStars.it_9_players_hud_window_positions" */
        private String GetSettingNameForHudWindowPositions(PokerClient client, int numberOfPlayers)
        {
            return String.Format("{0}_{1}_players_hud_window_positions", client.Name, numberOfPlayers);
        }

        public override String GetSettingsFilename()
        {
            return "UserSettings.xml";
        }
    }
}
