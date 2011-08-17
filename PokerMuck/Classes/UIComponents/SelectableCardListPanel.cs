using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace PokerMuck
{
    class SelectableCardListPanel : CardListPanel
    {
        public delegate void CardSelectedHandler(Card c);
        public event CardSelectedHandler CardSelected; 

        private Color highlightColor;
        [Description("The color the card should be highlighted with on mouseover"),
         Category("Values")]
        public Color HighlightColor
        {
            get
            {
                return highlightColor;
            }

            set
            {
                highlightColor = value;

                foreach (SelectableCardPictureBox c in cardPictures)
                {
                    c.HighlightColor = value;
                }
            }
        }

        private int highlightTransparency;
        [Description("The transparency of the hightlight color when the card is highlighted"),
         Category("Values")]
        public int HighlightTransparency
        {
            get
            {
                return highlightTransparency;
            }

            set
            {
                if (value > 255) value = 255;
                else if (value < 0) value = 0;
                highlightTransparency = value;

                foreach (SelectableCardPictureBox c in cardPictures)
                {
                    c.HighlightTransparency = value;
                }
            }
        }

        public SelectableCardListPanel()
            : base()
        {
            highlightTransparency = 100;
            highlightColor = Color.White;
        }

        protected override void LoadCardPictures()
        {
            if (cardListToDisplay != null)
            {
                // Generate the card picture boxes for each card in the hand
                foreach (Card c in cardListToDisplay)
                {
                    SelectableCardPictureBox pictureBox = new SelectableCardPictureBox(c);
                    pictureBox.CardSelected += new SelectableCardPictureBox.CardSelectedHandler(pictureBox_CardSelected);
                    pictureBox.HighlightTransparency = HighlightTransparency;
                    pictureBox.HighlightColor = HighlightColor;

                    cardPictures.Add(pictureBox);
                }
            }
        }

        void pictureBox_CardSelected(Card c)
        {
            // bubble up
            if (CardSelected != null) CardSelected(c);
        }


    }
}
