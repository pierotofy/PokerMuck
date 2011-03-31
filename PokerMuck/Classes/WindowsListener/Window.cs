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
    class Window
    {
        /* Get the text of a window */
        [DllImport("user32.dll")]
        public static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        /* Additionally we'll need this one to find the X-Y coordinate of the window */
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(int hWnd, out RECT lpRect);

        /* We need also to define a RECT structure */
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        /* This one will help us detect when a window gets closed */
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern int FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        private int handle;
        public int Handle { get { return handle; } }

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

        public bool HasBeenMinimized { get; set; }

        public bool Minimized
        {
            get
            {
                Rectangle windowRect = GetRect();

                // On MS Windows, when a window is resized it is moved to -32000 both on X and Y coordinate
                // Is there a better way to code this?
                bool ret = (windowRect.X == -32000 && windowRect.Y == -32000);

                // Keep track of the ever been minimized variable
                if (ret) HasBeenMinimized = true;

                return ret;
            }
        }


        /* User defined handle */
        public Window(int handle)
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

        public Rectangle GetRect()
        {
            return GetWindowRectFromHandle(handle);
        }



        // Static members

        public static bool Exists(String windowTitle)
        {
            return FindWindowByCaption(IntPtr.Zero, windowTitle) != 0;
        }

        public static String GetWindowTitleFromHandle(int handle)
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

        public static Rectangle GetWindowRectFromWindowTitle(String windowTitle)
        {
            int handle = FindWindowByCaption(IntPtr.Zero, windowTitle);
            return GetWindowRectFromHandle(handle);
        }

        public static Rectangle GetWindowRectFromHandle(int handle)
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
                Debug.Print("A new window became the foreground window, but I couldn't figure out its position and size.");
            }

            return windowRect;
        }
    }
}
