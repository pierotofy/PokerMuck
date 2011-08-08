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
    public partial class HoldemCardDisplayDialog : Form
    {
        public HoldemCardDisplayDialog()
        {
            InitializeComponent();
        }

        public void SelectCards(List<String> cards)
        {
            cardsSelector.SelectCards(cards);
        }
    }
}
