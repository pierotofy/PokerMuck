using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    class CardRecognitionManager
    {
        private Table table;
        private ColorMap colorMap;

        public CardRecognitionManager(Table table)
        {
            Debug.Assert(table.Game != PokerGame.Unknown, "Cannot create a card recognition manager without knowing the game of the table");
            
            this.table = table;
            this.colorMap = ColorMap.Create(table.Game);
        }
    }
}
