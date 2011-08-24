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
    public class Hud
    {
        private HudUserSettings settings;

        /* Reference to the table who owns this hud */
        private Table table;

        /* Windows List */
        private HudWindowsList windowsList;

        /* Visible */
        private bool visible;
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                this.visible = value;
                if (visible)
                {
                    windowsList.Show();
                }
                else
                {
                    windowsList.Hide();
                }
            }
        }

        public Hud(Table table)
        {
            this.table = table;
            this.settings = new HudUserSettings();
            this.windowsList = new HudWindowsList();
            this.visible = true;
        }

        public void DisplayAndUpdate(){
            bool setupInitialWindowPositions = false; //Default

            // Do we have user specified positions?
            List<Point> positions = settings.RetrieveHudWindowPositions(table.PokerClientName, table.MaxSeatingCapacity, table.GameType);
            if (positions.Count > 0)
            {
                if (table.PlayerSeatingPositionIsRelative)
                {
                    positions = GetEffectivePositions(positions, table.PlayerList, table.CurrentHeroName, table.MaxSeatingCapacity);
                }
                Trace.Assert(positions.Count == table.MaxSeatingCapacity, "The number of available user defined hud positions is different that what we expected");
            }
            else
            {
                // We don't 

                // The first time we'll need to setup the initial position of the windows 
                setupInitialWindowPositions = true;
            }

            // Check for windows that have been set to be destroyed
            DisposeFlaggedWindows();

            // Now we're ready

            /* Check each player:
             * If the player doesn't have a hud window and is playing, create a window for him
             */
            foreach (Player p in table.PlayerList)
            {
                // Create window
                if (p.HudWindow == null && p.IsPlaying)
                {
                    HudWindow window = HudWindowFactory.CreateHudWindow(table);

                    Trace.WriteLine("Create window for " + p.Name + " (" + p.SeatNumber + ")");

                    // We set a 1:1 association between the player and the HudWindow
                    p.HudWindow = window;

                    window.RegisterHandlers(this, table, p);

                    windowsList.Add(window);
                    window.Show(); // Without this we cannot move the windows

                    
                    // Move it to its proper location (if available)
                    if (positions.Count > 0)
                    {
                        window.SetAbsolutePosition(positions[p.SeatNumber - 1], table.WindowRect);
                    }
                }
            }

            if (setupInitialWindowPositions) SetupHudInitialPosition();

            UpdateHudData();

            table.PostHudDisplayAction();
        }

        public void holdemWindow_OnPlayerPreflopPushingRangeNeedToBeDisplayed(HoldemHudWindow sender)
        {
            HoldemPlayer p = (HoldemPlayer)FindPlayerAssociatedWith((HudWindow)sender);
            if (p != null)
            {
                p.DisplayPreflopPushingRangeWindow();
            }
        }

        public void window_OnPlayerStatisticsNeedToBeDisplayed(HudWindow sender)
        {
            Player p = FindPlayerAssociatedWith(sender);
            if (p != null)
            {
                table.DisplayWindow.DisplayStatistics(p);
            }
            else
            {
                Trace.WriteLine("A command to display the statistics of a player was received, but I couldn't find the player in the list.");
            }
        }

        private Player FindPlayerAssociatedWith(HudWindow w)
        {
            // Find which player is associated with this hud
            foreach (Player p in table.PlayerList)
            {
                if (p.HudWindow.Equals(w))
                {
                    // Found!
                    return p;
                }
            }

            return null;
        }

        /* Make sure we store the new locations */
        public void window_LocationChanged(object sender, EventArgs e)
        {
            StoreHudWindowPositions();
            settings.Save(); // Commit to file
        }

        /* If the window of a table has been moved around, we need to shift the 
         * hud windows associated with it */
        public void Shift()
        {
            /* Simply move the windows according to their relative position to the table 
             * windowRect */

            List<Point> positions = settings.RetrieveHudWindowPositions(table.PokerClientName, table.MaxSeatingCapacity, table.GameType);
            if (positions.Count > 0)
            {
                if (table.PlayerSeatingPositionIsRelative)
                {
                    positions = GetEffectivePositions(positions, table.PlayerList, table.CurrentHeroName, table.MaxSeatingCapacity);
                }

                foreach (Player p in table.PlayerList)
                {
                    if (p.HudWindow != null) p.HudWindow.SetAbsolutePosition(positions[p.SeatNumber - 1], table.WindowRect);
                }
            }
        }

        /* We can remove a hud (the game is over?) or we're closing the application? */
        public void RemoveHud()
        {
            StoreHudWindowPositions();

            windowsList.RemoveAll();
        }

        /* Checks whether this window is overlapping the view of one of the hud windows
         * it this is the case, then we need to hide that particular hud window */
        public void CheckForWindowOverlay(String windowTitle, Rectangle windowRect)
        {
            // Proceed only if this is not our table window and the hud is visible
            if (windowTitle != table.WindowTitle && Visible)
            {
                windowsList.HideWindowsIntersectingWith(windowRect);
                windowsList.ShowWindowsAwayFrom(windowRect);
            }

            // If the window is the our table window, make sure we are displaying it!
            else if (windowTitle == table.WindowTitle)
            {
                Visible = true;
            }
        }

        /* Save the position of the hud windows associated with this table */
        private void StoreHudWindowPositions()
        {
            Trace.Assert(table.MaxSeatingCapacity != 0, "Table max seating capacity is unknown, impossible to display hud.");

            /* Foreach player, we need to find the position of the associated window
             * Note that if a seat is empty (ex. table with 9 seats and 3 players)
             * we'll use the value that was previously stored in the configuration */
            List<Point> positions = settings.RetrieveHudWindowPositions(table.PokerClientName, table.MaxSeatingCapacity, table.GameType);
            if (table.PlayerSeatingPositionIsRelative)
            {
                positions = GetEffectivePositions(positions, table.PlayerList, table.CurrentHeroName, table.MaxSeatingCapacity);
            }
            
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

            List<Player> playerList = table.PlayerList;
            foreach (Player p in playerList)
            {
                if (p.HudWindow != null)
                {
                    positions[p.SeatNumber - 1] = p.HudWindow.GetRelativePosition(table.WindowRect);
                }
            }

            if (table.PlayerSeatingPositionIsRelative)
            {
                positions = GetTransposedPositions(positions, playerList, table.CurrentHeroName, table.MaxSeatingCapacity);
            }

            // Finally, store the new positions in the settings!
            settings.StoreHudWindowPositions(table.PokerClientName, positions, table.GameType);
        }

        /* @param userID nickname of the player that the list of players is to be moved around
         *      the player with that nickname will gain seat #1 and all the other players positions will be assigned
         *      a seat # relative to him */
        private List<Point> GetTransposedPositions(List<Point> effectivePositions, List<Player> playerList, String userID, int maxSeatingCapacity)
        {
            // If there are no players, we can't do any transposition
            if (playerList.Count == 0 || effectivePositions.Count == 0) return effectivePositions;

            List<Point> result = new List<Point>(effectivePositions.Count);
            foreach (Point p in effectivePositions)
            {
                result.Add(p);
            }
            int heroSeat = FindHeroSeat(playerList, userID);
            
            // Reassign seat numbers
            foreach (Player p in playerList)
            {
                int actualSeat = GetRelativeSeat(p.SeatNumber, heroSeat, maxSeatingCapacity);
                Point originalPoint = effectivePositions[p.SeatNumber - 1];
                result[actualSeat - 1] = originalPoint;
            }

            return result;
        }

        /* Given a list of transposed positions, it returns the effective positions
         * @param userID nickname of the user that is at the center (same as indicated in GetTransposedPlayerList) */
        private List<Point> GetEffectivePositions(List<Point> transposedPositions, List<Player> playerList, String heroName, int maxSeatingCapacity)
        {
            // If there are no players, we can't do any transposition
            if (playerList.Count == 0 || transposedPositions.Count == 0) return transposedPositions;

            List<Point> result = new List<Point>(transposedPositions.Count);
            foreach (Point p in transposedPositions)
            {
                result.Add(p);
            }

            int heroSeat = FindHeroSeat(playerList, heroName);

            // Transpose points
            foreach (Player p in playerList)
            {
                int actualSeat = GetRelativeSeat(p.SeatNumber, heroSeat, maxSeatingCapacity);
                Point originalPoint = transposedPositions[actualSeat - 1];
                result[p.SeatNumber - 1] = originalPoint;
            }

            return result;
        }

        /* Returns the seat number of a player relative to hero */
        private int GetRelativeSeat(int playerSeat, int heroSeat, int maxSeatingCapacity)
        {
            if (playerSeat == heroSeat)
            {
                return 1;
            }
            else if (playerSeat < heroSeat)
            {
                return maxSeatingCapacity - (heroSeat - 1) + playerSeat;
            }
            else if (playerSeat > heroSeat)
            {
                return playerSeat - heroSeat + 1;
            }

            return -1; // Never to be executed
        }

        private int FindHeroSeat(List<Player> playerList, String userID)
        {
            int heroSeat = -1;

            // Find where the user is seating
            foreach (Player p in playerList)
            {
                if (p.Name == userID)
                {
                    heroSeat = p.SeatNumber;
                    break;
                }
            }
            Trace.Assert(heroSeat != -1, "Failed to find hero seat, seat number was not found");

            return heroSeat;
        }

        private void DisposeFlaggedWindows()
        {
            windowsList.RemoveFlaggedWindows();
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
                    Trace.WriteLine("A command to update the hub data has been received, but no window is associated with this player: " + p.Name + ". Did we remove it?");
                }
            }
        }

        // Call this when you're done with the hud
        
        // Cleanup
        ~Hud()
        {
            // Commit to file
            settings.Save();
        }


        /* Tries to setup the position of the window huds in the best way possible */
        private void SetupHudInitialPosition()
        {
            Trace.Assert(windowsList != null, "Tried to rearrange the hud position for a windows list that doesn't exists");

            // No, an empty set was returned... initialize defaults
            windowsList.SetupDefaultPositions(table.WindowRect);

            // And save these values!
            StoreHudWindowPositions();
        }
    }
}
