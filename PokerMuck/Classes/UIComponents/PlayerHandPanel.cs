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
    public partial class PlayerHandPanel : UserControl
    {
        public PlayerHandPanel()
        {
            InitializeComponent();            
        }


        // On resize, we need to recompute sizes and positions
        protected override void OnResize(EventArgs e)
        {
            AdjustComponents();
            base.OnResize(e);
        }

        // Adjust sizes and positions of components
        private void AdjustComponents()
        {
            // Sizes
            handPanel.Size = new Size(this.Size.Width - lblPlayerName.Size.Width - 28, this.Size.Height - 10);
            lblline.Size = new Size(this.Size.Width - 4, 2);
            lblPlayerName.Size = new Size(lblPlayerName.Size.Width, this.Size.Height - 16);

            // Positions
            lblline.Top = handPanel.Top + handPanel.Height + 4;
        }

        [Description("Sets the player name displayed in the component"),
         Category("Values"),
         DefaultValue("playerName")]
        public String PlayerName
        {
            get
            {
                return lblPlayerName.Text;
            }

            set
            {
                lblPlayerName.Text = value;
            }
        }

        [Description("Sets the hand displayed in the component"),
         Category("Values"),
         DefaultValue(null)]
        public Hand HandToDisplay
        {
            get
            {
                return handPanel.HandToDisplay;
            }

            set
            {
                handPanel.HandToDisplay = value;
            }
        }
    }
}
