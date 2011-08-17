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
    public partial class CardButton : UserControl
    {
        private bool selected;
        [Description("Whether the control is selected"),
         Category("Values")]
        public bool Selected
        {
            get
            {
                return selected;
            }

            set
            {
                selected = value;
                if (selected) lblText.BackColor = selectedColor;
                else lblText.BackColor = backColor;
            }
        }

        private Color selectedColor;
        [Description("Color of the button when selected"),
         Category("Values"),
         DefaultValueAttribute(typeof(Color), "0xD77F12")]
        public Color SelectedColor
        {
            get
            {
                return selectedColor;
            }

            set
            {
                selectedColor = value;
            }
        }

        private Color hoverColor;
        [Description("Color of the button on mouse over"),
         Category("Values"),
         DefaultValueAttribute(typeof(Color), "0xDCB079")]
        public Color HoverColor
        {
            get
            {
                return hoverColor;
            }

            set
            {
                hoverColor = value;
            }
        }

        private Color backColor;
        [Description("Color of the button"),
         Category("Values")]
        public override Color BackColor
        {
            get
            {
                return backColor;
            }

            set
            {
                lblText.BackColor = value;
                backColor = value;
            }
        }

        [Description("Text of the control"),
         Category("Values")]
        public String Value
        {
            get
            {
                return lblText.Text;
            }

            set
            {
                lblText.Text = value;
            }
        }

        public CardButton()
        {
            InitializeComponent();
            hoverColor = Color.FromArgb(255, 220, 176, 121);
            selectedColor = Color.FromArgb(255, 215, 127, 18);

            lblText.BackColor = System.Drawing.SystemColors.Control;
            lblText.Text = "";
            selected = false;
        }

        private void lblText_MouseLeave(object sender, EventArgs e)
        {
            if (!selected) lblText.BackColor = backColor;
        }

        private void lblText_MouseEnter(object sender, EventArgs e)
        {
            if (!selected) lblText.BackColor = hoverColor;
        }

        private void lblText_Click(object sender, EventArgs e)
        {
            Selected = !Selected;

            this.OnClick(e);
        }
    }
}
