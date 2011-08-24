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

        /* Constants */

        private static float SKIP_ANGLE = (float)Math.PI / 3;
        private static float HUD_DISTANCE_FROM_PLAYERS_ANGLE = (float)Math.PI / 6;
        
        /* If you take the game window width, what's the factor to multiply by to obtain
         * the relative X position of the center of the table */
        private static float TABLE_CENTER_X_FACTOR_RELATED_TO_WINDOW_WIDTH = 0.5f;

        /* Since the Y position of the center of the table is always (assume) proportional
        * to the relative X position, we can obtain the Y position of the center of the table
        * by multiplying by this factor */
        private static float TABLE_CENTER_Y_FACTOR_RELATED_TO_TABLE_CENTER_X = 0.6f;
        
        /* The distance in the X direction of a player from the center is proportional
         * to the position of the center (X) */
        private static float PLAYER_X_DISTANCE_FROM_CENTER_FACTOR_RELATED_TO_CENTER_X = 0.8f;

        /* Same for Y */
        private static float PLAYER_Y_DISTANCE_FROM_CENTER_FACTOR_RELATED_TO_CENTER_Y = 0.7f;

        public HudWindowsList()
        {
            windowsList = new List<HudWindow>(9);
        }

        public void Add(HudWindow window)
        {
            windowsList.Add(window);
        }

        public void Remove(HudWindow window)
        {
            Trace.Assert(window != null, "Trying to remove a null reference to a hud window");
            window.Close();
            windowsList.Remove(window);
        }

        /* Deletes every window that has the Dispose flag set to true */
        public void RemoveFlaggedWindows()
        {
            // Before deleting them, make sure we are closing them
            foreach (HudWindow w in windowsList)
            {
                if (w.DisposeFlag) w.Close();
            }

            windowsList.RemoveAll(
                delegate(HudWindow w)
                {
                    return w.DisposeFlag;
                }
            );
        }

        /* Remove all hud windows */
        public void RemoveAll()
        {
            foreach (HudWindow w in windowsList)
            {
                w.Close();
            }

            windowsList.Clear();
        }

        public void HideWindowsIntersectingWith(Rectangle rect)
        {
            foreach (HudWindow w in windowsList)
            {
                // Computes the rectangle relative to the screen of each window hud
                Rectangle windowRect = w.RectangleToScreen(w.DisplayRectangle);
                
                if (windowRect.IntersectsWith(rect))
                {
                    w.Hide();
                }
            }
        }

        public void ShowWindowsAwayFrom(Rectangle rect)
        {
            foreach (HudWindow w in windowsList)
            {
                // Computes the rectangle relative to the screen of each window hud
                Rectangle windowRect = w.RectangleToScreen(w.DisplayRectangle);

                if (!windowRect.IntersectsWith(rect))
                {
                    w.Show();
                }
            }
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

        /* Setup the initial default position of the windows hud instances 
         * The guess is often not perfect, but the user can adjust the position 
         * later. */
        public void SetupDefaultPositions(Rectangle windowRect)
        {
            Trace.Assert(windowRect.Width != 0, "Invalid windowRect sent for setting up hud windows default position. Zero.");

            Point relativeTableCenter = FindRelativeTableCenter(windowRect.Width);
            Point absoluteTableCenter = new Point(windowRect.X + relativeTableCenter.X,
                                                  windowRect.Y + relativeTableCenter.Y);
                                                   
            int numberOfWindows = windowsList.Count;
            float skipAngle = SKIP_ANGLE;
            float angleBetweenPlayers = HUD_DISTANCE_FROM_PLAYERS_ANGLE;

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
            return (int)((float)center.X * PLAYER_X_DISTANCE_FROM_CENTER_FACTOR_RELATED_TO_CENTER_X);
        }

        private int GetDistanceYFromCenter(Point center)
        {
            return (int)((float)center.Y * PLAYER_Y_DISTANCE_FROM_CENTER_FACTOR_RELATED_TO_CENTER_Y);
        }


        /* Given the windowWidth, finds the position of the center of the table
         * Using the proportion factors given by the client */
        private Point FindRelativeTableCenter(int windowWidth)
        {
            // Find the center

            int tableCenterX = (int)((float)windowWidth * TABLE_CENTER_X_FACTOR_RELATED_TO_WINDOW_WIDTH);
            int tableCenterY = (int)((float)tableCenterX * TABLE_CENTER_Y_FACTOR_RELATED_TO_TABLE_CENTER_X);

            return new Point(tableCenterX, tableCenterY);
        }
    }
}
