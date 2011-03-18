using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    interface INewFilesMonitorHandler
    {
        void NewFileWasCreated(String filename);
    }
}
