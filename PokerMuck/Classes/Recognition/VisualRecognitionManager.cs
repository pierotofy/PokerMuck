using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace PokerMuck
{
    class VisualRecognitionManager
    {
        private Table table;
        private VisualRecognitionMap recognitionMap;
        private TimedScreenshotTaker timedScreenshotTaker;

        public VisualRecognitionManager(Table table)
        {
            Debug.Assert(table.Game != PokerGame.Unknown, "Cannot create a visual recognition manager without knowing the game of the table");
            Debug.Assert(table.WindowRect != Rectangle.Empty, "Cannot create a visual recognition manager without knowing the window rect");

            this.table = table;
            this.recognitionMap = new VisualRecognitionMap(table.VisualRecognitionMapLocation, ColorMap.Create(table.Game));

            Window w = new Window(table.WindowTitle);

            this.timedScreenshotTaker = new TimedScreenshotTaker(5000, (IntPtr)w.Handle);
            this.timedScreenshotTaker.ScreenshotTaken += new TimedScreenshotTaker.ScreenshotTakenHandler(timedScreenshotTaker_ScreenshotTaken);
            this.timedScreenshotTaker.Start();
        }

        void  timedScreenshotTaker_ScreenshotTaken(Bitmap screenshot)
        {
 	        screenshot.Save("1.bmp", ImageFormat.Bmp);
        }

        public void Cleanup(){
            if (timedScreenshotTaker != null) timedScreenshotTaker.Stop();
        }


    }
}
