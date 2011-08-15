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
        private Hashtable mapData;
        private String mapLocation;
        private ColorMap colorMap;

        public VisualRecognitionMap(String mapLocation, ColorMap colorMap)
        {
            Debug.Assert(System.IO.File.Exists(mapLocation), "Cannot create a visualrecognitionmap without a proper map location: " + mapLocation);
            this.mapLocation = mapLocation;
            this.colorMap = colorMap;
            this.mapData = new Hashtable();

            AnaliseMap();
        }

        private void AnaliseMap()
        {
            Bitmap mapImage = new Bitmap(mapLocation);

            ColorFiltering filter = new ColorFiltering();

            foreach (String action in colorMap.Actions)
            {
                Color actionColor = colorMap.GetColorFor(action);
                
                filter.Red = new IntRange(actionColor.R, actionColor.R);
                filter.Green = new IntRange(actionColor.G, actionColor.G);
                filter.Blue = new IntRange(actionColor.B, actionColor.B);
                
                Bitmap colorFilteredMapImage = filter.Apply(mapImage);

                // Once we have isolated the action color on the map image
                // we need to find the biggest rectangle left in the map (as there could be noise)

                BlobCounter bc = new BlobCounter();
                bc.ProcessImage(colorFilteredMapImage);
                Rectangle[] rects = bc.GetObjectsRectangles();
                Debug.Assert(rects.Count<Rectangle>() > 0, "No rectangles were found while processing " + mapLocation + " for " + actionColor.ToString());

                Rectangle biggestRect = Rectangle.Empty;
                foreach (Rectangle rect in rects)
                {
                    // Compare areas
                    if (rect.Width * rect.Height > biggestRect.Width * biggestRect.Height){
                        biggestRect = rect;
                    }
                }

                // Found
                mapData[action] = biggestRect;
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
