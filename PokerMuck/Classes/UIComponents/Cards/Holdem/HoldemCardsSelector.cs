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
    public partial class HoldemCardsSelector : UserControl
    {
        const int BUTTON_SIZE = 30;

        public HoldemCardsSelector()
        {
            InitializeComponent();
            Populate();
        }

        /* @param cards as in the starting hand notation (ex. AKs, AA, KQo) */
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
            int topPosition = 0;
            int leftPosition = 0;

            for (int i = 0; i < faces.Count<CardFace>(); i++)
            {
                CardFace f1 = faces[i];

                // Save the top and left position of the current line being created
                int lineTopPosition = topPosition;
                int lineLeftPosition = leftPosition;

                for (int j = i; j < faces.Count<CardFace>(); j++)
                {
                    CardFace f2 = faces[j];

                    // Suited
                    CardButton suitedButton = CreateCardButton(f1, f2, true);
                    suitedButton.Location = new System.Drawing.Point(leftPosition, topPosition);

                    // Moving to the right
                    leftPosition += BUTTON_SIZE;

                    if (f1 == f2)
                    {
                        suitedButton.BackColor = System.Drawing.Color.LightBlue;
                    }
                    else
                    {
                        suitedButton.BackColor = System.Drawing.Color.LightGreen;
                    }
                    this.Controls.Add(suitedButton);
                }

                leftPosition = lineLeftPosition;

                for (int j = i; j < faces.Count<CardFace>(); j++)
                {
                    CardFace f2 = faces[j];

                    // Offsuit
                    CardButton offsuitedButton = CreateCardButton(f1, f2, false);
                    offsuitedButton.Location = new System.Drawing.Point(leftPosition, topPosition);

                    // Moving to the bottom
                    topPosition += BUTTON_SIZE;

                    offsuitedButton.BackColor = System.Drawing.Color.LightSalmon;
                    this.Controls.Add(offsuitedButton);
                }

                topPosition = lineTopPosition + BUTTON_SIZE;
                leftPosition = lineLeftPosition + BUTTON_SIZE;
            }
        }

        private CardButton CreateCardButton(CardFace f1, CardFace f2, bool suited)
        {
            CardButton button = new CardButton();

            button.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            button.Selected = false;
            button.SelectedColor = System.Drawing.Color.Yellow;
            button.Size = new System.Drawing.Size(BUTTON_SIZE, BUTTON_SIZE);
            button.TabIndex = 0;

            String value;
            if (suited) value = HoldemHand.ConvertToString(f1, CardSuit.Clubs, f2, CardSuit.Clubs);
            else value = HoldemHand.ConvertToString(f1, CardSuit.Clubs, f2, CardSuit.Diamonds);

            button.Name = "btn" + value;
            button.Value = value;

            return button;
        }
    }

           
}
