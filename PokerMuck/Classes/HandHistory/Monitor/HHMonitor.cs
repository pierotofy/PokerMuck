using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;

namespace PokerMuck
{
    class HHMonitor
    {
        private bool monitoring;
        private FileSystemWatcher fwatcher = null;
        private String handHistoryFilename;
        private String directory;
        private IHHMonitorHandler handler;
        private FilesLineTracker filesLineTracker;

        private Object thisLock = new Object(); // Used for thread safety synchronization

        public HHMonitor(String handHistoryFilePath, IHHMonitorHandler handler)
        {
            this.handler = handler;
            this.monitoring = false;
            this.directory = Path.GetDirectoryName(handHistoryFilePath);
            this.handHistoryFilename = Path.GetFileName(handHistoryFilePath);
            this.filesLineTracker = new FilesLineTracker();

            CreateSystemWatcher();
        }

        public void StartMonitoring(){
            if (!monitoring && fwatcher != null){
                monitoring = true;
                fwatcher.EnableRaisingEvents = true;
            }
        }

        public void StopMonitoring(){
            if (monitoring){
                monitoring = false;
                fwatcher.EnableRaisingEvents = false;
            }
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
                        using (StreamReader reader = new StreamReader(handHistoryFilePath))
                        {
                            int lastLineRead = filesLineTracker.GetLineCount(handHistoryFilename);

                            // If we started reading this file before, we can skip to the line of interest...
                            if (lastLineRead > 0) SkipNLines(reader, lastLineRead);

                            int linesRead = 0;

                            while (!reader.EndOfStream)
                            {
                                handler.NewLineArrived(handHistoryFilename, reader.ReadLine());
                                linesRead++;
                            }

                            // Raise the end of file reached event if we have read at least one line
                            if (linesRead > 0) handler.EndOfFileReached(handHistoryFilename);

                            // Update files line tracker
                            filesLineTracker.IncreaseLineCount(handHistoryFilename, linesRead);
                        }
                    }
                    catch (IOException)
                    {
                        Debug.Print(String.Format("Cannot read {0}, trying again later?", handHistoryFilePath));
                    }
                }
            }
        }

        private void File_Changed(object sender, FileSystemEventArgs e){
            // Is this the file we are monitoring?

            if (e.Name == handHistoryFilename)
            {
                CheckForFileChanges();
            }
            else
            {
                Debug.Print("Change detected, but filename is different: {0} != {1}", e.Name, handHistoryFilename);
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

        /* Initializes the system watcher (if needed) */
        private void CreateSystemWatcher()
        {
            if (fwatcher == null)
            {
                // First make sure the directory exists
                if (Directory.Exists(directory))
                {
                    fwatcher = new FileSystemWatcher(directory);
                    fwatcher.Changed += new FileSystemEventHandler(File_Changed);
                }
                else
                {
                    Debug.Print("Directory doesn't exist: {0}, skipping initialization of file system watcher.", directory);
                }
            }
        }
    }
}
