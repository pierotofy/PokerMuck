using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

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
                  * ex. 20 Chip Deep - 25/50 - T33435 Table 1
                  *     Carom 2 - 0.01/0.02 - No Limit Holdem
                  */
                regex.Add("game_window_title_to_recognize_games", @"^((.+ - (?<gameId>T[\d]+) Table [\d]+)|(?<gameId>.+) - .*Limit Holdem.*)$");

                /* Recognize the Hand History game phases */
                regex.Add("hand_history_begin_preflop_phase_token", @"\*\*\* HOLE CARDS \*\*\*");
                regex.Add("hand_history_begin_flop_phase_token", @"\*\*\* FLOP \*\*\* \[(?<flopCards>[\d\w ]+)\]");
                regex.Add("hand_history_begin_turn_phase_token", @"\*\*\* TURN \*\*\* \[(?<turnCard>[\d\w ]+)\]");
                regex.Add("hand_history_begin_river_phase_token", @"\*\*\* RIVER \*\*\* \[(?<riverCard>[\d\w ]+)\]");
                regex.Add("hand_history_begin_showdown_phase_token", @"\*\*\* ((Side|Main) )?Pot ([\d] )?Show Down \*\*\*"); // There's no showdown in SWC?
                regex.Add("hand_history_begin_summary_phase_token", @"\*\*\* SUMMARY \*\*\*");


                /* Recognize the Hand History gameID 
                 * SWC does not provide a ID for the game. We return some other ID that identifies the hand.
                 Ex. Hand #4837351: No Limit Holdem - 0.01/0.02
                 */
                regex.Add("hand_history_game_id_token", @"^Hand #(?<gameId>[\d]+):.+");

                regex.Add("hand_history_blind_amounts", @"^Hand #[\d]+:.+ - (?<smallBlindAmount>[\d\.\,]+)\/(?<bigBlindAmount>[\d\.\,]+)$");

                /* Recognize the table ID and max seating capacity (if available)*/
                regex.Add("hand_history_table_token", @"^Table '(?<tableId>[^']+)'");

                /* Recognize game type (Hold'em, Omaha, No-limit, limit, etc.) */
                regex.Add("hand_history_game_token", @"^Hand #[\d]+: (?<gameType>.+) - .+");

                /* Recognize players */
                regex.Add("hand_history_detect_player_in_game", @"^Seat (?<seatNumber>[\d]+): (?<playerName>.+) \((€|\$)?[\d\.\,]+\)");

                /* Recognize winners of a hand  */
                regex.Add("hand_history_detect_hand_winner", @"^(?<playerName>.+) wins pot \(");

                /* Recognize all-ins */
                regex.Add("hand_history_detect_all_in_push", @"^(?<playerName>.+) (bets|calls|raises).+ \(all in\)");

                /* Detect who is the small/big blind */
                regex.Add("hand_history_detect_small_blind", @"^(?<playerName>.+): posts the small blind");
                regex.Add("hand_history_detect_big_blind", @"^(?<playerName>.+): posts the big blind");

                /* Detect who the button is */
                regex.Add("hand_history_detect_button", @"Seat (?<seatNumber>[\d]+) is the button");

                /* Detect who our hero is (what's his nickname)  */
                regex.Add("hand_history_detect_hero_name", @"Dealt to (?<heroName>.+) \[[\w\d ]+\]");

                /* Detect calls */
                regex.Add("hand_history_detect_player_call", @"(?<playerName>.+) calls \$?(?<amount>[\d\.\,]+[\d]*),?");

                /* Detect bets */
                regex.Add("hand_history_detect_player_bet", @"(?<playerName>.+) bets \$?(?<amount>[\d\.\,]+[\d]*),?");

                /* Detect folds */
                regex.Add("hand_history_detect_player_fold", @"(?<playerName>.+) folds");

                /* Detect checks */
                regex.Add("hand_history_detect_player_check", @"(?<playerName>.+) checks");

                /* Detect raises  */
                regex.Add("hand_history_detect_player_raise", @"(?<playerName>.+) raises to \$?(?<raiseAmount>[\d\.\,]+[\d]*),?");

                /* Recognize end of round character sequence (in SWC it's a blank line) */
                regex.Add("hand_history_detect_end_of_round", @"^$");

                /* Game description (as shown in the hand history) */
                config.Add("game_description_holdem", "Holdem");

            }

            /* Number of sequences required to raise the OnRoundHasTerminated event.
             * This refers to the hand_history_detect_end_of_round regex, on SealsWithClubs
             * we need 1 occurrences (1 blank line) */
            config.Add("hand_history_end_of_round_number_of_tokens_required", 1);

        }

        /* Given a game description, returns the corresponding PokerGame */
        public override PokerGame GetPokerGameFromGameDescription(string gameDescription)
        {
            Trace.WriteLine("Found game description: " + gameDescription);

            if (gameDescription.Contains((String)config["game_description_holdem"])) return PokerGame.Holdem;

            return PokerGame.Unknown; //Default
        }

        public override List<KeyValuePair<String, String>> GetMuckedHands(String currentGameId) {
            var result = new List<KeyValuePair<String, String>>();

            String content = getXmlHandData(currentGameId);
            
            if (content != "") {
                // We use a simple regex to parse XML since our case is simple enough
                MatchCollection matches = Regex.Matches(content, "<display_cards name=\"(?<playerName>[^\"]+)\" cards=\"(?<cards>[^\"]+)\"/>");
                foreach (Match m in matches) {
                    result.Add(new KeyValuePair<string, string>(m.Groups["playerName"].Value, m.Groups["cards"].Value));
                }
            } else {
                Trace.WriteLine("Could not get mucked hands from external source.");
            }

            return result;
        }


        private String getXmlHandData(String currentGameId) {
            if (currentGameId == "") return "";

            String swcXmlClientDataFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\swcpoker\Client Game Data\" + currentGameId + ".xml";

            // Lock until file is created
            while (!File.Exists(swcXmlClientDataFile)) Thread.Sleep(5);

            if (File.Exists(swcXmlClientDataFile)) {
                return File.ReadAllText(swcXmlClientDataFile);
            } else {
                return "";
            }
        }

        public override PokerGameType GetPokerGameTypeFromWindowTitle(string windowTitle)
        {
            return PokerGameType.Unknown; // It doesn't make a difference to know the game type
        }

        public override int InferMaxSeatingCapacity(string line, String filename, String currentGameId)
        {
            // SWC does not provide seating information in the hand history itself
            // but it writes it in the more specific XML file hand history

            String content = getXmlHandData(currentGameId);
            if (content != "") {
                Match m = Regex.Match(content, "<table seats=\"(?<seatCount>[\\d]+)\"");
                if (m.Groups["seatCount"].Success) {
                    int seatCount = Int32.Parse(m.Groups["seatCount"].Value);
                    Trace.WriteLine("Found seat count from XML");
                    return seatCount;
                } else {
                    return MAX_SEATING;
                }
            }else{
                // Return default value, hoping for the best
                return MAX_SEATING;
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

                return gameId.Replace("/", "-").Replace("#", "");
            }
            else
            {
                return String.Empty;
            }

        }

        public override String GetCurrentHandHistorySubdirectory() {
            // Seals with clubs organizes hand history files in directories by day
            // Format: 2015-12-05\{TODAY'S FILES} (2015 = year, 12 = month, 05 = day) 

            // TODO: save timezone information somewhere? (If you play in a different country you can specify manually the server's timezone)
            //DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"));

            // Uncomment in production (works for 99% of the users)
            DateTime now = DateTime.Now;

            return String.Format(@"{0}-{1}-{2}", now.Year.ToString("D4"), now.Month.ToString("D2"), now.Day.ToString("D2"));
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
