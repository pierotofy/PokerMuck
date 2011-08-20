using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;

namespace PokerMuck
{
    class VisualRecognitionManager
    {
        /* How often do we pick a new screenshot and elaborate the images? */
        const int REFRESH_TIME = 4000;

        private Table table;
        private IVisualRecognitionManagerHandler handler;
        private VisualRecognitionMap recognitionMap;
        private ColorMap colorMap;
        private TimedScreenshotTaker timedScreenshotTaker;
        private VisualMatcher matcher;

        public VisualRecognitionManager(Table table, IVisualRecognitionManagerHandler handler)
        {
            Debug.Assert(table.Game != PokerGame.Unknown, "Cannot create a visual recognition manager without knowing the game of the table");
            Debug.Assert(table.WindowRect != Rectangle.Empty, "Cannot create a visual recognition manager without knowing the window rect");

            this.table = table;
            this.handler = handler;
            this.colorMap = ColorMap.Create(table.Game);
            this.recognitionMap = new VisualRecognitionMap(table.VisualRecognitionMapLocation, colorMap);
            this.matcher = new VisualMatcher(Globals.UserSettings.CurrentPokerClient);

            this.timedScreenshotTaker = new TimedScreenshotTaker(REFRESH_TIME, new Window(table.WindowTitle));
            this.timedScreenshotTaker.ScreenshotTaken += new TimedScreenshotTaker.ScreenshotTakenHandler(timedScreenshotTaker_ScreenshotTaken);
            this.timedScreenshotTaker.Start();
        }

        /* Update spawn location for the card select dialog (could have changed) */
        private void UpdateCardMatchDialogSpawnLocation()
        {
            Rectangle winRect = table.WindowRect;
            matcher.SetCardMatchDialogSpawnLocation(winRect.X + 30, winRect.Y + 30);
        }

        void timedScreenshotTaker_ScreenshotTaken(Bitmap screenshot)
        {
            UpdateCardMatchDialogSpawnLocation();

            /* Try to match player cards */
            List<Bitmap> playerCardImages = new List<Bitmap>();
            ArrayList playerCardsActions = colorMap.GetPlayerCardsActions(table.CurrentHeroSeat);

            foreach(String action in playerCardsActions){
                Rectangle actionRect = recognitionMap.GetRectangleFor(action);
                if (!actionRect.Equals(Rectangle.Empty))
                {
                    playerCardImages.Add(ScreenshotTaker.Slice(screenshot, actionRect));
                }
                else
                {
                    Trace.WriteLine("Warning: could not find a rectangle for action " + action);
                }
            }

            CardList playerCards = matcher.MatchCards(playerCardImages, false);
            if (playerCards != null)
            {
                Trace.WriteLine("Matched player cards! " + playerCards.ToString());
                handler.PlayerHandRecognized(playerCards);
            }

            // Dispose
            foreach (Bitmap image in playerCardImages) if (image != null) image.Dispose();

            /* If community cards are supported, try to match them */
            if (colorMap.SupportsCommunityCards)
            {
                List<Bitmap> communityCardImages = new List<Bitmap>();
                ArrayList communityCardsActions = colorMap.GetCommunityCardsActions();

                foreach (String action in communityCardsActions)
                {
                    Rectangle actionRect = recognitionMap.GetRectangleFor(action);
                    if (!actionRect.Equals(Rectangle.Empty))
                    {
                        communityCardImages.Add(ScreenshotTaker.Slice(screenshot, actionRect));
                    }
                    else
                    {
                        Trace.WriteLine("Warning: could not find a rectangle for action " + action);
                    }
                }

                // We try to identify as many cards as possible
                CardList communityCards = matcher.MatchCards(communityCardImages, true);
                if (communityCards != null && communityCards.Count > 0)
                {
                    Trace.WriteLine("Matched board cards! " + communityCards.ToString());
                    handler.BoardRecognized(communityCards);
                }

                // Dispose
                foreach (Bitmap image in communityCardImages) if (image != null) image.Dispose();
            }

            // Dispose screenshot
            if (screenshot != null) screenshot.Dispose();
        }

        public void Cleanup(){
            if (timedScreenshotTaker != null) timedScreenshotTaker.Stop();
        }


    }
}
