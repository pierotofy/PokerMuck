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
    public partial class ControlListContainer : UserControl
    {
        public ControlListContainer()
        {
            InitializeComponent();
        }


        /* Adds a new control to our canvas */
        public void AddPanel(Control control)
        {
            control.Dock = DockStyle.Left;
            control.Anchor = AnchorStyles.Right;
            Controls.Add(control);

            AdjustSizes();
            AdjustPositions();
        }

        /* Each component is put - like in a list - one after the other from top to bottom */
        private void AdjustPositions()
        {
            if (Controls.Count > 0)
            {
                int posY = 0;
                foreach (Control c in Controls)
                {
                    c.Top = posY;
                    posY += c.Height;
                }
            }
        }

        /* Space in a container is limited, so we need to make sure that there's enough space for everybody
         * or we need to resize stuff */
        private void AdjustSizes()
        {
            int numControls = Controls.Count;
            if (numControls > 0)
            {
                Control firstControl = Controls[0];
                int availableHeight = this.ClientSize.Height;
                int controlHeight = firstControl.Height;

                // Need to resize?
                if (availableHeight / numControls < controlHeight)
                {
                    int newHeight = (int)Math.Floor((double)availableHeight / (double)numControls);

                    foreach (Control c in Controls)
                    {
                        c.Height = newHeight;
                    }
                }
            }

        }

        /* Remove all controls */
        public void ClearAll()
        {
            Controls.Clear();
        }
    }
}
