using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    class CardRecognitionMap
    {
        private String mapLocation;
        private ColorMap colorMap;

        public CardRecognitionMap(String mapLocation, ColorMap colorMap)
        {
            this.mapLocation = mapLocation;
            this.colorMap = colorMap;
        }
    }
}
