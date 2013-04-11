using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections;

namespace PokerMuck
{
    /* Enum of the possible game phases in a Hold'em game */
    public enum HoldemGamePhase { Preflop, Flop, Turn, River, Showdown, Summary };

    /* Enum of the possible player actions in a Hold'em game */
    public enum HoldemPlayerAction { Call, Bet, Raise, Fold, Check, CheckRaise, CheckCall, CheckFold, None };
    

    /* Hold'em Hand History Parser */
    class HoldemHHParser : HHParser
    {
        private HoldemGamePhase currentGamePhase;

        /* Certain poker clients do not tell whether showdown ever occured. We keep track of when players start
         * showing their hands and a flag that avoids double event raising to guess when this happens. Should work for most cases.
         * It might guess wrong when on the river a player raises, the other folds, and the raisor shows voluntarely his hand. */
        private bool PlayerHasShowedThisRound;
        private bool ShowdownEventRaised;

        /* Some poker clients tell you what the final board is explicitally, others don't, so you have to reconstruct it
         * from flop, turn and river cards */
        private List<Card> boardCards;

        /* Some poker clients do not tell explicitly who the blinds are. So we need to keep track of the player
         * seats and the button. Then from the button we infere who the big/small blind is. */
        private Hashtable playerSeats; // Key = seat number, Value = player name
        private int lastButtonSeatNumber;

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

        /* We found how much the blinds are */
        public delegate void FoundSmallBlindAmountHandler(float amount);
        public event FoundSmallBlindAmountHandler FoundSmallBlindAmount;

        protected void OnFoundSmallBlindAmount(float amount)
        {
            if (FoundSmallBlindAmount != null) FoundSmallBlindAmount(amount);
        }

        public delegate void FoundBigBlindAmountHandler(float amount);
        public event FoundBigBlindAmountHandler FoundBigBlindAmount;

        protected void OnFoundBigBlindAmount(float amount)
        {
            if (FoundBigBlindAmount != null) FoundBigBlindAmount(amount);
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

        public HoldemHHParser(PokerClient pokerClient, String handhistoryFilename)
            : base(pokerClient, handhistoryFilename)
        {
            // Initialize vars
            PlayerHasShowedThisRound = false;
            ShowdownEventRaised = false;
            boardCards = new List<Card>(5);

            playerSeats = new Hashtable(); 
            lastButtonSeatNumber = 0;

            // Setup handler from parent class that detects the end of a round
            base.RoundHasTerminated += new RoundHasTerminatedHandler(HoldemHHParser_RoundHasTerminated);

            // Also call it the first time from here
            SetupNewRound();
        }

        // At the end of the round, we setup variables for a new round
        void HoldemHHParser_RoundHasTerminated()
        {
            SetupNewRound();
        }

        private void SetupNewRound()
        {
            PlayerHasShowedThisRound = false;
            ShowdownEventRaised = false;
            boardCards.Clear();
            playerSeats.Clear();
            currentGamePhase = HoldemGamePhase.Preflop;
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
            }
            
            if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_table_token"), out matchResult))
            {
                currentTableId = matchResult.Groups["tableId"].Value;
                String maxSeatingCapacity = matchResult.Groups["tableSeatingCapacity"].Value;

                Trace.WriteLine("Table: " + currentTableId);
                Trace.WriteLine("Max seating capacity: " + maxSeatingCapacity);

                // Note: often currentGameId will be null because we don't have that information
                OnNewTableHasBeenCreated(currentGameId, currentTableId);

                // Abemus max seating capacity?
                if (maxSeatingCapacity != String.Empty)
                {
                    OnFoundTableMaxSeatingCapacity(Int32.Parse(maxSeatingCapacity));
                }
                else
                {
                    // We didn't find the exact seating capacity

                    // If we have a more specific regex, we'll wait until we get the line containing the maximum number of seats
                    // Otherwise, we need to infer
                    if (!pokerClient.HasRegex("hand_history_max_seating_capacity"))
                    {
                        int inferredMaxCapacity = pokerClient.InferMaxSeatingCapacity(line, handhistoryFilename);
                        Trace.WriteLine("Inferred max seating capacity: " + inferredMaxCapacity);
                        OnFoundTableMaxSeatingCapacity(inferredMaxCapacity);
                    }
                    else
                    {
                        Trace.WriteLine("Seating capacity not found, but we will find it later with a specialized regex");
                    }
                }
            }

            /* Detect the exact max seating capacity */
            if (pokerClient.HasRegex("hand_history_max_seating_capacity") && (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_max_seating_capacity"), out matchResult)))
            {
                String maxSeatingCapacity = matchResult.Groups["tableSeatingCapacity"].Value;
                
                Trace.Assert(maxSeatingCapacity != String.Empty, "Table max seating capacity regex found, but empty result.");
                Trace.WriteLine("Found certain max seating capacity from regex: " + maxSeatingCapacity);

                OnFoundTableMaxSeatingCapacity(Int32.Parse(maxSeatingCapacity));
            }

            /* Detect the exact blind amounts */
            if (pokerClient.HasRegex("hand_history_blind_amounts") && (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_blind_amounts"), out matchResult)))
            {
                float bigBlindAmount = float.Parse(matchResult.Groups["bigBlindAmount"].Value);
                float smallBlindAmount = float.Parse(matchResult.Groups["smallBlindAmount"].Value);

                Trace.WriteLine(String.Format("Found certain blind amounts: ({0}/{1})", smallBlindAmount, bigBlindAmount));

                OnFoundBigBlindAmount(bigBlindAmount);
                OnFoundSmallBlindAmount(smallBlindAmount);

                InfereBlindsFromButton(lastButtonSeatNumber);
            }

            /* Detect raises, calls, folds, bets */
            if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_call"), out matchResult))
            {
                Trace.Assert(currentGamePhase != HoldemGamePhase.Showdown && currentGamePhase != HoldemGamePhase.Summary, "We detected a call during an impossible game phase");

                String playerName = matchResult.Groups["playerName"].Value;
                float amount = float.Parse(matchResult.Groups["amount"].Value);
                OnPlayerCalled(playerName, amount, currentGamePhase);
            }
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_bet"), out matchResult))
            {
                Trace.Assert(currentGamePhase != HoldemGamePhase.Showdown && currentGamePhase != HoldemGamePhase.Summary, "We detected a bet during an impossible game phase");

                String playerName = matchResult.Groups["playerName"].Value;
                float amount = float.Parse(matchResult.Groups["amount"].Value);
                OnPlayerBet(playerName, amount, currentGamePhase);
            }
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_fold"), out matchResult))
            {
                Trace.Assert(currentGamePhase != HoldemGamePhase.Showdown && currentGamePhase != HoldemGamePhase.Summary, "We detected a fold during an impossible game phase");

                String playerName = matchResult.Groups["playerName"].Value;

                Trace.WriteLine(playerName + " folds");
                OnPlayerFolded(playerName, currentGamePhase);
            }
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_check"), out matchResult))
            {
                Trace.Assert(currentGamePhase != HoldemGamePhase.Showdown && currentGamePhase != HoldemGamePhase.Summary, "We detected a check during an impossible game phase");

                String playerName = matchResult.Groups["playerName"].Value;
                OnPlayerChecked(playerName, currentGamePhase);
            }

            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_raise"), out matchResult))
            {
                Trace.Assert(currentGamePhase != HoldemGamePhase.Showdown && currentGamePhase != HoldemGamePhase.Summary, "We detected a raise during an impossible game phase");

                String playerName = matchResult.Groups["playerName"].Value;
                float raiseAmount = float.Parse(matchResult.Groups["raiseAmount"].Value);
                
                Trace.WriteLine(playerName + " raises " + raiseAmount);
                
                OnPlayerRaised(playerName, raiseAmount, currentGamePhase);
            }

            /* Search for table seating patterns */
            else if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_player_in_game"), out matchResult))
            {
                // Retrieve player name
                String playerName = matchResult.Groups["playerName"].Value;
                int seatNumber = Int32.Parse(matchResult.Groups["seatNumber"].Value);

                // Some poker clients rewrite the list of players at the summary, but we don't need this extra information
                if (currentGamePhase != HoldemGamePhase.Summary)
                {
                    // Raise event
                    OnPlayerIsSeated(playerName, seatNumber);

                    // Save
                    playerSeats.Add(seatNumber, playerName);
                }
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
                // Somebody had to show... keep track of this
                PlayerHasShowedThisRound = true;

                // Retrieve player name and hand
                String playerName = matchResult.Groups["playerName"].Value;
                String cardsText = matchResult.Groups["cards"].Value;

                List<Card> cards = GenerateCardsFromText(cardsText);

                // Sometimes a regex will return only one card (when a player decides to show only one card)
                if (cards.Count == 2){
                    Hand hand = new HoldemHand(cards[0], cards[1]);
                
                    OnPlayerMuckHandAvailable(playerName, hand);
                }
            }

            /* Search for final board */
            else if (pokerClient.HasRegex("hand_history_detect_final_board") && (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_final_board"), out matchResult)))
            {
                // Retrieve cards
                String cardsText = matchResult.Groups["cards"].Value;
                List<Card> cards = GenerateCardsFromText(cardsText);

                HandleFinalBoard(cards);
            }

            /* Detect small/big blind */
            else if (pokerClient.HasRegex("hand_history_detect_small_blind") && (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_small_blind"), out matchResult)))
            {
                String playerName = matchResult.Groups["playerName"].Value;
                float amount = float.Parse(matchResult.Groups["smallBlindAmount"].Value);
                OnFoundSmallBlind(playerName);
                OnFoundSmallBlindAmount(amount);
            }
            else if (pokerClient.HasRegex("hand_history_detect_big_blind") && (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_big_blind"), out matchResult)))
            {
                String playerName = matchResult.Groups["playerName"].Value;
                float amount = float.Parse(matchResult.Groups["bigBlindAmount"].Value);
                OnFoundBigBlind(playerName);
                OnFoundBigBlindAmount(amount);
            }

            /* Find the button */
            if (LineMatchesRegex(line, pokerClient.GetRegex("hand_history_detect_button"), out matchResult))
            {
                Trace.Assert(matchResult.Groups["seatNumber"].Success || matchResult.Groups["playerName"].Success, "Detect button line matched, but no seatNumber or playerName set");
                int seatNumber = -1;

                if (matchResult.Groups["seatNumber"].Success)
                {
                    seatNumber = Int32.Parse(matchResult.Groups["seatNumber"].Value);
                }
                else if (matchResult.Groups["playerName"].Success)
                {
                    String playerName = matchResult.Groups["playerName"].Value;
                    foreach (int seat in playerSeats.Keys)
                    {
                        if ((String)playerSeats[seat] == playerName)
                        {
                            seatNumber = seat;
                            break;
                        }
                    }
                    Trace.Assert(seatNumber != -1, "Button's player name found, but he's not in our players list");
                }

                OnFoundButton(seatNumber);
                lastButtonSeatNumber = seatNumber; // Save
            }
        }

        /* Given a string representation of 1 or more cards, it returns a list with
         * the corresponding card items */
        private List<Card> GenerateCardsFromText(String cardsText)
        {
            // Some clients add a comma between the cards... we can safely remove it
            while (true)
            {
                int commaPosition = cardsText.IndexOf(',');
                if (commaPosition == -1) break;
                cardsText = cardsText.Remove(commaPosition, 1);
            }

            // Also trim
            cardsText = cardsText.Trim();

            String[] cards = cardsText.Split(' ');
            Trace.Assert(cards.Length > 0, "No cards were extracted from " + cardsText + " but GenerateCardsFromText was called. Regex error?");
            
            List<Card> result = new List<Card>(cards.Length);
            foreach (String card in cards)
            {
                result.Add(pokerClient.GenerateCardFromString(card));
            }

            return result;
        }

        /* Given the button's seat number, we deduce who the blinds are. 
         * This will give the wrong results when the table is skipping a blind. 
         * But without any extra information this has to be our best guess */
        private void InfereBlindsFromButton(int buttonSeatNumber)
        {
            if (buttonSeatNumber != 0)
            {
                int numPlayers = playerSeats.Count;               

                // Need at least two players to figure out who is the BB and SB
                if (numPlayers >= 2)
                {
                    // Convert our hash table into an array
                    int[] seatNumbers = new int[numPlayers];
                    playerSeats.Keys.CopyTo(seatNumbers, 0);

                    // Reverse it so that the lower seat #s are closer to the 0 index
                    Array.Reverse(seatNumbers);

                    // Find button's index in the array
                    int buttonIndex = -1;
                    for (int i = 0; i < numPlayers; i++)
                    {
                        if (seatNumbers[i] == lastButtonSeatNumber)
                        {
                            buttonIndex = i;
                            break;
                        }
                    }
                    Trace.Assert(buttonIndex != -1, "Button Index was not found while searching for the blinds. This should have not happened");

                    int bigBlindIndex = -1;
                    int smallBlindIndex = -1;

                    // Case 1: there are only two players in game
                    if (numPlayers == 2)
                    {
                        // The button is also the small blind
                        smallBlindIndex = buttonIndex;

                        // The big blind is the other player
                        bigBlindIndex = buttonIndex == 0 ? 1 : 0;
                    }

                    // Case 2: in the list we have two or more players after of the button
                    else if (numPlayers - buttonIndex - 1 >= 2)
                    {
                        // The the BB and SB are the ones directly after him
                        smallBlindIndex = buttonIndex + 1;
                        bigBlindIndex = buttonIndex + 2;
                    }

                    // Case 3: in the list we have only one player after the button
                    else if (buttonIndex == numPlayers - 2)
                    {
                        // The small blind is the one after him
                        smallBlindIndex = buttonIndex + 1;

                        // The big blind is the first element of the list
                        bigBlindIndex = 0;
                    }

                    // Case 4: the button is the last player of the list
                    else if (buttonIndex == numPlayers - 1)
                    {
                        // Small blind is the first player of the list, bib blind is the second player
                        smallBlindIndex = 0;
                        bigBlindIndex = 1;
                    }
                    else
                    {
                        Trace.Assert(false, "The button index seem to be out of the possible cases. Have we missed something?");
                    }

                    String bigBlindPlayerName = (String)playerSeats[seatNumbers[bigBlindIndex]];
                    String smallBlindPlayerName = (String)playerSeats[seatNumbers[smallBlindIndex]];
                    Trace.WriteLine(String.Format("Inferred from button small blind ({0}) and big blind ({1})",smallBlindPlayerName, bigBlindPlayerName));

                    // Raise events
                    OnFoundBigBlind(bigBlindPlayerName);
                    OnFoundSmallBlind(smallBlindPlayerName);
                }
                else
                {
                    Trace.WriteLine("I couldn't figure out who the blinds are because I don't have enough seats information (but I have the button #).");
                }
            }
            else
            {
                Trace.WriteLine("I couldn't figure out who the blinds are because the button was not specified.");
            }
        }

        /* Will modify the value of currentGamePhase 
           and it might raise a ShowdownWillBegin event */
        private bool ParseForGamePhaseChanges(String line)
        {
            bool foundMatch = false;
            Match matchResult;

            /* Check changes in the game phase */
            if (foundMatch = LineMatchesRegex(line, pokerClient.GetRegex("hand_history_begin_preflop_phase_token")))
            {
                currentGamePhase = HoldemGamePhase.Preflop;
                OnHoleCardsWillBeDealt();
            }
            else if (foundMatch = LineMatchesRegex(line, pokerClient.GetRegex("hand_history_begin_flop_phase_token"), out matchResult))
            {
                // Detect flop cards
                String flopCards = matchResult.Groups["flopCards"].Value;
                List<Card> cards = GenerateCardsFromText(flopCards);

                // Add them to the board
                foreach (Card card in cards) boardCards.Add(card);

                currentGamePhase = HoldemGamePhase.Flop;
            }
            else if (foundMatch = LineMatchesRegex(line, pokerClient.GetRegex("hand_history_begin_turn_phase_token"), out matchResult))
            {                
                // Detect turn card
                String turnCard = matchResult.Groups["turnCard"].Value;
                List<Card> cards = GenerateCardsFromText(turnCard);

                // Add it to the board
                foreach (Card card in cards) boardCards.Add(card);

                currentGamePhase = HoldemGamePhase.Turn;
            }
            else if (foundMatch = LineMatchesRegex(line, pokerClient.GetRegex("hand_history_begin_river_phase_token"), out matchResult))
            {
                // Detect turn card
                String riverCard = matchResult.Groups["riverCard"].Value;
                List<Card> cards = GenerateCardsFromText(riverCard);

                // Add it to the board
                foreach (Card card in cards) boardCards.Add(card);

                currentGamePhase = HoldemGamePhase.River;

                // Call handleFinalBoard this way only if we don't have a more reliable way to find the board
                if (!pokerClient.HasRegex("hand_history_detect_final_board"))
                {
                    HandleFinalBoard(boardCards);
                }
            }
            else if (foundMatch = LineMatchesRegex(line, pokerClient.GetRegex("hand_history_begin_summary_phase_token")))
            {
                currentGamePhase = HoldemGamePhase.Summary;
            }


            else if (pokerClient.HasRegex("hand_history_begin_showdown_phase_token") && (foundMatch = LineMatchesRegex(line, pokerClient.GetRegex("hand_history_begin_showdown_phase_token"))))
            {
                // Avoid double event calling if there are multiple showdowns for main/side pots
                if (currentGamePhase != HoldemGamePhase.Showdown)
                {
                    // Not all poker clients tell us when (and if) showdown begins, in this case we have it (and found it in the line)
                    currentGamePhase = HoldemGamePhase.Showdown;
                    ShowdownEventRaised = true;
                    OnShowdownWillBegin();
                }
            }
            else if (!pokerClient.HasRegex("hand_history_begin_showdown_phase_token"))
            {
                // We don't know when showdown really begins. But if somebody showed his hand, it must have begun
                if (PlayerHasShowedThisRound && !ShowdownEventRaised && currentGamePhase == HoldemGamePhase.River)
                {
                    currentGamePhase = HoldemGamePhase.Showdown;
                    ShowdownEventRaised = true;
                    Trace.WriteLine("Guessing that showdown will begin");
                    OnShowdownWillBegin();
                }
            }


            return foundMatch; 
        }

        /* This method takes care of generating a board (if requirements are met) and raising
         * an event when proper */
        private void HandleFinalBoard(List<Card> cards){

            // We check whether this board is a final board by checking how many cards we've detected
            // We're not interested into boards with less than 5 cards
            if (cards.Count == 5)
            {
                Board board = new HoldemBoard(cards[0], cards[1], cards[2], cards[3], cards[4]);

                OnFinalBoardAvailable(board);
            }
            else
            {
                Trace.WriteLine("Board detected, but only " + cards.Count + " cards in there. Skipping...");
            }
        }
    }
}
