using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace PokerMuck
{
    /* In this class are stored the hud window instances
     * for a specific table */
    class HudWindowsList
    {
        /* This is where the instances of the hud windows are stored */
        List<HudWindow> windowsList;

        /* What client are we using */
        PokerClient client;

        /* The last windowRect that we have been using for our positioning
         * This is useful when we have to shift the huds around, keeping a reference
         * to our previous windowRect allows us to compute the offset (X, Y) that the windows
         * have to be moved by */
        Rectangle previousWindowRect;

        /* This variable holds the maximum number of windows that we have been dealing with
         * with this particular window list */
        private int maxNumberOfWindowsManaged;
        public int MaxNumberOfWindowsManaged;

        public HudWindowsList(PokerClient client)
        {
            this.client = client;
            this.previousWindowRect = new Rectangle();
            this.maxNumberOfWindowsManaged = 0;
            windowsList = new List<HudWindow>(9);
        }

        public void Add(HudWindow window)
        {
            windowsList.Add(window);

            // Save the new max
            if (windowsList.Count > maxNumberOfWindowsManaged) maxNumberOfWindowsManaged = windowsList.Count;
        }

        public void Remove(HudWindow window)
        {
            Debug.Assert(window != null, "Trying to remove a null reference to a hud window");
            window.Close();
            windowsList.Remove(window);
        }

        /* Hides all windows hud instances in the collection */
        public void Hide()
        {
            foreach (HudWindow w in windowsList)
            {
                w.Hide();
            }
        }

        /* Shows all windows hud instances in the collection */
        public void Show()
        {
            foreach (HudWindow w in windowsList)
            {
                w.Show();
            }
        }

        /* Shifts the windows from the previousWindowRect to a new one */
        public void ShiftWindowPositions(Rectangle windowRect)
        {
            foreach (HudWindow w in windowsList)
            {
                Point offset = new Point(windowRect.X - previousWindowRect.X,
                                        windowRect.Y - previousWindowRect.Y);

                w.Location = new Point(w.Location.X + offset.X, w.Location.Y + offset.Y);
            }

            SaveWindowRect(windowRect);
        }

        /* Retrieve the list of positions of all windows in the collection,
         * relative to another window rectangle */
        public List<Point> RetrieveWindowPositions(Rectangle windowRect)
        {
            SaveWindowRect(windowRect);

            List<Point> result = new List<Point>(windowsList.Count);

            foreach (HudWindow w in windowsList)
            {
                result.Add(new Point(w.Location.X - windowRect.X, w.Location.Y - windowRect.Y));
            }

            return result;
        }

        /* Given a list of points, it moves the hud windows according to those 
        * points (relative to the windowRect) */
        public void SetupWindowPositions(List<Point> positions, Rectangle windowRect)
        {
            SaveWindowRect(windowRect);

            Debug.Assert(positions.Count >= windowsList.Count, "The number of positions available is less than number of hud windows!");

            int i = 0;

            // In order
            foreach (Point relativePosition in positions)
            {
                Point absolutePosition = new Point(relativePosition.X + windowRect.X, relativePosition.Y + windowRect.Y);

                HudWindow w = windowsList[i];
                w.Location = absolutePosition;

                i++;
            }
        }

        /* Returns the number of windows in the list */
        public int Count { get { return windowsList.Count; } }

        /* Setup the initial default position of the windows hud instances 
         * The guess is often not perfect, but the user can adjust the position 
         * later. */
        public void SetupDefaultPositions(Rectangle windowRect)
        {
            SaveWindowRect(windowRect);

            Point relativeTableCenter = FindRelativeTableCenter(windowRect.Width);
            Point absoluteTableCenter = new Point(windowRect.X + relativeTableCenter.X,
                                                  windowRect.Y + relativeTableCenter.Y);
                                                   
            int numberOfWindows = windowsList.Count;
            float skipAngle = FindSkipAngle(numberOfWindows);
            float angleBetweenPlayers = client.GetConfigFloat("hud_distance_between_players_angle");

            int distanceFromCenterX = GetDistanceXFromCenter(relativeTableCenter);
            int distanceFromCenterY = GetDistanceYFromCenter(relativeTableCenter);
            
            /* Here's how we setup the initial position */

            // 1. Set current angle at -PI/2 (top of the clock) + skip angle (skip one!)
            float currentAngle = -(float)Math.PI / 2 + skipAngle;

            // 2. Draw lower half of windows and increment by angleBetweenPlayers
            int lowerHalf = (int)Math.Floor((double)numberOfWindows / 2);

            int i; // We'll use this later too
            for (i = 0; i < lowerHalf; i++)
            {
                HudWindow w = windowsList[i];
                Point newPosition = CalculatePoint(currentAngle, distanceFromCenterX, distanceFromCenterY, absoluteTableCenter);
                SetWindowLocation(w, newPosition);

                currentAngle += angleBetweenPlayers;
            }
 

            // 3. (optional) If we had an odd number of windows, this is where middle windows gets placed
            if (numberOfWindows % 2 == 1)
            {                
                float sixClockAngle = (float)Math.PI / 2; // 6 o'clock angle
                HudWindow w = windowsList[i];
                Point newPosition = CalculatePoint(sixClockAngle, distanceFromCenterX, distanceFromCenterY, absoluteTableCenter);
                SetWindowLocation(w, newPosition);
                i++;
            } 

            // 4. Draw the remaining half, we start at the top of the clock again, 
            // but this time we go backwards (from last to first and counter clockwise) 
            
            currentAngle = -(float)Math.PI / 2 - skipAngle;
            int remainingWindows = numberOfWindows - i;

            for (int j = 0; j < remainingWindows; j++)
            {
                HudWindow w = windowsList[numberOfWindows - 1 - j];
                Point newPosition = CalculatePoint(currentAngle, distanceFromCenterX, distanceFromCenterY, absoluteTableCenter);
                SetWindowLocation(w, newPosition);

                currentAngle -= angleBetweenPlayers;
            }
        }

        /* Stores a copy of the windowRect */
        private void SaveWindowRect(Rectangle windowRect)
        {
            previousWindowRect = windowRect;
        }

        private Point CalculatePoint(float currentAngle, float distanceFromCenterX, float distanceFromCenterY, Point absoluteCenter)
        {
            /* Trig 101
            * The X distance from center is given by distanceFromCenterX * Cos(angle)
            * The Y distance from center is given by distanceFromCenterY * Sin(angle) */
            int xDistanceFromCenter = (int)((float)distanceFromCenterX * Math.Cos(currentAngle));
            int yDistanceFromCenter = (int)((float)distanceFromCenterY * Math.Sin(currentAngle));

            // So, the new position is:
            Point newPosition = new Point(absoluteCenter.X + xDistanceFromCenter, absoluteCenter.Y + yDistanceFromCenter);
            return newPosition;
        }

        private void SetWindowLocation(HudWindow w, Point location)
        {
            w.Location = location;
        }

        private int GetDistanceXFromCenter(Point center){
            return (int)((float)center.X * client.GetConfigFloat("hud_player_x_distance_from_center_factor_related_to_center_x"));
        }

        private int GetDistanceYFromCenter(Point center)
        {
            return (int)((float)center.Y * client.GetConfigFloat("hud_player_y_distance_from_center_factor_related_to_center_y"));
        }


        /* Find the skip angle to use
         * If it cannot find a perfect match, it returns a default value
         * Sometimes the client might not specify a particular situation, so 
         * we make checks on every value */
        private float FindSkipAngle(int numberOfWindows)
        {
            if (client.ProvidesConfig("hud_10_seats_skip_angle") && numberOfWindows == 10)
            {
                return client.GetConfigFloat("hud_10_seats_skip_angle");
            }
            else if (client.ProvidesConfig("hud_9_seats_skip_angle") && numberOfWindows == 9)
            {
                return client.GetConfigFloat("hud_9_seats_skip_angle");
            }
            else if (client.ProvidesConfig("hud_6_seats_skip_angle") && numberOfWindows == 6)
            {
                return client.GetConfigFloat("hud_6_seats_skip_angle");
            }
            else if (client.ProvidesConfig("hud_2_seats_skip_angle") && numberOfWindows == 2)
            {
                return client.GetConfigFloat("hud_2_seats_skip_angle");
            }
            else
            {
                // We don't know!
                Debug.Print("Skip angle unknown for " + numberOfWindows + " windows, setting to default");
                return (float)Math.PI / 3; 
            }
        }

        /* Given the windowWidth, finds the position of the center of the table
         * Using the proportion factors given by the client */
        private Point FindRelativeTableCenter(int windowWidth)
        {
            // Find the center
            float tableCenterXFactorRelativeToWindowWidth = client.GetConfigFloat("hud_table_center_x_factor_related_to_window_width");
            float tableCenterYFactorRelativeToTableCenterX = client.GetConfigFloat("hud_table_center_y_factor_related_to_table_center_x");

            int tableCenterX = (int)((float)windowWidth * tableCenterXFactorRelativeToWindowWidth);
            int tableCenterY = (int)((float)tableCenterX * tableCenterYFactorRelativeToTableCenterX);

            return new Point(tableCenterX, tableCenterY);
        }
    }
}
