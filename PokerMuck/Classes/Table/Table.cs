using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace PokerMuck
{
    public class Table : IHHMonitorHandler, IVisualRecognitionManagerHandler
    {
        /* Hand history monitor */
        private HHMonitor hhMonitor;

        /* What client is table using? */
        private PokerClient pokerClient;

        /* Reference to the player's database */
        PlayerDatabase playerDatabase;

        /* Holds a list of the players currently seated at the table
         * Note that they're going to be added in order of seating
         * (Seat #1 Player will be stored as the first element) */
        private List<Player> playerList;
        public List<Player> PlayerList { get { return playerList; } }

        /* The last final board available */
        private Board finalBoard;
        public Board FinalBoard { get { return finalBoard; } }

        /* Game Type */
        private PokerGameType gameType;
        public PokerGameType GameType { get { return gameType; } }

        /* Game ID associated with this table */
        public String GameID { get; set; }

        /* Identification string of the table */
        public String TableId { get; set; }

        /* Max seating capacity */
        private int maxSeatingCapacity;
        public int MaxSeatingCapacity { get { return maxSeatingCapacity; } }

        private String currentHeroName;
        public String CurrentHeroName { get { return currentHeroName; } }

        /* If not known, this is set to 0 */
        private int currentHeroSeat;
        public int CurrentHeroSeat { get { return currentHeroSeat; } }

        /* Game type of the table */
        public PokerGame Game { get; set; }

        /* Reference to the TableDisplayWindow */
        private TableDisplayWindow displayWindow;
        public TableDisplayWindow DisplayWindow { get { return displayWindow; } }

        /* The playing window's title currently associated with this table */
        public String WindowTitle
        {
            get
            {
                return window.Title;
            }
        }

        /* The window rectangle associated with this table */
        public Rectangle WindowRect
        {
            get
            {
                return window.Rectangle;
            }
        }

        /* Whether the current table is minimized */
        public bool Minimized
        {
            get
            {
                return window.Minimized;
            }
        }

        // Window associated with this table
        private Window window; 

        /* The hand history filename associated with this table */ 
        private String handHistoryFilePath;
        public String HandHistoryFilePath { get { return handHistoryFilePath; } }

        /* Every table keeps a reference to the parser used to read data from them */
        private HHParser handHistoryParser;
        public HHParser HandHistoryParser { get { return handHistoryParser; } }

        private VisualRecognitionManager visualRecognitionManager;

        /* Reference to the Hud Window associated with this table. */
        public Hud Hud { get; set; }

        /* Statistics related to this table */
        private TableStatistics statistics;

        /* Accessor for retrieving the name of the poker client in use by 
         * this table */
        public String PokerClientName
        {
            get
            {
                return pokerClient.Name;
            }
        }

        public bool PlayerSeatingPositionIsRelative
        {
            get
            {
                return pokerClient.IsPlayerSeatingPositionRelative(this.GameType);
            }
        }

        /* Location of the visual maps is given by this format:
         * {exe_path}/Resources/ColorMaps/{poker_client_name}/{game}_{max_table_capacity}-max_{theme}.bmp 
         
         * If the location is not known at the time (or there are no maps), it returns empty string */
        public String VisualRecognitionMapLocation
        {
            get
            {
                if (Game != PokerGame.Unknown && maxSeatingCapacity != 0)
                {
                    return String.Format(@"{0}\Resources\ColorMaps\{1}\{2}_{3}-max_{4}.bmp", Application.StartupPath, pokerClient.Name, Game.ToString(), MaxSeatingCapacity, pokerClient.CurrentTheme);
                }
                else return String.Empty;
            }
        }

        public bool IsVisualRecognitionPossible()
        {
            return pokerClient.SupportsVisualRecognition && 
                VisualRecognitionMapLocation != "" &&
                System.IO.File.Exists(VisualRecognitionMapLocation);
        }

        public Table(String handHistoryFilePath, Window window, PokerClient pokerClient, PlayerDatabase playerDatabase)
        {
            this.handHistoryFilePath = handHistoryFilePath;
            this.window = window;
            this.pokerClient = pokerClient;
            this.playerDatabase = playerDatabase;
            this.Game = PokerGame.Unknown;
            this.maxSeatingCapacity = 0; // We don't know yet
            this.TableId = String.Empty; // We don't know yet
            this.GameID = String.Empty; // We don't know yet
            this.currentHeroName = String.Empty;
            this.currentHeroSeat = 0;
            this.gameType = pokerClient.GetPokerGameTypeFromWindowTitle(WindowTitle);
            this.statistics = new TableStatistics(this); // We don't know what specific kind
            this.Hud = new Hud(this);
            this.visualRecognitionManager = null; // Not all tables have a visual recognition manager
            this.displayWindow = PokerMuck.TableDisplayWindow.CreateForTable(this);

            // By default we use the universal parser
            handHistoryParser = new UniversalHHParser(pokerClient, System.IO.Path.GetFileName(handHistoryFilePath));

            // But as soon as we find what kind of game we're using, we're going to update our parser */
            ((UniversalHHParser)handHistoryParser).GameDiscovered += new UniversalHHParser.GameDiscoveredHandler(handHistoryParser_GameDiscovered);

            playerList = new List<Player>(10); //Usually no more than 10 players per table

            // Init hand history monitor
            hhMonitor = new HHMonitor(handHistoryFilePath, this);
            hhMonitor.StartMonitoring();
        }

        /* Certain stuff needs to be done AFTER the hud is done updating.
         * This is the right place */
        public void PostHudDisplayAction()
        {
            RemoveDeadPlayers();
        }

        /* Remove from the table all players who have the IsPlaying flag set to false */
        private void RemoveDeadPlayers()
        {
            playerList.RemoveAll(
                delegate(Player p)
                {
                    return !p.IsPlaying;
                }
            );
        }


        /* Asynchronous, forces the table to check for changes in the hand history */
        public void ParseHandHistoryNow()
        {
            // Do it in another thread so we don't stop other processing from being done
            Thread t = new Thread
                         (delegate()
                         {
                             hhMonitor.CheckForFileChanges();
                         });
            t.Start(); 
        }

        /* Hand history monitor handler, a new line has been read from a file */
        public void NewLineArrived(String filename, String line)
        {
            HandHistoryParser.ParseLine(line);
        }

        /* Hand history monitor handler, an end of file has been reached
         * this means that the UI can finally be updated. */
        public void EndOfFileReached(String filename)
        {
            // We cannot display the UI if a proper parser hasn't been found
            if (Game != PokerGame.Unknown)
            {
                UpdateUI();
            }
            else
            {
                Trace.WriteLine("Cannot update UI until a valid parser is found.");
            }
        }


        /* The user requested that all statistics get reset */
        public void window_OnResetAllStatisticsButtonPressed(HudWindow sender)
        {
            statistics.PrepareStatisticsForNewRound();

            foreach (Player p in PlayerList)
            {
                p.ResetAllStatistics();
                if (p.HudWindow != null) p.HudWindow.DisplayStatistics(p.GetStatistics()); // TODO: is this thread safe?
            }
        }

        public void ShowTableDisplayWindow()
        {
            Globals.Director.RunFromGUIThread((Action)delegate()
            {
                if (DisplayWindow != null) DisplayWindow.Show();
            }, false);
        }

        public void HideTableDisplayWindow()
        {
            Globals.Director.RunFromGUIThread((Action)delegate()
            {
                if (DisplayWindow != null) DisplayWindow.Hide();
            }, false);
        }

        /* In this method we take care of updating UI information
         * about this table */
        private void UpdateUI()
        {
            Trace.WriteLine("Refresh UI for " + GameID);

            Globals.Director.OnDisplayStatus("Displaying Table #" + TableId);

            // Flag to keep track of whether at least one mucked hand is available
            bool muckedHandsAvailable = false;

            // Check which players have shown their cards
            List<Player> showdownPlayers = new List<Player>();
            foreach (Player p in PlayerList)
            {
                // If it has showed and it's not us
                if (p.HasShowedThisRound && p.Name != CurrentHeroName)
                {
                    showdownPlayers.Add(p);
                    muckedHandsAvailable = true;
                }
            }

            if (muckedHandsAvailable)
            {
                Globals.Director.RunFromGUIThread(
                    (Action)delegate()
                    {
                        if (DisplayWindow != null) DisplayWindow.ClearMuck();
                    }, false
                    );

                foreach (Player p in showdownPlayers)
                {
                    Globals.Director.RunFromGUIThread(
                        (Action)delegate()
                        {
                            if (DisplayWindow != null) DisplayWindow.DisplayPlayerMuckedHand(p.Name, p.MuckedHand);
                        }, false
                        );
                }

                if (FinalBoard != null && !FinalBoard.Displayed){
                    Globals.Director.RunFromGUIThread(
                        (Action)delegate()
                        {
                            if (DisplayWindow != null) DisplayWindow.DisplayFinalBoard(FinalBoard);
                        }, false
                    );   
                    FinalBoard.Displayed = true;
                }
            }


            Globals.Director.RunFromGUIThread(
                (Action)delegate()
                {
                    // Display hud information
                    if (Hud != null) Hud.DisplayAndUpdate();

                    // Update statistics
                    if (DisplayWindow != null) DisplayWindow.UpdateStatistics();
                }, true
             );
        }

        void handHistoryParser_PlayerIsSeated(string playerName, int seatNumber)
        {
            Trace.WriteLine(String.Format("Player added: {0}", playerName));

            // Is this player already in the table's player's list?
            Player result = FindPlayer(playerName);

            // We found a new player. Yay!
            if (result == null) CreatePlayer(playerName);

            // Make sure he is still playing
            Player p = FindPlayer(playerName);
            p.IsPlaying = true;
            p.HasPlayedLastRound = true;

            // Also update his seat number
            p.SeatNumber = seatNumber;

            // And GameID
            p.GameID = GameID;
        }

        void handHistoryParser_RoundHasTerminated()
        {
            Trace.WriteLine("Round has terminated");

            /* 1. Perform last statistics calculations
             * 2. Clear the statistics information relative to a single round
             * 3. Any player that hasn't played last round should be flagged as non-playing (and the hud window, removed)
             * 4. Set every player's HasPlayedLastRound flag to false, as to identify who will get eliminated
             * in future rounds
             * 5. Clear hand information from display window */

            for (int i = 0; i<PlayerList.Count; i++)
            {
                Player p = PlayerList[i];
                p.CalculateEndOfRoundStatistics();
                p.PrepareStatisticsForNewRound();

                if (!p.HasPlayedLastRound)
                {
                    p.DisposeHud();
                    p.IsPlaying = false;
                }
                p.HasPlayedLastRound = false;
            }

            /* Clear the table statistics relative to a single round */
            statistics.PrepareStatisticsForNewRound();

            if (DisplayWindow != null) DisplayWindow.ClearHandInformation();
        }

        private void handHistoryParser_GameDiscovered(string game)
        {
            Trace.WriteLine(String.Format("Game discovered! {0}",game));

            // Find to what game this game string corresponds
            Game = pokerClient.GetPokerGameFromGameDescription(game);

            bool foundParser = false;

            // Holdem?
            if (foundParser = (Game == PokerGame.Holdem))
            {
                handHistoryParser = new HoldemHHParser(pokerClient, System.IO.Path.GetFileName(handHistoryFilePath));
                statistics = new HoldemTableStatistics(this);
            }
            else if (Game == PokerGame.Unknown)
            {
                Trace.WriteLine("We weren't able to find a better parser for this Game");
            }

            // If we replaced our parser, we need to register the event handlers
            if (foundParser)
            {
                // Generic handlers (all game types)
                handHistoryParser.PlayerIsSeated += new HHParser.PlayerIsSeatedHandler(handHistoryParser_PlayerIsSeated);
                handHistoryParser.RoundHasTerminated += new HHParser.RoundHasTerminatedHandler(handHistoryParser_RoundHasTerminated);
                handHistoryParser.NewTableHasBeenCreated += new HHParser.NewTableHasBeenCreatedHandler(handHistoryParser_NewTableHasBeenCreated);
                handHistoryParser.FoundTableMaxSeatingCapacity += new HHParser.FoundTableMaxSeatingCapacityHandler(handHistoryParser_FoundTableMaxSeatingCapacity);
                handHistoryParser.HeroNameFound += new HHParser.HeroNameFoundHandler(handHistoryParser_HeroNameFound);

                // Game specific handlers
                if (Game == PokerGame.Holdem)
                {
                    ((HoldemHHParser)handHistoryParser).FinalBoardAvailable += new HoldemHHParser.FinalBoardAvailableHandler(handHistoryParser_FinalBoardAvailable);
                    statistics.RegisterParserHandlers(handHistoryParser);
                }

                // Also, resend the last line to the new parser!
                hhMonitor.ResendLastLine();
            }

            if (Game != PokerGame.Unknown)
            {
                // Close temporary window
                if (displayWindow != null) displayWindow.Dispose();

                Globals.Director.RunFromGUIThread(
                    (Action)delegate()
                    {
                        if (displayWindow != null)
                        {
                            displayWindow = TableDisplayWindow.CreateForTable(this);
                            displayWindow.Show();
                        }
                    }, false
                 );              
            }
        }

        /* Holdem specific handlers */

        void handHistoryParser_FinalBoardAvailable(Board board)
        {
            finalBoard = board;
        }

        /* Generic handlers */

        void handHistoryParser_HeroNameFound(string heroName)
        {
            this.currentHeroName = heroName;

            // Attempt to find his seat
            bool foundSeat = false;
            foreach (Player p in PlayerList)
            {
                if (p.Name == heroName)
                {
                    currentHeroSeat = p.SeatNumber;
                    foundSeat = true;
                    break;
                }
            }

            if (!foundSeat) currentHeroSeat = 0;
        }

        void handHistoryParser_FoundTableMaxSeatingCapacity(int maxSeatingCapacity)
        {
            this.maxSeatingCapacity = maxSeatingCapacity;

            // Usually the max seating capacity is the last piece of information we need in order to create a visual recognition manager

            // Attempt to create the visual recognition manager
            if (IsVisualRecognitionPossible())
            {
                if (visualRecognitionManager == null)
                {
                    visualRecognitionManager = new VisualRecognitionManager(this, this);
                }

                // TODO REMOVE
                
                CardList cards = new CardList();
                //cards.AddCard(new Card(CardFace.Six, CardSuit.Spades));
                //cards.AddCard(new Card(CardFace.Eight, CardSuit.Spades));
                cards.AddCard(new Card(CardFace.Ace, CardSuit.Clubs));
                cards.AddCard(new Card(CardFace.Seven, CardSuit.Hearts));

                PlayerHandRecognized(cards);

                CardList board = new CardList();
                cards.AddCard(new Card(CardFace.Ace, CardSuit.Hearts));
                cards.AddCard(new Card(CardFace.Seven, CardSuit.Spades));
                cards.AddCard(new Card(CardFace.Six, CardSuit.Hearts));

                BoardRecognized(board);

                Globals.Director.RunFromGUIThread((Action)delegate()
                {
                    if (displayWindow != null) displayWindow.SetVisualRecognitionSupported(true);
                }, false);
            }
            else
            {
                Trace.WriteLine("Visual recognition is not supported for " + this.ToString());

                Globals.Director.RunFromGUIThread((Action)delegate()
                {
                    if (displayWindow != null) displayWindow.SetVisualRecognitionSupported(false);
                }, false);
            }
        }

        /* Visual Recognition Manager handlers */
        public void PlayerHandRecognized(CardList playerCards)
        {
            Globals.Director.RunFromGUIThread((Action)delegate()
            {
                if (DisplayWindow != null) DisplayWindow.DisplayPlayerHand(playerCards);
            }, true);
            
        }

        public void BoardRecognized(CardList board)
        {
            Globals.Director.RunFromGUIThread((Action)delegate()
            {
                if (Game == PokerGame.Holdem && DisplayWindow != null){
                    ((HoldemTableDisplayWindow)DisplayWindow).DisplayBoard(board);
                }                
            }, true);
        }

        void handHistoryParser_NewTableHasBeenCreated(string gameId, string tableId)
        {
            if (this.TableId != String.Empty && this.TableId != tableId)
            {

                Trace.WriteLine("An existing table has just changed its tableID... starting transition.");
                Trace.WriteLine("Previous ID: " + this.TableId);
                Trace.WriteLine("New ID: " + tableId);

                // Clear the list of players (new ones are coming)
                foreach (Player p in PlayerList)
                {
                    // If the player has a hud associated, also mark that hud as disposable
                    p.DisposeHud();
                }

                PlayerList.Clear();
            }

            this.GameID = gameId;
            this.TableId = tableId;
            //this.maxSeatingCapacity = 0;
        }



        /* Adds a player to the table. If a player has already been added with the same name, this method
         * ignores the request. If the player exists in our database, instead of creating a new player it loads the information
         * from the database */
        private void CreatePlayer(String playerName)
        {
            //Trace.Assert(GameID != String.Empty, "We are trying to create a player with no GameID");

            // Do we need to create a new one?
            if (!playerDatabase.Contains(playerName, GameID))
            {
                // Create a new player
                Player p = PlayerFactory.CreatePlayer(playerName, Game); 
                playerList.Add(p);

                // Also add it to our database
                playerDatabase.Store(p);
            }
            else
            {
                // No, we have the player in our database
                Player p = playerDatabase.Retrieve(playerName, GameID);
                playerList.Add(p);

                Trace.WriteLine("Retrieved " + playerName + " from our database!");
            }
        }

        /* Remove a player from the table */
        private void RemovePlayer(String playerName)
        {
            Trace.WriteLine("Removing " + playerName);
            playerList.RemoveAll(
                delegate(Player p)
                {
                    return p.Name == playerName;
                }
            );

        }
        
        /* Finds a player given its player name
         * It could return null */
        public Player FindPlayer(String playerName)
        {
            // Has this player already been added?
            Player result = playerList.Find(
                 delegate(Player p)
                 {
                     return p.Name == playerName;
                 }
            );

            return result;
        }

        /* We are done with this table */
        public void Terminate()
        {
            PlayerList.Clear();
            if (hhMonitor != null) hhMonitor.StopMonitoring();
            if (visualRecognitionManager != null) visualRecognitionManager.Cleanup();

            Globals.Director.RunFromGUIThread((Action)delegate()
            {
                if (DisplayWindow != null)
                {
                    displayWindow.Close();
                    displayWindow = null;
                }
            }, false);

            Globals.Director.RunFromGUIThread((Action)delegate()
            {            
                if (Hud != null)
                {
                    Hud.RemoveHud();
                    Hud = null;
                }
            }, false);
        }

        public override string ToString()
        {
            return String.Format("Table {0}: {1} max-seating: {2}, possible visual map location: {3}", this.TableId, this.Game.ToString(), this.MaxSeatingCapacity, this.VisualRecognitionMapLocation);
        }
    }
}
