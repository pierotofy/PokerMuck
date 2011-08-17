using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PokerMuck
{
    public partial class SingleCardSelectDialog : Form
    {
        public String SelectedCard
        {
            get;
            set;
        }

        public SingleCardSelectDialog()
        {
            InitializeComponent();
            SelectedCard = "";
        }

        private void cardsSelector_CardSelected(string card)
        {
            SelectedCard = card;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
