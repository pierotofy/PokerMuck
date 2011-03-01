using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;

namespace PokerMuck
{

    class WindowsListener
    {
        /* Will need these functions to find the current foreground window */
        [DllImport("user32.dll")]
        static extern int GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(int hWnd, StringBuilder text, int count); 

        /* Additionally we'll need this one to find the X-Y coordinate of the window */
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(int hWnd, out RECT lpRect);

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
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        /* Object that will receive the notifications when something changes */
        private IDetectWindowsChanges handler;

        /* Loop flag */
        private bool listening;

        /* Listen interval in milliseconds (default: 1000) */
        public int ListenInterval { get; set; }

        /* Current foreground window title */
        public String CurrentForegroundWindowTitle { get; set; }

        /* Current foreground window rectangle */
        public Rectangle CurrentForegroundWindowRect { get; set; }

        private bool currentWindowExists;

        public WindowsListener(IDetectWindowsChanges handler)
        {
            this.handler = handler;
            this.ListenInterval = 1000;
            this.currentWindowExists = false;
            
            // Set these the first time
            CurrentForegroundWindowTitle = GetForegroundWindowTitle();
            CurrentForegroundWindowRect = GetForegroundWindowRect();
        }

        /* Control methods */
        public void StartListening(){
            listening = true;
            Thread t = new Thread(ListenLoop);
            t.Start();
        }

        public void StopListening(){
            listening = false;
        }

        /* Loop method */
        private void ListenLoop(){
            while(listening){
                // Copy current foreground window title and window rect
                String previousForegroundWindowTitle = CurrentForegroundWindowTitle;
                Rectangle previousForegroundWindowRect = CurrentForegroundWindowRect;

                // Retrieve window handle
                int handle = GetForegroundWindowHandle();

                // Update current foreground window title and window rect
                CurrentForegroundWindowTitle = GetWindowTitleFromHandle(handle);
                CurrentForegroundWindowRect = GetWindowRectFromHandle(handle);


                // If the previous window existed and now it doesn't exists, it means that we closed it
                if (currentWindowExists && !WindowExists(previousForegroundWindowTitle))
                {
                    handler.WindowClosed(previousForegroundWindowTitle);
                }
                else
                {

                    // Title Different?
                    if (CurrentForegroundWindowTitle != String.Empty && CurrentForegroundWindowTitle != previousForegroundWindowTitle)
                    {
                        // Notify handler
                        handler.NewForegroundWindow(CurrentForegroundWindowTitle);

                        // Check if the window actually exists
                        currentWindowExists = WindowExists(CurrentForegroundWindowTitle);
                    }

                    // Rectangle different?
                    if (!CurrentForegroundWindowRect.Equals(previousForegroundWindowRect))
                    {
                        // Notify
                        handler.ForegroundWindowPositionChanged(CurrentForegroundWindowTitle, CurrentForegroundWindowRect);
                    }

                }


                Thread.Sleep(ListenInterval);
            }
        }

        /* Windows helper functions */
        private int GetForegroundWindowHandle()
        {
            int handle = 0;
            handle = GetForegroundWindow();
            return handle;
        }

        private bool WindowExists(String windowTitle)
        {
            return FindWindowByCaption(IntPtr.Zero, windowTitle) != IntPtr.Zero;
        }

        private String GetWindowTitleFromHandle(int handle){
            const int maxChars = 256;
            StringBuilder buffer = new StringBuilder(maxChars);
            if ( GetWindowText(handle, buffer, maxChars) > 0 )
            {
                return buffer.ToString();
            }else{
                return "";
            }
        }

        private String GetForegroundWindowTitle(){
            int handle = GetForegroundWindowHandle();
            return GetWindowTitleFromHandle(handle);
        }

        private Rectangle GetForegroundWindowRect()
        {
            int handle = GetForegroundWindowHandle();
            return GetWindowRectFromHandle(handle);
        }

        private Rectangle GetWindowRectFromHandle(int handle)
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
