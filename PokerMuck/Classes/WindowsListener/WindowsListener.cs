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
        static extern IntPtr GetForegroundWindow();

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

        /* List of windows to monitor for existance. (When one of these windows is closed, a proper event is fired) */
        private List<Window> windowsListToMonitor;

        /* Hashtable that helps us keep track of whether we have notified the handler of a window minimize/maximize */
        private Hashtable windowMinimizeNotificationSent;
        private Hashtable windowMaximizeNotificationSent;

        // This temporary list keeps track of which windows should be removed from our monitor list
        List<Window> removeWindowList = new List<Window>();

        private bool currentWindowExists;

        public WindowsListener(IDetectWindowsChanges handler)
        {
            this.handler = handler;
            this.ListenInterval = 1000;
            this.currentWindowExists = false;
            this.windowsListToMonitor = new List<Window>();
            this.windowMinimizeNotificationSent = new Hashtable();
            this.windowMaximizeNotificationSent = new Hashtable();



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

        /* Adds a window title to the list for monitoring for window closed events
         * duplicates will be ignored */
        public void AddToMonitorList(String windowTitle)
        {
            Window w = windowsListToMonitor.Find(delegate(Window window)
            {
                return window.Title == windowTitle;
            });

            if (w == null) windowsListToMonitor.Add(new Window(windowTitle));
        }

        /* Removes a window title from the list */
        public void RemoveFromMonitorList(String windowTitle)
        {
            windowsListToMonitor.RemoveAll(
                delegate(Window w)
                {
                    return w.Title == windowTitle;
                }
            );
        }

        /* Clears the monitor list */
        public void ClearMonitorList()
        {
            windowsListToMonitor.Clear();
        }

        /* Loop method */
        private void ListenLoop(){
            while(listening){
                // Copy current foreground window title and window rect
                String previousForegroundWindowTitle = CurrentForegroundWindowTitle;
                Rectangle previousForegroundWindowRect = CurrentForegroundWindowRect;

                // Retrieve window handle
                IntPtr handle = GetForegroundWindowHandle();

                // Update current foreground window title and window rect
                CurrentForegroundWindowTitle = Window.GetWindowTitleFromHandle(handle);
                CurrentForegroundWindowRect = Window.GetWindowRectFromHandle(handle);


                // Title Different?
                if (CurrentForegroundWindowTitle != String.Empty && CurrentForegroundWindowTitle != previousForegroundWindowTitle)
                {
                    // Notify handler
                    handler.NewForegroundWindow(CurrentForegroundWindowTitle, CurrentForegroundWindowRect);

                    // Check if the window actually exists
                    currentWindowExists = Window.Exists(CurrentForegroundWindowTitle);
                }

                // Rectangle different?
                if (!CurrentForegroundWindowRect.Equals(previousForegroundWindowRect))
                {
                    // Notify
                    handler.ForegroundWindowPositionChanged(CurrentForegroundWindowTitle, CurrentForegroundWindowRect);
                }

                // Check the windows that have been specifically added to our list, one by one
                CheckWindowsMonitorList();

                // Wait
                Thread.Sleep(ListenInterval);
            }
        }

        private void CheckWindowsMonitorList()
        {
            // Check each of the windows that we are monitoring
            int items = windowsListToMonitor.Count;
            for (int i = 0; i < items; i++)
            {
                Window window = windowsListToMonitor[i];

                // (!window.Minimized && !window.Visible) is currently there for fixing a glitch in the SWC
                // client, which does not destroy the windows it creates but simply sets them invisible
                if (!window.Exists() || (!window.Minimized && !window.Visible))
                {
                    // Notify handler
                    handler.WindowClosed(window.Title);

                    // Add it to our remove list
                    removeWindowList.Add(window);
                }
                else
                {
                    // Still exist... is it resized?
                    if (window.Minimized)
                    {
                        // Window is minimized... allow for maximize notifications to be sent again
                        windowMaximizeNotificationSent[window] = false;

                        // Did we already notify the handler?
                        if (!HasMinimizeNotificationBeenSent(window))
                        {
                            // Nop, notify
                            handler.WindowMinimized(window.Title);

                            // Set notification sent
                            windowMinimizeNotificationSent[window] = true;
                        }
                    }
                    else if (window.HasBeenMinimized)
                    {
                        // Window is visible... allow for minimize notifications to be sent again
                        windowMinimizeNotificationSent[window] = false;

                        // Did we already notify the handler?
                        if (!HasMaximizeNotificationBeenSent(window))
                        {
                            // Nop, notify
                            handler.WindowMaximized(window.Title);

                            // Set notification sent
                            windowMaximizeNotificationSent[window] = true;
                        }
                    }
                }
            }

            // Actually remove the windows
            foreach (Window w in removeWindowList)
            {
                windowsListToMonitor.Remove(w);
            }

            // Cleanup
            removeWindowList.Clear();
        }

        /* Helper method to check whether we have sent a maximize notification to the handler about a window */
        private bool HasMaximizeNotificationBeenSent(Window w)
        {
            // New item?
            if (!windowMaximizeNotificationSent.Contains(w)) windowMaximizeNotificationSent[w] = false;

            return (bool)windowMaximizeNotificationSent[w];
        }

        /* Helper method to check whether we have sent a minimize notification to the handler about a window */
        private bool HasMinimizeNotificationBeenSent(Window w)
        {
            // New item?
            if (!windowMinimizeNotificationSent.Contains(w)) windowMinimizeNotificationSent[w] = false;

            return (bool)windowMinimizeNotificationSent[w];
        }

        /* Windows helper functions */
        private IntPtr GetForegroundWindowHandle()
        {
            IntPtr handle = IntPtr.Zero;
            handle = GetForegroundWindow();
            return handle;
        }

        private String GetForegroundWindowTitle(){
            IntPtr handle = GetForegroundWindowHandle();
            return Window.GetWindowTitleFromHandle(handle);
        }

        private Rectangle GetForegroundWindowRect()
        {
            IntPtr handle = GetForegroundWindowHandle();
            return Window.GetWindowRectFromHandle(handle);
        }


    }
}
