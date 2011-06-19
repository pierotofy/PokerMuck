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
        protected const int DEFAULT_MAX_SEATING_CAPACITY = 9;
        protected const int MAX_SEATING_CAPACITY_HEADS_UP = 2;

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
        public abstract String XmlName { get; }
            
        public abstract ArrayList SupportedLanguages { get; }
        public abstract ArrayList SupportedGameModes { get; }

        /* The first language in the SupportedLanguages list is the default language */
        public String DefaultLanguage
        {
            get
            {
                Debug.Assert(SupportedLanguages.Count > 0, "There are no languages in this client");
                return (String)SupportedLanguages[0];
            }
        }
        
        protected abstract RegexOptions regexOptions { get; }

        /* Given the title of the game window, this method returns the hand history equivalent filename regex */
        public abstract String GetHandHistoryFilenameRegexPatternFromWindowTitle(String windowTitle);

        /* Simply returns a directory that might be in between the main hand history directory and the hand history files
         * (For example on PartyPoker we have subdirectories in the main directory organized day by day) */
        public abstract String GetCurrentHandHistorySubdirectory();

        /* Given a game description in string format, it returns the corresponding PokerGameType */
        public abstract PokerGameType GetPokerGameTypeFromGameDescription(String gameDescription);

        /* Given the table token line, it tries to infer the maximum number of seats available
         * (some clients' histories do not explicitly specify this value) */
        public abstract int InferMaxSeatingCapacity(String line);

        /* Given a string representing a card, returns the equivalent card object 
           This seems to be a standard format across poker client.
           Cards are represented by two chars, the first indicating the face
         * and the second indicating the suit. Ex. Ks, Ah, etc. */
        public virtual Card GenerateCardFromString(String card)
        {
            // This should never be different than 2
            Debug.Assert(card.Length == 2, "A string representation of a card was found to be of invalid length: " + card.Length + " instead of 2");

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

        public float GetConfigFloat(String key)
        {
            return (float)GetConfig(key);
        }

        public Object GetConfig(String key)
        {
            Debug.Assert(config.ContainsKey(key), String.Format("The derived PokerClient class does not include the config key: " + key));

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
