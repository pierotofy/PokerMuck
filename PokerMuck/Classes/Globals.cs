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

        /* Director
         * There's only one director active at a time. Certain operations need to be executed on the GUI
         * thread, and the director is the one that allows it. We could tricke down a reference to the director
         * in every routine that needs to use it, or we could simply set a global istance of it. */
        public static PokerMuckDirector Director;
    }
}
