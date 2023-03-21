namespace UCSD_Application
{
    partial class LineOptionControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.HDDSelComboBox = new System.Windows.Forms.ComboBox();
            this.SelectionListBox = new System.Windows.Forms.CheckedListBox();
            this.CycleRateUpDown = new System.Windows.Forms.NumericUpDown();
            this.LOC_ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.NetSelComboBox = new System.Windows.Forms.ComboBox();
            this.NetworkCardToolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.CycleRateUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(122, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 26);
            this.label7.TabIndex = 10;
            this.label7.Text = "HDD Usage \r\nSingle Selection:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(123, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Cycle Rate:";
            // 
            // HDDSelComboBox
            // 
            this.HDDSelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.HDDSelComboBox.FormattingEnabled = true;
            this.HDDSelComboBox.Location = new System.Drawing.Point(207, 34);
            this.HDDSelComboBox.Name = "HDDSelComboBox";
            this.HDDSelComboBox.Size = new System.Drawing.Size(50, 21);
            this.HDDSelComboBox.Sorted = true;
            this.HDDSelComboBox.TabIndex = 8;
            this.LOC_ToolTip.SetToolTip(this.HDDSelComboBox, "Note: If drives are partitioned\r\nthen only first physical drive\r\nletter is shown." +
        "");
            this.HDDSelComboBox.SelectedIndexChanged += new System.EventHandler(this.HDDSelComboBox_SelectedIndexChanged);
            // 
            // SelectionListBox
            // 
            this.SelectionListBox.CheckOnClick = true;
            this.SelectionListBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.SelectionListBox.FormattingEnabled = true;
            this.SelectionListBox.Items.AddRange(new object[] {
            "CPU Usage",
            "RAM Usage",
            "HDD Usage Single",
            "HDD Usage List",
            "Network Usage",
            "Date / Time",
            "Up Time"});
            this.SelectionListBox.Location = new System.Drawing.Point(2, 1);
            this.SelectionListBox.Name = "SelectionListBox";
            this.SelectionListBox.Size = new System.Drawing.Size(115, 109);
            this.SelectionListBox.TabIndex = 6;
            this.SelectionListBox.ThreeDCheckBoxes = true;
            this.SelectionListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.SelectionListBox_ItemCheck);
            // 
            // CycleRateUpDown
            // 
            this.CycleRateUpDown.Location = new System.Drawing.Point(207, 2);
            this.CycleRateUpDown.Name = "CycleRateUpDown";
            this.CycleRateUpDown.Size = new System.Drawing.Size(50, 20);
            this.CycleRateUpDown.TabIndex = 7;
            this.LOC_ToolTip.SetToolTip(this.CycleRateUpDown, "When multiple items are checked,\r\nwe will iterate through the selected items,\r\nch" +
        "anging every ## refreshes.");
            this.CycleRateUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // LOC_ToolTip
            // 
            this.LOC_ToolTip.AutoPopDelay = 32000;
            this.LOC_ToolTip.InitialDelay = 300;
            this.LOC_ToolTip.IsBalloon = true;
            this.LOC_ToolTip.ReshowDelay = 100;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(125, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Network Card:";
            // 
            // NetSelComboBox
            // 
            this.NetSelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NetSelComboBox.DropDownWidth = 50;
            this.NetSelComboBox.FormattingEnabled = true;
            this.NetSelComboBox.Location = new System.Drawing.Point(128, 79);
            this.NetSelComboBox.Name = "NetSelComboBox";
            this.NetSelComboBox.Size = new System.Drawing.Size(129, 21);
            this.NetSelComboBox.TabIndex = 12;
            this.NetSelComboBox.SelectedIndexChanged += new System.EventHandler(this.NetSelComboBox_SelectedIndexChanged);
            // 
            // LineOptionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.NetSelComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.HDDSelComboBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.CycleRateUpDown);
            this.Controls.Add(this.SelectionListBox);
            this.Name = "LineOptionControl";
            this.Size = new System.Drawing.Size(262, 115);
            ((System.ComponentModel.ISupportInitialize)(this.CycleRateUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox HDDSelComboBox;
        private System.Windows.Forms.NumericUpDown CycleRateUpDown;
        private System.Windows.Forms.CheckedListBox SelectionListBox;
        private System.Windows.Forms.ToolTip LOC_ToolTip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox NetSelComboBox;
        private System.Windows.Forms.ToolTip NetworkCardToolTip;
    }
}
