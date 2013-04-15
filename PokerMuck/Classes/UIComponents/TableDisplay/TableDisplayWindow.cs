using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PokerMuck
{
    public abstract partial class TableDisplayWindow : Form
    {
        public static TableDisplayWindow CreateForTable(Table t)
        {
            switch(t.Game){
                case PokerGame.Holdem:
                    return new HoldemTableDisplayWindow(t);
                default:
                    return new InvisibleTableDisplayWindow(t);
            }
        }

        /* Maximum height of card list panels */
        private const int MAXIMUM_CARD_LIST_PANEL_HEIGHT = 100;
        
        protected const int CARD_SPACING = 6;
        protected const int CARD_BORDER_PADDING = 1;

        private Table table; //Reference to the table that owns this display

        protected Hand lastPlayerHand; // Keep track of the last hand for which we displayed information about

        protected OddsCalculator oddsCalculator;

        public TableDisplayWindow(Table table)
        {
            InitializeComponent();
            this.lastPlayerHand = null;
            this.table = table;
            this.oddsCalculator = OddsCalculator.CreateFor(table.Game);
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Size = LoadWindowSize();
            this.Location = LoadAbsoluteWindowPosition();
        }

        /* Hands are different objects depending on the game type
         * this methods returns null if the the hand cannot be created (maybe because of invalid number of cards) */
        protected abstract Hand CreateHandFromCardList(CardList cardList);

        public void SetVisualRecognitionSupported(bool supported)
        {
            lblVisualRecognitionNotSupported.Visible = !supported;
        }

        public void DisplayPlayerHand(CardList playerCards){
            // First create the hand
            Hand playerHand = CreateHandFromCardList(playerCards);

            if (playerHand != null){
                UpdateHandTab(playerHand);

                // Save reference to the current hand
                lastPlayerHand = playerHand;
            }
        }

        public void DisplayFinalBoard(Board board)
        {
            CardListPanel clp = new CardListPanel();
            clp.CardListToDisplay = board;
            clp.CardSpacing = 4;
            clp.BackColor = Color.Transparent;

            /* We set the initial size of the component to the largest possible, the
                * addPanel method will take care of setting the proper size */
            clp.Size = entityHandsContainer.Size;

            entityHandsContainer.AddPanel(clp, MAXIMUM_CARD_LIST_PANEL_HEIGHT);
        }

        public void DisplayPlayerMuckedHand(String playerName, Hand hand)
        {
            EntityCardListPanel ehp = new EntityCardListPanel();
            ehp.EntityName = playerName;
            ehp.CardListToDisplay = hand;

            /* We set the initial size of the component to the largest possible, the
             * addPanel method will take care of setting the proper size */
            ehp.Size = entityHandsContainer.Size;

            entityHandsContainer.AddPanel(ehp, MAXIMUM_CARD_LIST_PANEL_HEIGHT);
        }

        public void ClearMuck()
        {
            entityHandsContainer.ClearAll();
        }

        /* To be called at the end of a round */
        public virtual void ClearHandInformation()
        {
            lastPlayerHand = null;
        }

        /* The position of the poker client window might have changed */
        public void UpdatePosition(){
            this.Location = LoadAbsoluteWindowPosition();
        }

        public void DisplayStatistics(Player p)
        {
            statisticsDisplay.Visible = true;
            tabControl.SelectedIndex = tabControl.TabCount - 1;
            statisticsDisplay.DisplayStatistics(p);
        }

        public void UpdateStatistics()
        {
            statisticsDisplay.UpdateStatistics();
        }

        /* Checks whether this window is overlapping this object
         * if it is, and it's not our playing window, we need to hide ourselves */
        public virtual void CheckForWindowOverlay(String windowTitle, Rectangle windowRect)
        {
            if (windowTitle == this.Text) return;

            // Proceed only if this is not our table window and the hud is visible
            if (windowTitle != table.WindowTitle)
            {
                Rectangle ourRect = this.RectangleToScreen(this.ClientRectangle);

                if (windowRect.IntersectsWith(ourRect))
                {
                    Visible = false;
                }
                else
                {
                    Visible = true;
                }
            }

            // If the window is the our table window, make sure we are displaying it!
            else if (windowTitle == table.WindowTitle)
            {
                Visible = true;
            }
        }

        protected bool PlayerHandHasChanged(Hand playerHand)
        {
            return lastPlayerHand == null || !playerHand.Equals(lastPlayerHand);
        }

        protected virtual void UpdateHandTab(Hand playerHand)
        {
            // Update only if the hand is different than the previous one
            if (PlayerHandHasChanged(playerHand))
            {
                // Clear previous stuff
                handControlLayout.Controls.Clear();

                // Display current hand
                CardListPanel playerCardsPanel = new CardListPanel();
                playerCardsPanel.BackColor = Color.Transparent;
                playerCardsPanel.CardSpacing = CARD_SPACING;
                playerCardsPanel.BorderPadding = CARD_BORDER_PADDING;

                playerCardsPanel.CardListToDisplay = playerHand;
                playerCardsPanel.Height = 80;
                handControlLayout.Controls.Add(playerCardsPanel);

                DisplayOdds(playerHand);
            }
        }

        protected virtual void DisplayOdds(Hand playerHand)
        {
            // Display odds
            List<Statistic> odds = oddsCalculator.Calculate(playerHand);

            StatisticItemListDisplay statisticListDisplay = CreateStatisticItemListDisplay();
            statisticListDisplay.Add(odds);

            handControlLayout.Controls.Add(statisticListDisplay);
        }

        protected StatisticItemListDisplay CreateStatisticItemListDisplay()
        {
            StatisticItemListDisplay statisticListDisplay = new StatisticItemListDisplay();
            statisticListDisplay.BackColor = Color.Transparent;
            statisticListDisplay.TopMargin = 0;
            statisticListDisplay.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            statisticListDisplay.Width = handControlLayout.Width - handControlLayout.Padding.Right - handControlLayout.Padding.Left;
            statisticListDisplay.StatisticsSpacing = 2;
            statisticListDisplay.AutoSize = true;
            return statisticListDisplay;
        }

        private Point LoadAbsoluteWindowPosition()
        {
            Point relativePosition = LoadRelativeWindowPosition();
            Rectangle tableRect = table.WindowRect;

            return new Point(tableRect.X + relativePosition.X, tableRect.Y + relativePosition.Y);
        }

        /* Relative to the poker client table window */
        private Point LoadRelativeWindowPosition()
        {
            return Globals.UserSettings.TableDisplayRelativeWindowPosition;
        }

        private void SaveRelativeWindowPosition()
        {
            // Don't save if it's minimized, it will compute a big relativePosition and make the hud disappear
            if (!table.Minimized)
            {
                Point absolutePosition = this.Location;
                Rectangle tableRect = table.WindowRect;
                Point relativePosition = new Point(absolutePosition.X - tableRect.X, absolutePosition.Y - tableRect.Y);

                Globals.UserSettings.TableDisplayRelativeWindowPosition = relativePosition;
                Globals.UserSettings.Save();
            }
        }

        private Size LoadWindowSize()
        {
            return Globals.UserSettings.TableDisplayWindowSize;
        }

        private void SaveWindowSize()
        {
            Globals.UserSettings.TableDisplayWindowSize = this.Size;
            Globals.UserSettings.Save();
        }


        private const int cGrip = 18;      // Grip size
        private const int cCaption = 25;   // Caption bar height;

        // Thanks stackoverflow
        // This code allows us to move and resize our window without having a border
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x20)
            {  // Trap WM_SETCUROR
                if ((m.LParam.ToInt32() & 0xffff) == 2)
                { // Trap HTCAPTION
                    Cursor.Current = Cursors.SizeAll;
                    m.Result = (IntPtr)1;  // Processed
                    return;
                }
            }

            if (m.Msg == 0x84)
            {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;  // HTCAPTION
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }

            base.WndProc(ref m);
        }

        private void TableDisplayWindow_LocationChanged(object sender, EventArgs e)
        {
            SaveRelativeWindowPosition();
        }

        private void TableDisplayWindow_ResizeEnd(object sender, EventArgs e)
        {
            SaveWindowSize();
        }
    }
}
