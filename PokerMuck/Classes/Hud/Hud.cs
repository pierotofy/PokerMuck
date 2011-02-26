using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

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
        Hashtable tableHuds;
        PokerClient client;

        public Hud(PokerClient client)
        {
            this.client = client;
            tableHuds = new Hashtable();
        }

        public void DisplayTable(Table t){
            // Have we ever took care of this table?
            if (tableHuds.ContainsKey(t))
            {
                // Simply update
                UpdateHudData(t);
            }
            else
            {
                // First timer, we need to create the appropriate windows
                HudWindowsList windowsList = new HudWindowsList(client);

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
                tableHuds[t] = windowsList;

                SetupHudInitialPosition(t);
                UpdateHudData(t);
            }
            
        }

        /* Updates the information displayed by the hud associated with t */
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
        public void SetupHudInitialPosition(Table t) // TODO put private!!!
        {
            HudWindowsList windowsList = (HudWindowsList)tableHuds[t];
            Debug.Assert(windowsList != null, "Tried to rearrange the hud position for a windows list that doesn't exists");

            windowsList.SetupInitialPositions(t.WindowRect);
        }
    }
}
