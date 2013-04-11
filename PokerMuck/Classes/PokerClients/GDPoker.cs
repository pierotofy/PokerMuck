using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace PokerMuck
{
    class GDPoker : PokerClient
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        private const int MAX_SEATING = 10;
        private const string CASH_GAME_FILENAME = "handhistory0";

        public GDPoker()
        {
        }

        public GDPoker(String language)
        {
            InitializeLanguage(language);
        }

        protected override void InitializeData()
        {
            if (CurrentLanguage == "English")
            {
                /* To recognize a valid full tilt poker game window
                  * ex.Table #1, TURBO €  3 Normandia    Heads Up, No-limit Texas Hold'em, €3 (real money), 10/20 [ID=T5-43557381-0][S-1556-758114] */
                regex.Add("game_window_title_to_recognize_games", @"^.+\[(?<gameId>.+)\]$");

                regex.Add("game_window_title_to_recognize_ring_games", @"\((€|\$)?[\d\.]+\/(€|\$)?[\d\.]+\)");

                /* Recognize the Hand History game phases */
                regex.Add("hand_history_begin_preflop_phase_token", @"Dealing pocket cards");
                regex.Add("hand_history_begin_flop_phase_token", @"--- Dealing flop \[(?<flopCards>[\d\w, ]+)\]");
                regex.Add("hand_history_begin_turn_phase_token", @"--- Dealing turn \[(?<turnCard>[\d\w]+)\]");
                regex.Add("hand_history_begin_river_phase_token", @"--- Dealing river \[(?<riverCard>[\d\w]+)\]");
                regex.Add("hand_history_begin_summary_phase_token", @"^Summary:$");


                /* Recognize the Hand History gameID 
                 Ex. ***** History for hand T5-43557381-2 (TOURNAMENT: "TURBO €  3 Normandia    Heads Up", S-1556-758114, buy-in: €3) *****
                 */
                regex.Add("hand_history_game_id_token", @"\*\*\*\*\* History for hand (?<handId>[^(]+) \([^,]+, (?<gameId>[^,]+), .+\) \*\*\*\*\*");

                /* Recognize the table ID and max seating capacity (if available) 
                 ex. Table: Table #1 [43557381] (NO_LIMIT TEXAS_HOLDEM 10/20, Chips) */
                regex.Add("hand_history_table_token", @"Table: (Table )?(?<tableId>.+) \[");

                /* Recognize game type (Hold'em, Omaha, No-limit, limit, etc.) 
                 * Table: Table #1 [43557381] (NO_LIMIT TEXAS_HOLDEM 10/20, Chips) */
                regex.Add("hand_history_game_token", @"Table: (Table )?.+ \[.+\] \((?<gameType>[^\d]+) (€|\$)?[\d\.]+\/(€|\$)?[\d\.]+, .+\)");

                /* Recognize players 
                 Ex. Seat 6: millino (1590)
                 */
                regex.Add("hand_history_detect_player_in_game", @"Seat (?<seatNumber>[\d]+): (?<playerName>.+) \((€|\$)?[\d\.]+\)$");

                /* Recognize mucked hands
                 Ex. Seat 8: stallion089 (1900), net: +90, [6s, Th] (PAIR SIX) */
                regex.Add("hand_history_detect_mucked_hand", @"Seat [\d]+: (?<playerName>[^(]+) .* \[(?<cards>[\d\w, ]+)\]");

                /* Recognize winners of a hand
                 * Ex. Main pot: 180 won by stallion089 (180) */
                regex.Add("hand_history_detect_hand_winner", @": .+ won by (?<playerName>.+) \((€|\$)?[\d\.\,]+\)");

                /* Recognize all-ins
                 * Ex. millino raises 1250 to 1350 [all in] */
                regex.Add("hand_history_detect_all_in_push", @"(?<playerName>.+) (bets|calls|raises).+ \[all in\]$");

                /* Detect who is the small/big blind
                   Ex. stallion089 posts small blind (75)
                        millino posts big blind (150) */
                regex.Add("hand_history_detect_small_blind", @"(?<playerName>.+) posts small blind \((€|\$)?(?<smallBlindAmount>[\d\.\,]+)\)");
                regex.Add("hand_history_detect_big_blind", @"(?<playerName>.+) posts big blind \((€|\$)?(?<bigBlindAmount>[\d\.\,]+)\)");

                /* Detect who the button is 
                   Button: seat 8 */
                regex.Add("hand_history_detect_button", @"Button: seat (?<seatNumber>[\d]+)");

                /* Detect who our hero is (what's his nickname) 
                 ex. User: stallion089 */
                regex.Add("hand_history_detect_hero_nickname", @"User: (?<heroNickname>.+)$");

                /* Detect calls
                 * ex. stallion089 calls 75 */
                regex.Add("hand_history_detect_player_call", @"(?<playerName>.+) calls (€|\$)?(?<amount>[\d\.\,]+)");

                /* Detect bets
                   ex. stallion089 bets 150 */
                regex.Add("hand_history_detect_player_bet", @"(?<playerName>.+) bets (€|\$)?(?<amount>[\d\.\,]+)");

                /* Detect folds
                 * ex. millino folds */
                regex.Add("hand_history_detect_player_fold", @"(?<playerName>.+) folds");

                /* Detect checks
                 * ex. millino checks */
                regex.Add("hand_history_detect_player_check", @"(?<playerName>.+) checks");

                /* Detect raises 
                 * ex. millino raises 2050 to 2125 [all in] */
                regex.Add("hand_history_detect_player_raise", @"(?<playerName>.+) raises (€|\$)?([\d\.]+) to (€|\$)?(?<raiseAmount>[\d\.]+)");

                /* Recognize end of round character sequence (on GDPoker is a pattern) */
                regex.Add("hand_history_detect_end_of_round", @"\*\*\*\*\* End of hand .+ \*\*\*\*\*");

                /* Game description (as shown in the hand history) */
                config.Add("game_description_no_limit_holdem", "NO_LIMIT TEXAS_HOLDEM");

            }

            /* Number of sequences required to raise the OnRoundHasTerminated event.
             * This refers to the hand_history_detect_end_of_round regex, on GDPoker
             * we need only one occurrence */
            config.Add("hand_history_end_of_round_number_of_tokens_required", 1);

        }

        /* Given a game description, returns the corresponding PokerGame */
        public override PokerGame GetPokerGameFromGameDescription(string gameDescription)
        {
            Trace.WriteLine("Found game description: " + gameDescription);

            if (gameDescription == (String)config["game_description_no_limit_holdem"]) return PokerGame.Holdem;

            return PokerGame.Unknown; //Default
        }

        public override PokerGameType GetPokerGameTypeFromWindowTitle(string windowTitle)
        {
            Match ringMatch = GetRegex("game_window_title_to_recognize_ring_games").Match(windowTitle);
            if (ringMatch.Success)
            {
                return PokerGameType.Ring;
            }
            else
            {
                return PokerGameType.Tournament;
            }
        }

        public override int InferMaxSeatingCapacity(string line, String filename)
        {
           // On GB Poker, seats are relative to the player (view is always centered reative to the player position)
           // So for hud positioning purposes, we just say that there are always MAX_SEATING seats
            // the layout of the table doesn't change depending on the number of players
            return MAX_SEATING;
        }

        /**
         * This function matches an open window title with patterns to recognize which hand history
         * the current window refers to (if it is even a poker game window). It will return an empty
         * string if it cannot match any pattern */
        public override String GetHandHistoryFilenameRegexPatternFromWindowTitle(String windowTitle)
        {
            // TODO REMOVE IN PRODUCTION
            if (windowTitle == "test.txt - Notepad") return "test.txt";
            if (windowTitle == "test2.txt - Notepad") return "test2.txt";

            Regex regex = GetRegex("game_window_title_to_recognize_games");
            Match match = regex.Match(windowTitle);
            if (match.Success)
            {
                // We matched a game window
                // gameId = R-880-81
                String gameId = match.Groups["gameId"].Value;

                /* If this is a cash game, the filename is always handhistory0.txt
                 * otherwise it's just gameId.txt */

                Match ringMatch = GetRegex("game_window_title_to_recognize_ring_games").Match(windowTitle);
                if (ringMatch.Success)
                {
                    return CASH_GAME_FILENAME;
                }
                else
                {
                    return gameId;
                }
            }
            else
            {
                return String.Empty;
            }

        }

        public override void DoStartupProcessing(string storedHandHistoryDirectory)
        {
            base.DoStartupProcessing(storedHandHistoryDirectory);

            /* The hard link we created for the cash games might need to be refreshed... so if it exists, we delete 
             * (we'll recreate it anyway later) */
            String file = storedHandHistoryDirectory + @"\" + GetCurrentHandHistorySubdirectory() + @"\" + CASH_GAME_FILENAME + ".txt";
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                    Trace.WriteLine("Deleted " + file);
                }
                catch (IOException)
                {
                    Trace.WriteLine("Failed to delete: " + file + ". Is it being used?");
                }

            }
        }

        public override void DoPregameProcessing(string storedHandHistoryDirectory)
        {
            base.DoPregameProcessing(storedHandHistoryDirectory);

            /* GD Poker stores cash games hand history in the root hand history directory, rather than in a subdirectory
             * this is an issue because it screws up our hand history monitor. So we make a hard link of it in the appropriate
             * subdirectory (symbolic links need administrator privileges... yikes!) */
            String fileToCreate = storedHandHistoryDirectory + @"\" + GetCurrentHandHistorySubdirectory() + @"\" + CASH_GAME_FILENAME + ".txt";
            String fileToLink = storedHandHistoryDirectory + @"\" + CASH_GAME_FILENAME + ".txt";

            if (!File.Exists(fileToCreate) && File.Exists(fileToLink))
            {
                if (CreateHardLink(fileToCreate, fileToLink, IntPtr.Zero))
                {
                    Trace.WriteLine("Created hard link for " + fileToLink + ".txt");
                }
                else
                {
                    Trace.WriteLine("Failed to create hard link for " + fileToLink + ".txt");
                }
              
            }
        }

        public override String GetCurrentHandHistorySubdirectory()
        {
            return "Tournaments";
        }

        public override bool IsPlayerSeatingPositionRelative(PokerGameType gameType){
            if (gameType == PokerGameType.Tournament) return true;
            else if (gameType == PokerGameType.Ring) return false;
            else
            {
                Trace.WriteLine("Asked about player seating position relativity, but game type is unknown, guessing...");
                return true;
            }
        }

        public override String Name
        {
            get
            {
                return "GD Poker";
            }
        }

        public override string XmlName
        {
            get {
                return "GD_Poker";
            }
        }

        public override ArrayList SupportedLanguages
        {
            get
            {
                ArrayList languages = new ArrayList();
                languages.Add("English");
                return languages;
            }
        }

        public override ArrayList SupportedGameModes
        {
            get
            {
                ArrayList supportedGameModes = new ArrayList();
                supportedGameModes.Add("No Limit Hold'em"); //TODO CHECK

                return supportedGameModes;
            }
        }

        protected override RegexOptions regexOptions
        {
            get
            {
                return RegexOptions.IgnoreCase | RegexOptions.Compiled;
            }
        }
    }
}
