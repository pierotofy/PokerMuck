using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;

namespace PokerMuck
{
    /* This class helps us keep track of information about a specific window 
     * Plus it provides some nice static methods for Win32 windows handling */
    public class Window
    {
        /* Get the text of a window */
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        /* Additionally we'll need this one to find the X-Y coordinate of the window */
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ClientToScreen(IntPtr hWnd, out POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


        /* We need also to define a RECT structure */
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        /* And POINT */
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        /* This one will help us detect when a window gets closed */
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        /* Detect the visibility of a window */
        [DllImport("user32.dll", EntryPoint = "IsWindowVisible", SetLastError = true)]
        public static extern bool IsWindowVisible(IntPtr hWnd);
        
        private IntPtr handle;
        public IntPtr Handle { get { return (IntPtr)handle; } }

        // Keep track of the latest valid window title
        private String latestValidWindowTitle = String.Empty;

        /* Returns the most current, valid window title
         * An empty title is not a valid window title */
        public String Title
        {
            get
            {
                String newTitle = GetWindowTitleFromHandle(handle);
                if (newTitle != String.Empty) latestValidWindowTitle = newTitle;
                return latestValidWindowTitle;
            }
        }

        public bool Visible
        {
            get
            {
                return IsWindowVisible(handle);
            }
        }

        public Size Size
        {
            get{
                Rectangle rect = this.Rectangle;
                return new Size(rect.Width, rect.Height);
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                return GetWindowRectFromHandle(handle);
            }
        }

        /* Returns the rectangle in terms of absolute screen coordinates, not relative to the container */
        public Rectangle ClientRectangle{
            get
            {
                return GetClientRectFromHandle(handle);
            }
        }

        public bool HasBeenMinimized { get; set; }

        public bool Minimized
        {
            get
            {
                // On MS Windows, when a window is resized it is moved to -32000 both on X and Y coordinate
                // Is there a better way to code this?
                bool ret = (Rectangle.X == -32000 && Rectangle.Y == -32000);

                // Keep track of the ever been minimized variable
                if (ret) HasBeenMinimized = true;

                return ret;
            }
        }


        /* User defined handle */
        public Window(IntPtr handle)
        {
            this.handle = handle;
            Initialize();
        }

        /* Find the handle given the window title */
        public Window(String windowTitle)
        {
            this.handle = Window.FindWindowByCaption(IntPtr.Zero, windowTitle);
            Initialize();
        }

        private void Initialize()
        {
            this.HasBeenMinimized = false;
        }

        public bool Exists()
        {
            return Title != String.Empty && Exists(Title);
        }

        public bool Resize(Size newSize, bool repaint = true)
        {
            return Window.ResizeWindow(handle, newSize, repaint);
        }


        // Static members

        public static bool Exists(String windowTitle)
        {
            return FindWindowByCaption(IntPtr.Zero, windowTitle) != IntPtr.Zero;
        }

        public static String GetWindowTitleFromHandle(IntPtr handle)
        {
            const int maxChars = 256;
            StringBuilder buffer = new StringBuilder(maxChars);
            if (GetWindowText(handle, buffer, maxChars) > 0)
            {
                return buffer.ToString();
            }
            else
            {
                return "";
            }
        }

        public static bool ResizeWindow(IntPtr handle, Size newSize, bool repaint)
        {
            Rectangle windowRect = GetWindowRectFromHandle(handle);
            return MoveWindow(handle, windowRect.X, windowRect.Y, newSize.Width, newSize.Height, repaint);
        }

        public static Rectangle GetWindowRectFromWindowTitle(String windowTitle)
        {
            IntPtr handle = FindWindowByCaption(IntPtr.Zero, windowTitle);
            return GetWindowRectFromHandle(handle);
        }

        public static Rectangle GetClientRectFromHandle(IntPtr handle)
        {
            RECT rct; // C++ style
            Rectangle clientRect = new Rectangle(); // C# style
            if (GetClientRect(handle, out rct))
            {
                // Translate to screen cordinates
                POINT topleft;
                topleft.x = rct.Left;
                topleft.y = rct.Top;

                POINT bottomright;
                bottomright.x = rct.Right;
                bottomright.y = rct.Bottom;

                ClientToScreen(handle, out topleft);
                ClientToScreen(handle, out bottomright);

                clientRect.X = topleft.x;
                clientRect.Y = topleft.y;
                clientRect.Width = bottomright.x - topleft.x;
                clientRect.Height = bottomright.y - topleft.y;
            }
            else
            {
                Trace.WriteLine("I couldn't figure out client position and size of window handle " + handle.ToString());
            }



            return clientRect;
        }

        public static Rectangle GetWindowRectFromHandle(IntPtr handle)
        {
            // Ok, let's figure out it's position

            RECT rct; // C++ style
            Rectangle windowRect = new Rectangle(); // C# style
            if (GetWindowRect(handle, out rct))
            {
                windowRect.X = rct.Left;
                windowRect.Y = rct.Top;
                windowRect.Width = rct.Right - rct.Left;
                windowRect.Height = rct.Bottom - rct.Top;
            }
            else
            {
                Trace.WriteLine("I couldn't figure out position and size of window handle " + handle.ToString());
            }

            return windowRect;
        }
    }
}
