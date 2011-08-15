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
        private Bitmap current;
        public Bitmap Current { get { return current; } }

        public ScreenshotTaker()
        {
        }

        public void Take(){
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            Take(bounds);
        }

        public void Take(Rectangle bounds)
        {
            current = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(current))
            {
                g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
            }
        }

        public void Take(IntPtr windowHandle)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(windowHandle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(windowHandle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(windowHandle, hdcSrc);

            // get a .NET image object for it
            current = Bitmap.FromHbitmap(hBitmap);

            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);
        }

        public static Bitmap Slice(Bitmap screenshot, Rectangle bounds)
        {
            Bitmap result = new Bitmap(bounds.Width, bounds.Height);
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

        // Thanks to: http://www.developerfusion.com/code/4630/capture-a-screen-shot/
        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        }

        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }
    }
}
