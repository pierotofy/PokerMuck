namespace PokerMuck
{
    partial class HoldemHudWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblImmediateStats = new System.Windows.Forms.Label();
            this.picEasySteal = new System.Windows.Forms.PictureBox();
            this.picButtonStealer = new System.Windows.Forms.PictureBox();
            this.picCallingStation = new System.Windows.Forms.PictureBox();
            this.picSolidPlayer = new System.Windows.Forms.PictureBox();


            ((System.ComponentModel.ISupportInitialize)(this.picEasySteal)).BeginInit();
            this.SuspendLayout();
            // 
            // lblImmediateStats
            // 
            this.lblImmediateStats.BackColor = System.Drawing.Color.Transparent;
            this.lblImmediateStats.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.lblImmediateStats.Location = new System.Drawing.Point(0, 35);
            this.lblImmediateStats.Name = "lblImmediateStats";
            this.lblImmediateStats.Size = new System.Drawing.Size(188, 21);
            this.lblImmediateStats.TabIndex = 1;
            this.lblImmediateStats.Text = "VPF";
            this.lblImmediateStats.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.HoldemHudWindow_MouseDoubleClick);
            this.lblImmediateStats.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblImmediateStats_MouseDown);
            this.lblImmediateStats.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblImmediateStats_MouseMove);
            this.lblImmediateStats.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblImmediateStats_MouseUp);
            // 
            // picEasySteal
            // 
            this.picEasySteal.Location = new System.Drawing.Point(3, 18);
            this.picEasySteal.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.picEasySteal.Name = "picEasySteal";
            this.picEasySteal.Image = global::PokerMuck.Properties.Resources.GoldIco;
            this.picEasySteal.BackColor = System.Drawing.Color.Transparent;
            this.picEasySteal.Size = new System.Drawing.Size(16, 14);          
            this.picEasySteal.TabIndex = 2;
            this.picEasySteal.TabStop = false;

            // 
            // picButtonStealer
            // 
            this.picButtonStealer.Location = new System.Drawing.Point(21, 18);
            this.picButtonStealer.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.picButtonStealer.Name = "picButtonStealer";
            this.picButtonStealer.Image = global::PokerMuck.Properties.Resources.StealMaskIco;
            this.picButtonStealer.BackColor = System.Drawing.Color.Transparent;
            this.picButtonStealer.Size = new System.Drawing.Size(16, 14);
            this.picButtonStealer.TabIndex = 2;
            this.picButtonStealer.TabStop = false;

            // 
            // picCallingStation
            // 
            this.picCallingStation.Location = new System.Drawing.Point(39, 18);
            this.picCallingStation.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.picCallingStation.Name = "picCallingStation";
            this.picCallingStation.Image = global::PokerMuck.Properties.Resources.TelephoneIco;
            this.picCallingStation.BackColor = System.Drawing.Color.Transparent;
            this.picCallingStation.Size = new System.Drawing.Size(16, 14);
            this.picCallingStation.TabIndex = 2;
            this.picCallingStation.TabStop = false;

            // 
            // picSolidPlayer
            // 
            this.picSolidPlayer.Location = new System.Drawing.Point(58, 18);
            this.picSolidPlayer.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.picSolidPlayer.Name = "picSolidPlayer";
            this.picSolidPlayer.Image = global::PokerMuck.Properties.Resources.AnvilIco;
            this.picSolidPlayer.BackColor = System.Drawing.Color.Transparent;
            this.picSolidPlayer.Size = new System.Drawing.Size(16, 14);
            this.picSolidPlayer.TabIndex = 2;
            this.picSolidPlayer.TabStop = false;

            // 
            // HoldemHudWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(130, 54);
            this.Controls.Add(this.picEasySteal);
            this.Controls.Add(this.picButtonStealer);
            this.Controls.Add(this.picCallingStation);
            this.Controls.Add(this.picSolidPlayer);

            this.Controls.Add(this.lblImmediateStats);
            this.Name = "HoldemHudWindow";
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.HoldemHudWindow_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblImmediateStats_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblImmediateStats_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblImmediateStats_MouseUp);
            this.Controls.SetChildIndex(this.lblImmediateStats, 0);
            this.Controls.SetChildIndex(this.picEasySteal, 0);
            ((System.ComponentModel.ISupportInitialize)(this.picEasySteal)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblImmediateStats;
        private System.Windows.Forms.PictureBox picEasySteal;
        private System.Windows.Forms.PictureBox picButtonStealer;
        private System.Windows.Forms.PictureBox picCallingStation;
        private System.Windows.Forms.PictureBox picSolidPlayer;

    }
}