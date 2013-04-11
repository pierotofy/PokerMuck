using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace PokerMuck
{
    class PokerStars : PokerClient
    {
        public PokerStars(){
            // Other init stuff?
        }

        public PokerStars(String language)
        {
            InitializeLanguage(language);
        }

        protected override void InitializeData()
        {
            if (CurrentLanguage == "English")
            {
                /* To recognize the Game ID given the table window (including any prefixes such as T for tournament)
                * ex. T1234567990 from €2.60+€0.40 EUR Hold'em No Limit [Heads-up Turbo] - Tournament 1234567990 - 1 on 1 - ...
                  ex. California X - €0.01/€0.02 EUR - No Limit Hold'em [AAMS ID: M2BCE3000C59C5PF] */
                regex.Add("game_window_title_to_recognize_tournament_game_id", @"]? - (?<tournamentId>[^ ]+ [0-9]+) [^$]+$");
                regex.Add("game_window_title_to_recognize_play_money_game_description", @"(?<gameDescription>[^-]+)-[^-]+ Play Money");
                regex.Add("game_window_title_to_recognize_cash_game_description", @"(?<gameDescription>.+) - .[\d\.]+\/.[\d\.]+ [\w]{3} - ");


                /* Recognize the Hand History game phases */
                regex.Add("hand_history_begin_preflop_phase_token", @"\*\*\* HOLE CARDS \*\*\*");
                regex.Add("hand_history_begin_flop_phase_token", @"\*\*\* FLOP \*\*\* \[(?<flopCards>[\d\w ]+)\]");
                regex.Add("hand_history_begin_turn_phase_token", @"\*\*\* TURN \*\*\* \[([\d\w ]+)\] \[(?<turnCard>[\d\w ]+)\]");
                regex.Add("hand_history_begin_river_phase_token", @"\*\*\* RIVER \*\*\* \[([\d\w ]+)\] \[(?<riverCard>[\d\w ]+)\]");
                regex.Add("hand_history_begin_showdown_phase_token", @"\*\*\* SHOW DOWN \*\*\*");
                regex.Add("hand_history_begin_summary_phase_token", @"\*\*\* SUMMARY \*\*\*");


                /* Recognize the Hand History gameID 
                 Ex. PokerStars Game #59534069543: Tournament #377151618
                 */
                regex.Add("hand_history_game_id_token", @"PokerStars (Game|Hand) #(?<handId>[\d]+): (Tournament #(?<gameId>[\d]+)|(?<gameId>.+ \(.[\d\.]+\/.[\d\.]+\)))");
               
                /* Recognize the table ID and max seating capacity */
                regex.Add("hand_history_table_token", @"Table '(?<tableId>.+)' (?<tableSeatingCapacity>[\d]+)-max");

                /* Recognize game (Hold'em, Omaha, No-limit, limit, etc.) 
                   Note that for PokerStars.it the only valid currency is EUR (and FPP), but this might be different 
                   on other clients. This regex works for both play money and tournaments
                 * ex. PokerStars Game #66066115721:  Hold'em No Limit (5/10) - 2011/08/16 0:15:57 CET [2011/08/15 18:15:57 ET]
                 *     PokerStars Hand #73955493608:  Hold'em No Limit (25/50) - 2012/01/16 19:16:20 CET [2012/01/16 13:16:20 ET]
                 *     PokerStars Game #69014135963: Tournament #455266512, 300+20 Hold'em No Limit - Level I (10/20) - 2011/10/15 20:20:37 EET [2011/10/15 13:20:37 ET]*/
                regex.Add("hand_history_game_token", @"(PokerStars (Game|Hand) #[\d]+: Tournament #[\d]+, .?[\d\.]+\+.?[\d\.]+ (?<gameType>[^-]+) -)|([\d]+FPP (?<gameType>[^-]+) -)|((EUR|USD) (?<gameType>[^-]+) -)|(PokerStars (Game|Hand) #[\d]+:  (?<gameType>[^(]+) \(.?[\d\.]+\/.?[\d\.]+( [\w]{3})?\))");

                /* Recognize players 
                 Ex. Seat 1: stallion089 (2105 in chips) => 1,"stallion089" 
                 * It ignores those who are marked as "out of hand"
                 */
                regex.Add("hand_history_detect_player_in_game", @"Seat (?<seatNumber>[\d]+): (?<playerName>.+) .*\(.?[\d\.]+ in chips\) (?!out of hand)");

                /* Recognize mucked hands
                 Ex. Seat 1: stallion089 (button) (small blind) mucked [5d 5s]
                 Or: piedesa: shows [Td 6d]*/
                // TODO! What if a player has a "(" in his nickname?
                regex.Add("hand_history_detect_mucked_hand", @"((Seat [\d]+: (?<playerName>[^(]+) .*(showed|mucked) \[(?<cards>[\d\w ]+)\])|(?<playerName>.+): shows \[(?<cards>[\d\w ]+)\])");

                /* Recognize winners of a hand 
                 * Ex. cord80 collected 40 from pot */
                regex.Add("hand_history_detect_hand_winner", @"(?<playerName>.+) collected .?[\d\.]+ from pot");

                /* Recognize all-ins
                 * Ex. stallion089: raises 605 to 875 and is all-in */
                regex.Add("hand_history_detect_all_in_push", @"(?<playerName>[^:]+): .+ is all-in");

                /* Recognize the final board */
                regex.Add("hand_history_detect_final_board", @"Board \[(?<cards>[\d\w ]+)\]");

                /* Detect who is the small/big blind
                   Ex. stallion089: posts small blind 15 */
                regex.Add("hand_history_detect_small_blind", @"(?<playerName>[^:]+): posts small blind .?(?<smallBlindAmount>[\d\.]+)");
                regex.Add("hand_history_detect_big_blind", @"(?<playerName>[^:]+): posts big blind .?(?<bigBlindAmount>[\d\.]+)");

                /* Detect who the button is */
                regex.Add("hand_history_detect_button", @"#(?<seatNumber>[\d]+) is the button");

                /* Detect who our hero is (what's his nickname) */
                regex.Add("hand_history_detect_hero_name", @"Dealt to (?<heroName>.+) \[[\w\d ]+\]$");

                /* Detect calls
                 * ex. stallion089: calls 10 */
                regex.Add("hand_history_detect_player_call", @"(?<playerName>[^:]+): calls .?(?<amount>[\d\.]+)");

                /* Detect bets 
                   ex. stallion089: bets 20 */
                regex.Add("hand_history_detect_player_bet", @"(?<playerName>[^:]+): bets .?(?<amount>[\d\.]+)");

                /* Detect folds
                 * ex. preferiti90: folds */
                regex.Add("hand_history_detect_player_fold", @"(?<playerName>[^:]+): folds");

                /* Detect checks
                 * ex. DOTTORE169: checks */
                regex.Add("hand_history_detect_player_check", @"(?<playerName>[^:]+): checks");

                /* Detect raises 
                 * ex. zanzara za: raises 755 to 1155 and is all-in */
                regex.Add("hand_history_detect_player_raise", @"(?<playerName>[^:]+): raises .?([\d\.]+) to .?(?<raiseAmount>[\d\.]+)");

                /* Recognize end of round character sequence (in PokerStars.it it's
                 * a blank line */
                regex.Add("hand_history_detect_end_of_round", @"^$");

                /* Hand history file format. Example: HH20111216 T123456789 ... .txt */
                config.Add("hand_history_tournament_filename_format", "HH[0-9]+ {0}{1}");
                config.Add("hand_history_play_and_real_money_filename_format", "HH[0-9]+ {0}");

                /* Game description (as shown in the hand history) */
                regex.Add("game_description_holdem", "(Hold'em No Limit|Hold'em Limit)");

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
            if (windowTitle == "test3.txt - Notepad") return "test3.txt";
            if (windowTitle == "test4.txt - Notepad") return "test4.txt";
            if (windowTitle == "test5.txt - Notepad") return "test5.txt";


            /* Tricky, title format is significantly different for tournaments and play money on PokerStars.it
             * so we need to make two checks */
            Regex regex = GetRegex("game_window_title_to_recognize_tournament_game_id");
            Match match = regex.Match(windowTitle);
            if (match.Success)
            {
                // We matched a tournament game window
                // tournamentID = Tournament 123456789, we need T12345689
                String tournamentID = match.Groups["tournamentId"].Value;
                String[] parts = tournamentID.Split(' ');

                String prefix = parts[0].Substring(0,1); //T
                String gameID = parts[1];

                return String.Format(GetConfigString("hand_history_tournament_filename_format"), prefix, gameID);
            }
            else
            {
                // No luck, try with play money
                regex = GetRegex("game_window_title_to_recognize_play_money_game_description");
                match = regex.Match(windowTitle);
                if (match.Success)
                {
                    string gameDescription = match.Groups["gameDescription"].Value;
                    
                    // We matched a play money game window, need to convert the description into a filename friendly format
                    return String.Format(GetConfigString("hand_history_play_and_real_money_filename_format"), gameDescription);
                }
                else
                {

                    // No luck again, try with cash games
                    regex = GetRegex("game_window_title_to_recognize_cash_game_description");
                    match = regex.Match(windowTitle);
                    if (match.Success)
                    {
                        string gameDescription = match.Groups["gameDescription"].Value;

                        // We matched a real money game window, need to convert the description into a filename friendly format
                        return String.Format(GetConfigString("hand_history_play_and_real_money_filename_format"), gameDescription);
                    }
                    else
                    {
                        return String.Empty; //Could not find any valid match... must be a title we're not interested into
                    }
                }
            }
        }

        public override String GetCurrentHandHistorySubdirectory()
        {
            return String.Empty; //Not necessary for PokerStars
        }

        public override bool IsPlayerSeatingPositionRelative(PokerGameType gameType)
        {
            return false;
        }

        public override PokerGameType GetPokerGameTypeFromWindowTitle(string windowTitle)
        {
            return PokerGameType.Unknown; // It doesn't make a difference to know the game type
        }

        public override String Name {
            get
            {
                return "Poker Stars";
            }
        }

        public override string XmlName
        {
            get {
                return "Poker_Stars";
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
            get {
                ArrayList supportedGameModes = new ArrayList();
                supportedGameModes.Add("No Limit Hold'em"); //TODO CHECK
                supportedGameModes.Add("Limit Hold'em");


                return supportedGameModes;
            }
        }

        protected override RegexOptions regexOptions{
            get{
                return RegexOptions.IgnoreCase | RegexOptions.Compiled;
            }
        }
    }
}
