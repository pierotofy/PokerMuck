using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PokerMuck
{
    /* Hold'em Hand History Parser */
    class HoldemHHParser : HHParser
    {
        /* Enum of the possible game phases in a Hold'em game */
        public enum HoldemGamePhase { Preflop, Flop, Turn, River, Showdown, Summary };
        private HoldemGamePhase currentGamePhase;

        public HoldemHHParser(PokerClient pokerClient) : base(pokerClient)
        {
            
        }

        public override void ParseLine(string line)
        {
            // Call base class method FIRST
            base.ParseLine(line);

            // Declare match variable that will hold the results
            Match matchResult;

            /* Check game phase changes */
            bool gamePhaseChanged = ParseForGamePhaseChanges(line);
            if (gamePhaseChanged) return;

            /* Compare line to extract game id or table id */
            if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_game_id_token"), out matchResult))
            {
                currentGameId = matchResult.Groups["gameId"].Value;
                OnNewGameHasStarted(currentGameId);
            }
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_table_id_token"), out matchResult))
            {
                currentTableId = matchResult.Groups["tableId"].Value;
                if (currentGameId != String.Empty) OnNewTableHasBeenCreated(currentGameId, currentTableId);
                else
                {
                    Debug.Print("Table ID {0} found but no game ID has been assigned to this parser yet. Ignoring event.");
                }
            }

            /* Search for table seating patterns */
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_in_game"), out matchResult))
            {
                // Retrieve player name
                String playerName = matchResult.Groups["playerName"].Value;

                // Raise event
                OnPlayerIsSeated(playerName);
            }

            /* Search for mucked hands */
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_mucked_hand"), out matchResult))
            {
                // Retrieve player name and hand
                String playerName = matchResult.Groups["playerName"].Value;
                String cardsText = matchResult.Groups["cards"].Value;
                String[] cards = cardsText.Split(' ');

                // Now that we have the string representation of the cards, we need to convert the string into an Hand object
                Debug.Assert(cards.Length == 2, "Less or more than two cards were identified in this string: " + cardsText);

                Card first = pokerClient.GenerateCardFromString(cards[0]);
                Card second = pokerClient.GenerateCardFromString(cards[1]);
                Hand hand = new HoldemHand(first, second);
                
                // Raise event if we're at showdown
                if (currentGamePhase == HoldemGamePhase.Summary || currentGamePhase == HoldemGamePhase.Showdown || currentGamePhase == HoldemGamePhase.Preflop)
                {
                    OnPlayerMuckHandAvailable(playerName, hand);
                }
                else
                {
                    Debug.Print("Muck hand detected, but we're not at showdown, summary or preflop?");
                }
            }
        }

        /* Will modify the value of currentGamePhase 
           and it might raise a ShowdownWillBegin event */
        private bool ParseForGamePhaseChanges(String line)
        {
            bool foundMatch = false;

            /* Check changes in the game phase */
            if (foundMatch = LineMatchesRegex(line, pokerClient.GetRegex("hand_history_begin_preflop_phase_token")))
            {
                currentGamePhase = HoldemGamePhase.Preflop;
            }
            else if (foundMatch = LineMatchesRegex(line, pokerClient.GetRegex("hand_history_begin_flop_phase_token")))
            {
                currentGamePhase = HoldemGamePhase.Flop;
            }
            else if (foundMatch = LineMatchesRegex(line, pokerClient.GetRegex("hand_history_begin_turn_phase_token")))
            {
                currentGamePhase = HoldemGamePhase.Turn;
            }
            else if (foundMatch = LineMatchesRegex(line, pokerClient.GetRegex("hand_history_begin_river_phase_token")))
            {
                currentGamePhase = HoldemGamePhase.River;
            }
            else if (foundMatch = LineMatchesRegex(line, pokerClient.GetRegex("hand_history_begin_showdown_phase_token")))
            {
                currentGamePhase = HoldemGamePhase.Showdown;
                OnShowdownWillBegin();
            }
            else if (foundMatch = LineMatchesRegex(line, pokerClient.GetRegex("hand_history_begin_summary_phase_token")))
            {
                currentGamePhase = HoldemGamePhase.Summary;
            }

            return foundMatch; 
        }
    }
}
