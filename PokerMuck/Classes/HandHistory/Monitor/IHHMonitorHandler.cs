using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    interface IHHMonitorHandler
    {
        void NewLineArrived(String filename, String line);
        void EndOfFileReached(String filename);
    }
}
