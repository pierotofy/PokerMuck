using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace PokerMuck
{
    class TimedScreenshotTaker
    {
        private ScreenshotTaker scrTaker;
        private int msInterval;

        /* Notifies the delegate that a new screenshot has been taken */
        public delegate void ScreenshotTakenHandler(Bitmap screenshot);
        public event ScreenshotTakenHandler ScreenshotTaken;

        private bool running;
        private Window window;

        public TimedScreenshotTaker(int msInterval, Window window)
        {
            this.scrTaker = new ScreenshotTaker();
            this.msInterval = msInterval;
            this.window = window;
            this.running = false;
        }

        private void Run()
        {
            Thread.Sleep(msInterval);

            while (running)
            {
                Bitmap screenshot = scrTaker.Take(window, true, ScreenshotTaker.Mode.PrintScreen);

                if (ScreenshotTaken != null && screenshot != null) ScreenshotTaken(screenshot);

                Thread.Sleep(msInterval);
            }
        }

        public void Start()
        {
            running = true;

            Thread t = new Thread(new ThreadStart(Run));
            t.Start();
        }

        public void Stop()
        {
            running = false;
        }
    }
}
