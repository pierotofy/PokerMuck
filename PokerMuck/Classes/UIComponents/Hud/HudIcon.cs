using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace PokerMuck
{
    public partial class HudIcon : UserControl
    {
        private Image questionIcon;
        private Image originalIcon;
        private ToolTip toolTipMaker = null; // Might be null
        private String toolTipMessage;

        public HudIcon(Bitmap icon, String toolTipMessage = "")
        {
            InitializeComponent();

            this.toolTipMessage = toolTipMessage;

            // Save reference to original image
            this.originalIcon = icon;

            // Set icon
            picIcon.Image = icon;

            // Set question icon
            questionIcon = global::PokerMuck.Properties.Resources.QuestionIco;

            // Initialize tooltip
            if (toolTipMessage != "")
            {
                /* Create tooltips */
                toolTipMaker = new ToolTip();

                toolTipMaker.ShowAlways = true;

                toolTipMaker.InitialDelay = 0;
                toolTipMaker.AutoPopDelay = 0;
                toolTipMaker.ReshowDelay = 0;
                toolTipMaker.IsBalloon = true;

                toolTipMaker.SetToolTip(picIcon, toolTipMessage);
            }
        }

        /* Displays/Hides a forbidden sign on top of the icon */
        public void SetQuestionSignVisible(bool visibleFlag)
        {
            // Remove all tooltips (we will create new ones)
            toolTipMaker.RemoveAll();

            if (visibleFlag)
            {
                picIcon.Image = questionIcon;

                if (toolTipMaker != null)
                {
                    toolTipMaker.SetToolTip(picIcon, "Don't know if player is a " + toolTipMessage);
                }
            }
            else
            {
                picIcon.Image = originalIcon;

                if (toolTipMaker != null)
                {
                    toolTipMaker.SetToolTip(picIcon, toolTipMessage);
                }
            }
        }

        /* Modified from http://stackoverflow.com/questions/4623165/make-overlapping-picturebox-transparent-in-c-net
         * Thanks Hans Passant */
        public static Bitmap MergeImages(Image image1, Image image2)
        {
            //a holder for the result
            Bitmap result = new Bitmap(Math.Max(image1.Width, image2.Width), Math.Max(image1.Height, image2.Height));

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
               
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                 
                //draw the images into the target bitmap
                graphics.DrawImage(image1, 0, 0, result.Width, result.Height);
                graphics.DrawImage(image2, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }

    }
}
