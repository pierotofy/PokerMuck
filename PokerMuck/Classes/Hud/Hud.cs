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
        private HudUserSettings settings;

        /* Reference to the table who owns this hud */
        private Table table;

        /* Windows List */
        private HudWindowsList windowsList;

        public Hud(Table table)
        {
            this.table = table;
            this.settings = new HudUserSettings();
        }

        public void DisplayAndUpdate(){
            bool setupInitialWindowPositions = false; //Default

            // Have we ever took care of this table?
            if (windowsList == null)
            {
                // First timer, we need to create the appropriate window list
                windowsList = new HudWindowsList();

           }

            // Do we have user specified positions?
            List<Point> positions = settings.RetrieveHudWindowPositions(table.PokerClientName, table.MaxSeatingCapacity);
            if (positions.Count > 0)
            {
                Debug.Assert(positions.Count == table.MaxSeatingCapacity, "The number of available user defined hud positions is different that what we expected");
            }
            else
            {
                // We don't 

                // The first time we'll need to setup the initial position of the windows 
                setupInitialWindowPositions = true;
            }

            // Now we're ready

            /* Check each player:
             * 1. If the player doesn't have a hud window and is playing, create a window for him
             * 2. If the player has a window and is not playing, we need to remove the window from him
             */
            foreach (Player p in table.PlayerList)
            {
                // Create window
                if (p.HudWindow == null && p.IsPlaying)
                {
                    HudWindow window = HudWindowFactory.CreateHudWindow(table);

                    // We set a 1:1 association between the player and the HudWindow
                    p.HudWindow = window;

                    windowsList.Add(window);
                    window.Show(); // Without this we cannot move the windows
                    
                    // Move it to its proper location (if available)
                    if (positions.Count > 0)
                    {
                        window.SetAbsolutePosition(positions[p.SeatNumber - 1], table.WindowRect);
                    }
                }
                else
                {
                    // If he's not playing and he has a window, we need to remove it
                    if (p.HudWindow != null && !p.IsPlaying)
                    {
                        Debug.Print("Remove! " + p.Name);

                        windowsList.Remove(p.HudWindow);
                        p.HudWindow = null;
                    }
                }
            }

            if (setupInitialWindowPositions) SetupHudInitialPosition();

            UpdateHudData();

            table.PostHudDisplayAction();
        }

        /* If the window of a table has been moved around, we need to shift the 
         * hud windows associated with it */
        public void Shift()
        {
            // Shift!
            windowsList.ShiftWindowPositions(table.WindowRect);       
        }

        /* We can remove a hud (the game is over?) or we're closing the application? */
        public void RemoveHud()
        {
            StoreHudWindowPositions();

            windowsList.RemoveAll();
        }

        /* Save the position of the hud windows associated with this table */
        private void StoreHudWindowPositions()
        {
            /* Foreach player, we need to find the position of the associated window
             * Note that if a seat is empty (ex. table with 9 seats and 3 players)
             * we'll use the value that was previously stored in the configuration */
            List<Point> positions = settings.RetrieveHudWindowPositions(table.PokerClientName, table.MaxSeatingCapacity);

            // If the configuration didn't return us any result, we set dummy points
            // This should occur on the first time
            if (positions.Count == 0)
            {
                positions = new List<Point>(table.MaxSeatingCapacity);
                for (int i = 0; i < table.MaxSeatingCapacity; i++)
                {
                    positions.Add(new Point(0, 0));
                }
            }

            foreach (Player p in table.PlayerList)
            {
                if (p.HudWindow != null)
                {
                    positions[p.SeatNumber - 1] = p.HudWindow.GetRelativePosition(table.WindowRect);
                }
            }

            // Finally, store the new positions in the settings!
            settings.StoreHudWindowPositions(table.PokerClientName, positions);
        }

        /* Updates the information displayed by the hud associated with it */
        private void UpdateHudData()
        {
            foreach (Player p in table.PlayerList)
            {
                if (p.HudWindow != null)
                {
                    p.HudWindow.DisplayPlayerName(p.Name);
                    p.HudWindow.DisplayStatistics(p.GetStatistics());
                }
                else
                {
                    // Should never happen
                    Debug.Print("A command to update the hub data has been received, but no window is associated with this player: " + p.Name + ". Did we remove it?");
                }
            }
        }

        // Call this when you're done with the hud
        
        // Cleanup
        ~Hud()
        {
            StoreHudWindowPositions();

            // Commit to file
            settings.Save();
        }


        /* Tries to setup the position of the window huds in the best way possible */
        private void SetupHudInitialPosition()
        {
            Debug.Assert(windowsList != null, "Tried to rearrange the hud position for a windows list that doesn't exists");

            // No, an empty set was returned... initialize defaults
            windowsList.SetupDefaultPositions(table.WindowRect);

            // And save these values!
            StoreHudWindowPositions();
        }
    }
}
