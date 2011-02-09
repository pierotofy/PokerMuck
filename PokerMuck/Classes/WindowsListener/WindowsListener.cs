using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace PokerMuck
{

    class WindowsListener
    {
        /* Will need these functions to find the current foreground window */
        [DllImport("user32.dll")]
        static extern int GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(int hWnd, StringBuilder text, int count); 

        /* Object that will receive the notifications when something changes */
        private IDetectWindowsChanges handler;

        /* Loop flag */
        private bool listening;

        /* Listen interval in milliseconds (default: 1000) */
        public int ListenInterval { get; set; }

        /* Current foreground window title */
        public String CurrentForegroundWindowTitle { get; set; }

        public WindowsListener(IDetectWindowsChanges handler)
        {
            this.handler = handler;
            this.ListenInterval = 1000;
            
            // Set this the first time
            CurrentForegroundWindowTitle = GetForegroundWindowTitle();
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
                // Copy current foreground window title
                String previousForegroundWindowTitle = CurrentForegroundWindowTitle;

                // Update current foreground window title
                CurrentForegroundWindowTitle = GetForegroundWindowTitle();

                // Different?
                if (CurrentForegroundWindowTitle != String.Empty && CurrentForegroundWindowTitle != previousForegroundWindowTitle){
                    // Notify handler
                    handler.NewForegroundWindow(CurrentForegroundWindowTitle);
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
    }
}
