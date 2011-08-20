namespace PokerMuck
{
    partial class TableDisplayWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableDisplayWindow));
            this.tabControl = new Dotnetrix.Controls.TabControlEX();
            this.tabMuck = new Dotnetrix.Controls.TabPageEX();
            this.entityHandsContainer = new PokerMuck.ControlListContainer();
            this.tabHand = new Dotnetrix.Controls.TabPageEX();
            this.handControlList = new PokerMuck.ControlListContainer();
            this.tabStats = new Dotnetrix.Controls.TabPageEX();
            this.statisticsDisplay = new PokerMuck.StatisticsDisplay();
            this.lblPlayerStatisticsNote = new System.Windows.Forms.Label();
            this.lblVisualRecognitionNotSupported = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabMuck.SuspendLayout();
            this.tabHand.SuspendLayout();
            this.tabStats.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Appearance = Dotnetrix.Controls.TabAppearanceEX.FlatButton;
            this.tabControl.BackColor = System.Drawing.Color.Transparent;
            this.tabControl.Controls.Add(this.tabMuck);
            this.tabControl.Controls.Add(this.tabHand);
            this.tabControl.Controls.Add(this.tabStats);
            this.tabControl.Location = new System.Drawing.Point(2, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 1;
            this.tabControl.SelectedTabColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(115)))), ((int)(((byte)(72)))));
            this.tabControl.Size = new System.Drawing.Size(231, 340);
            this.tabControl.TabIndex = 0;
            this.tabControl.UseVisualStyles = false;
            // 
            // tabMuck
            // 
            this.tabMuck.BackColor = System.Drawing.Color.Transparent;
            this.tabMuck.BackgroundImage = global::PokerMuck.Properties.Resources.TableDisplayTabBackground;
            this.tabMuck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabMuck.Controls.Add(this.entityHandsContainer);
            this.tabMuck.Location = new System.Drawing.Point(4, 25);
            this.tabMuck.Name = "tabMuck";
            this.tabMuck.Padding = new System.Windows.Forms.Padding(5);
            this.tabMuck.Size = new System.Drawing.Size(223, 311);
            this.tabMuck.TabIndex = 0;
            this.tabMuck.Text = "Muck";
            // 
            // entityHandsContainer
            // 
            this.entityHandsContainer.BackColor = System.Drawing.Color.Transparent;
            this.entityHandsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityHandsContainer.Location = new System.Drawing.Point(5, 5);
            this.entityHandsContainer.Name = "entityHandsContainer";
            this.entityHandsContainer.Size = new System.Drawing.Size(213, 301);
            this.entityHandsContainer.TabIndex = 5;
            // 
            // tabHand
            // 
            this.tabHand.BackColor = System.Drawing.Color.Transparent;
            this.tabHand.BackgroundImage = global::PokerMuck.Properties.Resources.TableDisplayTabBackground;
            this.tabHand.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabHand.Controls.Add(this.lblVisualRecognitionNotSupported);
            this.tabHand.Controls.Add(this.handControlList);
            this.tabHand.Location = new System.Drawing.Point(4, 25);
            this.tabHand.Name = "tabHand";
            this.tabHand.Padding = new System.Windows.Forms.Padding(5);
            this.tabHand.Size = new System.Drawing.Size(223, 311);
            this.tabHand.TabIndex = 1;
            this.tabHand.Text = "Hand";
            // 
            // handControlList
            // 
            this.handControlList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.handControlList.Location = new System.Drawing.Point(5, 5);
            this.handControlList.Name = "handControlList";
            this.handControlList.Size = new System.Drawing.Size(213, 301);
            this.handControlList.TabIndex = 0;
            // 
            // tabStats
            // 
            this.tabStats.BackColor = System.Drawing.Color.Transparent;
            this.tabStats.BackgroundImage = global::PokerMuck.Properties.Resources.TableDisplayTabBackground;
            this.tabStats.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabStats.Controls.Add(this.statisticsDisplay);
            this.tabStats.Controls.Add(this.lblPlayerStatisticsNote);
            this.tabStats.Location = new System.Drawing.Point(4, 25);
            this.tabStats.Name = "tabStats";
            this.tabStats.Padding = new System.Windows.Forms.Padding(5);
            this.tabStats.Size = new System.Drawing.Size(223, 311);
            this.tabStats.TabIndex = 2;
            this.tabStats.Text = "Stats";
            // 
            // statisticsDisplay
            // 
            this.statisticsDisplay.BackColor = System.Drawing.Color.White;
            this.statisticsDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statisticsDisplay.Location = new System.Drawing.Point(5, 5);
            this.statisticsDisplay.Name = "statisticsDisplay";
            this.statisticsDisplay.Size = new System.Drawing.Size(213, 301);
            this.statisticsDisplay.StatisticsSpacing = 2;
            this.statisticsDisplay.TabIndex = 8;
            this.statisticsDisplay.TopMargin = 8;
            this.statisticsDisplay.Visible = false;
            // 
            // lblPlayerStatisticsNote
            // 
            this.lblPlayerStatisticsNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPlayerStatisticsNote.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayerStatisticsNote.Location = new System.Drawing.Point(5, 5);
            this.lblPlayerStatisticsNote.Name = "lblPlayerStatisticsNote";
            this.lblPlayerStatisticsNote.Size = new System.Drawing.Size(213, 301);
            this.lblPlayerStatisticsNote.TabIndex = 7;
            this.lblPlayerStatisticsNote.Text = "Double-Click on a Hud Window to display player\'s statistics.";
            this.lblPlayerStatisticsNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVisualRecognitionNotSupported
            // 
            this.lblVisualRecognitionNotSupported.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblVisualRecognitionNotSupported.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVisualRecognitionNotSupported.Location = new System.Drawing.Point(5, 5);
            this.lblVisualRecognitionNotSupported.Name = "lblVisualRecognitionNotSupported";
            this.lblVisualRecognitionNotSupported.Size = new System.Drawing.Size(213, 301);
            this.lblVisualRecognitionNotSupported.TabIndex = 8;
            this.lblVisualRecognitionNotSupported.Text = resources.GetString("lblVisualRecognitionNotSupported.Text");
            this.lblVisualRecognitionNotSupported.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVisualRecognitionNotSupported.Visible = false;
            // 
            // TableDisplayWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::PokerMuck.Properties.Resources.TableDisplayWindowBackground;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(234, 344);
            this.Controls.Add(this.tabControl);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TableDisplayWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TableDisplayWindow";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this.ResizeEnd += new System.EventHandler(this.TableDisplayWindow_ResizeEnd);
            this.LocationChanged += new System.EventHandler(this.TableDisplayWindow_LocationChanged);
            this.tabControl.ResumeLayout(false);
            this.tabMuck.ResumeLayout(false);
            this.tabHand.ResumeLayout(false);
            this.tabStats.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Dotnetrix.Controls.TabControlEX tabControl;
        private Dotnetrix.Controls.TabPageEX tabMuck;
        private Dotnetrix.Controls.TabPageEX tabHand;
        private ControlListContainer entityHandsContainer;
        private Dotnetrix.Controls.TabPageEX tabStats;
        private System.Windows.Forms.Label lblPlayerStatisticsNote;
        protected ControlListContainer handControlList;
        private StatisticsDisplay statisticsDisplay;
        private System.Windows.Forms.Label lblVisualRecognitionNotSupported;


    }
}