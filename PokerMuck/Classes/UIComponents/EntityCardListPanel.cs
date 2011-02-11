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
    public partial class EntityCardListPanel : UserControl
    {
        public EntityCardListPanel()
        {
            InitializeComponent();            
        }

        [Description("Sets the entity name displayed in the component"),
         Category("Values"),
         DefaultValue("entityName")]
        public String EntityName
        {
            get
            {
                return lblEntityName.Text;
            }

            set
            {
                lblEntityName.Text = value;
            }
        }

        [Description("Sets the hand displayed in the component"),
         Category("Values"),
         DefaultValue(null)]
        public CardList CardListToDisplay
        {
            get
            {
                return CardListPanel.CardListToDisplay;
            }

            set
            {
                CardListPanel.CardListToDisplay = value;
            }
        }
    }
}
