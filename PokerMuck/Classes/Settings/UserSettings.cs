/* Based on the UserProfile and UserSettings class by Giovanni Casinelli from 3D Blackjack Trainer */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace PokerMuck
{
    /* This class holds the configuration of the software */
    public class UserSettings
    {
        // Directory where all application data can be memorized
        private String applicationDataPath;
        public String ApplicationDataPath { get { return applicationDataPath; } }
        
        // Directory where all settings of the application are stored
        public String SettingsPath { get { return ApplicationDataPath + @"\Settings\"; } }

        // Hashtable containing the settings
        private Hashtable settingsTable;

        public UserSettings(String programName)
        {
            Trace.Assert(programName != String.Empty,"programName cannot be empty when initializing a UserSettings object");

            applicationDataPath = Environment.GetEnvironmentVariable("APPDATA") + @"\" + programName;
            settingsTable = new Hashtable();

            // Make sure the path we are working with exists
            CreateDirectoryIfNonExistent(applicationDataPath);
            CreateDirectoryIfNonExistent(SettingsPath);

            // Make sure we have a file to work with
            CreateFileIfNonExistent(GetSettingsFilePath());

            // Load the settings
            LoadSettingsFromFile();
        }

        /* This is where you can initialize the default values for the configuration */
        protected virtual void InitializeDefaultValues()
        {

        }

        private void LoadSettingsFromFile()
        {
            //If the profile is not empty
            if (File.ReadAllText(GetSettingsFilePath()).Length > 0)
            {
                //Add properties to the hashtable from the file
                XmlTextReader xmlReader = new XmlTextReader(GetSettingsFilePath());
                Object lastElement = null;
                //Skip the first Element (profile)
                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            lastElement = xmlReader.Name;
                            break;
                        case XmlNodeType.Text:
                            settingsTable.Add(lastElement, xmlReader.Value);
                            break;
                    }
                }
                //Close the xmlReader
                xmlReader.Close();
            }
            else
            {
                InitializeDefaultValues();
            }
        }


        // Helper method to return the fill path to the settings file
        private String GetSettingsFilePath()
        {
            Trace.Assert(SettingsPath != String.Empty, "SettingsPath is empty");
            return SettingsPath + GetSettingsFilename();
        }

        /* Override this in a subclass to create multiple settings on different files */
        public virtual String GetSettingsFilename()
        {
            return "UserSettings.xml"; //default
        }


        /* Retrieve settings */
        public Object GetSetting(Object key)
        {
            if (settingsTable.ContainsKey(key))
                return settingsTable[key];
            return null;
        }

        /* Version to support int */
        public int GetIntSetting(String key)
        {
            Object res = GetSetting((Object)key);
            if (res != null) return int.Parse(res.ToString());
            else return -1;
        }

        /* Version to support bool */
        public bool GetBoolSetting(String key)
        {
            int res = GetIntSetting(key);
            return res != 0;
        }

        /* Version to support string */
        public String GetStringSetting(String key)
        {
            Object res = GetSetting((Object)key);
            if (res != null) return res.ToString();
            else return String.Empty;
        }

        /* Overloaded version to set String settings */
        public void SetSetting(String key, String value)
        {
            SetSetting((Object)key, (Object)value);
        }

        /* Overloaded version to set int settings */
        public void SetSetting(string key, int value)
        {
            SetSetting((Object)key, (Object)value);
        }

        /* Overloaded to set bool settings */
        public void SetSetting(String key, bool value)
        {
            int number = value ? 1 : 0;
            SetSetting((Object)key, number);
        }

        /* Set settings method 
           Note that no changes to the file will be made
           A call to Save() has to be done before the changes are permanent! */
        protected void SetSetting(Object key, Object value)
        {
            if (settingsTable.ContainsKey(key))
                settingsTable[key] = value;
            else
                settingsTable.Add(key, value);
        }

        //Save the Settings Hashtable on the Settings File
        public void Save()
        {
            //Open the Xml File
            XmlTextWriter xmlWriter = new XmlTextWriter(GetSettingsFilePath(), null);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("settings");
            //Write each key in the file with the relative value
            foreach (Object key in settingsTable.Keys)
            {
                xmlWriter.WriteStartElement(key.ToString());
                xmlWriter.WriteString((String)settingsTable[key].ToString());
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            //Close the Xml File
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        /* Verifies whether a particular setting has been set or not */
        public bool HasSetting(String key)
        {
            return settingsTable.ContainsKey(key);
        }

        /* Two helper methods to help create directories and files */
        private void CreateFileIfNonExistent(String filePath)
        {
            FileInfo info = new FileInfo(filePath);
            if (!info.Exists)
            {
                File.Create(filePath).Close();
                
                // Initialize default config, this is the first execution!
                InitializeDefaultValues();
            }
        }

        private void CreateDirectoryIfNonExistent(String directoryPath)
        {
            DirectoryInfo info = new DirectoryInfo(directoryPath);
            if (!info.Exists)
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

    }
}
