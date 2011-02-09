using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PokerMuck
{
    abstract class HHParser
    {
        /* Events */

        /* One of the players has showed his hand */
        public delegate void PlayerMuckHandAvailableHandler(String playerName, Hand hand);
        public event PlayerMuckHandAvailableHandler PlayerMuckHandAvailable;

        protected void OnPlayerMuckHandAvailable(String playerName, Hand hand)
        {
            if (PlayerMuckHandAvailable != null) PlayerMuckHandAvailable(playerName, hand);
        }        

        /* A player is sitting at the table with us */
        public delegate void PlayerIsSeatedHandler(String playerName, HHParserEventArgs args);
        public event PlayerIsSeatedHandler PlayerIsSeated;

        protected void OnPlayerIsSeated(String playerName)
        {
            if (PlayerIsSeated != null) PlayerIsSeated(playerName, GenerateHHParserEventArgs());
        }
        
        /* A new game has begun */
        public delegate void NewGameHasStartedHandler(String gameId);
        public event NewGameHasStartedHandler NewGameHasStarted;

        protected void OnNewGameHasStarted(String gameId)
        {
            if (NewGameHasStarted != null) NewGameHasStarted(gameId);
        }

        /* A new table has been created for a game */
        public delegate void NewTableHasBeenCreatedHandler(String gameId, String tableId);
        public event NewTableHasBeenCreatedHandler NewTableHasBeenCreated;

        protected void OnNewTableHasBeenCreated(String gameId, String tableId)
        {
            if (NewTableHasBeenCreated != null) NewTableHasBeenCreated(gameId, tableId);
        }

        /* A round has just terminated */
        public delegate void RoundHasTerminatedHandler();
        public event RoundHasTerminatedHandler RoundHasTerminated;

        protected void OnRoundHasTerminated()
        {
            if (RoundHasTerminated != null) RoundHasTerminated();
        }

        /* Reference to the poker client in use */
        protected PokerClient pokerClient;

        /* What gameID are we currently parsing? */
        protected String currentGameId;

        /* What tableID are we currently parsing? */
        protected String currentTableId;

        /* This will force subclasses to have a PokerClient in their constructor as well */
        public HHParser(PokerClient pokerClient)
        {
            this.pokerClient = pokerClient;
        }

        public abstract void ParseLine(String line);

        protected bool LineMatchesRegex(String line, Regex regex, out Match result)
        {
            result = regex.Match(line);
            return result.Success;
        }

        protected bool LineMatchesRegex(String line, Regex regex)
        {
            Match result;
            return LineMatchesRegex(line, regex, out result);
        }

        /* Returns the current event args (GameID, TableID, etc.) structure */
        protected HHParserEventArgs GenerateHHParserEventArgs()
        {
            return new HHParserEventArgs(currentGameId, currentTableId);
        }
    }
}