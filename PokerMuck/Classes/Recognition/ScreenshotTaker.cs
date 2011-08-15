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

        private Bitmap current;
        public Bitmap Current { get { return current; } }

        public ScreenshotTaker()
        {
        }

        public void Take(){
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            Take(bounds);
        }

        public void Take(Window window)
        {
            // We cannot use this method if the window is minimized
            if (window.Minimized) return;

            Rectangle rect = window.Rectangle;

            try
            {
                Bitmap result = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(result);
                IntPtr hdc = g.GetHdc();
                PrintWindow(window.Handle, hdc, 0);
                current = result;
                g.ReleaseHdc(hdc);
            }
            catch (Exception)
            {
                Debug.Print("Failed to take screenshot of " + window.Title);
            }
        }

        public void Take(Rectangle bounds)
        {
            current = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(current))
            {
                g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
            }
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

        public Bitmap CurrentSlice(Rectangle bounds)
        {
            Debug.Assert(current != null, "Cannot slice a screenshot that hasn't been taken");
            return ScreenshotTaker.Slice(Current, bounds);
        }

    }
}
