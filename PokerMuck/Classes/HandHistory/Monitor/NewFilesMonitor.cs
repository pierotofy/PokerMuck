using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace PokerMuck
{
    /* Notifies of newly created files in a specific directory */
    class NewFilesMonitor
    {
        private FileSystemWatcher fwatcher = null;
        private INewFilesMonitorHandler handler;
        private String directoryToMonitor;


        public NewFilesMonitor(String directoryToMonitor, INewFilesMonitorHandler handler)
        {
            this.directoryToMonitor = directoryToMonitor;
            this.handler = handler;

            // First make sure the directory exists
            if (Directory.Exists(directoryToMonitor))
            {
                fwatcher = new FileSystemWatcher(directoryToMonitor);
                fwatcher.Created += new FileSystemEventHandler(File_Created);
            }
            else
            {
                Trace.WriteLine(String.Format("Directory doesn't exist: {0}, skipping initialization of file system watcher in newfilesmonitor class.", directoryToMonitor));
            }
        }
                            
        private void File_Created(object sender, FileSystemEventArgs e)
        {
            handler.NewFileWasCreated(e.Name);
        }

        public void StartMonitoring()
        {
            if (fwatcher != null)
                fwatcher.EnableRaisingEvents = true;
        }

        public void StopMonitoring()
        {
            if (fwatcher != null)
                fwatcher.EnableRaisingEvents = false;
        }
    }
}
