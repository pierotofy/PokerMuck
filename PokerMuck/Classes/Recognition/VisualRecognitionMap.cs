using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    class VisualRecognitionMap
    {
        private String mapLocation;
        private ColorMap colorMap;

        public VisualRecognitionMap(String mapLocation, ColorMap colorMap)
        {
            this.mapLocation = mapLocation;
            this.colorMap = colorMap;
        }
    }
}
