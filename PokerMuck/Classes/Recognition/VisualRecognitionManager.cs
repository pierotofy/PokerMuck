using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    class VisualRecognitionManager
    {
        private Table table;
        private ColorMap colorMap;

        public VisualRecognitionManager(Table table)
        {
            Debug.Assert(table.Game != PokerGame.Unknown, "Cannot create a visual recognition manager without knowing the game of the table");
            
            this.table = table;
            this.colorMap = ColorMap.Create(table.Game);
        }
    }
}
