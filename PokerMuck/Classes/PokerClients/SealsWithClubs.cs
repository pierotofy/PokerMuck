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
    class SealsWithClubs : PokerClient
    {
        private const int MAX_SEATING = 10;

        public SealsWithClubs()
        {
        }

        public SealsWithClubs(String language)
        {
            InitializeLanguage(language);
        }

        protected override void InitializeData()
        {
            if (CurrentLanguage == "English")
            {
                /* To recognize a valid game window title
                  * ex. NLHE 9max normal 10 #2 - Table 1 - NL Hold'em (9+1) - Logged in as stallion 
                  *     Every Hour Three Prizes - Table 5 - NL Hold'em (0+0) - Logged in as stallion
                 *      PLO 9max 1/1 #1 - PL Omaha (200 - 100) - Logged in as stallion */
                regex.Add("game_window_title_to_recognize_games", @"^((?<gameId>.+ - Table [\d]+) - .+ - Logged in as .+|(?<gameId>.+) - .+ \([\d- ]+\) - Logged in as .+)$");

                /* Recognize the Hand History game phases */
                regex.Add("hand_history_begin_preflop_phase_token", @"\*\* Hole Cards \*\*");
                regex.Add("hand_history_begin_flop_phase_token", @"\*\* Flop \*\* \[(?<flopCards>[\d\w ]+)\]");
                regex.Add("hand_history_begin_turn_phase_token", @"\*\* Turn \*\* \[(?<turnCard>[\d\w ]+)\]");
                regex.Add("hand_history_begin_river_phase_token", @"\*\* River \*\* \[(?<riverCard>[\d\w ]+)\]");
                regex.Add("hand_history_begin_showdown_phase_token", @"\*\* ((Side|Main) )?Pot ([\d] )?Show Down \*\*");
                regex.Add("hand_history_begin_summary_phase_token", @"\*\* Summary \*\*"); // There's no summary in SWC, but we need to include it


                /* Recognize the Hand History gameID 
                 * SWC does not provide a ID for the game. We return some other ID that identifies the hand.
                 Ex. Hand #4837351-13 - 2013-04-02 11:12:09
                 */
                regex.Add("hand_history_game_id_token", @"^Hand #(?<handId>[\d]+) - ");

                /* Recognize the table ID and max seating capacity (if available) 
                 ex. Table: Every Hour Three Prizes - Table 6 
                     Table: NLHE 9max 1/1 #3*/
                regex.Add("hand_history_table_token", @"^Table: (?<tableId>.+)");

                /* Recognize game type (Hold'em, Omaha, No-limit, limit, etc.) 
                 * ex. Game: NL Hold'em (0+0) - Blinds 75/150 
                       Game: NL Hold'em (20 - 100) - Blinds 1/1*/
                regex.Add("hand_history_game_token", @"^Game: (?<gameType>.+) \([\d\+\- \.]+\) - ");

                /* Recognize players 
                 Ex. Seat 1: lolsupp123 (900)
                 *   Seat 3: shiftysean (330.02)
                 */
                regex.Add("hand_history_detect_player_in_game", @"^Seat (?<seatNumber>[\d]+): (?<playerName>.+) \((€|\$)?[\d\.]+\)");

                /* Recognize mucked hands
                 Ex. gobulldogs shows [Ac 7c] (Three of a Kind, Aces +T7) */
                regex.Add("hand_history_detect_mucked_hand", @"^(?<playerName>.+) .*(shows|mucks) \[(?<cards>[\d\w ]+)\]");

                /* Recognize winners of a hand
                 * Ex. lolsupp123 wins Pot (2075) */
                regex.Add("hand_history_detect_hand_winner", @"^(?<playerName>.+) wins Pot \(");

                /* Recognize all-ins
                 * Ex. stallion raises to 1120 (All-in) */
                regex.Add("hand_history_detect_all_in_push", @"^(?<playerName>.+) (bets|calls|raises).+ \(All-in\)");

                /* Detect who is the small/big blind
                   Ex. dest04nab posts small blind 100
                       lolsupp123 posts big blind 200 */
                regex.Add("hand_history_detect_small_blind", @"^(?<playerName>.+) posts small blind (€|\$)?(?<smallBlindAmount>[\d\.\,]+)");
                regex.Add("hand_history_detect_big_blind", @"^(?<playerName>.+) posts big blind (€|\$)?(?<bigBlindAmount>[\d\.\,]+)");

                /* Detect who the button is
                   jycg has the dealer button */
                regex.Add("hand_history_detect_button", @"^(?<playerName>.+) has the dealer button");

                /* Detect who our hero is (what's his nickname) 
                 ex. Dealt to stallion [Qc Qh] */
                regex.Add("hand_history_detect_hero_name", @"Dealt to (?<heroName>.+) \[[\w\d ]+\]");

                /* Detect calls
                 * ex. SILJCAR calls $0.02 */
                regex.Add("hand_history_detect_player_call", @"(?<playerName>.+) calls \$?(?<amount>[\d\.\,]+[\d]*)");

                /* Detect bets
                   ex. kan-ikkje bets $0.02 */
                regex.Add("hand_history_detect_player_bet", @"(?<playerName>.+) bets \$?(?<amount>[\d\.\,]+[\d]*)");

                /* Detect folds
                 * ex. kan-ikkje folds */
                regex.Add("hand_history_detect_player_fold", @"(?<playerName>.+) folds");

                /* Detect checks
                 * ex. italystallion89 checks */
                regex.Add("hand_history_detect_player_check", @"(?<playerName>.+) checks");

                /* Detect raises 
                 * ex. SILJCAR raises to $0.04 */
                regex.Add("hand_history_detect_player_raise", @"(?<playerName>.+) raises to \$?(?<raiseAmount>[\d\.\,]+[\d]*)");

                /* Recognize end of round character sequence (in SWC it's a blank line) */
                regex.Add("hand_history_detect_end_of_round", @"^$");

                /* Game description (as shown in the hand history) */
                config.Add("game_description_holdem", "Hold'em");

            }

            /* Number of sequences required to raise the OnRoundHasTerminated event.
             * This refers to the hand_history_detect_end_of_round regex, on SealsWithClubs
             * we need 3 occurrences (3 blank lines) */
            config.Add("hand_history_end_of_round_number_of_tokens_required", 3);

        }

        /* Given a game description, returns the corresponding PokerGame */
        public override PokerGame GetPokerGameFromGameDescription(string gameDescription)
        {
            Trace.WriteLine("Found game description: " + gameDescription);

            if (gameDescription.Contains((String)config["game_description_holdem"])) return PokerGame.Holdem;

            return PokerGame.Unknown; //Default
        }

        public override PokerGameType GetPokerGameTypeFromWindowTitle(string windowTitle)
        {
            return PokerGameType.Unknown; // It doesn't make a difference to know the game type
        }

        public override int InferMaxSeatingCapacity(string line, String filename)
        {
            // SWC does not provide seating information in the hand history itself
            // but it writes some clues in the filename

            // ex.HH20130405 NLHE 9max 1-1 1.txt
            //    HH20130405 NLHE HU 1-1 1.txt

            // If we find the a referene to heads-up play, this should be a two seat table
            if (line.IndexOf(" HU ") != -1) return 2;

            else
            {
                Regex r = new Regex(@" (?<maxSeatingCapacity>[\d]+)max ");

                // If we find the word "max" with a number before it, then we guess that that's the max number of seats
                Match m = r.Match(line);
                if (m.Success)
                {
                    String maxSeatingCapacity = m.Groups["maxSeatingCapacity"].Value;
                    int maxCapacityGuess = Int32.Parse(maxSeatingCapacity);

                    Trace.WriteLine("Matched max seating capacity: " + maxSeatingCapacity + " from " + line);

                    return maxCapacityGuess;
                }
                else
                {
                    // No luck, return default value, hoping for the best
                    return MAX_SEATING;
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
            //if (windowTitle == "HH20130405 NLHE 9max 1-1 1.txt - Notepad") return "HH20130405 NLHE 9max 1-1 1.txt";

            Regex regex = GetRegex("game_window_title_to_recognize_games");
            Match match = regex.Match(windowTitle);
            if (match.Success)
            {
                // We matched a game window
                // gameId = Every Hour Three Prizes - Table 1
                // Filename = 
                String gameId = match.Groups["gameId"].Value;

                DateTime now = DateTime.Now;
                String date = String.Format(@"{0}{1}{2}", now.Year.ToString("D4"), now.Month.ToString("D2"), now.Day.ToString("D2"));

                return ("HH" + date + " " + gameId).Replace("/", "-").Replace("#", "");
            }
            else
            {
                return String.Empty;
            }

        }

        public override String GetCurrentHandHistorySubdirectory()
        {
            return String.Empty; // Not needed
        }

        public override bool IsPlayerSeatingPositionRelative(PokerGameType gameType){
            return false;
        }

        public override String Name
        {
            get
            {
                return "Seals With Clubs";
            }
        }

        public override string XmlName
        {
            get {
                return "Seals_With_Clubs";
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
