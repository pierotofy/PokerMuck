using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;

namespace PokerMuck
{
    /* This enumerates the types of poker game */
    public enum PokerGame
    {
        Holdem, Omaha, FiveCardStud, Unknown
    }

    public enum PokerGameType
    {
        Tournament, Ring, Unknown
    }

    public abstract class PokerClient
    {
        protected const int DEFAULT_MAX_SEATING_CAPACITY = 9;
        protected const int MAX_SEATING_CAPACITY_HEADS_UP = 2;

        protected Hashtable regex;
        protected Hashtable config;

        private ArrayList supportedVisualRecognitionThemes;
        public ArrayList SupportedVisualRecognitionThemes { get { return supportedVisualRecognitionThemes; } }

        public bool SupportsVisualRecognition { get { return supportedVisualRecognitionThemes.Count > 0; } }

        private String currentLanguage; //Don't allow subclasses to change this, force them to use InitializeLanguage
        public String CurrentLanguage { get { return currentLanguage; } }

        private String currentTheme;
        public String CurrentTheme { get { return currentTheme != null ? currentTheme : ""; } }

        /* Set the current language and calls the initializedata method */
        public void InitializeLanguage(String language)
        {
            Trace.Assert(SupportedLanguages.Contains(language), "A poker client was initialized but it doesn't support the language: " + language);

            // Avoid repeated initializations
            if (currentLanguage != language)
            {
                ResetData();
                currentLanguage = language;
                InitializeData();
            }
        }

        public void SetTheme(String theme)
        {
            if (SupportedVisualRecognitionThemes.Contains(theme)) this.currentTheme = theme;
            else this.currentTheme = "";
        }

        /* Each poker client needs to initialize regex, config and currentLanguage */
        protected abstract void InitializeData();

        public abstract String Name { get; }
        public abstract String XmlName { get; }
            
        public abstract ArrayList SupportedLanguages { get; }
        public abstract ArrayList SupportedGameModes { get; }

        /* The first language in the SupportedLanguages list is the default language */
        public String DefaultLanguage
        {
            get
            {
                Trace.Assert(SupportedLanguages.Count > 0, "There are no languages in this client");
                return (String)SupportedLanguages[0];
            }
        }
        
        protected abstract RegexOptions regexOptions { get; }

        /* Given the title of the game window, this method returns the hand history equivalent filename regex */
        public abstract String GetHandHistoryFilenameRegexPatternFromWindowTitle(String windowTitle);

        /* Simply returns a directory that might be in between the main hand history directory and the hand history files
         * (For example on PartyPoker we have subdirectories in the main directory organized day by day) */
        public abstract String GetCurrentHandHistorySubdirectory();

        /* Given a game description in string format, it returns the corresponding PokerGame */
        public abstract PokerGame GetPokerGameFromGameDescription(String gameDescription);

        /* Given a window title, it returns what kind of game this is */
        public abstract PokerGameType GetPokerGameTypeFromWindowTitle(String windowTitle);

        /* Given the table token line, it tries to infer the maximum number of seats available
         * from the hand_history_table_token line of the hand history
         * (some clients' histories do not explicitly specify this value) 
         * @param line line that matches the hand_history_table_token regex
         * @param filename the name of the hand history file that is getting currently processed
         */
        public virtual int InferMaxSeatingCapacity(String line, String filename)
        {
            Trace.WriteLine("Inferred max seating capacity, but the method is not implemented in derived class.");

            return DEFAULT_MAX_SEATING_CAPACITY;
        }

        /* Certain poker clients display players on screen centered relative to our hero
         * if this is the case, child classes should return true
         * This is needed in order to decide how to store the hud window positions on file */
        public abstract bool IsPlayerSeatingPositionRelative(PokerGameType gameType);

        /* Execute certain operations before a game is to start */
        public virtual void DoPregameProcessing(String storedHandHistoryDirectory){}

        /* Execute certain operations when the application starts */
        public virtual void DoStartupProcessing(String storedHandHistoryDirectory){}

        /* Given a string representing a card, returns the equivalent card object 
           This seems to be a standard format across poker client.
           Cards are represented by two chars, the first indicating the face
         * and the second indicating the suit. Ex. Ks, Ah, etc. */
        public virtual Card GenerateCardFromString(String card)
        {
            // This should never be different than 2
            Trace.Assert(card.Length == 2, "A string representation of a card was found to be of invalid length: " + card.Length + " instead of 2");

            // Uppercase to simplify checks
            String cardValues = card.ToUpper();

            // Extract components
            Char faceComponent = cardValues[0];
            Char suitComponent = cardValues[1];

            CardFace face = Card.CharToCardFace(faceComponent);
            CardSuit suit = Card.CharToCardSuit(suitComponent);

            Card result = new Card(face, suit);
            return result;
        }

        /* Converts an input string into a format compatible with regex */
        protected String StringToRegexPattern(String str)
        {
            // 1. Convert / to -
            str = str.Replace("/", "-");

            // 2. Convert $ to \$
            str = str.Replace("$", @"\$");

            // 3. Convert ( to \( and ) to \)
            str = str.Replace(")", @"\)");
            str = str.Replace("(", @"\(");

            // 4. Convert . to \.
            str = str.Replace(".", @"\.");

            // 5. Convert + to \+
            str = str.Replace("+", @"\+");

            return str;
        }



        /* Default constructor, initializes the regex and config hashtables */
        protected PokerClient()
        {
            regex = new Hashtable();
            config = new Hashtable();
            currentTheme = "";

            LoadSupportedVisualRecognitionThemes();
        }

        /* Themes that have a color map in the appropriate directory */
        private void LoadSupportedVisualRecognitionThemes()
        {
            supportedVisualRecognitionThemes = new ArrayList();
            
            try
            {
                String[] files = System.IO.Directory.GetFiles(Application.StartupPath + @"\\Resources\\ColorMaps\\" + this.Name);

                Regex themeName = new Regex(@"[\w]+_[\d]+-max_(?<theme>[\w]+)\.bmp");

                foreach (String file in files)
                {
                    Match m = themeName.Match(file);
                    if (m.Success){
                        String theme = m.Groups["theme"].Value;

                        Trace.WriteLine("Found valid color map for " + this.Name + ": " + theme);
                        if (!supportedVisualRecognitionThemes.Contains(theme))
                        {
                            supportedVisualRecognitionThemes.Add(theme);
                        }
                    }else{
                        Trace.WriteLine("Detected invalid card map filename format: " + file + ", skipping...");
                    }

                }
            }
            catch (DirectoryNotFoundException)
            {
                Trace.WriteLine("CardMaps directory for " + this.Name + " not found, skipping...");
            }
        }

        public bool HasRegex(String key)
        {
            return regex.ContainsKey(key);
        }

        public Regex GetRegex(String key){
            Trace.Assert(regex.ContainsKey(key),String.Format("The derived PokerClient class does not include the regex key: " + key));
            
            string pattern = (String)regex[key];
            return new Regex(pattern, regexOptions);
        }


        public String GetConfigString(String key)
        {            
            return (String)GetConfig(key);
        }

        public int GetConfigInt(String key)
        {
            return (int)GetConfig(key);
        }

        public float GetConfigFloat(String key)
        {
            return (float)GetConfig(key);
        }

        public Object GetConfig(String key)
        {
            Trace.Assert(config.ContainsKey(key), String.Format("The derived PokerClient class does not include the config key: " + key));

            return config[key];
        }

        public bool ProvidesConfig(String key)
        {
            return config.ContainsKey(key);
        }

        /* This method clears everything in regex and config */
        private void ResetData()
        {
            regex.Clear();
            config.Clear();
        }
    }
}
