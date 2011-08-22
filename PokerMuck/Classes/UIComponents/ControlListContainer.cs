using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace PokerMuck
{
    /* This controls allows us to put one or more controls one after the other
     * top to bottom. It also allows us to specify the max height of each control we add. */
    public partial class ControlListContainer : UserControl
    {
        /* This hashtable maps every control to a maximum height */
        Hashtable maxHeightTable;

        public ControlListContainer()
        {
            maxHeightTable = new Hashtable();
            InitializeComponent();
        }


        /* Adds a new control to our canvas */
        public void AddPanel(Control control, int maximumHeight = 0)
        {
            control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(control);
            maxHeightTable[control] = maximumHeight;


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

                int newHeight = (int)Math.Floor((double)availableHeight / (double)numControls);

                foreach (Control c in Controls)
                {
                    // Is the new height bigger than what's allowed (zero means no limit)?
                    int allowedMaxHeight = (int)maxHeightTable[c];
                    if (allowedMaxHeight != 0 && newHeight > allowedMaxHeight)
                    {
                        // Set the control's height to the max allowed height instead
                        c.Height = allowedMaxHeight;
                    }
                    else
                    {
                        // We're good, new height is OK
                        c.Height = newHeight;
                    }

                    // The width is always the max we have at disposal
                    c.Width = this.Width;
                }
            }

        }

        /* Remove all controls */
        public void ClearAll()
        {
            Controls.Clear();
            maxHeightTable.Clear();
        }

        private void ControlListContainer_Resize(object sender, EventArgs e)
        {
            AdjustSizes();
            AdjustPositions();
        }
    }
}
