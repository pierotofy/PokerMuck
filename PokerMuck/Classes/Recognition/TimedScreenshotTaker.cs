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
        private IntPtr windowHandle;

        public TimedScreenshotTaker(int msInterval, IntPtr windowHandle)
        {
            this.scrTaker = new ScreenshotTaker();
            this.msInterval = msInterval;
            this.windowHandle = windowHandle;
            this.running = false;
        }

        private void Run()
        {
            while (running)
            {
                Thread.Sleep(msInterval);

                scrTaker.Take(windowHandle);
                if (ScreenshotTaken != null) ScreenshotTaken(scrTaker.Current);
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
