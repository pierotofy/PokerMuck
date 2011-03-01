using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Drawing;

namespace PokerMuck
{
    class Table
    {
        /* What client is table using? */
        private PokerClient pokerClient;

        /* Holds a list of the players currently seated at the table
         * Note that they're going to be added in order of seating
         * (Seat #1 Player will be stored as the first element) */
        private List<Player> playerList;
        public List<Player> PlayerList { get { return playerList; } }

        /* The last final board available */
        private Board finalBoard;
        public Board FinalBoard { get { return finalBoard; } }

        /* Game ID associated with this table */
        public String GameID { get; set; }

        /* Identification string of the table */
        public String TableId { get; set; }

        /* Max seating capacity */
        private int maxSeatingCapacity;
        public int MaxSeatingCapacity { get { return maxSeatingCapacity; } }

        /* Game type of the table */
        public PokerGameType GameType { get; set; }

        /* The playing window's title currently associated with this table */
        public String WindowTitle { get; set; }

        /* The window rectangle associated with this table */
        public Rectangle WindowRect { get; set; }

        /* The hand history filename associated with this table */ 
        private String handHistoryFilename;
        public String HandHistoryFilename { get { return handHistoryFilename; } }

        /* Every table keeps a reference to the parser used to read data from them */
        private HHParser handHistoryParser;
        public HHParser HandHistoryParser { get { return handHistoryParser; } }

        /* Notifies the delegate that data has changed */
        public delegate void DataHasChangedHandler(Table sender);
        public event DataHasChangedHandler DataHasChanged;

        /* Information about the blinds */
        public float BigBlindAmount { get; set; }
        public float SmallBlindAmount { get; set; }

        /* Reference to the Hud Window associated with this table. */
        public Hud Hud { get; set; }

        /* Accessor for retrieving the name of the poker client in use by 
         * this table */
        public String PokerClientName
        {
            get
            {
                return pokerClient.Name;
            }
        }
        

        public Table(String handHistoryFilename, String windowTitle, PokerClient pokerClient)
        {
            this.handHistoryFilename = handHistoryFilename;
            this.WindowTitle = windowTitle;
            this.pokerClient = pokerClient;
            this.maxSeatingCapacity = 0; // We don't know yet
            this.TableId = String.Empty; // We don't know yet
            this.GameType = PokerGameType.Unknown; // We don't know
            this.WindowRect = new Rectangle();
            this.Hud = new Hud(this);

            // By default we use the universal parser
            handHistoryParser = new UniversalHHParser(pokerClient);

            // But as soon as we find what kind of game we're using, we're going to update our parser */
            ((UniversalHHParser)handHistoryParser).GameTypeDiscovered += new UniversalHHParser.GameTypeDiscoveredHandler(handHistoryParser_GameTypeDiscovered);

            playerList = new List<Player>(10); //Usually no more than 10 players per table
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
            for (int i = 0; i < PlayerList.Count; i++)
            {
                Player p = PlayerList[i];
                if (!p.IsPlaying) RemovePlayer(p.Name);
            }
        }

        /* Find a player with playerName name and set its mucked hand */
        void handHistoryParser_PlayerMuckHandAvailable(string playerName, Hand hand)
        {
            Player player = FindPlayer(playerName);
            Debug.Assert(player != null, "Player " + playerName + " mucked hand became available, but this player is not in our list");

            player.MuckedHand = hand;
            player.HasShowedLastRound = true;
        }

        void handHistoryParser_PlayerIsSeated(string playerName, int seatNumber)
        {
            Debug.Print("Player added: {0}", playerName);
            AddPlayer(playerName);

            // Make sure he is still playing
            Player p = FindPlayer(playerName);
            p.HasPlayedLastRound = true;

            // Also update his seat number
            p.SeatNumber = seatNumber;
        }

        void handHistoryParser_RoundHasTerminated()
        {
            if (DataHasChanged != null) DataHasChanged(this);


            /* 1. Clear the statistics information relative to a single round
             * 2. Any player that hasn't played last round should be flagged as non-playing
             * 3. Set every other player's HasPlayedLastRound flag to false, as to identify who will get eliminated
             * in future rounds */

            for (int i = 0; i<PlayerList.Count; i++)
            {
                Player p = PlayerList[i];

                p.PrepareStatisticsForNewRound();

                if (!p.HasPlayedLastRound)
                {
                    p.IsPlaying = false;
                }
                else
                {
                    p.HasPlayedLastRound = false;
                }
            }

        }

        void handHistoryParser_HoleCardsWillBeDealt()
        {
            // Mark every player as not having shown their hands during the last showdown
            foreach (Player p in playerList)
            {
                p.HasShowedLastRound = false;
                p.IsDealtHoleCards();
            }
        }

        private void handHistoryParser_GameTypeDiscovered(string gameType)
        {
            Debug.Print("GameType discovered! {0}",gameType);

            // Find to what game this gametype string corresponds
            GameType = pokerClient.GetPokerGameTypeFromGameDescription(gameType);

            bool foundParser = false;

            // Holdem?
            if (foundParser = (GameType == PokerGameType.Holdem))
            {
                handHistoryParser = new HoldemHHParser(pokerClient);
            }
            else if (GameType == PokerGameType.Unknown)
            {
                Debug.Print("We weren't able to find a better parser for this GameType");
            }

            // If we replaced our parser, we need to register the event handlers
            if (foundParser)
            {
                // Generic handlers (all game types)
                handHistoryParser.PlayerIsSeated += new HHParser.PlayerIsSeatedHandler(handHistoryParser_PlayerIsSeated);
                handHistoryParser.PlayerMuckHandAvailable += new HHParser.PlayerMuckHandAvailableHandler(handHistoryParser_PlayerMuckHandAvailable);
                handHistoryParser.RoundHasTerminated += new HHParser.RoundHasTerminatedHandler(handHistoryParser_RoundHasTerminated);
                handHistoryParser.NewTableHasBeenCreated += new HHParser.NewTableHasBeenCreatedHandler(handHistoryParser_NewTableHasBeenCreated);
                handHistoryParser.HoleCardsWillBeDealt += new HHParser.HoleCardsWillBeDealtHandler(handHistoryParser_HoleCardsWillBeDealt);

                // Game specific handlers
                if (GameType == PokerGameType.Holdem)
                {
                    ((HoldemHHParser)handHistoryParser).FinalBoardAvailable += new HoldemHHParser.FinalBoardAvailableHandler(handHistoryParser_FinalBoardAvailable);
                    ((HoldemHHParser)handHistoryParser).FoundBigBlind += new HoldemHHParser.FoundBigBlindHandler(handHistoryParser_FoundBigBlind);
                    ((HoldemHHParser)handHistoryParser).FoundSmallBlind += new HoldemHHParser.FoundSmallBlindHandler(handHistoryParser_FoundSmallBlind);
                    ((HoldemHHParser)handHistoryParser).PlayerBet += new HoldemHHParser.PlayerBetHandler(handHistoryParser_PlayerBet);
                    ((HoldemHHParser)handHistoryParser).PlayerCalled += new HoldemHHParser.PlayerCalledHandler(handHistoryParser_PlayerCalled);
                    ((HoldemHHParser)handHistoryParser).PlayerFolded += new HoldemHHParser.PlayerFoldedHandler(handHistoryParser_PlayerFolded);
                    ((HoldemHHParser)handHistoryParser).PlayerRaised += new HoldemHHParser.PlayerRaisedHandler(handHistoryParser_PlayerRaised);
                }
            }
        }

        /* Holdem specific handlers */

        void handHistoryParser_PlayerRaised(string playerName, float initialPot, float raiseAmount, HoldemGamePhase gamePhase)
        {
            HoldemPlayer p = (HoldemPlayer)FindPlayer(playerName);
            p.HasRaised(gamePhase);
        }

        void handHistoryParser_PlayerFolded(string playerName, HoldemGamePhase gamePhase)
        {
            HoldemPlayer p = (HoldemPlayer)FindPlayer(playerName);
            p.HasFolded(gamePhase);
        }

        void handHistoryParser_PlayerCalled(string playerName, float amount, HoldemGamePhase gamePhase)
        {
            HoldemPlayer p = (HoldemPlayer)FindPlayer(playerName);

            // If we are preflop and the call is the same amount as the big blind, this is also a limp
            if (amount == BigBlindAmount && gamePhase == HoldemGamePhase.Preflop)
            {
                // HasLimped() makes further checks to avoid duplicate counts and whether the player is the big blind or small blind
                p.HasLimped();
            }

            p.HasCalled(gamePhase);
        }

        void handHistoryParser_PlayerBet(string playerName, float amount, HoldemGamePhase gamePhase)
        {
            HoldemPlayer p = (HoldemPlayer)FindPlayer(playerName);
            p.HasBet(gamePhase);
        }

        void handHistoryParser_FinalBoardAvailable(Board board)
        {
            finalBoard = board;
        }

        void handHistoryParser_FoundBigBlind(String playerName, float amount)
        {
            // Save current blind
            BigBlindAmount = amount;

            // Keep track of who is the big blind
            HoldemPlayer p = (HoldemPlayer)FindPlayer(playerName);
            p.IsBigBlind = true;
        }

        void handHistoryParser_FoundSmallBlind(String playerName, float amount)
        {
            // Save current blind
            SmallBlindAmount = amount;

            // Keep track of who is the small blind
            HoldemPlayer p = (HoldemPlayer)FindPlayer(playerName);
            p.IsSmallBlind = true;
        }


        /* Generic handlers */

        void handHistoryParser_NewTableHasBeenCreated(string gameId, string tableId, String maxSeatingCapacity)
        {
            if (this.TableId != String.Empty && this.TableId != tableId)
            {

                Debug.Print("An existing table has just changed its tableID... just fwi");
                Debug.Print("Previous ID: " + this.TableId);
                Debug.Print("New ID: " + tableId);

            }

            this.GameID = gameId;
            this.TableId = tableId;
            this.maxSeatingCapacity = Int32.Parse(maxSeatingCapacity);
        }



        /* Adds a player to the table. If a player has already been added with the same name, this method
         * ignores the request */
        private void AddPlayer(String playerName)
        {
            Player result = FindPlayer(playerName);

            // We found a new player. Yay!
            if (result == null)
            {
                playerList.Add(PlayerFactory.CreatePlayer(playerName, GameType));
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
        private Player FindPlayer(String playerName)
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
        }
    }
}
