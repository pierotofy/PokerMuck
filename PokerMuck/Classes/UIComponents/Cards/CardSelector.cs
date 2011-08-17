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
    public partial class CardSelector : UserControl
    {
        const int BUTTON_SIZE = 30;

        public delegate void CardSelectedHandler(String card);
        public event CardSelectedHandler CardSelected;

        public delegate void CardDeselectedHandler(String card);
        public event CardDeselectedHandler CardDeselected; 

        public CardSelector()
        {
            InitializeComponent();
            Populate();
        }

        /* @param cards as in the text translation (Ace of spades = As) */
        public void SelectCards(List<String> cards)
        {
            foreach (CardButton b in this.Controls)
            {
                b.Selected = false;
            }

            foreach(String card in cards){
                ((CardButton)this.Controls["btn" + card]).Selected = true;
            }
        }

        private void Populate()
        {
            CardFace[] faces = { CardFace.Ace, CardFace.King, CardFace.Queen, CardFace.Jack, CardFace.Ten, 
                                 CardFace.Nine, CardFace.Eight, CardFace.Seven, CardFace.Six, CardFace.Five, 
                                 CardFace.Four, CardFace.Three, CardFace.Two };
            CardSuit[] suits = { CardSuit.Clubs, CardSuit.Diamonds, CardSuit.Hearts, CardSuit.Spades };

            int topPosition = 0;
            int leftPosition = 0;

            foreach(CardSuit suit in suits)
            {
                foreach (CardFace face in faces)
                {
                    CardButton button = CreateCardButton(face, suit);
                    button.Location = new System.Drawing.Point(leftPosition, topPosition);

                    this.Controls.Add(button);

                    topPosition += BUTTON_SIZE;
                }

                leftPosition += BUTTON_SIZE;
                topPosition = 0;
            }
        }

        private CardButton CreateCardButton(CardFace face, CardSuit suit)
        {
            CardButton button = new CardButton();

            button.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            button.Selected = false;
            button.SelectedColor = System.Drawing.Color.Yellow;
            button.Size = new System.Drawing.Size(BUTTON_SIZE, BUTTON_SIZE);
            button.TabIndex = 0;

            // Make sure the suit is lower cased
            String value = Card.CardFaceToChar(face) + new String(Card.CardSuitToChar(suit), 1).ToLower();
            button.Name = "btn" + value;
            button.Value = value;
            button.Click += new EventHandler(button_Click);

            return button;
        }

        public List<String> GetSelectedCards()
        {
            List<String> result = new List<String>();

            foreach (CardButton c in this.Controls)
            {
                if (c.Selected)
                {
                    result.Add(c.Value);
                }
            }

            return result;
        }

        void button_Click(object sender, EventArgs e)
        {
            CardButton button = (CardButton)sender;

            if (button.Selected)
            {
                if (CardSelected != null) CardSelected(button.Value);
            }
            else
            {
                if (CardDeselected != null) CardDeselected(button.Value);
            }
        }
    }

           
}
