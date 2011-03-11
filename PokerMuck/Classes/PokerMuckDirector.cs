using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

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

        /* Player's database */
        private PlayerDatabase playerDatabase;

        /* Configuration */
        private PokerMuckUserSettings userSettings;
        public PokerMuckUserSettings UserSettings { get { return userSettings; } }

        /* Tell the UI that we need to display a hand */
        public delegate void DisplayPlayerMuckedHandHandler(Player player);
        public event DisplayPlayerMuckedHandHandler DisplayPlayerMuckedHand;

        /* Tell the UI that we need to display a final board */
        public delegate void DisplayFinalBoardHandler(Board board);
        public event DisplayFinalBoardHandler DisplayFinalBoard;


        /* Tell the UI to clear the list of mucked hands */
        public delegate void ClearAllPlayerMuckedHandsHandler();
        public event ClearAllPlayerMuckedHandsHandler ClearAllPlayerMuckedHands;

        /* Tell the UI to clear the last final board */
        public delegate void ClearFinalBoardHandler();
        public event ClearFinalBoardHandler ClearFinalBoard;

        /* Tell the UI to display a status message */
        public delegate void DisplayStatusHandler(String status);
        public event DisplayStatusHandler DisplayStatus;

        /* Tell the UI to display hud information */
        public delegate void DisplayHudHandler(Table t);
        public event DisplayHudHandler DisplayHud;

        /* Tell the UI to shift the position of the hud */
        public delegate void ShiftHudHandler(Table t);
        public event ShiftHudHandler ShiftHud;

        /* Tell the UI to remove a hud */
        public delegate void RemoveHudHandler(Table t);
        public event RemoveHudHandler RemoveHud;

        /* Tell the UI to display the statistics of this player */
        public delegate void DisplayPlayerStatisticsHandler(Player p);
        public event DisplayPlayerStatisticsHandler DisplayPlayerStatistics;



        public PokerMuckDirector()
        {
            InitializeSupportedPokerClientList();

            // Initialize the list of tables (no more than 20 concurrent games to begin with right?)
            tables = new List<Table>(20);

            // Initialize the database
            playerDatabase = new PlayerDatabase();

            // Initialize the user configuration 
            userSettings = new PokerMuckUserSettings();

            // Get the poker client from the user settings
            ChangePokerClient(userSettings.CurrentPokerClient);

            // Init windows listener
            windowsListener = new WindowsListener(this);
            windowsListener.ListenInterval = 200;
            windowsListener.StartListening();

            // Init hand history monitor
            // TODO read config to find the directory to monitor
            hhMonitor = new HHMonitor(userSettings.HandHistoryDirectory, this);
            hhMonitor.StartMonitoring();
        }

        /* TEST CODE REMOVE IN PRODUCTION */
        public void Test()
        {
            

            String filename = "test.txt";
            //String filename = "HH20110305 T371715473 No Limit Hold'em €4.46 + €0.54.txt";
            Table newTable = new Table(filename, "test.txt - Notepad", new Rectangle(30, 30, 640, 480), pokerClient, playerDatabase);
            newTable.DataHasChanged += new Table.DataHasChangedHandler(table_DataHasChanged);
            newTable.DisplayPlayerStatistics += new Table.DisplayPlayerStatisticsHandler(newTable_DisplayPlayerStatistics);
            tables.Add(newTable);
            hhMonitor.ChangeHandHistoryFile(filename); // TODO REMOVE
            
        }

        /* Change the hand history directory */
        public void ChangeHandHistoryDirectory(String newDirectory)
        {
            UserSettings.HandHistoryDirectory = newDirectory;
            hhMonitor.ChangeHandHistoryFile(newDirectory);
        }

        /* Change the poker client */
        public void ChangePokerClient(PokerClient client)
        {
            UserSettings.CurrentPokerClient = client;
            pokerClient = client;
        }

        /* Hand history monitor handler, a new line has been read from a file */
        public void NewLineArrived(String filename, String line)
        {
            // Find the table associated with this filename (it should never be null)
            Table table = FindTableByHHFilename(filename);
            Debug.Assert(table != null, "A new line has arrived for a table that hasn't been created.");

            OnDisplayStatus("Parsing... please wait.");

            table.HandHistoryParser.ParseLine(line);
        }

        /* Hand history monitor handler, a new filename has been created in our folder, we might be interested 
         * into monitoring it. This happens when we first begin a game: the window title is matching, but we can't
         * figure the filename right away. */
        public void NewFileWasCreated(String filename)
        {
            Debug.Print("New file created: {0}", filename);

            NewForegroundWindow(windowsListener.CurrentForegroundWindowTitle, windowsListener.CurrentForegroundWindowRect);
        }

        /* Hand history monitor handler, an end of file has been reached
         * this means that the UI can finally be updated. */
        public void EndOfFileReached(String filename)
        {
            Table t = FindTableByHHFilename(filename);
            Debug.Assert(t != null, "End of file was reached by a table not in our table list");

            OnDisplayStatus("Displaying Table #" + t.TableId);

            // Tell the UI to clear any previous mucked hand from the screen
            if (ClearAllPlayerMuckedHands != null) ClearAllPlayerMuckedHands();

            // And the final board (if any)
            if (ClearFinalBoard != null) ClearFinalBoard();

            // Flag to keep track of whether at least one mucked hand is available
            bool muckedHandsAvailable = false;

            // Check which players need to be shown
            foreach (Player p in t.PlayerList)
            {
                // If it has showed and it's not us
                if (p.HasShowedLastRound && p.Name != UserSettings.UserID)
                {
                    // Inform the UI
                    if (DisplayPlayerMuckedHand != null) DisplayPlayerMuckedHand(p);

                    muckedHandsAvailable = true;
                }
            }

            // Display the final board (if any and if it hasn't been displayed before and if there were mucked hands)
            if (t.FinalBoard != null && !t.FinalBoard.Displayed && muckedHandsAvailable)
            {
                if (DisplayFinalBoard != null) DisplayFinalBoard(t.FinalBoard);

                t.FinalBoard.Displayed = true;
            }

            // Display hud information
            if (DisplayHud != null) DisplayHud(t);
        }


        /* Windows Listener event handler, a window changed its position */
        public void ForegroundWindowPositionChanged(string windowTitle, Rectangle windowRect)
        {
            /* We ignore any event that is caused by a window titled "HudWindow"
             * because the user might be simply interacting with our hud */
            if (windowTitle == "HudWindow") return;

            Debug.Print("Position X: {0}, Y: {1}", windowRect.X, windowRect.Y);

            Table t = FindTableByWindowTitle(windowTitle);
            if (t != null)
            {
                // The position of this window which is associated with a table has changed

                // Update information
                t.WindowRect = windowRect;

                // Inform the UI that we might need to shift the hud
                if (ShiftHud != null) ShiftHud(t);
            }

        }

        /* Windows Listener event handler, detects when a window closes */
        public void WindowClosed(string windowTitle)
        {
            /* We ignore any event that is caused by a window titled "HudWindow"
             * because the user might be simply interacting with our hud */
            if (windowTitle == "HudWindow") return;

            Debug.Print("Window closed: " + windowTitle);

            Table t = FindTableByWindowTitle(windowTitle);

            if (t != null)
            {
                // TODO TEMP REMOVE DIALOG
                DialogResult result = MessageBox.Show("Close the hud?", "PokerMuck", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes){

                    if (RemoveHud != null) RemoveHud(t);
                    t.Terminate();
                    tables.Remove(t);

                }
            }
        }

        /* Windows Listener event handler, detects when a new windows becomes the active window */
        public void NewForegroundWindow(string windowTitle, Rectangle windowRect)
        {
            /* We ignore any event that is caused by a window titled "HudWindow"
             * because the user might be simply interacting with our hud */
            if (windowTitle == "HudWindow") return;

            Debug.Print(String.Format("Window title: {0}", windowTitle));

            String pattern = pokerClient.GetHandHistoryFilenameRegexPatternFromWindowTitle(windowTitle);
            if (pattern != String.Empty)
            {
                String filename = HHDirectoryParser.GetHandHistoryFilenameFromRegexPattern(UserSettings.HandHistoryDirectory, pattern);

                if (filename != String.Empty)
                {
                    // A valid filename was found to be associated with a window title, see if we have a table already
                    Table table = FindTableByHHFilename(filename);

                    // Is there a game associated with this filename?
                    if (table == null)
                    {
                        // First time we see it, we need to create a table for this request
                        Table newTable = new Table(filename, windowTitle, windowRect, pokerClient, playerDatabase);

                        // Set a handler that notifies us of data changes
                        newTable.DataHasChanged += new Table.DataHasChangedHandler(table_DataHasChanged);
                        
                        // Set a handler that notifies of the necessity to display the 
                        // statistics of a player
                        newTable.DisplayPlayerStatistics += new Table.DisplayPlayerStatisticsHandler(newTable_DisplayPlayerStatistics);
                        

                        // and add it to our list
                        tables.Add(newTable);
                    }
                    else
                    {
                        // Yeah we have a table, but the title might have changed... make
                        // sure the table keeps track of this change!
                        table.WindowTitle = windowTitle;

                        // Inform the UI that we might need to shift the hud
                        if (ShiftHud != null) ShiftHud(table);
                    }

                    // Start monitoring the new file!
                    hhMonitor.ChangeHandHistoryFile(filename);
                    OnDisplayStatus("Now monitoring " + filename);
                    Debug.Print(String.Format("Valid window title match! Now monitoring {0}", filename));
                }
                else
                {
                    OnDisplayStatus("New game started on window: " + windowTitle + "?");
                    Debug.Print("A valid window title was found ({0}) but no filename associated with the window could be found using pattern {1}. Is this our first hand at the table and no hand history is available?", windowTitle, pattern);

                }
            }
            else
            {
                // The window is not a poker window... do what?
            }
        }

        void newTable_DisplayPlayerStatistics(Player p)
        {
            // Notify the GUI
            if (DisplayPlayerStatistics != null) DisplayPlayerStatistics(p);
        }

        /* Data in one of the tables has changed */
        void table_DataHasChanged(Table sender)
        {

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

        /* Finds a table given its window title. It can be null */
        private Table FindTableByWindowTitle(String windowTitle)
        {
            Table result = tables.Find(
                delegate(Table t)
                {
                    return t.WindowTitle == windowTitle;
                }
            );

            return result;
        }

        /* Initializes all of the supported poker clients 
           If you add a new client in the future, remember to add it
           to this list */
        private void InitializeSupportedPokerClientList()
        {
            PokerClientsList.Add(new FullTilt());
            PokerClientsList.Add(new PokerStarsIT());
        }

        // Cleanup stuff
        private void Cleanup()
        {
            if (windowsListener != null) windowsListener.StopListening();
            if (hhMonitor != null) hhMonitor.StopMonitoring();
        }

        /* Helper method to raise the DisplayStatus event */
        private void OnDisplayStatus(String status)
        {
            if (DisplayStatus != null) DisplayStatus(status);
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
