using System;
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
        /* This constant indicates the minimum area that a colored area should have
         * to be considered acceptable from the color map. Without this, a single pixel of the desired
         * color might screw up our detection */ 
        private const int MIN_AREA_THRESHOLD = 25;

        private Hashtable mapData;
        private String mapLocation;
        private ColorMap colorMap;
        private Size desiredMapSize;
        private Size originalMapSize;

        public Size OriginalMapSize { get { return originalMapSize; } }

        /* @param mapSize before analising the map we can specify a size we want the map to be resized to
         *  this is useful because sometimes the target window that we are going to match is going to be of
         *  a different size */
        public VisualRecognitionMap(String mapLocation, ColorMap colorMap, Size desiredMapSize)
        {
            Trace.Assert(System.IO.File.Exists(mapLocation), "Cannot create a visualrecognitionmap without a proper map location: " + mapLocation);
            this.mapLocation = mapLocation;
            this.mapData = new Hashtable();
            this.colorMap = colorMap;
            this.desiredMapSize = desiredMapSize;

            Bitmap map = new Bitmap(mapLocation);
            originalMapSize = map.Size;
            map.Dispose();

            ComputeForDesiredMapSize(desiredMapSize);
        }

        public VisualRecognitionMap(String mapLocation, ColorMap colorMap)
            : this(mapLocation, colorMap, Size.Empty)
        {

        }

        private void ComputeForDesiredMapSize(Size desiredMapSize)
        {
            Bitmap mapImage = new Bitmap(mapLocation);
            if (!desiredMapSize.Equals(Size.Empty))
            {
                Bitmap oldMapImage = mapImage;
                mapImage = ResizeMap(oldMapImage, desiredMapSize);
                oldMapImage.Dispose();
            }
            AnaliseMap(mapImage);
            SameSizeCheck(colorMap.GetSameSizeActions(), 1);

            mapImage.Dispose();
        }

        private Bitmap ResizeMap(Bitmap mapImage, Size desiredMapSize)
        {
            Bitmap result = new Bitmap(desiredMapSize.Width, desiredMapSize.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;

                g.DrawImage(mapImage, 0, 0, desiredMapSize.Width, desiredMapSize.Height);
            }
            return result;
        }

        private void AnaliseMap(Bitmap mapImage)
        {
            mapData.Clear();
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
                    if (!biggestRect.Equals(Rectangle.Empty) && (biggestRect.Width * biggestRect.Height > MIN_AREA_THRESHOLD))
                    {
                        Trace.WriteLine("Found rectangle for " + action + ": " + biggestRect.ToString());
                        mapData[action] = biggestRect;
                    }
                }
            }
        }

        /* Re-analyses the map using the new size (which typically reflects
         * the size of the latest taken screenshot) */
        public void AdjustToSize(Size newDesiredSize)
        {
            if (!this.desiredMapSize.Equals(newDesiredSize))
            {
                this.desiredMapSize = newDesiredSize;
                ComputeForDesiredMapSize(desiredMapSize);
            }
        }

        /* This function will raise an exception if the check fails
         * @param delta: how many pixels away two actions can be (ex. if delta = 1, two blocks can differ both in width
               and height by 1 pixel) */
        public void SameSizeCheck(ArrayList actions, int delta)
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

                if (Math.Abs(refRect.Width - compareRect.Width) > delta || Math.Abs(refRect.Height - compareRect.Height) > delta)
                {
                    differentEntries[action] = compareRect;
                }
            }

            // Check failed?
            if (differentEntries.Count > 0)
            {
                Trace.WriteLine("Invalid color map detected, displaying error message with details.");
                String errorMessage = String.Format("Your color map {0} contains some entries of invalid size (they should be of size: {1}x{2}px): \n", 
                    mapLocation, refRect.Width, refRect.Height);

                foreach (String action in differentEntries.Keys)
                {
                    Rectangle rect = (Rectangle)differentEntries[action];
                    errorMessage += String.Format("\n{0}: invalid size {1}x{2}px", action, rect.Width, rect.Height);
                }

                errorMessage += "\n\nThe program will now crash.";
                Globals.Director.RunFromGUIThread((Action)delegate()
                {
                    System.Windows.Forms.MessageBox.Show(errorMessage, "I need your help to fix this", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                }, false);
                throw new Exception(errorMessage);
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
