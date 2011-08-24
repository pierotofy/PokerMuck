﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using AForge.Imaging.Filters;
using AForge;
using AForge.Imaging;
using System.Collections;

namespace PokerMuck
{
    class VisualRecognitionMap
    {
        private Hashtable mapData;
        private String mapLocation;
        private ColorMap colorMap;

        /* @param mapSize before analising the map we can specify a size we want the map to be resized to
         *  this is useful because sometimes the target window that we are going to match is going to be of
         *  a different size */
        public VisualRecognitionMap(String mapLocation, ColorMap colorMap, Size mapSize)
        {
            Trace.Assert(System.IO.File.Exists(mapLocation), "Cannot create a visualrecognitionmap without a proper map location: " + mapLocation);
            this.mapLocation = mapLocation;
            this.colorMap = colorMap;
            this.mapData = new Hashtable();
            
            Bitmap mapImage = new Bitmap(mapLocation);
            if (!mapSize.Equals(Size.Empty)) mapImage = ResizeMap(mapImage, mapSize);
            AnaliseMap(mapImage);

            SameSizeCheck(colorMap.GetSameSizeActions());
        }

        public VisualRecognitionMap(String mapLocation, ColorMap colorMap)
            : this(mapLocation, colorMap, Size.Empty)
        {

        }

        private Bitmap ResizeMap(Bitmap mapImage, Size mapSize)
        {
            Bitmap result = new Bitmap(mapSize.Width, mapSize.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(mapImage, 0, 0, mapSize.Width, mapSize.Height);
            }
            return result;
        }

        private void AnaliseMap(Bitmap mapImage)
        {
            ColorFiltering filter = new ColorFiltering();
            Trace.WriteLine("Analysing color map " + mapLocation + "...");

            foreach (String action in colorMap.Actions)
            {
                Color actionColor = colorMap.GetColorFor(action);
                
                filter.Red = new IntRange(actionColor.R, actionColor.R);
                filter.Green = new IntRange(actionColor.G, actionColor.G);
                filter.Blue = new IntRange(actionColor.B, actionColor.B);

                using (Bitmap colorFilteredMapImage = filter.Apply(mapImage))
                {
                    // Once we have isolated the action color on the map image
                    // we need to find the biggest rectangle left in the map (as there could be noise)

                    BlobCounter bc = new BlobCounter();
                    bc.ProcessImage(colorFilteredMapImage);
                    Rectangle[] rects = bc.GetObjectsRectangles();
                    if (rects.Count<Rectangle>() == 0) Trace.WriteLine("Warning: No rectangles were found for " + actionColor.ToString());

                    Rectangle biggestRect = Rectangle.Empty;
                    foreach (Rectangle rect in rects)
                    {
                        // Compare areas
                        if (rect.Width * rect.Height > biggestRect.Width * biggestRect.Height)
                        {
                            biggestRect = rect;
                        }
                    }

                    // Did we find a rectangle?
                    if (!biggestRect.Equals(Rectangle.Empty))
                    {
                        Trace.WriteLine("Found rectangle for " + action + ": " + biggestRect.ToString());
                        mapData[action] = biggestRect;
                    }
                }
            }
        }

        /* This function will raise an exception if the check fails */
        public void SameSizeCheck(ArrayList actions)
        {
            Trace.WriteLine("Checking color map " + mapLocation + " for proper sizes...");
            if (actions.Count <= 1) return;

            Hashtable differentEntries = new Hashtable(); // holds the name of the actions that are different

            /* We use the first valid rectangle as comparison (if the first valid rectangle is the odd one,
             * all the others will be flagged) */
            Rectangle refRect = Rectangle.Empty; 
            foreach (String action in actions)
            {
                if (refRect.Equals(Rectangle.Empty))
                {
                    refRect = GetRectangleFor(action);
                }

                if (refRect.Equals(Rectangle.Empty)) continue; // Search for the first valid entry

                Rectangle compareRect = GetRectangleFor(action);
                if (compareRect.Equals(Rectangle.Empty)) continue; // Skip rectangles that are not in our list

                if (refRect.Width != compareRect.Width || refRect.Height != compareRect.Height)
                {
                    differentEntries[action] = compareRect;
                }
            }

            // Check failed?
            if (differentEntries.Count > 0)
            {
                String errorMessage = String.Format("Your color map {0} contains some entries of invalid size (they should be of size: {1}x{2}px): ", 
                    mapLocation, refRect.Width, refRect.Height);

                foreach (String action in differentEntries.Keys)
                {
                    Rectangle rect = (Rectangle)differentEntries[action];
                    errorMessage += String.Format("\n{0}: invalid size {2}x{3}px", action, rect.Width, rect.Height);
                    throw new Exception(errorMessage);
                }
            }
        }

        /* We expect that some maps will not include all possible actions,
         * so this method might return an empty rectangle if the action is not present in the map */
        public Rectangle GetRectangleFor(String action)
        {
            if (mapData.ContainsKey(action)) return (Rectangle)mapData[action];
            else return Rectangle.Empty;
        }

    }
}