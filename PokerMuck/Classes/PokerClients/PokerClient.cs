using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PokerMuck
{
    /* This enumerates the types of poker game */
    public enum PokerGameType
    {
        Holdem, Omaha, FiveCardStud, Unknown
    }

    abstract class PokerClient
    {
        protected Hashtable regex;
        protected Hashtable config;

        private String currentLanguage; //Don't allow subclasses to change this, force them to use InitializeLanguage
        public String CurrentLanguage { get { return currentLanguage; } }

        /* Set the current language and calls the initializedata method */
        public void InitializeLanguage(String language)
        {
            Debug.Assert(SupportedLanguages.Contains(language), "A poker client was initialized but it doesn't support the language: " + language);

            // Avoid repeated initializations
            if (currentLanguage != language)
            {
                ResetData();
                currentLanguage = language;
                InitializeData();
            }
        }
        
        /* Each poker client needs to initialize regex, config and currentLanguage */
        protected abstract void InitializeData();

        public abstract String Name { get; }
        public abstract ArrayList SupportedLanguages { get; }
        public abstract ArrayList SupportedGameModes { get; }
        
        protected abstract RegexOptions regexOptions { get; }

        
        /* Given the title of the game window, this method returns the hand history equivalent filename regex */
        public abstract String GetHandHistoryFilenameRegexPatternFromWindowTitle(String windowTitle);

        /* Given a game description in string format, it returns the corresponding PokerGameType */
        public abstract PokerGameType GetPokerGameTypeFromGameDescription(String gameDescription);
            
        /* Given a string representing a card, returns the equivalent card object */
        public abstract Card GenerateCardFromString(String card);

        /* Default constructor, initializes the regex and config hashtables */
        protected PokerClient()
        {
            regex = new Hashtable();
            config = new Hashtable();

        }

        public Regex GetRegex(String key){
            Debug.Assert(regex.ContainsKey(key),String.Format("The derived PokerClient class does not include the regex key: " + key));
            
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

        public Object GetConfig(String key)
        {
            Debug.Assert(config.ContainsKey(key), String.Format("The derived PokerClient class does not include the config key: " + key));

            return config[key];
        }

        /* This method clears everything in regex and config */
        private void ResetData()
        {
            regex.Clear();
            config.Clear();
        }
    }
}
