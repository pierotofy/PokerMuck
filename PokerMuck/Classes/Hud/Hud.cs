using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Drawing;

namespace PokerMuck
{
    /* This class takes care of displaying a hud with statistics
     * of a table */
    class Hud
    {
        /* This hash table stored a relation between
         * tables and the hudwindowlist
         * key => value
         * Table => Associated Hud Windows list */
        Hashtable hudWindowsList;
        PokerMuckUserSettings settings;

        public Hud(PokerMuckUserSettings settings)
        {
            this.settings = settings;
            hudWindowsList = new Hashtable();
        }

        public void DisplayTable(Table t){
            // Have we ever took care of this table?
            if (hudWindowsList.ContainsKey(t))
            {
                // Simply update
                UpdateHudData(t);
            }
            else
            {
                // First timer, we need to create the appropriate windows
                HudWindowsList windowsList = new HudWindowsList(settings.CurrentPokerClient);

                // For each player we need a new window
                foreach (Player p in t.PlayerList)
                {
                    HudWindow window = HudWindowFactory.CreateHudWindow(t);

                    // We set a 1:1 association between the player and the HudWindow
                    p.HudWindow = window;

                    windowsList.Add(window);
                    window.Show(); // Without this we cannot move the windows
                }

                // Assign the window list to the hash table (so we have a reference for the future)
                hudWindowsList[t] = windowsList;

                SetupHudInitialPosition(t);
                UpdateHudData(t);
            }
            
        }

        /* If the window of a table has been moved around, we need to shift the 
         * hud windows associated with it */
        public void ShiftHud(Table t)
        {
            HudWindowsList windowsList = (HudWindowsList)hudWindowsList[t];

            // Yes! Shift!
            windowsList.ShiftWindowPositions(t.WindowRect);       
        }

        /* We can remove a hud (the game is over?) or we're closing the application? */
        public void RemoveHud(Table t)
        {
            // TODO!

            SaveHudWindowPositions(t);
        }

        /* Save the position of the hud windows associated with this table */
        private void SaveHudWindowPositions(Table t)
        {
            HudWindowsList windowList = (HudWindowsList)hudWindowsList[t];

            // Retrieve the positions of the windows
            List<Point> positions = windowList.RetrieveWindowPositions(t.WindowRect);

            // Finally, store the new positions in the settings!
            settings.SetHudWindowPositions(settings.CurrentPokerClient, positions);
        }

        /* Updates the information displayed by the hud associated with it */
        private void UpdateHudData(Table t)
        {
            foreach (Player p in t.PlayerList)
            {
                if (p.HudWindow != null)
                {
                    p.HudWindow.DisplayPlayerName(p.Name);
                    p.HudWindow.DisplayStatistics(p.GetStatistics());                 
                }
                else
                {
                    // Should never happen
                    Debug.Print("A command to update the hub data has been received, but no window is associated with this player: " + p.Name);
                }
            }
        }


        /* Tries to setup the position of the window huds in the best way possible */
        private void SetupHudInitialPosition(Table t)
        {
            HudWindowsList windowsList = (HudWindowsList)hudWindowsList[t];
            Debug.Assert(windowsList != null, "Tried to rearrange the hud position for a windows list that doesn't exists");

            // Do we have user preferences?
            List<Point> positions = settings.GeHudWindowPositions(settings.CurrentPokerClient, windowsList.Count);
            if (positions.Count > 0)
            {
                // Yes! Load these!
                windowsList.SetupWindowPositions(positions, t.WindowRect);
            }
            else
            {
                // No, an empty set was returned... initialize defaults
                windowsList.SetupDefaultPositions(t.WindowRect);

                // And save these values!
                SaveHudWindowPositions(t);
            }

        }

        /* This has to be called when you're done with the hud */
        public void Terminate()
        {
            // Iterate through each table and remove its hud
            foreach (Table t in hudWindowsList.Keys)
            {
                RemoveHud(t);
            }
        }
    }
}
