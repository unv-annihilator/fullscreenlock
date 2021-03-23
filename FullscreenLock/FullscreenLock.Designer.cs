using System.Windows.Forms;

namespace FullscreenLock
{
    partial class FullscreenLock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FullscreenLock));
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.processBox = new System.Windows.Forms.ListBox();
            this.selectedBox = new System.Windows.Forms.ListBox();
            this.rightBox = new System.Windows.Forms.Button();
            this.leftBox = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectExeButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.xInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.yInput = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.wInput = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.hInput = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(137, 292);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 37);
            this.button1.TabIndex = 0;
            this.button1.Text = "Stop";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label1.Location = new System.Drawing.Point(55, 332);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(230, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Waiting for focus";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // processBox
            // 
            this.processBox.DisplayMember = "ProcessTitle";
            this.processBox.FormattingEnabled = true;
            this.processBox.Location = new System.Drawing.Point(12, 27);
            this.processBox.Name = "processBox";
            this.processBox.Size = new System.Drawing.Size(147, 264);
            this.processBox.TabIndex = 2;
            this.processBox.ValueMember = "Text";
            this.processBox.SelectedValueChanged += new System.EventHandler(this.processBox_SelectedValueChanged);
            this.processBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.processBox_MouseDoubleClick);
            // 
            // selectedBox
            // 
            this.selectedBox.DisplayMember = "ProcessTitle";
            this.selectedBox.FormattingEnabled = true;
            this.selectedBox.Location = new System.Drawing.Point(204, 27);
            this.selectedBox.Name = "selectedBox";
            this.selectedBox.Size = new System.Drawing.Size(147, 264);
            this.selectedBox.TabIndex = 3;
            this.selectedBox.ValueMember = "Text";
            this.selectedBox.SelectedValueChanged += new System.EventHandler(this.selectedBox_SelectedValueChanged);
            this.selectedBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.selectedBox_MouseDoubleClick);
            // 
            // rightBox
            // 
            this.rightBox.Location = new System.Drawing.Point(165, 70);
            this.rightBox.Name = "rightBox";
            this.rightBox.Size = new System.Drawing.Size(31, 32);
            this.rightBox.TabIndex = 4;
            this.rightBox.Text = "->";
            this.rightBox.UseVisualStyleBackColor = true;
            this.rightBox.Click += new System.EventHandler(this.rightBox_Click);
            // 
            // leftBox
            // 
            this.leftBox.Location = new System.Drawing.Point(165, 108);
            this.leftBox.Name = "leftBox";
            this.leftBox.Size = new System.Drawing.Size(31, 32);
            this.leftBox.TabIndex = 5;
            this.leftBox.Text = "<-";
            this.leftBox.UseVisualStyleBackColor = true;
            this.leftBox.Click += new System.EventHandler(this.leftBox_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "ScreenLock will continue running in the background";
            this.notifyIcon1.BalloonTipTitle = "Runinng in background";
            this.notifyIcon1.Text = "ScreenLock";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(363, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // selectExeButton
            // 
            this.selectExeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.selectExeButton.Location = new System.Drawing.Point(165, 27);
            this.selectExeButton.Name = "selectExeButton";
            this.selectExeButton.Size = new System.Drawing.Size(33, 37);
            this.selectExeButton.TabIndex = 7;
            this.selectExeButton.Text = "...";
            this.selectExeButton.UseVisualStyleBackColor = true;
            this.selectExeButton.Click += new System.EventHandler(this.selectExeButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Enabled = false;
            this.removeButton.Location = new System.Drawing.Point(165, 146);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(31, 32);
            this.removeButton.TabIndex = 8;
            this.removeButton.Text = "X";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // xInput
            // 
            this.xInput.Location = new System.Drawing.Point(128, 1);
            this.xInput.Name = "xInput";
            this.xInput.Size = new System.Drawing.Size(31, 20);
            this.xInput.TabIndex = 9;
            this.xInput.Text = "0";
            this.xInput.TextChanged += new System.EventHandler(this.xInput_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(55, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Offsets";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(114, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "x";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(162, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "y";
            // 
            // yInput
            // 
            this.yInput.Location = new System.Drawing.Point(176, 1);
            this.yInput.Name = "yInput";
            this.yInput.Size = new System.Drawing.Size(31, 20);
            this.yInput.TabIndex = 12;
            this.yInput.Text = "0";
            this.yInput.TextChanged += new System.EventHandler(this.yInput_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(215, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "w";
            // 
            // wInput
            // 
            this.wInput.Location = new System.Drawing.Point(229, 1);
            this.wInput.Name = "wInput";
            this.wInput.Size = new System.Drawing.Size(31, 20);
            this.wInput.TabIndex = 14;
            this.wInput.Text = "0";
            this.wInput.TextChanged += new System.EventHandler(this.wInput_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(265, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "h";
            // 
            // hInput
            // 
            this.hInput.Location = new System.Drawing.Point(279, 1);
            this.hInput.Name = "hInput";
            this.hInput.Size = new System.Drawing.Size(31, 20);
            this.hInput.TabIndex = 16;
            this.hInput.Text = "0";
            this.hInput.TextChanged += new System.EventHandler(this.hInput_TextChanged);
            // 
            // FullscreenLock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(363, 356);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.hInput);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.wInput);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.yInput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.xInput);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.selectExeButton);
            this.Controls.Add(this.leftBox);
            this.Controls.Add(this.rightBox);
            this.Controls.Add(this.selectedBox);
            this.Controls.Add(this.processBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000, 1000);
            this.MinimumSize = new System.Drawing.Size(245, 180);
            this.Name = "FullscreenLock";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "FullscreenLock";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FullscreenLock_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.FullscreenLock_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.Label label1;
        private ListBox processBox;
        private ListBox selectedBox;
        private Button rightBox;
        private Button leftBox;
        private NotifyIcon notifyIcon1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private Button selectExeButton;
        private Button removeButton;
        private TextBox xInput;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox yInput;
        private Label label5;
        private TextBox wInput;
        private Label label6;
        private TextBox hInput;
    }
}

