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


    class PokerMuckDirector : IDetectWindowsChanges, INewFilesMonitorHandler
    {
        private WindowsListener windowsListener;
        private PokerClient pokerClient;

        /* Table list */
        private List<Table> tables;

        /* Player's database */
        private PlayerDatabase playerDatabase;

        /* NewFilesMonitor instance */
        private NewFilesMonitor newFilesMonitor;

        /* Tells the UI to run a specific routine (certain actions like the creation of new windows cannot be performed
         * from different threads).
         * @param asychronous Specify whether the routine should be executed synchronously or asynchronously*/
        public delegate void RunGUIRoutineHandler(Action d, Boolean asynchronous);
        public event RunGUIRoutineHandler RunGUIRoutine;

        public delegate void DisplayStatusHandler(String status);
        public event DisplayStatusHandler DisplayStatus;
        public void OnDisplayStatus(String status)
        {
            if (DisplayStatus != null) DisplayStatus(status);
        }

        private Object createTableLock = new Object(); // Used for thread safety synchronization

        public PokerMuckDirector()
        {
            InitializeSupportedPokerClientList();

            // Initialize the list of tables (no more than 20 concurrent games to begin with right?)
            tables = new List<Table>(20);

            // Initialize the database
            playerDatabase = new PlayerDatabase();

            // Initialize the user configuration 
            Globals.UserSettings = new PokerMuckUserSettings();

            // First execution?
            
            if (Globals.UserSettings.FirstExecution)
            {
                ShowFirstExecutionWizard();

                // Reload settings
                Globals.UserSettings = new PokerMuckUserSettings();

                Globals.UserSettings.FirstExecution = false;

                // Save
                Globals.UserSettings.Save();
            }

            // Get the poker client from the user settings
            ChangePokerClient(Globals.UserSettings.CurrentPokerClient);

            // Init windows listener
            windowsListener = new WindowsListener(this);
            windowsListener.ListenInterval = 200;
            windowsListener.StartListening();

            // Init new files monitor
            newFilesMonitor = new NewFilesMonitor(Globals.UserSettings.HandHistoryDirectory, this);
            newFilesMonitor.StartMonitoring();
        }


        /* TEST CODE REMOVE IN PRODUCTION */
        public void Test()
        {
            

            String filename = "test.txt";
            //String filename = "HH20110305 T371715473 No Limit Hold'em €4.46 + €0.54.txt";
            Table newTable = new Table(filename, new Window("test.txt - Notepad"), pokerClient, playerDatabase);
            tables.Add(newTable);           
        }

        private void SetHudVisible(Table t, bool visible)
        {
            RunGUIRoutine((Action)delegate()
            {
                t.Hud.Visible = visible;
            }, 
            true);
        }

        /* Shift the position of the hud */
        void ShiftHud(Table t)
        {
            RunGUIRoutine((Action)delegate()
            {
                t.Hud.Shift();
            },
            true);
        }

        /* Note that some GUIoperations are still executed through a chain of events 
         * that eventually arrive to the director. All newer code should use this method to 
         * run code from the GUI thread */
        public void RunFromGUIThread(Action d, Boolean asynchronous)
        {
            RunGUIRoutine(d, asynchronous);
        }

        /* A window has been minimized... hide the hud associated with it */
        public void WindowMinimized(string windowTitle)
        {
            Trace.WriteLine("Minimized: " + windowTitle);
            Table t = FindTableByWindowTitle(windowTitle);
            if (t != null){
                SetHudVisible(t, false);
                t.HideTableDisplayWindow();
            }
        }

        /* A window has been maximized... show the hud */
        public void WindowMaximized(string windowTitle)
        {
            Trace.WriteLine("Maximized: " + windowTitle);
            Table t = FindTableByWindowTitle(windowTitle);
            if (t != null)
            {
                SetHudVisible(t, true);
                t.ShowTableDisplayWindow();
            }
        }

        private void ShowFirstExecutionWizard()
        {
            FrmFirstExecutionWizard firstExecutionWizard = new FrmFirstExecutionWizard();
            firstExecutionWizard.ShowDialog();
        }

        /* Change the hand history directory */
        public void ChangeHandHistoryDirectory(String newDirectory)
        {
            Globals.UserSettings.StoredHandHistoryDirectory = newDirectory;

            // We need to create a new monitor
            newFilesMonitor = new NewFilesMonitor(Globals.UserSettings.HandHistoryDirectory, this);
            newFilesMonitor.StartMonitoring();

            Trace.WriteLine("Changing hand history directory: " + Globals.UserSettings.HandHistoryDirectory);
        }

        /* Change the poker client */
        public void ChangePokerClient(PokerClient client)
        {
            Globals.UserSettings.CurrentPokerClient = client;
            pokerClient = client;

            // Also change directory
            ChangeHandHistoryDirectory(Globals.UserSettings.StoredHandHistoryDirectory);

            // On load certain operations might need to be done by the client
            Globals.UserSettings.CurrentPokerClient.DoStartupProcessing(Globals.UserSettings.StoredHandHistoryDirectory);
        }

      
        /* Windows Listener event handler, a window changed its position */
        public void ForegroundWindowPositionChanged(string windowTitle, Rectangle windowRect)
        {
            /* We ignore any event that is caused by a window titled "HudWindow"
             * because the user might be simply interacting with our hud */
            if (windowTitle == "HudWindow") return;

            Table t = FindTableByWindowTitle(windowTitle);
            if (t != null)
            {
                // The position of this window which is associated with a table has changed

                // Inform the UI that we might need to shift the hud
                RunGUIRoutine((Action)delegate()
                                {
                                    t.Hud.Shift();
                                    if (t.DisplayWindow != null) t.DisplayWindow.UpdatePosition();
                                }, true);

            }

            CheckForWindowsOverlaysOnConnectedWindows(windowTitle, windowRect);
        }

        private void CheckForWindowsOverlaysOnConnectedWindows(String windowTitle, Rectangle windowRect)
        {
            // Check for windows overlays on huds
            foreach (Table t in tables)
            {
                RunGUIRoutine((Action)delegate()
                {
                    if (t.Hud != null) t.Hud.CheckForWindowOverlay(windowTitle, windowRect);
                    if (t.DisplayWindow != null) t.DisplayWindow.CheckForWindowOverlay(windowTitle, windowRect);
                },
                  false);
            }
        }

        /* Windows Listener event handler, detects when a window closes */
        public void WindowClosed(string windowTitle)
        {
            Trace.WriteLine("Window closed: " + windowTitle);

            Table t = FindTableByWindowTitle(windowTitle);

            if (t != null)
            {
                // TODO TEMP REMOVE DIALOG
                DialogResult result = MessageBox.Show("Close the hud?", "PokerMuck", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes){
                    t.Terminate();
                    tables.Remove(t);
                }
            }
        }

        /* A new file was created! This might belong to one of the game windows */
        public void NewFileWasCreated(String filename)
        {
            CreateTableFromPokerWindow(windowsListener.CurrentForegroundWindowTitle);
        }

        /* Windows Listener event handler, detects when a new windows becomes the active window */
        public void NewForegroundWindow(string windowTitle, Rectangle windowRect)
        {
            if (windowTitle == "HudWindow" || windowTitle == "TableDisplayWindow") return; // Ignore hud and table display windows

            CreateTableFromPokerWindow(windowTitle);
            
            CheckForWindowsOverlaysOnConnectedWindows(windowTitle, windowRect);
        }

        private void CreateTableFromPokerWindow(string windowTitle)
        {
            /* We ignore any event that is caused by a window titled "HudWindow"
             * because the user might be simply interacting with our hud.
             * Same for "TableDisplayWindow" */
            if (windowTitle == "HudWindow" || windowTitle == "TableDisplayWindow") return;

            // Make sure the hand history directory is set (otherwise we cannot create a table)
            if (!System.IO.Directory.Exists(Globals.UserSettings.StoredHandHistoryDirectory))
            {
                Trace.WriteLine("A valid game is available on window " + windowTitle + ", but the hand history path is invalid. Fix the hand history path in your settings");
                return;
            }

            Trace.WriteLine(String.Format("Window title: {0}", windowTitle));

            String pattern = pokerClient.GetHandHistoryFilenameRegexPatternFromWindowTitle(windowTitle);
            if (pattern != String.Empty)
            {
                // If multiple events call this function while a table is added, multiple (non-expected) copies of a table will appear
                lock (createTableLock)
                {
                    // Valid poker window

                    pokerClient.DoPregameProcessing(Globals.UserSettings.StoredHandHistoryDirectory);

                    // We need to monitor this window for when it closes...
                    windowsListener.AddToMonitorList(windowTitle);

                    // Do we have a filename matching this window?
                    String filename = HHDirectoryParser.GetHandHistoryFilenameFromRegexPattern(Globals.UserSettings.HandHistoryDirectory, pattern);

                    if (filename != String.Empty)
                    {
                        String filePath = Globals.UserSettings.HandHistoryDirectory + @"\" + filename;

                        // A valid filename was found to be associated with a window title, see if we have a table already
                        Table table = FindTableByHHFilePath(filePath);

                        // Is there a game associated with this filename?
                        if (table == null)
                        {
                            // First time we see it, we need to create a table for this request
                            Table newTable = new Table(filePath, new Window(windowTitle), pokerClient, playerDatabase);

                            // and add it to our list
                            tables.Add(newTable);

                            Trace.WriteLine("Created new table: " + newTable.WindowTitle + " on " + newTable.HandHistoryFilePath);

                            OnDisplayStatus("Parsing for the first time... please wait.");

                            // Check for changes, now!
                            newTable.ParseHandHistoryNow();
                        }
                        else
                        {
                            // Inform the UI that we might need to shift the hud
                            ShiftHud(table);
                        }

                        OnDisplayStatus("Focus is on the table associated with " + filename);
                        Trace.WriteLine(String.Format("Valid window title match with {0}", filename));
                    }
                    else
                    {
                        OnDisplayStatus("New game started on window: " + windowTitle);
                        Trace.WriteLine(String.Format("A valid window title was found ({0}) but no filename associated with the window could be found using pattern {1}. Is this our first hand at the table and no hand history is available?", windowTitle, pattern));
                    }
                }
            }
            else
            {
                // The window is not a poker window... do what?
            }
        }

        /* Finds a table given its hand history filename. It can be null */
        private Table FindTableByHHFilePath(String hhFilePath)
        {
            Table result = tables.Find(
                delegate(Table t)
                {
                    return t.HandHistoryFilePath == hhFilePath;
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
            PokerClientsList.Add(new PokerStars());
            PokerClientsList.Add(new PartyPoker());
            PokerClientsList.Add(new GDPoker());
            PokerClientsList.Add(new SealsWithClubs());

            PokerClientsList.SetDefault(new PokerStars());
        }

        /* Tools stuff */


        public bool TakeActiveWindowScreenshot(bool clientOnly)
        {
            if (windowsListener != null)
            {
                Window w = new Window(windowsListener.CurrentForegroundWindowTitle);

                ScreenshotTaker st = new ScreenshotTaker();
                Bitmap screenshot = st.Take(w, clientOnly, ScreenshotTaker.Mode.PrintScreen);

                try
                {
                    screenshot.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\screenshot.png");
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        // Cleanup stuff
        private void Cleanup()
        {
            if (windowsListener != null) windowsListener.StopListening();
            if (newFilesMonitor != null) newFilesMonitor.StopMonitoring();

            foreach (Table t in tables)
            {
                t.Terminate();
            }
        }

        /* This method must be called when you are done with the director */
        public void Terminate()
        {
            Cleanup();

            // Save configuration
            Globals.UserSettings.Save();
        }

    }
}
