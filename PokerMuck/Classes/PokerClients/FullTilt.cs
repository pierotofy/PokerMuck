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
                regex.Add("hand_history_begin_flop_phase_token", @"\*\*\* FLOP \*\*\* \[(?<flopCards>[\d\w ]+)\]");
                regex.Add("hand_history_begin_turn_phase_token", @"\*\*\* TURN \*\*\* \[([\d\w ]+)\] \[(?<turnCard>[\d\w ]+)\]");
                regex.Add("hand_history_begin_river_phase_token", @"\*\*\* RIVER \*\*\* \[([\d\w ]+)\] \[(?<riverCard>[\d\w ]+)\]");
                regex.Add("hand_history_begin_showdown_phase_token", @"\*\*\* SHOW DOWN \*\*\*");
                regex.Add("hand_history_begin_summary_phase_token", @"\*\*\* SUMMARY \*\*\*");


                /* Recognize the Hand History gameID 
                 Ex. Full Tilt Poker Game #29428529378: $0.95 + $0.05
                 * Full Tilt Poker Game #29427316352: Table Stewart (6 max, shallow)
                 * Full Tilt Poker Game #29428122872: Table .COM Play 736 (6 max)
                 */
                regex.Add("hand_history_game_id_token", @"Full Tilt Poker Game #(?<handId>[\d]+): (?<gameId>[^,-]+) (-|,)");

                /* Recognize the table ID and max seating capacity (if available) 
                 ex. Full Tilt Poker Game #29459258249: Table Valley Of Fire (shallow) - $0.01/$0.02 - No Limit Hold'em - 19:52:33 ET - 2011/03/29 */
                regex.Add("hand_history_table_token", @"Table (?<tableId>.+) - \$?[\d\.]+\/\$?[\d\.]+ - .+ - [\d]{2}:[\d]{2}:[\d]{2} .* - [\d]{4}\/[\d]{2}\/[\d]{2}");

                /* Recognize game type (Hold'em, Omaha, No-limit, limit, etc.) 
                 * Full Tilt Poker Game #29428516957: $0.95 + $0.05 Heads Up Sit & Go (228858150), Table 1 - 10/20 - No Limit Hold'em - 18:30:27 ET - 2011/03/28*/
                regex.Add("hand_history_game_token", @"Full Tilt Poker Game #[\d]+: .+ - \$?[\d\.]+\/\$?[\d\.]+ - (?<gameType>.+) - [\d]{2}:[\d]{2}:[\d]{2} .* - [\d]{4}\/[\d]{2}\/[\d]{2}");

                /* Recognize players 
                 Ex. Seat 3: italystallion89 ($0.80)
                 * It ignores those who are marked as ", is sitting out"
                 */
                regex.Add("hand_history_detect_player_in_game", @"Seat (?<seatNumber>[\d]+): (?<playerName>.+) .*\(\$?[\d\.\,]+\)$");

                /* Recognize mucked hands
                 Ex. Seat 1: stallion089 (button) (small blind) mucked [5d 5s]*/
                // TODO! What if a player has a "(" in his nickname?
                regex.Add("hand_history_detect_mucked_hand", @"Seat [\d]+: (?<playerName>[^(]+) .*(showed|mucked) \[(?<cards>[\d\w ]+)\]");

                /* Recognize winners of a hand 
                 * Ex. mosby2 wins the pot (40) */
                regex.Add("hand_history_detect_hand_winner", @"(?<playerName>.+) wins the pot \(\$?[\d\.\,]+\)");

                /* Recognize all-ins
                 * Ex. mosby2 bets 400, and is all in */
                regex.Add("hand_history_detect_all_in_push", @"(?<playerName>.+) (bets|calls|raises).+, and is all in");

                /* Recognize the final board */
                regex.Add("hand_history_detect_final_board", @"Board: \[(?<cards>[\d\w ]+)\]");

                /* Detect who is the small/big blind
                   Ex. kan-ikkje posts the small blind of $0.01
                 * italystallion89 posts the big blind of $0.02 */
                regex.Add("hand_history_detect_small_blind", @"(?<playerName>.+) posts the small blind of \$?(?<smallBlindAmount>[\d\.\,]+)");
                regex.Add("hand_history_detect_big_blind", @"(?<playerName>.+) posts the big blind of \$?(?<bigBlindAmount>[\d\.\,]+)");

                /* Detect who the button is */
                regex.Add("hand_history_detect_button", @"The button is in seat #(?<seatNumber>[\d]+)");

                /* Detect who our hero is (what's his nickname)
                 * ex. Dealt to italystallion89 [Tc 3d] */
                regex.Add("hand_history_detect_hero_name", @"Dealt to (?<heroName>.+) \[[\w\d ]+\]$");

                /* Detect calls
                 * ex. SILJCAR calls $0.02 */
                regex.Add("hand_history_detect_player_call", @"(?<playerName>.+) calls \$?(?<amount>[\d\.\,]+[\d]+)");

                /* Detect bets
                   ex. kan-ikkje bets $0.02 */
                regex.Add("hand_history_detect_player_bet", @"(?<playerName>.+) bets \$?(?<amount>[\d\.\,]+[\d]+)");

                /* Detect folds
                 * ex. kan-ikkje folds */
                regex.Add("hand_history_detect_player_fold", @"(?<playerName>.+) folds");

                /* Detect checks
                 * ex. italystallion89 checks */
                regex.Add("hand_history_detect_player_check", @"(?<playerName>.+) checks");

                /* Detect raises 
                 * ex. SILJCAR raises to $0.04 */
                regex.Add("hand_history_detect_player_raise", @"(?<playerName>.+) raises to \$?(?<raiseAmount>[\d\.\,]+[\d]+)");

                /* Recognize end of round character sequence (in Full Tilt it's
                 * a blank line */
                regex.Add("hand_history_detect_end_of_round", @"^$");

                /* Hand history file formats. 
                 * Play money games Example: FT20110328 .COM Play 736 (6 max) - 1-2 - No Limit Hold'em.txt  */     
                config.Add("hand_history_ring_filename_format", @"FT[0-9]+ {0}");

                /* Tournaments example: FT20110328 $0.95 + $0.05 Heads Up Sit & Go (228858150), No Limit Hold'em.txt */
                config.Add("hand_history_tournament_filename_format", @"FT[0-9]+ {0}, {1}");

                /* Game description (as shown in the hand history) */
                config.Add("game_description_no_limit_holdem", "No Limit Hold'em");

            }

            /* Number of sequences required to raise the OnRoundHasTerminated event.
             * This refers to the hand_history_detect_end_of_round regex, on PokerStars.it
             * a round is over after 3 blank lines. Most clients might have only one line */
            config.Add("hand_history_end_of_round_number_of_tokens_required", 3);

        }

        /* Given a game description, returns the corresponding PokerGame */
        public override PokerGame GetPokerGameFromGameDescription(string gameDescription)
        {
            Trace.WriteLine("Found game description: " + gameDescription);

            if (gameDescription == (String)config["game_description_no_limit_holdem"]) return PokerGame.Holdem;

            return PokerGame.Unknown; //Default
        }

        public override int InferMaxSeatingCapacity(string line, String filename)
        {
            // line = Full Tilt Poker Game #29457232449: Table Escondido (shallow) - $0.01/$0.02 - No Limit Hold'em - 18:34:48 ET - 2011/03/29
            
            // If we find the word "heads up" this should be a two seat table
            if (line.IndexOf("Heads Up") != -1) return MAX_SEATING_CAPACITY_HEADS_UP;

            else{
                Regex r = new Regex(@"(?<maxSeatingCapacity>[\d]+) max");
                
                // If we find the word "max" with a number before it, then we guess that that's the max number of seats
                Match m = r.Match(line);
                if (m.Success){
                    String maxSeatingCapacity = m.Groups["maxSeatingCapacity"].Value;
                    int maxCapacityGuess = Int32.Parse(maxSeatingCapacity);

                    Trace.WriteLine("Matched max seating capacity: " + maxSeatingCapacity + " from " + line);

                    return maxCapacityGuess;
                }else{

                    // No luck, return default value, hoping for the best
                    return DEFAULT_MAX_SEATING_CAPACITY;
                }
            }
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

                // A conversion of a few characters needs to be done
                gameDescription = StringToRegexPattern(gameDescription);

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
                    gameDescription = StringToRegexPattern(gameDescription);

                    // Return pattern
                    return String.Format((String)GetConfig("hand_history_ring_filename_format"), gameDescription);
                }
                else
                {
                    return String.Empty; //Could not find any valid match... must be a title we're not interested into
                }
            }
        }

        public override String GetCurrentHandHistorySubdirectory()
        {
            return String.Empty; //Not necessary for FTP
        }

        public override bool IsPlayerSeatingPositionRelative(PokerGameType gameType)
        {
            return false;
        }

        public override PokerGameType GetPokerGameTypeFromWindowTitle(string windowTitle)
        {
            return PokerGameType.Unknown; // It doesn't make a difference to know the game type
        }

        public override String Name
        {
            get
            {
                return "Full Tilt Poker";
            }
        }

        public override string XmlName
        {
            get {
                return "Full_Tilt_Poker";
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
