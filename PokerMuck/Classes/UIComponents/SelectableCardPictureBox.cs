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
    class SelectableCardPictureBox : CardPictureBox
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

            }
        }

        // Is the card highlighted (mouse over?)
        bool highlighted;

        public SelectableCardPictureBox(Card card)
            : base(card)
        {
            highlighted = false;
            this.highlightTransparency = 100;
            this.highlightColor = Color.White;

            this.MouseEnter += new EventHandler(SelectableCardPictureBox_MouseEnter);
            this.MouseLeave += new EventHandler(SelectableCardPictureBox_MouseLeave);
            this.MouseClick += new MouseEventHandler(SelectableCardPictureBox_MouseClick);
        }

        void SelectableCardPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (CardSelected != null) CardSelected(this.card);
        }

        void SelectableCardPictureBox_MouseLeave(object sender, EventArgs e)
        {
            highlighted = false;
            this.Cursor = Cursors.Default;
            this.Refresh();
        }

        void SelectableCardPictureBox_MouseEnter(object sender, EventArgs e)
        {
            highlighted = true;
            this.Cursor = Cursors.Hand;
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (highlighted)
            {
                Color brushColor = Color.FromArgb(highlightTransparency, highlightColor);
                pe.Graphics.FillRectangle(new SolidBrush(brushColor), this.ClientRectangle);
            }
        }
        
    }
}
