using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PokerMuck
{
    /* Enum of the possible game phases in a Hold'em game */
    public enum HoldemGamePhase { Preflop, Flop, Turn, River, Showdown, Summary };
        

    /* Hold'em Hand History Parser */
    class HoldemHHParser : HHParser
    {
        private HoldemGamePhase currentGamePhase;

        /* The final board is available */
        public delegate void FinalBoardAvailableHandler(Board board);
        public event FinalBoardAvailableHandler FinalBoardAvailable;

        protected void OnFinalBoardAvailable(Board board)
        {
            if (FinalBoardAvailable != null) FinalBoardAvailable(board);
        }

        /* We found who the big/small blind is */
        public delegate void FoundSmallBlindHandler(String playerName);
        public event FoundSmallBlindHandler FoundSmallBlind;

        protected void OnFoundSmallBlind(String playerName)
        {
            if (FoundSmallBlind != null) FoundSmallBlind(playerName);
        }

        public delegate void FoundBigBlindHandler(String playerName);
        public event FoundBigBlindHandler FoundBigBlind;

        protected void OnFoundBigBlind(String playerName)
        {
            if (FoundBigBlind != null) FoundBigBlind(playerName);
        } 

        /* A player bet */
        public delegate void PlayerBetHandler(String playerName, float amount, HoldemGamePhase gamePhase);
        public event PlayerBetHandler PlayerBet;

        protected void OnPlayerBet(String playerName, float amount, HoldemGamePhase gamePhase)
        {
            if (PlayerBet != null) PlayerBet(playerName, amount, gamePhase);
        }

        /* A player called */
        public delegate void PlayerCalledHandler(String playerName, float amount, HoldemGamePhase gamePhase);
        public event PlayerCalledHandler PlayerCalled;

        protected void OnPlayerCalled(String playerName, float amount, HoldemGamePhase gamePhase)
        {
            if (PlayerCalled != null) PlayerCalled(playerName, amount, gamePhase);
        }

        /* A player folded */
        public delegate void PlayerFoldedHandler(String playerName, HoldemGamePhase gamePhase);
        public event PlayerFoldedHandler PlayerFolded;

        protected void OnPlayerFolded(String playerName, HoldemGamePhase gamePhase)
        {
            if (PlayerFolded != null) PlayerFolded(playerName, gamePhase);
        }

        /* A player raised */
        public delegate void PlayerRaisedHandler(String playerName, float initialPot, float raiseAmount, HoldemGamePhase gamePhase);
        public event PlayerRaisedHandler PlayerRaised;

        protected void OnPlayerRaised(String playerName, float initialPot, float raiseAmount, HoldemGamePhase gamePhase)
        {
            if (PlayerRaised != null) PlayerRaised(playerName, initialPot, raiseAmount, gamePhase);
        }

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

            /* Search for final board */
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_final_board"), out matchResult))
            {
                // Retrieve cards
                String cardsText = matchResult.Groups["cards"].Value;
                String[] cards = cardsText.Split(' ');

                // We check whether this board is a final board by checking how many cards we've detected
                // We're not interested into boards with less than 5 cards
                if (cards.Length == 5)
                {
                    Card first = pokerClient.GenerateCardFromString(cards[0]);
                    Card second = pokerClient.GenerateCardFromString(cards[1]);
                    Card third = pokerClient.GenerateCardFromString(cards[2]);
                    Card fourth = pokerClient.GenerateCardFromString(cards[3]);
                    Card fifth = pokerClient.GenerateCardFromString(cards[4]);

                    Board board = new HoldemBoard(first, second, third, fourth, fifth);

                    // Raise event if we're at summary
                    if (currentGamePhase == HoldemGamePhase.Summary)
                    {
                        OnFinalBoardAvailable(board);
                    }
                    else
                    {
                        Debug.Print("Board detected, but we're not at the summary?");
                    }
                }
                else
                {
                    Debug.Print("Board detected, but only " + cards.Length + " cards in there. Skipping...");
                }
            }

            /* Detect small/big blind */
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_small_blind"), out matchResult))
            {
                OnFoundSmallBlind(matchResult.Groups["playerName"].Value);          
            }
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_big_blind"), out matchResult))
            {
                OnFoundBigBlind(matchResult.Groups["playerName"].Value);
            }

            /* Detect raises, calls, folds, bets */
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_call"), out matchResult))
            {
                String playerName = matchResult.Groups["playerName"].Value;
                float amount = float.Parse(matchResult.Groups["amount"].Value);
                OnPlayerCalled(playerName,amount, currentGamePhase);
            }
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_bet"), out matchResult))
            {
                String playerName = matchResult.Groups["playerName"].Value;
                float amount = float.Parse(matchResult.Groups["amount"].Value);
                OnPlayerBet(playerName, amount, currentGamePhase);
            }
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_fold"), out matchResult))
            {
                String playerName = matchResult.Groups["playerName"].Value;
                OnPlayerFolded(playerName, currentGamePhase);
            }
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_raise"), out matchResult))
            {
                String playerName = matchResult.Groups["playerName"].Value;
                float initialPot = float.Parse(matchResult.Groups["initialPot"].Value);
                float raiseAmount = float.Parse(matchResult.Groups["raiseAmount"].Value);
                OnPlayerRaised(playerName, initialPot, raiseAmount, currentGamePhase);
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
                OnHoleCardsWillBeDealt();
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
