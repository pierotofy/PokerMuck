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

        public HudWindowsList(PokerClient client)
        {
            this.client = client;
            windowsList = new List<HudWindow>(9);
        }

        public void Add(HudWindow window)
        {
            windowsList.Add(window);
        }

        public void Remove(HudWindow window)
        {
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

        /* Setup the initial position of the windows hud instances */
        public void SetupInitialPositions(Rectangle windowRect)
        {
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

            int i; // We'll use this later
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

        // Thread safe
        private void SetWindowLocation(HudWindow w, Point location)
        {
            // Thread safe
            //w.Invoke((Action)delegate()
           // {
                w.Location = location;
           // });
        }

        private int GetDistanceXFromCenter(Point center){
            return (int)((float)center.X * client.GetConfigFloat("hud_player_x_distance_from_center_factor_related_to_center_x"));
        }

        private int GetDistanceYFromCenter(Point center)
        {
            return (int)((float)center.Y * client.GetConfigFloat("hud_player_y_distance_from_center_factor_related_to_center_y"));
        }


        /* Find the skip angle to use
         * If it cannot find a perfect match, it returns 25°
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
                Debug.Print("Skip angle unknown for " + numberOfWindows + " windows, setting to 25°");
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
