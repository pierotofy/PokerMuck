using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace PokerMuck
{
    class Table : IHHMonitorHandler
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

        /* Game type of the table */
        public PokerGame Game { get; set; }

        /* The playing window's title currently associated with this table */
        public String WindowTitle { get; set; }

        /* The window rectangle associated with this table */
        public Rectangle WindowRect { get; set; }

        /* The hand history filename associated with this table */ 
        private String handHistoryFilePath;
        public String HandHistoryFilePath { get { return handHistoryFilePath; } }

        /* Every table keeps a reference to the parser used to read data from them */
        private HHParser handHistoryParser;
        public HHParser HandHistoryParser { get { return handHistoryParser; } }

        /* Notifies the delegate that data has changed */
        public delegate void RefreshUIHandler(Table sender);
        public event RefreshUIHandler RefreshUI;

        /* The statistics of a player in this table need to be displayed */
        public delegate void DisplayPlayerStatisticsHandler(Player p);
        public event DisplayPlayerStatisticsHandler DisplayPlayerStatistics;

        public void OnDisplayPlayerStatistics(Player p)
        {
            if (DisplayPlayerStatistics != null) DisplayPlayerStatistics(p);
        }

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

        /* Player's nickname at the table? */
        private String userID;
        public String UserID
        {
            get
            {
                return userID;
            }
        }

        public bool PlayerSeatingPositionIsRelative
        {
            get
            {
                return pokerClient.IsPlayerSeatingPositionRelative(this.GameType);
            }
        }

        public Table(String handHistoryFilePath, String windowTitle, Rectangle windowRect, PokerClient pokerClient, PlayerDatabase playerDatabase, String userID)
        {
            this.handHistoryFilePath = handHistoryFilePath;
            this.WindowTitle = windowTitle;
            this.pokerClient = pokerClient;
            this.userID = userID;
            this.maxSeatingCapacity = 0; // We don't know yet
            this.TableId = String.Empty; // We don't know yet
            this.GameID = String.Empty; // We don't know yet
            this.gameType = pokerClient.GetPokerGameTypeFromWindowTitle(windowTitle);
            this.statistics = new TableStatistics(this); // We don't know what specific kind
            this.playerDatabase = playerDatabase;
            this.WindowRect = windowRect;
            this.Hud = new Hud(this);

            // By default we use the universal parser
            handHistoryParser = new UniversalHHParser(pokerClient);

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
            if (RefreshUI != null) RefreshUI(this);
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


        void handHistoryParser_PlayerIsSeated(string playerName, int seatNumber)
        {
            Debug.Print("Player added: {0}", playerName);

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
            Debug.Print("Round has terminated");

            /* 1. Perform last statistics calculations
             * 2. Clear the statistics information relative to a single round
             * 3. Any player that hasn't played last round should be flagged as non-playing (and the hud window, removed)
             * 4. Set every player's HasPlayedLastRound flag to false, as to identify who will get eliminated
             * in future rounds */

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
        }

        private void handHistoryParser_GameDiscovered(string game)
        {
            Debug.Print("Game discovered! {0}",game);

            // Find to what game this game string corresponds
            Game = pokerClient.GetPokerGameFromGameDescription(game);

            bool foundParser = false;

            // Holdem?
            if (foundParser = (Game == PokerGame.Holdem))
            {
                handHistoryParser = new HoldemHHParser(pokerClient);
                statistics = new HoldemTableStatistics(this);
            }
            else if (Game == PokerGame.Unknown)
            {
                Debug.Print("We weren't able to find a better parser for this Game");
            }

            // If we replaced our parser, we need to register the event handlers
            if (foundParser)
            {
                // Generic handlers (all game types)
                handHistoryParser.PlayerIsSeated += new HHParser.PlayerIsSeatedHandler(handHistoryParser_PlayerIsSeated);
                handHistoryParser.RoundHasTerminated += new HHParser.RoundHasTerminatedHandler(handHistoryParser_RoundHasTerminated);
                handHistoryParser.NewTableHasBeenCreated += new HHParser.NewTableHasBeenCreatedHandler(handHistoryParser_NewTableHasBeenCreated);
                handHistoryParser.FoundTableMaxSeatingCapacity += new HHParser.FoundTableMaxSeatingCapacityHandler(handHistoryParser_FoundTableMaxSeatingCapacity);

                // Game specific handlers
                if (Game == PokerGame.Holdem)
                {
                    ((HoldemHHParser)handHistoryParser).FinalBoardAvailable += new HoldemHHParser.FinalBoardAvailableHandler(handHistoryParser_FinalBoardAvailable);
                    statistics.RegisterParserHandlers(handHistoryParser);
                }

                // Also, resend the last line to the new parser!
                hhMonitor.ResendLastLine();
            }
        }

        /* Holdem specific handlers */

        void handHistoryParser_FinalBoardAvailable(Board board)
        {
            finalBoard = board;
        }

        /* Generic handlers */

        void handHistoryParser_FoundTableMaxSeatingCapacity(int maxSeatingCapacity)
        {
            this.maxSeatingCapacity = maxSeatingCapacity;
        }

        void handHistoryParser_NewTableHasBeenCreated(string gameId, string tableId)
        {
            if (this.TableId != String.Empty && this.TableId != tableId)
            {

                Debug.Print("An existing table has just changed its tableID... starting transition.");
                Debug.Print("Previous ID: " + this.TableId);
                Debug.Print("New ID: " + tableId);

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
            //Debug.Assert(GameID != String.Empty, "We are trying to create a player with no GameID");

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

                Debug.Print("Retrieved " + playerName + " from our database!");
            }
        }

        /* Remove a player from the table */
        private void RemovePlayer(String playerName)
        {
            Debug.Print("Removing " + playerName);
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
        }
    }
}
