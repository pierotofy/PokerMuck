using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using AForge.Imaging;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;

namespace PokerMuck
{
    class VisualMatcher
    {
        private bool trainingMode;
        private PokerClient client;
        private List<String> cardMatchFiles;

        private String CardMatchesDirectory
        {
            get
            {
                return String.Format(@".\Resources\CardMatches\{0}\{1}\", client.Name, client.CurrentTheme);
            }
        }


        public VisualMatcher(PokerClient client, bool trainingMode)
        {
            this.client = client;
            this.trainingMode = trainingMode;
            this.cardMatchFiles = new List<String>();

            ReplicateCommonCardsForCurrentClient();
            LoadCardMatchFilesList();
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
                Debug.Print(@"Copying {0}{1}", target.FullName, fi.Name);
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

        /* Tries to match a set of bitmaps into a card list. If any card
         * fails to match, the whole operation is aborted and null is returned */
        public CardList MatchCards(List<Bitmap> images){
            CardList result = new CardList();

            images[0].Save("1.bmp", ImageFormat.Bmp);
            images[1].Save("2.bmp", ImageFormat.Bmp);

            foreach (Bitmap image in images)
            {
                Card card = MatchCard(image);
                if (card != null)
                {
                    result.AddCard(card);
                }
                else
                {
                    return null;
                }
            }

            return result;
        }

        /* It returns null if there are no good matches */
        public Card MatchCard(Bitmap image)
        {
            float maxSimilarity = 0.0f;
            String bestMatchFilename = "";

            foreach (String cardMatchFile in cardMatchFiles)
            {
                Bitmap candidateImage = new Bitmap(cardMatchFile);

                // candidateImage should be smaller than image
                candidateImage = ScaleIfBiggerThan(image, candidateImage);

                ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
                TemplateMatch[] matchings = tm.ProcessImage(image, candidateImage);

                if (matchings[0].Similarity > maxSimilarity)
                {
                    maxSimilarity = matchings[0].Similarity;
                    bestMatchFilename = cardMatchFile;
                }
            }

            Debug.Print("Matched card with " + bestMatchFilename + "(" + maxSimilarity + ")");

            // If the best match is less than 70%, chances are we didn't match anything (or we need to improve our mapping)
            if (maxSimilarity > 0.7f) return Card.CreateFromPath(bestMatchFilename);
            else return null;
        }

        /* Keep proportions */
        private Bitmap ScaleIfBiggerThan(Bitmap otherImage, Bitmap sourceImage)
        {
            bool needsScaling = false;
            float widthScaleFactor = 0.0f;
            float heightScaleFactor = 0.0f;

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
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(sourceImage, 0, 0, newWidth, newHeight);
                g.Dispose();

                return result;
            }
        }
    }
}
