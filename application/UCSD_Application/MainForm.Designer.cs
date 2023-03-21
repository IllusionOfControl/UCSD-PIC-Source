namespace UCSD_Application
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.refreshLcdDisplayTimer = new System.Windows.Forms.Timer(this.components);
            this.mainFormStatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.connectionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.UCSDTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayIconContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HideWindowButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.DisplayTextLabel = new System.Windows.Forms.Label();
            this.ToggleBacklightButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.WebsiteLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.backlightStatusPlaceholder = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BacklightTimeoutBox = new UCSD_Application.NumericUpDownCustom();
            this.WinVerWaringLabel = new System.Windows.Forms.Label();
            this.MonitorStandbyEnable = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BacklightTimeoutToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.RefreshRateUpDown = new System.Windows.Forms.NumericUpDown();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Line1 = new System.Windows.Forms.TabPage();
            this.lineOptionControl1 = new UCSD_Application.LineOptionControl();
            this.Line2 = new System.Windows.Forms.TabPage();
            this.lineOptionControl2 = new UCSD_Application.LineOptionControl();
            this.Line3 = new System.Windows.Forms.TabPage();
            this.lineOptionControl3 = new UCSD_Application.LineOptionControl();
            this.Line4 = new System.Windows.Forms.TabPage();
            this.lineOptionControl4 = new UCSD_Application.LineOptionControl();
            this.mainFormStatusStrip.SuspendLayout();
            this.trayIconContextMenuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BacklightTimeoutBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RefreshRateUpDown)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.Line1.SuspendLayout();
            this.Line2.SuspendLayout();
            this.Line3.SuspendLayout();
            this.Line4.SuspendLayout();
            this.SuspendLayout();
            // 
            // refreshLcdDisplayTimer
            // 
            this.refreshLcdDisplayTimer.Enabled = true;
            this.refreshLcdDisplayTimer.Interval = 1000;
            this.refreshLcdDisplayTimer.Tick += new System.EventHandler(this.refreshLcdDisplayTimer_Tick);
            // 
            // mainFormStatusStrip
            // 
            this.mainFormStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.connectionStatusLabel});
            this.mainFormStatusStrip.Location = new System.Drawing.Point(0, 471);
            this.mainFormStatusStrip.Name = "mainFormStatusStrip";
            this.mainFormStatusStrip.Size = new System.Drawing.Size(315, 22);
            this.mainFormStatusStrip.TabIndex = 3;
            this.mainFormStatusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(83, 17);
            this.toolStripStatusLabel1.Text = "Device Status: ";
            // 
            // connectionStatusLabel
            // 
            this.connectionStatusLabel.Name = "connectionStatusLabel";
            this.connectionStatusLabel.Size = new System.Drawing.Size(78, 17);
            this.connectionStatusLabel.Text = "disconnected";
            // 
            // UCSDTrayIcon
            // 
            this.UCSDTrayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.UCSDTrayIcon.BalloonTipText = "UCSD is now in the system tray.\r\nDouble-click this icon restore window.";
            this.UCSDTrayIcon.ContextMenuStrip = this.trayIconContextMenuStrip;
            this.UCSDTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("UCSDTrayIcon.Icon")));
            this.UCSDTrayIcon.Text = "USB Computer Status Display";
            this.UCSDTrayIcon.Visible = true;
            this.UCSDTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.UCSDTrayIcon_MouseDoubleClick);
            // 
            // trayIconContextMenuStrip
            // 
            this.trayIconContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMenuItem,
            this.exitMenuItem});
            this.trayIconContextMenuStrip.Name = "contextMenuStrip1";
            this.trayIconContextMenuStrip.ShowImageMargin = false;
            this.trayIconContextMenuStrip.Size = new System.Drawing.Size(79, 48);
            // 
            // openMenuItem
            // 
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.Size = new System.Drawing.Size(78, 22);
            this.openMenuItem.Text = "Open";
            this.openMenuItem.Click += new System.EventHandler(this.openMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(78, 22);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // HideWindowButton
            // 
            this.HideWindowButton.Location = new System.Drawing.Point(16, 13);
            this.HideWindowButton.Name = "HideWindowButton";
            this.HideWindowButton.Size = new System.Drawing.Size(282, 23);
            this.HideWindowButton.TabIndex = 6;
            this.HideWindowButton.Text = "Hide Window (Minimize to tray)";
            this.HideWindowButton.UseVisualStyleBackColor = true;
            this.HideWindowButton.Click += new System.EventHandler(this.HideWindowButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Text On Display:";
            // 
            // DisplayTextLabel
            // 
            this.DisplayTextLabel.AutoSize = true;
            this.DisplayTextLabel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.DisplayTextLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DisplayTextLabel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DisplayTextLabel.Location = new System.Drawing.Point(16, 56);
            this.DisplayTextLabel.Margin = new System.Windows.Forms.Padding(3);
            this.DisplayTextLabel.MinimumSize = new System.Drawing.Size(290, 60);
            this.DisplayTextLabel.Name = "DisplayTextLabel";
            this.DisplayTextLabel.Size = new System.Drawing.Size(290, 60);
            this.DisplayTextLabel.TabIndex = 8;
            this.DisplayTextLabel.Text = "TEXT HERE\r\nLINE 2\r\nLINE 3\r\nLINE 4 XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            this.DisplayTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ToggleBacklightButton
            // 
            this.ToggleBacklightButton.Location = new System.Drawing.Point(193, 81);
            this.ToggleBacklightButton.Name = "ToggleBacklightButton";
            this.ToggleBacklightButton.Size = new System.Drawing.Size(82, 36);
            this.ToggleBacklightButton.TabIndex = 9;
            this.ToggleBacklightButton.Text = "Toggle\r\nBacklight";
            this.ToggleBacklightButton.UseVisualStyleBackColor = true;
            this.ToggleBacklightButton.Click += new System.EventHandler(this.ToggleBacklightButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Backlight Idle Timeout (Min):";
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.versionLabel.Location = new System.Drawing.Point(21, 447);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(266, 15);
            this.versionLabel.TabIndex = 12;
            this.versionLabel.Text = "Version X.X - Build Date: MMM dd yyyy";
            // 
            // WebsiteLinkLabel
            // 
            this.WebsiteLinkLabel.AutoSize = true;
            this.WebsiteLinkLabel.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WebsiteLinkLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.WebsiteLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(14, 0);
            this.WebsiteLinkLabel.LinkColor = System.Drawing.SystemColors.ControlDark;
            this.WebsiteLinkLabel.Location = new System.Drawing.Point(86, 428);
            this.WebsiteLinkLabel.Name = "WebsiteLinkLabel";
            this.WebsiteLinkLabel.Size = new System.Drawing.Size(123, 19);
            this.WebsiteLinkLabel.TabIndex = 14;
            this.WebsiteLinkLabel.Text = "Sergey Skorokhod";
            this.WebsiteLinkLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.WebsiteLinkLabel.UseCompatibleTextRendering = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Backlight Status:";
            // 
            // backlightStatusPlaceholder
            // 
            this.backlightStatusPlaceholder.AutoSize = true;
            this.backlightStatusPlaceholder.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.backlightStatusPlaceholder.Location = new System.Drawing.Point(99, 91);
            this.backlightStatusPlaceholder.MinimumSize = new System.Drawing.Size(40, 17);
            this.backlightStatusPlaceholder.Name = "backlightStatusPlaceholder";
            this.backlightStatusPlaceholder.Size = new System.Drawing.Size(40, 17);
            this.backlightStatusPlaceholder.TabIndex = 16;
            this.backlightStatusPlaceholder.Text = "off";
            this.backlightStatusPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BacklightTimeoutBox);
            this.groupBox1.Controls.Add(this.WinVerWaringLabel);
            this.groupBox1.Controls.Add(this.MonitorStandbyEnable);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ToggleBacklightButton);
            this.groupBox1.Controls.Add(this.backlightStatusPlaceholder);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 300);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(286, 125);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Display Backlighting";
            // 
            // BacklightTimeoutBox
            // 
            this.BacklightTimeoutBox.Location = new System.Drawing.Point(193, 55);
            this.BacklightTimeoutBox.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.BacklightTimeoutBox.Name = "BacklightTimeoutBox";
            this.BacklightTimeoutBox.Size = new System.Drawing.Size(82, 20);
            this.BacklightTimeoutBox.TabIndex = 20;
            this.BacklightTimeoutBox.Click += new System.EventHandler(this.BacklightTimeoutBox_Click);
            // 
            // WinVerWaringLabel
            // 
            this.WinVerWaringLabel.AutoSize = true;
            this.WinVerWaringLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WinVerWaringLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.WinVerWaringLabel.Location = new System.Drawing.Point(6, 34);
            this.WinVerWaringLabel.Name = "WinVerWaringLabel";
            this.WinVerWaringLabel.Size = new System.Drawing.Size(167, 13);
            this.WinVerWaringLabel.TabIndex = 22;
            this.WinVerWaringLabel.Text = "Windows Ver Warning Label";
            // 
            // MonitorStandbyEnable
            // 
            this.MonitorStandbyEnable.FormattingEnabled = true;
            this.MonitorStandbyEnable.Items.AddRange(new object[] {
            "Enabled",
            "Disabled"});
            this.MonitorStandbyEnable.Location = new System.Drawing.Point(193, 19);
            this.MonitorStandbyEnable.Name = "MonitorStandbyEnable";
            this.MonitorStandbyEnable.Size = new System.Drawing.Size(82, 30);
            this.MonitorStandbyEnable.TabIndex = 21;
            this.BacklightTimeoutToolTip.SetToolTip(this.MonitorStandbyEnable, "When enabled the backlight will\r\nturn off when the computer monitor\r\ngoes into a " +
        "power saver mode.\r\n(Feature requires Windows Vista +)");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(132, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Monitor standby detection:";
            // 
            // BacklightTimeoutToolTip
            // 
            this.BacklightTimeoutToolTip.AutomaticDelay = 1;
            this.BacklightTimeoutToolTip.AutoPopDelay = 10000;
            this.BacklightTimeoutToolTip.InitialDelay = 200;
            this.BacklightTimeoutToolTip.IsBalloon = true;
            this.BacklightTimeoutToolTip.ReshowDelay = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Refresh Rate (ms):";
            // 
            // RefreshRateUpDown
            // 
            this.RefreshRateUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.RefreshRateUpDown.Location = new System.Drawing.Point(205, 122);
            this.RefreshRateUpDown.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.RefreshRateUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.RefreshRateUpDown.Name = "RefreshRateUpDown";
            this.RefreshRateUpDown.Size = new System.Drawing.Size(82, 20);
            this.RefreshRateUpDown.TabIndex = 19;
            this.RefreshRateUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.RefreshRateUpDown.ValueChanged += new System.EventHandler(this.RefreshRateUpDown_ValueChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Line1);
            this.tabControl1.Controls.Add(this.Line2);
            this.tabControl1.Controls.Add(this.Line3);
            this.tabControl1.Controls.Add(this.Line4);
            this.tabControl1.Location = new System.Drawing.Point(16, 148);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(282, 146);
            this.tabControl1.TabIndex = 20;
            // 
            // Line1
            // 
            this.Line1.Controls.Add(this.lineOptionControl1);
            this.Line1.Location = new System.Drawing.Point(4, 22);
            this.Line1.Name = "Line1";
            this.Line1.Padding = new System.Windows.Forms.Padding(3);
            this.Line1.Size = new System.Drawing.Size(274, 120);
            this.Line1.TabIndex = 0;
            this.Line1.Text = "Line 1";
            this.Line1.UseVisualStyleBackColor = true;
            // 
            // lineOptionControl1
            // 
            this.lineOptionControl1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lineOptionControl1.Location = new System.Drawing.Point(4, 4);
            this.lineOptionControl1.Name = "lineOptionControl1";
            this.lineOptionControl1.Size = new System.Drawing.Size(262, 115);
            this.lineOptionControl1.TabIndex = 0;
            // 
            // Line2
            // 
            this.Line2.Controls.Add(this.lineOptionControl2);
            this.Line2.Location = new System.Drawing.Point(4, 22);
            this.Line2.Name = "Line2";
            this.Line2.Padding = new System.Windows.Forms.Padding(3);
            this.Line2.Size = new System.Drawing.Size(274, 120);
            this.Line2.TabIndex = 1;
            this.Line2.Text = "Line 2";
            this.Line2.UseVisualStyleBackColor = true;
            // 
            // lineOptionControl2
            // 
            this.lineOptionControl2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lineOptionControl2.Location = new System.Drawing.Point(4, 4);
            this.lineOptionControl2.Name = "lineOptionControl2";
            this.lineOptionControl2.Size = new System.Drawing.Size(262, 115);
            this.lineOptionControl2.TabIndex = 0;
            // 
            // Line3
            // 
            this.Line3.Controls.Add(this.lineOptionControl3);
            this.Line3.Location = new System.Drawing.Point(4, 22);
            this.Line3.Name = "Line3";
            this.Line3.Size = new System.Drawing.Size(274, 120);
            this.Line3.TabIndex = 2;
            this.Line3.Text = "Line 3";
            this.Line3.UseVisualStyleBackColor = true;
            // 
            // lineOptionControl3
            // 
            this.lineOptionControl3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lineOptionControl3.Location = new System.Drawing.Point(4, 4);
            this.lineOptionControl3.Name = "lineOptionControl3";
            this.lineOptionControl3.Size = new System.Drawing.Size(262, 115);
            this.lineOptionControl3.TabIndex = 0;
            // 
            // Line4
            // 
            this.Line4.Controls.Add(this.lineOptionControl4);
            this.Line4.Location = new System.Drawing.Point(4, 22);
            this.Line4.Name = "Line4";
            this.Line4.Size = new System.Drawing.Size(274, 120);
            this.Line4.TabIndex = 3;
            this.Line4.Text = "Line 4";
            this.Line4.UseVisualStyleBackColor = true;
            // 
            // lineOptionControl4
            // 
            this.lineOptionControl4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lineOptionControl4.Location = new System.Drawing.Point(4, 4);
            this.lineOptionControl4.Name = "lineOptionControl4";
            this.lineOptionControl4.Size = new System.Drawing.Size(262, 115);
            this.lineOptionControl4.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 493);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.RefreshRateUpDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.DisplayTextLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.HideWindowButton);
            this.Controls.Add(this.mainFormStatusStrip);
            this.Controls.Add(this.WebsiteLinkLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "USB Computer Status Display";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.mainFormStatusStrip.ResumeLayout(false);
            this.mainFormStatusStrip.PerformLayout();
            this.trayIconContextMenuStrip.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BacklightTimeoutBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RefreshRateUpDown)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.Line1.ResumeLayout(false);
            this.Line2.ResumeLayout(false);
            this.Line3.ResumeLayout(false);
            this.Line4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer refreshLcdDisplayTimer;
        private System.Windows.Forms.StatusStrip mainFormStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel connectionStatusLabel;
        private System.Windows.Forms.NotifyIcon UCSDTrayIcon;
        private System.Windows.Forms.Button HideWindowButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label DisplayTextLabel;
        private System.Windows.Forms.Button ToggleBacklightButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.LinkLabel WebsiteLinkLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label backlightStatusPlaceholder;
        private System.Windows.Forms.ContextMenuStrip trayIconContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolTip BacklightTimeoutToolTip;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown RefreshRateUpDown;
        private System.Windows.Forms.ListBox MonitorStandbyEnable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label WinVerWaringLabel;
        private NumericUpDownCustom BacklightTimeoutBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Line1;
        private System.Windows.Forms.TabPage Line2;
        private System.Windows.Forms.TabPage Line3;
        private System.Windows.Forms.TabPage Line4;
        private LineOptionControl lineOptionControl1;
        private LineOptionControl lineOptionControl2;
        private LineOptionControl lineOptionControl3;
        private LineOptionControl lineOptionControl4;

    }
}

