using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections;

namespace PokerMuck
{
    static class HHDirectoryParser
    {
        public class CompareFileByDate : IComparer
        {
            int IComparer.Compare(Object a, Object b)
            {
                FileInfo fia = new FileInfo((string)a);
                FileInfo fib = new FileInfo((string)b);

                DateTime cta = fia.CreationTime;
                DateTime ctb = fib.CreationTime;

                return DateTime.Compare(ctb, cta);
            }
        }


        /* Given a directory and a pattern to match, it returns the file that matches the pattern (if any)
         * it could return an empty string in case of failure */
        static public String GetHandHistoryFilenameFromRegexPattern(String directory, String pattern){
            if (pattern != String.Empty)
            {
                // Create regex
                Regex regex;
                try
                {
                    regex = new Regex(pattern);
                }
                catch (System.ArgumentException)
                {
                    // Invalid regex match
                    Trace.WriteLine("Warning! The pattern for the hand history could not be compiled: " + pattern);
                    return String.Empty;
                }

                try
                {
                    String[] files = System.IO.Directory.GetFiles(directory);

                    // Sorts the files by creation date DESC (newer first)
                    IComparer fileComparer = new CompareFileByDate();
                    Array.Sort(files, fileComparer);

                    foreach (String file in files)
                    {
                        Match match = regex.Match(file);
                        if (match.Success)
                        {
                            // Found a filename that corresponds to the pattern
                            return Path.GetFileName(file);
                        }
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    Trace.WriteLine(String.Format("Directory doesn't exist: {0}. Trying again later?", directory));
                    return String.Empty;
                }
            }

            return String.Empty;
        }
    }
}