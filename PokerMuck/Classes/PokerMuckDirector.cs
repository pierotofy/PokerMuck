using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace PokerMuck
{
    /* PokerMuckDirector orchestrates the program logic
     * - Initializes the file monitor and detects windows changes
     * - Receives notification of file changes
     * - Creates new tables
     * - etc.
     */
    class PokerMuckDirector : IDetectWindowsChanges, IHHMonitorHandler
    {
        private WindowsListener windowsListener;
        private HHMonitor hhMonitor;
        private PokerClient pokerClient;

        /* Table list */
        private List<Table> tables;

        /* Configuration */
        private PokerMuckUserSettings userSettings;
        public PokerMuckUserSettings UserSettings { get { return userSettings; } }

        public PokerMuckDirector()
        {
            InitializeSupportedPokerClientList();

            // Initialize the list of tables (no more than 20 concurrent games to begin with right?)
            tables = new List<Table>(20);

            // Initialize the user configuration 
            userSettings = new PokerMuckUserSettings();

            // Get the poker client from the user settings
            pokerClient = userSettings.CurrentPokerClient;

            // Init windows listener
            windowsListener = new WindowsListener(this);
            windowsListener.ListenInterval = 200;
            windowsListener.StartListening();

            // Init hand history monitor
            // TODO read config to find the directory to monitor
            hhMonitor = new HHMonitor(userSettings.HandHistoryDirectory, this);
            hhMonitor.StartMonitoring();


            /* TEST CODE REMOVE IN PRODUCTION */
            /*
            String filename = "test.txt";
            Table newTable = new Table(filename, "Test", pokerClient);
            newTable.DataHasChanged += new Table.DataHasChangedHandler(table_DataHasChanged);
            tables.Add(newTable);
            hhMonitor.ChangeHandHistoryFile(filename); // TODO REMOVE
             * */

        }

        /* Change the hand history directory */
        public void ChangeHandHistoryDirectory(String newDirectory)
        {
            UserSettings.HandHistoryDirectory = newDirectory;
            hhMonitor.ChangeHandHistoryFile(newDirectory);
        }

        /* Hand history monitor handler, a new line has been read from a file */
        public void NewLineArrived(String filename, String line)
        {
            // Find the table associated with this filename (it should never be null)
            Table table = FindTableByHHFilename(filename);
            Debug.Assert(table != null, "A new line has arrived for a table that hasn't been created.");

            table.HandHistoryParser.ParseLine(line);

        }

        /* Hand history monitor handler, a new filename has been created in our folder, we might be interested 
         * into monitoring it. This happens when we first begin a game: the window title is matching, but we can't
         * figure the filename right away. */
        public void NewFileWasCreated(String filename)
        {
            Debug.Print("New file created: {0}", filename);

            NewForegroundWindow(windowsListener.CurrentForegroundWindowTitle);
        }


        /* Windows Listener event handler, detects when a new windows becomes the active window */
        public void NewForegroundWindow(string windowTitle)
        {
            Debug.Print(String.Format("Window title: {0}", windowTitle));

            String pattern = pokerClient.GetHandHistoryFilenameRegexPatternFromWindowTitle(windowTitle);
            if (pattern != String.Empty)
            {
                String filename = HHDirectoryParser.GetHandHistoryFilenameFromRegexPattern(@"C:\Users\piero\AppData\Local\PokerStars.IT\HandHistory\stallion089", pattern);

                if (filename != String.Empty)
                {
                    // A valid filename was found to be associated with a window title, see if we have a table already
                    Table table = FindTableByHHFilename(filename);

                    // Is there a game associated with this filename?
                    if (table == null)
                    {
                        // First time we see it, we need to create a table for this request
                        Table newTable = new Table(filename, windowTitle, pokerClient);

                        // Set a handler that notifies us of data changes
                        newTable.DataHasChanged += new Table.DataHasChangedHandler(table_DataHasChanged);

                        // and add it to our list
                        tables.Add(newTable);
                    }

                    // Start monitoring the new file!
                    hhMonitor.ChangeHandHistoryFile(filename);
                    Debug.Print(String.Format("Valid window title match! Now monitoring {0}", filename));
                }
                else
                {
                    Debug.Print("A valid window title was found ({0}) but no filename associated with the window could be found using pattern {1}. Is this our first hand at the table and no hand history is available?", windowTitle, pattern);

                }
            }
            else
            {
                // The window is not a poker window... do what?
            }
        }

        /* Data in one of the tables has changed, refresh the UI */
        void table_DataHasChanged(Table sender)
        {
            Debug.Print("CALLED!!!!!");
        }

        /* Finds a table given its hand history filename. It can be null */
        private Table FindTableByHHFilename(String hhFilename)
        {
            Table result = tables.Find(
                delegate(Table t)
                {
                    return t.HandHistoryFilename == hhFilename;
                }
            );

            return result;
        }

        /* Initializes all of the supported poker clients 
           If you add a new client in the future, remember to add it
           to this list */
        private void InitializeSupportedPokerClientList()
        {
            PokerClientsList.Add(new PokerStarsIT());
        }

        // Cleanup stuff
        private void Cleanup()
        {
            if (windowsListener != null) windowsListener.StopListening();
            if (hhMonitor != null) hhMonitor.StopMonitoring();
        }

        /* This method must be called when you are done with the director */
        public void Terminate()
        {
            Cleanup();

            // Save configuration
            UserSettings.Save();
        }

    }
}
