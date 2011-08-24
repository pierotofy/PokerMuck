using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace PokerMuck
{
    /* This class takes care of tracking the line numbers on multiple files
     * ex. "file1.txt" => line #4
           "file2.txt" => line #54 
     This is used by the HHMonitor class to keep track of which lines have been read already and which haven't */
    class FilesLineTracker
    {
        /* Associative structure file => currentLine */
        private Hashtable filesData;

        public FilesLineTracker(){
            filesData = new Hashtable();
        }

        // Delete all previous records
        public void ResetAll()
        {
            filesData.Clear();
        }

        public void Reset(String filename)
        {
            if (filesData.ContainsKey(filename)) filesData.Remove(filename);
        }

        public void IncreaseLineCount(String filename, int increment = 1)
        {
            Trace.Assert(filename != String.Empty, "Tried to increase the line count for an empty string file");

            int previousValue = 0;
            if (filesData.ContainsKey(filename))
            {
                previousValue = (int)filesData[filename];
            }
            filesData[filename] = previousValue + increment;
        }

        public int GetLineCount(String filename)
        {
            if (filesData.ContainsKey(filename)) return (int)filesData[filename];
            else return 0;
        }
    }
}
