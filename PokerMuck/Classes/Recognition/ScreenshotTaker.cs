using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PokerMuck
{
    class ScreenshotTaker
    {
        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        const int WM_PRINT = 0x0317;
        const uint WM_PRINTCLIENTONLY = 1;
        const int WM_SETREDRAW = 0xB;

        public ScreenshotTaker()
        {
        }

        public Bitmap Take(){
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            return Take(bounds);
        }

        /* @param size: if different than window size, resizes the window before taking the screenshot
         *    note that this is different than resizing a bitmap, we are literally changing the window size */ 
        public Bitmap Take(Window window, bool clientOnly, Size size)
        {
            // We cannot use this method if the window is minimized
            if (window.Minimized) return null;

            Size originalWindowSize = window.Size;
            bool needResize = !originalWindowSize.Equals(size);

            try
            {
                if (needResize)
                {
                    // If we are taking the client only, we don't need the extra repaint
                    window.Resize(size, clientOnly ? false : true);
                }

                Rectangle screenshotRect = clientOnly ? window.ClientRectangle : window.Rectangle;

                Bitmap result = new Bitmap(screenshotRect.Width, screenshotRect.Height, PixelFormat.Format24bppRgb);                
                Graphics g = Graphics.FromImage(result);
                IntPtr hdc = g.GetHdc();
                uint nFlags = (clientOnly ? WM_PRINTCLIENTONLY : 0);
                PrintWindow(window.Handle, hdc, nFlags);
                g.ReleaseHdc(hdc);

                // Restore original dimension
                if (needResize)
                {
                    window.Resize(originalWindowSize, true); // OK, repaint now!
                }

                return result;
            }
            catch (Exception e)
            {
                Trace.WriteLine("Failed to take screenshot of " + window.Title + ": " + e.Message);
                return null;
            }
        }

        public Bitmap Take(Window window, bool clientOnly)
        {
            Rectangle rect = window.Rectangle;
            Size winSize = new Size(rect.Width, rect.Height);

            return Take(window, clientOnly, winSize);
        }

        public Bitmap Take(Rectangle bounds)
        {
            Bitmap result = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
            }

            return result;
        }

        public static Bitmap Slice(Bitmap screenshot, Rectangle bounds)
        {
            if (screenshot == null) return null;

            Bitmap result = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(screenshot, 0, 0, bounds, GraphicsUnit.Pixel);
            }

            return result;
        }
    }
}
