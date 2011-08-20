using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Threading;

namespace PokerMuck
{
    class HHMonitor
    {
        private const int CHECK_INTERVAL = 2000;

        private bool monitoring;
        private String handHistoryFilename;
        private long currentFilesize;
        private String directory;
        private IHHMonitorHandler handler;
        private FilesLineTracker filesLineTracker;

        /* Keeps track of the last line sent */
        private string lastLine;

        private Object thisLock = new Object(); // Used for thread safety synchronization

        public HHMonitor(String handHistoryFilePath, IHHMonitorHandler handler)
        {
            this.handler = handler;
            this.monitoring = false;
            this.directory = Path.GetDirectoryName(handHistoryFilePath);
            this.handHistoryFilename = Path.GetFileName(handHistoryFilePath);
            this.filesLineTracker = new FilesLineTracker();
            this.lastLine = String.Empty;
            this.currentFilesize = 0;
        }

        public void StartMonitoring(){
            if (!monitoring){
                monitoring = true;

                Thread t = new Thread(MonitorLoop);
                t.Start();
            }
        }

        public void StopMonitoring(){
            if (monitoring){
                monitoring = false;
            }
        }

        private void MonitorLoop()
        {
            String filePath = GetFullHandHistoryPath();

            while (monitoring)
            {
                // First make sure the file exists
                if (File.Exists(filePath))
                {
                    // Get file size
                    FileInfo info = new FileInfo(filePath);
                    long newFilesize = info.Length;

                    // Different?
                    if (newFilesize != currentFilesize)
                    {
                        // File changed
                        CheckForFileChanges();

                        currentFilesize = newFilesize;
                    }
                }

                Thread.Sleep(CHECK_INTERVAL);
            }
        }

        /* Sends again to the handler the last line */
        public void ResendLastLine()
        {
            if (lastLine != String.Empty) handler.NewLineArrived(handHistoryFilename, lastLine);
        }

        public void CheckForFileChanges()
        {
            // You don't want multiple threads to be reading the same file at once, do you?
            // TODO: necessary?
            lock (thisLock)
            {
                String handHistoryFilePath = GetFullHandHistoryPath();

                /* When you first join a game the file hasn't been created yet... let's check if the files exist before 
                 * checking for changes */
                if (File.Exists(handHistoryFilePath))
                {
                    try
                    {
                        FileStream fileStream = new FileStream(handHistoryFilePath,
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.ReadWrite);

                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            int lastLineRead = filesLineTracker.GetLineCount(handHistoryFilename);

                            // If we started reading this file before, we can skip to the line of interest...
                            if (lastLineRead > 0) SkipNLines(reader, lastLineRead);

                            int linesRead = 0;

                            while (!reader.EndOfStream)
                            {
                                string newLine = reader.ReadLine();
                                newLine = newLine.Replace('�', '€');
                                
                                lastLine = newLine;
                                handler.NewLineArrived(handHistoryFilename, newLine);
                                linesRead++;
                            }

                            // Raise the end of file reached event if we have read at least one line
                            if (linesRead > 0) handler.EndOfFileReached(handHistoryFilename);

                            // Update files line tracker
                            filesLineTracker.IncreaseLineCount(handHistoryFilename, linesRead);
                        }
                    }
                    catch (IOException e)
                    {
                        Trace.WriteLine(e.ToString());
                        Trace.WriteLine(String.Format("Cannot read {0}, trying again later?", handHistoryFilePath));
                    }
                }
            }
        }

        /* Helper methods */
        private String GetFullHandHistoryPath()
        {
            return String.Format("{0}\\{1}", directory, handHistoryFilename);
        }

        private void SkipNLines(StreamReader s, int n)
        {
            for (int i = 0; i < n; i++)
                s.ReadLine();
        }

    }
}
