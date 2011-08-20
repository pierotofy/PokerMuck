using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    interface IVisualRecognitionManagerHandler
    {
        void PlayerHandRecognized(CardList playerCards);
        void BoardRecognized(CardList board);
    }
}
