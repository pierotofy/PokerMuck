using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PokerMuck
{
    interface IDetectWindowsChanges
    {
        void NewForegroundWindow(string windowTitle);
        void ForegroundWindowPositionChanged(string windowTitle, Rectangle windowRect);
        void WindowClosed(string windowTitle);
    }
}
