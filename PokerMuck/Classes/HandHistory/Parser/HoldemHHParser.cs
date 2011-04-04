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

        /* We found who the big/small blind is (and the amount of it) */
        public delegate void FoundSmallBlindHandler(String playerName, float amount);
        public event FoundSmallBlindHandler FoundSmallBlind;

        protected void OnFoundSmallBlind(String playerName, float amount)
        {
            if (FoundSmallBlind != null) FoundSmallBlind(playerName, amount);
        }

        public delegate void FoundBigBlindHandler(String playerName, float amount);
        public event FoundBigBlindHandler FoundBigBlind;

        protected void OnFoundBigBlind(String playerName, float amount)
        {
            if (FoundBigBlind != null) FoundBigBlind(playerName, amount);
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
        public delegate void PlayerRaisedHandler(String playerName, float raiseAmount, HoldemGamePhase gamePhase);
        public event PlayerRaisedHandler PlayerRaised;

        protected void OnPlayerRaised(String playerName, float raiseAmount, HoldemGamePhase gamePhase)
        {
            if (PlayerRaised != null) PlayerRaised(playerName, raiseAmount, gamePhase);
        }

        /* A player checked */
        public delegate void PlayerCheckedHandler(String playerName, HoldemGamePhase gamePhase);
        public event PlayerCheckedHandler PlayerChecked;

        protected void OnPlayerChecked(String playerName, HoldemGamePhase gamePhase)
        {
            if (PlayerChecked != null) PlayerChecked(playerName, gamePhase);
        }

        /* We found the button */
        public delegate void FoundButtonHandler(int seatNumber);
        public event FoundButtonHandler FoundButton;

        protected void OnFoundButton(int seatNumber)
        {
            if (FoundButton != null) FoundButton(seatNumber);
        }

        /* We found a winner(s) for the current hand */
        public delegate void FoundWinnerHandler(String playerName);
        public event FoundWinnerHandler FoundWinner;

        protected void OnFoundWinner(String playerName)
        {
            if (FoundWinner != null) FoundWinner(playerName);
        }

        /* A player pushed all-in */
        public delegate void PlayerPushedAllInHandler(String playerName, HoldemGamePhase gamePhase);
        public event PlayerPushedAllInHandler PlayerPushedAllIn;

        protected void OnPlayerPushedAllIn(String playerName, HoldemGamePhase gamePhase)
        {
            if (PlayerPushedAllIn != null) PlayerPushedAllIn(playerName, gamePhase);
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

            /* An all-in doesn't exclude a raise or a call, so it's not part of the if-elseif block
             * Ex. Player: raises and is all-in is both an all-in and a raise */
            if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_all_in_push"), out matchResult))
            {
                String playerName = matchResult.Groups["playerName"].Value;

                // Make sure that the game phase is a valid one (being all-in because the blinds force us to is not really a push)
                if (currentGamePhase == HoldemGamePhase.Preflop || currentGamePhase == HoldemGamePhase.Flop ||
                    currentGamePhase == HoldemGamePhase.Turn || currentGamePhase == HoldemGamePhase.River)
                OnPlayerPushedAllIn(playerName, currentGamePhase);
            }

            /* Compare line to extract game id or table id */
            if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_game_id_token"), out matchResult))
            {
                currentGameId = matchResult.Groups["gameId"].Value;
                OnNewGameHasStarted(currentGameId);
            }
            
            if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_table_token"), out matchResult))
            {
                currentTableId = matchResult.Groups["tableId"].Value;
                String maxSeatingCapacity = matchResult.Groups["tableSeatingCapacity"].Value;

                Debug.Print("Table: " + currentTableId);
                Debug.Print("Max seating capacity: " + maxSeatingCapacity);

                if (maxSeatingCapacity == String.Empty)
                {
                    // We didn't find the exact seating capacity, ask the client to guess it
                    maxSeatingCapacity = pokerClient.InferMaxSeatingCapacity(line).ToString();

                    Debug.Print("Inferred max seating capacity: " + maxSeatingCapacity);
                }


                if (currentGameId != String.Empty) OnNewTableHasBeenCreated(currentGameId, currentTableId, maxSeatingCapacity);
                else
                {
                    Debug.Print("Table ID {0} found but no game ID has been assigned to this parser yet. Ignoring event.");
                }
            }

            /* Detect raises, calls, folds, bets */
            if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_call"), out matchResult))
            {
                Debug.Assert(currentGamePhase != HoldemGamePhase.Showdown && currentGamePhase != HoldemGamePhase.Summary, "We detected a call during an impossible game phase");

                String playerName = matchResult.Groups["playerName"].Value;
                float amount = float.Parse(matchResult.Groups["amount"].Value);
                OnPlayerCalled(playerName, amount, currentGamePhase);
            }
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_bet"), out matchResult))
            {
                Debug.Assert(currentGamePhase != HoldemGamePhase.Showdown && currentGamePhase != HoldemGamePhase.Summary, "We detected a bet during an impossible game phase");

                String playerName = matchResult.Groups["playerName"].Value;
                float amount = float.Parse(matchResult.Groups["amount"].Value);
                OnPlayerBet(playerName, amount, currentGamePhase);
            }
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_fold"), out matchResult))
            {
                Debug.Assert(currentGamePhase != HoldemGamePhase.Showdown && currentGamePhase != HoldemGamePhase.Summary, "We detected a fold during an impossible game phase");

                String playerName = matchResult.Groups["playerName"].Value;

                Debug.Print(playerName + " folds");
                OnPlayerFolded(playerName, currentGamePhase);
            }
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_check"), out matchResult))
            {
                Debug.Assert(currentGamePhase != HoldemGamePhase.Showdown && currentGamePhase != HoldemGamePhase.Summary, "We detected a check during an impossible game phase");

                String playerName = matchResult.Groups["playerName"].Value;
                OnPlayerChecked(playerName, currentGamePhase);
            }

            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_raise"), out matchResult))
            {
                Debug.Assert(currentGamePhase != HoldemGamePhase.Showdown && currentGamePhase != HoldemGamePhase.Summary, "We detected a raise during an impossible game phase");

                String playerName = matchResult.Groups["playerName"].Value;
                float raiseAmount = float.Parse(matchResult.Groups["raiseAmount"].Value);
                
                Debug.Print(playerName + " raises " + raiseAmount);
                
                OnPlayerRaised(playerName, raiseAmount, currentGamePhase);
            }

            /* Search for table seating patterns */
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_in_game"), out matchResult))
            {
                // Retrieve player name
                String playerName = matchResult.Groups["playerName"].Value;
                int seatNumber = Int32.Parse(matchResult.Groups["seatNumber"].Value);

                // Raise event
                OnPlayerIsSeated(playerName, seatNumber);
            }

            /* Search for a winner */
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_hand_winner"), out matchResult))
            {
                // Retrieve player name
                String playerName = matchResult.Groups["playerName"].Value;

                // Raise event
                OnFoundWinner(playerName);
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
                
                OnPlayerMuckHandAvailable(playerName, hand);
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
                String playerName = matchResult.Groups["playerName"].Value;
                float amount = float.Parse(matchResult.Groups["smallBlindAmount"].Value);
                OnFoundSmallBlind(playerName, amount);          
            }
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_big_blind"), out matchResult))
            {
                String playerName = matchResult.Groups["playerName"].Value;
                float amount = float.Parse(matchResult.Groups["bigBlindAmount"].Value);
                OnFoundBigBlind(playerName, amount);
            }

            /* Find the button */
            if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_button"), out matchResult))
            {
                int seatNumber = Int32.Parse(matchResult.Groups["seatNumber"].Value);
                OnFoundButton(seatNumber);
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
