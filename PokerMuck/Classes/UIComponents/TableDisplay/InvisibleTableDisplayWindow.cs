using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    class InvisibleTableDisplayWindow : TableDisplayWindow
    {
        public InvisibleTableDisplayWindow(Table table)
            : base(table)
        {
            this.Visible = false;
        }

        // This window actually never generates a hand
        protected override Hand CreateHandFromCardList(CardList cardList)
        {
            return null;
        }

        // We keep the window hidden at all times
        public override void CheckForWindowOverlay(string windowTitle, System.Drawing.Rectangle windowRect)
        {
            return;
        }
    }
}
