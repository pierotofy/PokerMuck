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
            FirstExecution = true;
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

        public bool FirstExecution
        {
            get
            {
                return GetBoolSetting("first_execution");
            }

            set
            {
                SetSetting("first_execution", value);
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

        public override String GetSettingsFilename()
        {
            return "UserSettings.xml";
        }
    }
}
