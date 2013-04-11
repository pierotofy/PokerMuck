using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace PokerMuck
{
    class PartyPoker : PokerClient
    {
        public PartyPoker()
        {
        }

        public PartyPoker(String language)
        {
            InitializeLanguage(language);
        }

        protected override void InitializeData()
        {
            if (CurrentLanguage == "English")
            {
                /* To recognize a valid party poker game window
                  * ex. Turbo #2341732 -  NL  Hold'em - €3 Buy-in */
                regex.Add("game_window_title_to_recognize_games", @"^(?<gameDescription>[^-]+) - .+(Buy-in|.[\d\.]+\/.[\d\.]+|NL  Hold'em)");
                
                //

                /* Recognize the Hand History game phases */
                regex.Add("hand_history_begin_preflop_phase_token", @"\*\* Dealing down cards \*\*");
                regex.Add("hand_history_begin_flop_phase_token", @"\*\* Dealing Flop \*\* \[(?<flopCards>[\d\w \,]+)\]");
                regex.Add("hand_history_begin_turn_phase_token", @"\*\* Dealing Turn \*\* \[(?<turnCard>[\d\w \,]+)\]");
                regex.Add("hand_history_begin_river_phase_token", @"\*\* Dealing River \*\* \[(?<riverCard>[\d\w \,]+)\]");
                regex.Add("hand_history_begin_summary_phase_token", @"^( $| Game #[0-9]+ starts\.$)"); // It's not exactly a summary indicator, but it tells us that the hand ended


                /* Recognize the Hand History gameID 
                 Ex. NL Texas Hold'em €3 EUR Buy-in Trny: 61535376 Level: 1  Blinds(20/40) - Friday, June 17, 23:14:29 CEST 2011
                 * NL Texas Hold'em €3 EUR Buy-in Trny: 61534554 Level: 1  Blinds(20/40) - Friday, June 17, 
                 */
                regex.Add("hand_history_game_id_token", @": (?<gameId>[0-9]+)( Level: )?");

                /* Recognize the table ID and max seating capacity (if available) 
                 ex. Table Table  185503 (Real Money) */
                regex.Add("hand_history_table_token", @"Table .+(?<tableId>[0-9]+) \(");

                /* Recognize the maximum number of seats available */
                regex.Add("hand_history_max_seating_capacity", @"Total number of players : ([\d]+)/(?<tableSeatingCapacity>[\d]+)");

                /* Recognize game type (Hold'em, Omaha, No-limit, limit, etc.) 
                 * NL Texas Hold'em €3 EUR Buy-in Trny: 61535376 Level: 1  Blinds(20/40) - Friday, June 17, 23:14:29 CEST 2011
                 * NL Texas Hold'em  Trny: 65167142 Level: 1  Blinds(20/40) - Sunday, November 27, 20:30:30 CET 2011
                 * €2 EUR NL Texas Hold'em - Monday, August 01, 21:27:54 CEST 2011 */
                regex.Add("hand_history_game_token", @"(((?<gameType>.+) (.[\d\.\,]+ [\w]{3} Buy\-in| Trny:))|((\$|€)[\d\.]+ [A-Z]{3} (?<gameType>.+) - ))");

                /* Recognize players 
                 Ex. Seat 1: Renik87 ( 2,000 )
                 * It ignores those who are marked as ", is sitting out"
                 */
                regex.Add("hand_history_detect_player_in_game", @"Seat (?<seatNumber>[\d]+): (?<playerName>.+) .*\( (€|\$)?[\d\.\,]+( [\w]{3})? \)$");

                /* Recognize mucked hands
                 Ex. lucianoasso shows [ As, Js ]high card Ace.*/
                regex.Add("hand_history_detect_mucked_hand", @"(?<playerName>.+) (shows|doesn't show) \[(?<cards>[\d\w \,]+)\]");

                /* Recognize winners of a hand 
                 * Ex. Renik87 wins 2,480 chips */
                regex.Add("hand_history_detect_hand_winner", @"(?<playerName>.+) wins (\$|€)?[\d\.\,]+ chips");

                /* Recognize all-ins
                 * Ex. stallion089 is all-In  [1,440] */
                regex.Add("hand_history_detect_all_in_push", @"(?<playerName>.+) is all-In");

                /* Detect who the button is
                 * Seat 2 is the button */
                regex.Add("hand_history_detect_button", @"Seat (?<seatNumber>[\d]+) is the button");

                /* Detect who our hero is (what's his nickname)
                 * ex. Dealt to stallion089 [  5d 9c ] */
                regex.Add("hand_history_detect_hero_name", @"Dealt to (?<heroName>.+) \[[\w\d ]+\]$");

                /* Detect calls
                 * ex. stallion089 calls [120] */
                regex.Add("hand_history_detect_player_call", @"(?<playerName>.+) calls \[(\$|€)?(?<amount>[\d\.\,]+)( [\w]{3})?\]");

                /* Detect bets
                   ex. stallion089 bets [160] */
                regex.Add("hand_history_detect_player_bet", @"(?<playerName>.+) bets \[(\$|€)?(?<amount>[\d\.\,]+)( [\w]{3})?\]");

                /* Detect folds
                 * ex. stallion089 folds */
                regex.Add("hand_history_detect_player_fold", @"(?<playerName>.+) folds");

                /* Detect checks
                 * ex. stallion089 checks */
                regex.Add("hand_history_detect_player_check", @"(?<playerName>.+) checks");

                /* Detect raises 
                 * ex. Renik87 raises [140] */
                regex.Add("hand_history_detect_player_raise", @"(?<playerName>.+) (raises|is all-In)[ ]+\[(\$|€)?(?<raiseAmount>[\d\.\,]+)( [\w]{3})?\]");

                /* Detect blind amounts (tournaments only)
                 * ex. Trny: 61674977 Level: 1 Blinds(20/40) */
                regex.Add("hand_history_blind_amounts", @"Blinds\((?<smallBlindAmount>[\d\.\,]+)\/(?<bigBlindAmount>[\d\.\,]+)\)");

                /* Detect who is the small/big blind (cash games)
                   Ex. stallion089: posts small blind 15 */
                regex.Add("hand_history_detect_small_blind", @"(?<playerName>.+) posts small blind \[(\$|€)?(?<smallBlindAmount>[\d\.\,]+)( [\w]{3})?\]");
                regex.Add("hand_history_detect_big_blind", @"(?<playerName>.+) posts big blind \[(\$|€)?(?<bigBlindAmount>[\d\.\,]+)( [\w]{3})?\]");


                /* Recognize end of round character sequence (in PartyPoker it's
                 * a blank line */
                regex.Add("hand_history_detect_end_of_round", @"^$");

                /* Hand history file formats. 
                 * In PartyPoker is standard for all game types (the first part is a description found in the window title)  */     
                config.Add("hand_history_filename_format", @"{0}_[0-9]+");

                /* Game description (as shown in the hand history) */
                regex.Add("game_description_holdem", "(NL Texas Hold'em|FL Texas Hold'em)");

            }

            /* Number of sequences required to raise the OnRoundHasTerminated event.
             * This refers to the hand_history_detect_end_of_round regex, on PartyPoker
             * a round is over after 1 blank line. */
            config.Add("hand_history_end_of_round_number_of_tokens_required", 1);

        }

        /* Given a game description, returns the corresponding PokerGame */
        public override PokerGame GetPokerGameFromGameDescription(string gameDescription)
        {
            Trace.WriteLine("Found game description: " + gameDescription);

            Match match = GetRegex("game_description_holdem").Match(gameDescription);
            if (match.Success)
            {
                return PokerGame.Holdem;
            }

            return PokerGame.Unknown; //Default
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

            /* On Party Poker, we have subdirectories within the hand history directory. 
               ex. stallion089\20110617\{TODAY'S FILES} (2011 = year, 06 = month, 17 = day) 
               Additionally, each filename is composed by a first part which reflects the window title
               and a second part that has some extra information (which we do not care about)
               */

            Regex regex = GetRegex("game_window_title_to_recognize_games");
            Match match = regex.Match(windowTitle);
            if (match.Success)
            {
                // We matched a game window
                // gameDescription = Turbo #2341732
                // gameType = No Limit Hold'em
                String gameDescription = match.Groups["gameDescription"].Value;

                // Convert the characters to regex-valid format
                gameDescription = StringToRegexPattern(gameDescription);

                // output: Turbo #2341732_[0-9]+
                return String.Format(GetConfigString("hand_history_filename_format"), gameDescription);
            }
            else
            {
                return String.Empty; //Could not find any valid match... must be a title we're not interested into
            }
        }

        public override String GetCurrentHandHistorySubdirectory()
        {
            // Party poker organizes hand history files in directories by day
            // Format: 20110617\{TODAY'S FILES} (2011 = year, 06 = month, 17 = day) 
            
            // TODO: save timezone information somewhere? (If you play in a different country you can specify manually the server's timezone)
            //DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"));
            
            // Uncomment in production (works for 99% of the users)
            DateTime now = DateTime.Now;

            return String.Format(@"{0}{1}{2}", now.Year.ToString("D4"), now.Month.ToString("D2"), now.Day.ToString("D2"));
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
                return "Party Poker";
            }
        }

        public override string XmlName
        {
            get {
                return "Party_Poker";
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
