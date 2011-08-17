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
        private Table table;
        private VisualRecognitionMap recognitionMap;
        private ColorMap colorMap;
        private TimedScreenshotTaker timedScreenshotTaker;
        private VisualMatcher matcher;

        public VisualRecognitionManager(Table table)
        {
            Debug.Assert(table.Game != PokerGame.Unknown, "Cannot create a visual recognition manager without knowing the game of the table");
            Debug.Assert(table.WindowRect != Rectangle.Empty, "Cannot create a visual recognition manager without knowing the window rect");

            this.table = table;
            this.colorMap = ColorMap.Create(table.Game);
            this.recognitionMap = new VisualRecognitionMap(table.VisualRecognitionMapLocation, colorMap);
            this.matcher = new VisualMatcher(Globals.UserSettings.CurrentPokerClient, false);

            // TODO custom time refresh?
            this.timedScreenshotTaker = new TimedScreenshotTaker(5000, new Window(table.WindowTitle));
            this.timedScreenshotTaker.ScreenshotTaken += new TimedScreenshotTaker.ScreenshotTakenHandler(timedScreenshotTaker_ScreenshotTaken);
            this.timedScreenshotTaker.Start();
        }

        void timedScreenshotTaker_ScreenshotTaken(Bitmap screenshot)
        {
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
                    Debug.Print("Warning: could not find a rectangle for action " + action);
                }
            }

            CardList playerCards = matcher.MatchCards(playerCardImages, false);
            if (playerCards != null)
            {
                Debug.Print("Matched player cards! " + playerCards.ToString());
            }

            // Dispose
            foreach (Bitmap image in playerCardImages) image.Dispose();

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
                        Debug.Print("Warning: could not find a rectangle for action " + action);
                    }
                }

                // We try to identify as many cards as possible
                CardList communityCards = matcher.MatchCards(communityCardImages, true);
                if (communityCards != null && communityCards.Count > 0)
                {
                    Debug.Print("Matched board cards! " + communityCards.ToString());
                }

                // Dispose
                foreach (Bitmap image in communityCardImages) image.Dispose();
            }

            // Dispose screenshot
            screenshot.Dispose();
        }

        public void Cleanup(){
            if (timedScreenshotTaker != null) timedScreenshotTaker.Stop();
        }


    }
}
