using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PokerMuck
{
    public class CardPictureBox : System.Windows.Forms.PictureBox
    {
        protected Card card;

        // Store the original image height/width ratio
        private float heightWidthRatio;
        public float HeightWidthRatio { get { return heightWidthRatio; } }

        public CardPictureBox(Card card)
        {
            this.card = card;
            this.SizeMode = PictureBoxSizeMode.StretchImage;
            this.ClientSize = GetImageSize();
            this.heightWidthRatio = (float)ClientSize.Height / (float)ClientSize.Width;

            ReloadImage();
        }

        public void DisplayCard(Card card){
            this.card = card;
            ReloadImage();
        }

        /* High quality scaling
         * http://stackoverflow.com/questions/566245/how-to-draw-smooth-images-with-c
         */
        protected override void OnPaint(PaintEventArgs pe)
        {
            // This is the only line needed for anti-aliasing to be turned on.
            pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // the next two lines of code (not comments) are needed to get the highest 
            // possible quiality of anti-aliasing. Remove them if you want the image to render faster.
            //pe.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            pe.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            // this line is needed for .net to draw the contents.
            base.OnPaint(pe);
        }


        /* Helper methods */

        private void ReloadImage(){
            this.ImageLocation = GetImagePath();
        }

        private String GetImageFilename()
        {
            return String.Format("{0}_{1}.png", (int)card.Suit, (int)card.Face);
        }

        private String GetImagePath()
        {
            return "Resources/Cards/" + GetImageFilename();
        }

        private Size GetImageSize()
        {
            Image i = Image.FromFile(GetImagePath());
            return new Size(i.Width, i.Height);
        }
    }
}
