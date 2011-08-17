using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace PokerMuck
{
    public partial class CardMatchSelectDialog : Form
    {
        public Card SelectedCard { get; set; }

        public CardMatchSelectDialog()
        {
            InitializeComponent();
            cardListPanel.CardListToDisplay = new CardList();
        }

        public void AddPossibleCardMatch(Card card)
        {
            cardListPanel.AddCardToList(card);
        }

        public void DisplayImageToMatch(Image image)
        {
            picCardToMatch.Image = image;
        }

        private void cardListPanel_CardSelected(Card c)
        {
            SelectedCard = c;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();               
        }

        private void btnViewAll_Click(object sender, EventArgs e)
        {
            SingleCardSelectDialog d = new SingleCardSelectDialog();
            d.ShowDialog();

            if (d.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                SelectedCard = Card.CreateFromString(d.SelectedCard);
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.Close();
        }
    }
}
