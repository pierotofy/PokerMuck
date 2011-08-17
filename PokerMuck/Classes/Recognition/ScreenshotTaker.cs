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

        public ScreenshotTaker()
        {
        }

        public Bitmap Take(){
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            return Take(bounds);
        }

        public Bitmap Take(Window window)
        {
            // We cannot use this method if the window is minimized
            if (window.Minimized) return null;

            Rectangle rect = window.Rectangle;

            try
            {
                Bitmap result = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(result);
                IntPtr hdc = g.GetHdc();
                PrintWindow(window.Handle, hdc, 0);
                g.ReleaseHdc(hdc);
                return result;
            }
            catch (Exception)
            {
                Debug.Print("Failed to take screenshot of " + window.Title);
                return null;
            }
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
            Bitmap result = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(screenshot, 0, 0, bounds, GraphicsUnit.Pixel);
            }

            return result;
        }
    }
}
