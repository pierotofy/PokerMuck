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
            CurrentPokerClient = new PokerStars("English");
            WindowPosition = new Point(480, 320); // We assume monitors will be bigger than this resolution
            WindowSize = new Size(344, 456); // Designer size
            TableDisplayRelativeWindowPosition = new Point(100, 100);
            TableDisplayWindowSize = new Size(234, 344);
            FirstExecution = true;
            TrainingModeEnabled = false;
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

        public String GetHandHistoryDirectoryFor(PokerClient client)
        {
            String key = client.XmlName + "_hand_history_directory";
            // Has the user ever specified a directory for this client?
            if (HasSetting(key))
            {
                return (String)GetSetting(key);
            }
            else return String.Empty;
        }

        public void SetHandHistoryDirectoryFor(PokerClient client, String directory)
        {
            String key = client.XmlName + "_hand_history_directory";
            SetSetting(key, directory);
        }

        public bool TrainingModeEnabled
        {
            get
            {
                return GetBoolSetting("training_mode_enabled");
            }

            set
            {
                SetSetting("training_mode_enabled", value);
                Trace.WriteLine("Setting training mode to " + value);
            }
        }

        /* This is the actual location of the hand history files (includes dynamic subdirectories) */
        public String HandHistoryDirectory
        {
            get
            {
                PokerClient currentClient = CurrentPokerClient;
                Trace.Assert(currentClient != null, "Current poker client is null but hand history directory was accessed.");
                return StoredHandHistoryDirectory + @"\" + currentClient.GetCurrentHandHistorySubdirectory() + @"\";
            }
        }

        /* Returns the hand history directory of the current poker client
         * (NOT including dynamic subdirectories, see PartyPoker) */
        public String StoredHandHistoryDirectory
        {
            get
            {
                PokerClient currentClient = CurrentPokerClient;
                Trace.Assert(currentClient != null, "Current poker client is null but stored hand history directory was accessed.");
                return GetHandHistoryDirectoryFor(currentClient);
            }

            set
            {
                PokerClient currentClient = CurrentPokerClient;
                Trace.Assert(currentClient != null, "Current poker client is null but stored hand history directory was set.");
                SetHandHistoryDirectoryFor(currentClient, value);
            }
        }

        public PokerClient CurrentPokerClient
        {
            get
            {
                String pokerClientName = GetStringSetting("poker_client_name");
                
                // Find in our list of available poker clients a poker client that reflect the name in the config (if any)
                PokerClient result = PokerClientsList.Find(pokerClientName);
                
                String pokerClientLanguage = GetStringSetting("poker_client_language");
                String pokerClientTheme = GetStringSetting(result.XmlName + "_theme");

                result.InitializeLanguage(pokerClientLanguage);
                result.SetTheme(pokerClientTheme);

                return result;
            }

            set
            {
                SetSetting("poker_client_name", value.Name);
                SetSetting("poker_client_language", value.CurrentLanguage);
                SetSetting(value.XmlName + "_theme", value.CurrentTheme);                
            }
        }

        public Size TableDisplayWindowSize
        {
            get
            {
                int width = GetIntSetting("table_display_window_width");
                int height = GetIntSetting("table_display_window_height");
                return new Size(width, height);
            }

            set
            {
                SetSetting("table_display_window_width", value.Width);
                SetSetting("table_display_window_height", value.Height);
            }
        }

        public Point TableDisplayRelativeWindowPosition
        {
            get
            {
                int posX = GetIntSetting("table_display_relative_window_position_x");
                int posY = GetIntSetting("table_display_relative_window_position_y");
                return new Point(posX, posY);
            }

            set
            {
                SetSetting("table_display_relative_window_position_x", value.X);
                SetSetting("table_display_relative_window_position_y", value.Y);
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
