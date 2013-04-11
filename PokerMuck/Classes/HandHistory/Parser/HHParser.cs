using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace PokerMuck
{
    public abstract class HHParser
    {
        /* Events */

        /* One of the players has showed his hand */
        public delegate void PlayerMuckHandAvailableHandler(String playerName, Hand hand);
        public event PlayerMuckHandAvailableHandler PlayerMuckHandAvailable;

        protected void OnPlayerMuckHandAvailable(String playerName, Hand hand)
        {
            if (PlayerMuckHandAvailable != null) PlayerMuckHandAvailable(playerName, hand);
        }

        /* We found hero's nickname */
        public delegate void HeroNameFoundHandler(String heroName);
        public event HeroNameFoundHandler HeroNameFound;

        protected void OnHeroNameFound(String heroName)
        {
            if (HeroNameFound != null) HeroNameFound(heroName);
        }

        /* A player is sitting at the table with us */
        public delegate void PlayerIsSeatedHandler(String playerName, int seatNumber);
        public event PlayerIsSeatedHandler PlayerIsSeated;

        protected void OnPlayerIsSeated(String playerName, int seatNumber)
        {
            if (PlayerIsSeated != null) PlayerIsSeated(playerName, seatNumber);
        }

        /* Cards are about to be dealt */
        public delegate void HoleCardsWillBeDealtHandler();
        public event HoleCardsWillBeDealtHandler HoleCardsWillBeDealt;

        protected void OnHoleCardsWillBeDealt()
        {
            if (HoleCardsWillBeDealt != null) HoleCardsWillBeDealt();
        }

        /* A showdown is about to happen */
        public delegate void ShowdownWillBeginHandler();
        public event ShowdownWillBeginHandler ShowdownWillBegin;

        protected void OnShowdownWillBegin()
        {
            if (ShowdownWillBegin != null) ShowdownWillBegin();
        }

        /* A new table has been created for a game */
        public delegate void NewTableHasBeenCreatedHandler(String gameId, String tableId);
        public event NewTableHasBeenCreatedHandler NewTableHasBeenCreated;

        protected void OnNewTableHasBeenCreated(String gameId, String tableId)
        {
            if (NewTableHasBeenCreated != null) NewTableHasBeenCreated(gameId, tableId);
        }

        /* We found the max seating capacity for a table */
        public delegate void FoundTableMaxSeatingCapacityHandler(int maxSeatingCapacity);
        public event FoundTableMaxSeatingCapacityHandler FoundTableMaxSeatingCapacity;

        protected void OnFoundTableMaxSeatingCapacity(int maxSeatingCapacity)
        {
            if (FoundTableMaxSeatingCapacity != null) FoundTableMaxSeatingCapacity(maxSeatingCapacity);
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

        /* What is the filename of the current hand history? */
        protected String handhistoryFilename;

        /* Counter for end of round tokens.
         * Every time this variable reaches a certain value (specified in the pokerclient class)
         * OnRoundHasTerminated() is raised.
         * This counter increases everytime a certain end of round regex matches */
        protected int endOfRoundTokensDetected;

        /* This will force subclasses to have a PokerClient in their constructor as well */
        public HHParser(PokerClient pokerClient, String handhistoryFilename)
        {
            this.pokerClient = pokerClient;
            this.handhistoryFilename = handhistoryFilename;

            endOfRoundTokensDetected = 0;
        }

        /* Every type of game has an end to a round. This is why we detect it here. 
           A subclass can still disable this mechanism by overriding CheckForEndOfRound() */
        public virtual void ParseLine(String line)
        {
            Trace.Assert(pokerClient != null, "pokerClient has not been initialized in the constructor before ParseLine has been called!");

            CheckForEndOfRound(line);
        }
        
        /* Every hand history file ends with a sequence of tokens (1 or more)
         * If we detect the end of a game we raise the appropriate event */
        protected virtual void CheckForEndOfRound(String line)
        {
            Match matchResult;

            if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_end_of_round")))
            {
                // Increment
                endOfRoundTokensDetected++;

                // Have we reached the limit?
                int numberRequired = pokerClient.GetConfigInt("hand_history_end_of_round_number_of_tokens_required");
                if (endOfRoundTokensDetected >= numberRequired)
                {
                    Trace.WriteLine("End of round");
                    OnRoundHasTerminated();
                    endOfRoundTokensDetected = 0;
                }
            }

            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_hero_name"), out matchResult))
            {
                String heroName = matchResult.Groups["heroName"].Value;
                OnHeroNameFound(heroName);
                Trace.WriteLine("Found hero's name: " + heroName);
            }
        }

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

    }
}