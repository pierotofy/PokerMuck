using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace PokerMuck
{
    class FullTilt : PokerClient
    {
        public FullTilt()
        {
            // Other init stuff?
        }

        public FullTilt(String language)
        {
            InitializeLanguage(language);
        }

        protected override void InitializeData()
        {
            if (CurrentLanguage == "English")
            {
                /* To recognize a valid full tilt poker game window
                  * ex. $0.95 + $0.05 Heads Up Sit & Go (228858150), Table 1 - 10/20 - No Limit Hold'em - Logged In As italystallion89 
                  *     .COM Play 736 (6 max) - 1/2 - No Limit Hold'em - Logged In As italystallion89 */
                regex.Add("game_window_title_to_recognize_tournament_games", @"(?<gameDescription>[^,]+), Table [\d]+ - [\d]+/[\d]+ - (?<gameType>.+) - Logged In As");
                regex.Add("game_window_title_to_recognize_ring_games", @"(?<gameDescription>[^-]+ - [^-]+ - [^-]+) - Logged In As");

                /* Recognize the Hand History game phases */
                regex.Add("hand_history_begin_preflop_phase_token", @"\*\*\* HOLE CARDS \*\*\*");
                regex.Add("hand_history_begin_flop_phase_token", @"\*\*\* FLOP \*\*\*");
                regex.Add("hand_history_begin_turn_phase_token", @"\*\*\* TURN \*\*\*");
                regex.Add("hand_history_begin_river_phase_token", @"\*\*\* RIVER \*\*\*");
                regex.Add("hand_history_begin_showdown_phase_token", @"\*\*\* SHOW DOWN \*\*\*");
                regex.Add("hand_history_begin_summary_phase_token", @"\*\*\* SUMMARY \*\*\*");


                /* Recognize the Hand History gameID 
                 Ex. PokerStars Game #59534069543: Tournament #377151618
                 */
                regex.Add("hand_history_game_id_token", @"PokerStars Game #(?<handId>[\d]+): Tournament #(?<gameId>[\d]+)");

                /* Recognize the table ID and max seating capacity */
                regex.Add("hand_history_table_token", @"Table '(?<tableId>.+)' (?<tableSeatingCapacity>[\d]+)-max");

                /* Recognize game type (Hold'em, Omaha, No-limit, limit, etc.) 
                   Note that for PokerStars.it the only valid currency is EUR (and FPP), but this might be different 
                   on other clients. This regex works for both play money and tournaments */
                regex.Add("hand_history_game_type_token", @"([\d]+FPP (?<gameType>[^-]+) -)|(EUR (?<gameType>[^-]+) -)|(PokerStars Game #[\d]+:  (?<gameType>[^(]+) \([\d]+/[\d]+\))");

                /* Recognize players 
                 Ex. Seat 1: stallion089 (2105 in chips) => 1,"stallion089" 
                 * It ignores those who are marked as "out of hand"
                 */
                regex.Add("hand_history_detect_player_in_game", @"Seat (?<seatNumber>[\d]+): (?<playerName>.+) .*\([\d]+ in chips\) (?!out of hand)");

                /* Recognize mucked hands
                 Ex. Seat 1: stallion089 (button) (small blind) mucked [5d 5s]*/
                // TODO! What if a player has a "(" in his nickname?
                regex.Add("hand_history_detect_mucked_hand", @"Seat [\d]+: (?<playerName>[^(]+) .*(showed|mucked) \[(?<cards>[\d\w ]+)\]");

                /* Recognize winners of a hand 
                 * Ex. cord80 collected 40 from pot */
                regex.Add("hand_history_detect_hand_winner", @"(?<playerName>.+) collected [\d]+ from pot");

                /* Recognize all-ins
                 * Ex. stallion089: raises 605 to 875 and is all-in */
                regex.Add("hand_history_detect_all_in_push", @"(?<playerName>[^:]+): .+ is all-in");

                /* Recognize the final board */
                regex.Add("hand_history_detect_final_board", @"Board \[(?<cards>[\d\w ]+)\]");

                /* Detect who is the small/big blind
                   Ex. stallion089: posts small blind 15 */
                regex.Add("hand_history_detect_small_blind", @"(?<playerName>[^:]+): posts small blind (?<smallBlindAmount>[\d]+)");
                regex.Add("hand_history_detect_big_blind", @"(?<playerName>[^:]+): posts big blind (?<bigBlindAmount>[\d]+)");

                /* Detect who the button is */
                regex.Add("hand_history_detect_button", @"#(?<seatNumber>[\d]+) is the button");


                /* Detect calls
                 * ex. stallion089: calls 10 */
                regex.Add("hand_history_detect_player_call", @"(?<playerName>[^:]+): calls (?<amount>[\d]+)");

                /* Detect bets 
                   ex. stallion089: bets 20 */
                regex.Add("hand_history_detect_player_bet", @"(?<playerName>[^:]+): bets (?<amount>[\d]+)");

                /* Detect folds
                 * ex. preferiti90: folds */
                regex.Add("hand_history_detect_player_fold", @"(?<playerName>[^:]+): folds");

                /* Detect checks
                 * ex. DOTTORE169: checks */
                regex.Add("hand_history_detect_player_check", @"(?<playerName>[^:]+): checks");

                /* Detect raises 
                 * ex. zanzara za: raises 755 to 1155 and is all-in */
                regex.Add("hand_history_detect_player_raise", @"(?<playerName>[^:]+): raises (?<initialPot>[\d]+) to (?<raiseAmount>[\d]+)");

                /* Recognize end of round character sequence (in PokerStars.it it's
                 * a blank line */
                regex.Add("hand_history_detect_end_of_round", @"^$");

                /* Hand history file formats. 
                 * Play money games Example: FT20110328 .COM Play 736 (6 max) - 1-2 - No Limit Hold'em.txt  */     
                config.Add("hand_history_ring_filename_format", @"FT[0-9]+ {0}");

                /* Tournaments example: FT20110328 $0.95 + $0.05 Heads Up Sit & Go (228858150), No Limit Hold'em.txt */
                config.Add("hand_history_tournament_filename_format", @"FT[0-9]+ {0}, {1}");

                /* Game description (as shown in the hand history) */
                config.Add("game_description_no_limit_holdem", "Hold'em No Limit");

            }

            /* Number of sequences required to raise the OnRoundHasTerminated event.
             * This refers to the hand_history_detect_end_of_round regex, on PokerStars.it
             * a round is over after 3 blank lines. Most clients might have only one line */
            config.Add("hand_history_end_of_round_number_of_tokens_required", 3);

        }

        /* Given a game description, returns the corresponding PokerGameType */
        public override PokerGameType GetPokerGameTypeFromGameDescription(string gameDescription)
        {
            if (gameDescription == (String)config["game_description_no_limit_holdem"]) return PokerGameType.Holdem;

            return PokerGameType.Unknown; //Default
        }

        /**
         * This function matches an open window title with patterns to recognize which hand history
         * the current window refers to (if it is even a poker game window). It will return an empty
         * string if it cannot match any pattern */
        public override String GetHandHistoryFilenameRegexPatternFromWindowTitle(String windowTitle)
        {
            // TODO REMOVE IN PRODUCTION
            if (windowTitle == "test.txt - Notepad") return "test.txt";

            /* On Full Tilt, window titles match the filename almost completely.
             * Slashes / are converted to dashes - and a prefix of the form
             * FT{ddmmyy} is added to it. Also, the window title will have a suffix
             * of the form ( - Logged in as <playerName>) which does not appear in the filename. */

            /* There are two different kind of formats:
             * 1 for tournaments and 1 for ring games/play money
             * We'll take care of tournaments first */

            // Ex. $0.95 + $0.05 Heads Up Sit & Go (228858150), Table 1 - 10/20 - No Limit Hold'em - Logged In as stallion089
            Regex regex = GetRegex("game_window_title_to_recognize_tournament_games");
            Match match = regex.Match(windowTitle);
            if (match.Success)
            {
                // We matched a tournament game window
                // gameDescription = $0.95 + $0.05 Heads Up Sit & Go (228858150)
                // gameType = No Limit Hold'em
                String gameDescription = match.Groups["gameDescription"].Value;
                String gameType = match.Groups["gameType"].Value;

                // output: FT20110328 $0.95 + $0.05 Heads Up Sit & Go (228858150), No Limit Hold'em
                return String.Format(GetConfigString("hand_history_tournament_filename_format"), gameDescription, gameType);
            }
            else
            {
                // No luck, try with ring games
                regex = GetRegex("game_window_title_to_recognize_ring_games");
                match = regex.Match(windowTitle);
                if (match.Success)
                {
                    String gameDescription = match.Groups["gameDescription"].Value;

                    /* On full tilt for ring games and play money the game description
                     * pretty much matches the filename, but a conversion of a few characters needs
                     * to be done. */

                    // 1. Convert / to -
                    gameDescription = gameDescription.Replace("/", "-");

                    // Return pattern
                    return String.Format((String)GetConfig("hand_history_ring_filename_format"), gameDescription);
                }
                else
                {
                    return String.Empty; //Could not find any valid match... must be a title we're not interested into
                }
            }
        }

        public override String Name
        {
            get
            {
                return "Full Tilt Poker";
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
