using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    static class Globals
    {
        /* Configuration
         * There's only one configuration active at a time, this is why we make it globally accessible
         * (it's a better trade-off than passing the configuration through constructors and methods) */
        public static PokerMuckUserSettings UserSettings;
    }
}
