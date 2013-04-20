using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using AForge.Imaging;
using AForge.Math;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Collections;

namespace PokerMuck
{
    class VisualMatcher
    {
        const double PERFECT_MATCH_HISTOGRAM_THRESHOLD = 0.001d;
        const double POSSIBLE_MATCH_TEMPLATE_THRESHOLD = 0.7d;

        private PokerClient client;
        private List<String> cardMatchFiles;
        private Point cardMatchDialogSpawnLocation; 

        private String CardMatchesDirectory
        {
            get
            {
                return String.Format(@".\Resources\CardMatches\{0}\{1}\", client.Name, client.CurrentTheme);
            }
        }


        public VisualMatcher(PokerClient client)
        {
            this.client = client;
            this.cardMatchFiles = new List<String>();
            this.cardMatchDialogSpawnLocation = new Point(200, 200);

            ReplicateCommonCardsForCurrentClient();
            LoadCardMatchFilesList();
        }

        public void SetCardMatchDialogSpawnLocation(int x, int y)
        {
            cardMatchDialogSpawnLocation = new Point(x, y);
        }


        private void LoadCardMatchFilesList()
        {

            DirectoryInfo di = new DirectoryInfo(CardMatchesDirectory);

            // Copy each file into it’s new directory.
            foreach (FileInfo fi in di.GetFiles())
            {
                cardMatchFiles.Add(fi.FullName);
            }
        }

        /* Card matches are done with a common set of cards
            this is preliminary however, as different decks have different images (different fonts, shapes, even colors)
            so matches made with this set will not be 100% correct. As the system learns, cards in the common set
            for a client are replaced by more accurate images (taken directly from the poker client interface) */

        /* We make copies of the common set of cards so that we do not delete the originals 
         * Every poker client and every theme of a poker client needs a different set */
        private void ReplicateCommonCardsForCurrentClient()
        {
            if (!Directory.Exists(CardMatchesDirectory))
            {
                CopyDirectoryAll(new DirectoryInfo(@".\Resources\CardMatches\Common\"), new DirectoryInfo(CardMatchesDirectory));
            }
        }

        // Taken from: http://xneuron.wordpress.com/2007/04/12/copy-directory-and-its-content-to-another-directory-in-c/
        public static void CopyDirectoryAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it’s new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Trace.WriteLine(String.Format(@"Copying {0}{1}", target.FullName, fi.Name));
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                // Ignore .svn directories
                if (diSourceSubDir.Name == ".svn") continue;

                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectoryAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        /* Tries to match a set of bitmaps into a card list. 
         * @param allowPartialMatch If any card fails to match, the operation is aborted and the results obtained so far are returned
         *  (but they might be incomplete). If this parameter is set to false, null is returned on failure. */
        public CardList MatchCards(List<Bitmap> images, bool allowPartialMatch)
        {
            if (images.Count == 0) return null;

            CardList result = new CardList();

            foreach (Bitmap image in images)
            {
                Card card = MatchCard(image);
                if (card != null)
                {
                    result.AddCard(card);
                }
                else if (allowPartialMatch)
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }

            return result;
        }

        /* Given two images it makes a histogram comparison of its color channels
         * and returns a double indicating the level of similarity.
         * The smaller the value, the more similar the images are */
        private double HistogramBitmapDifference(Bitmap image1, Bitmap image2)
        {
            ImageStatistics stats1 = new ImageStatistics(image1);
            ImageStatistics stats2 = new ImageStatistics(image2);

            double blueDiff = Math.Abs(stats1.Blue.Mean - stats2.Blue.Mean);
            double greenDiff = Math.Abs(stats1.Green.Mean - stats2.Green.Mean);
            double redDiff = Math.Abs(stats1.Red.Mean - stats2.Red.Mean);

            return ((redDiff + blueDiff + greenDiff) / 3.0d);
        }

        /* Assume the two images are in the correct format
         * 1 = perfectly equal
           0 = totally different */
        private double TemplateMatchingSimilarity(Bitmap image, Bitmap template)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(image, template);
            return (double)matchings[0].Similarity;
        }

        /* It returns null if there are no good matches 
         * If training mode is enabled, a window might ask the user to aid the algorithm find the best match */
        public Card MatchCard(Bitmap image)
        {
            if (image == null) return null;

            double minDifference = Double.MaxValue;
            double maxSimilarity = 0.0d;
            String bestMatchFilename = "";

            /* We keep a hashtable of possible matches (filename => match %)
             * so that if training mode is enabled we can access it to figure out which
             * are the most likely candidates */
            Hashtable possibleMatches = new Hashtable();


            foreach (String cardMatchFile in cardMatchFiles)
            {
                Bitmap candidateImage = new Bitmap(cardMatchFile);

                bool sizesMatch = candidateImage.Height == image.Height && candidateImage.Width == image.Width;

                // For template matching template should be smaller than image
                candidateImage = ScaleIfBiggerThan(image, candidateImage);


                double difference = HistogramBitmapDifference(image, candidateImage);
                double similarity = TemplateMatchingSimilarity(image, candidateImage);

                if (difference < minDifference)
                {
                    minDifference = difference;
                    bestMatchFilename = cardMatchFile;
                }

                if (similarity > maxSimilarity)
                {
                    maxSimilarity = similarity;
                }

                /* We use histogram difference to match perfect copies of the image (after the training phase is over)
                 * but if they are different, we use the template matching as an indicator of similarity */

                if (difference > PERFECT_MATCH_HISTOGRAM_THRESHOLD && similarity > POSSIBLE_MATCH_TEMPLATE_THRESHOLD)
                {
                    /* Our set of common cards is typically larger than the actual client matches 
                     * (although there could be rare exceptions that would not influence negatively the behavior of the algorithm).
                     * So, given that same size images have already been matched by the user during training mode,
                     * we can pretty safely ignore them, since
                     * they are likely to be perfectly matched with another target (and this block would have not been executed) */

                    if (!sizesMatch) possibleMatches.Add(cardMatchFile, similarity);
                }

                candidateImage.Dispose();
            }

            //Trace.WriteLine("Matched " + bestMatchFilename + " (Difference: " + minDifference + ")");

            Card matchedCard = null; // Hold the return value

            /* If we have a possible match, but we are not too sure about it, we can ask the user to confirm our guesses */
            if (Globals.UserSettings.TrainingModeEnabled)
            {
                if (minDifference > PERFECT_MATCH_HISTOGRAM_THRESHOLD && maxSimilarity > POSSIBLE_MATCH_TEMPLATE_THRESHOLD)
                {
                    Trace.WriteLine("Min difference too high (" + minDifference + ") and max similarity above threshold, asking user to confirm our guesses");

                    Card userCard = null;
                    Globals.Director.RunFromGUIThread((Action)delegate()
                            {
                                userCard = AskUserToConfirm(image, possibleMatches);
                            },false
                    );
                    
                    if (userCard != null)
                    {
                        matchedCard = userCard;
                    }
                }
            }

            // If the user has selected a card, matchedCard is an object and this is skipped
            if (minDifference < PERFECT_MATCH_HISTOGRAM_THRESHOLD && matchedCard == null && bestMatchFilename != "") matchedCard = Card.CreateFromPath(bestMatchFilename);

            return matchedCard;
        }

        /* If the user selects a card, returns the card, otherwise null */
        private Card AskUserToConfirm(Bitmap targetImage, Hashtable possibleMatches)
        {
            /*
            if (possibleMatches.Count == 0)
            {
                Trace.WriteLine("Warning: We should ask the user to confirm an image, but there are no possible matches...");
                return null;
            }*/

            CardMatchSelectDialog dialog = new CardMatchSelectDialog();
            dialog.Location = cardMatchDialogSpawnLocation;
            dialog.DisplayImageToMatch(targetImage);

            // Create list
            List<String> orderedFilenames = new List<String>();

            foreach (String cardMatchFile in possibleMatches.Keys)
            {
                orderedFilenames.Add(cardMatchFile);

                Trace.WriteLine(cardMatchFile + " has similarity of " + possibleMatches[cardMatchFile]);
            }

            orderedFilenames.Sort((delegate(String file1, String file2)
            {
                return ((double)possibleMatches[file2]).CompareTo((double)possibleMatches[file1]);
            }));

            // Create cards from filenames
            const int MAX_CARDS_TO_DISPLAY = 7;
            int i = 0;
            foreach (String filename in orderedFilenames)
            {
                dialog.AddPossibleCardMatch(Card.CreateFromPath(filename));
                i++;
                if (i == MAX_CARDS_TO_DISPLAY) break;
            }

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // The user selected a card
                Card userCard = dialog.SelectedCard;

                /* Before returning to the caller, we replace the image associated with the card
                 * with the targetimage. This process allows us to slowly replace the common card images templates
                 * with images that come directly from the poker client, allowing us to achieve an almost perfect match
                 * the next time the same card is compared */
                ReplaceTemplateImageWith(userCard, targetImage);

                return userCard;
            }

            return null;
        }

        private void ReplaceTemplateImageWith(Card matchedCard, Bitmap newImage)
        {
            // Find the path of the template associated with the card
            String targetFile = String.Format("{0}{1}", CardMatchesDirectory, matchedCard.ToFilename("bmp"));

            try
            {
                newImage.Save(targetFile, ImageFormat.Bmp);
                Trace.WriteLine("Successfully replaced " + targetFile + " with new image");
            }
            catch (Exception)
            {
                Trace.WriteLine("Warning! Tried to replace " + targetFile + " with a new image but failed because of an exception");
            }
        }

        /* Keep proportions
         * If scaling is done, the source image is disposed */
        private Bitmap ScaleIfBiggerThan(Bitmap otherImage, Bitmap sourceImage)
        {
            bool needsScaling = false;
            float widthScaleFactor = 1.0f;
            float heightScaleFactor = 1.0f;

            if (sourceImage.Width > otherImage.Width)
            {
                needsScaling = true;
                widthScaleFactor = (float)otherImage.Width / (float)sourceImage.Width;
            }

            if (sourceImage.Height > otherImage.Height)
            {
                needsScaling = true;
                heightScaleFactor = (float)otherImage.Height / (float)sourceImage.Height;
            }

            if (!needsScaling) return sourceImage;
            else
            {
                float scaleFactor = Math.Min(widthScaleFactor, heightScaleFactor);
                int newWidth = (int)((float)sourceImage.Width * scaleFactor);
                int newHeight = (int)((float)sourceImage.Height * scaleFactor);

                Bitmap result = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage((System.Drawing.Image)result);

                //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;

                g.DrawImage(sourceImage, 0, 0, newWidth, newHeight);
                g.Dispose();
                sourceImage.Dispose();

                return result;
            }
        }
    }
}
