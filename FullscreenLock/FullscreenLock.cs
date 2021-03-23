using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FullscreenLock.Checker;

namespace FullscreenLock
{
    public partial class FullscreenLock : Form
    {
        SettingsForm settingsForm;

        public FullscreenLock()
        {
            InitializeComponent();
        }

        private Checker c;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.c = new Checker(label1, this, processBox, selectedBox, notifyIcon1);

            settingsForm = new SettingsForm(c);
        }

        private void button1_Click(object sender, EventArgs e)
        {
           c.toggle(this.button1,this.label1);
        }
        public void labelset(string s)
        {
            this.label1.Text = s;
        }

        private void rightBox_Click(object sender, EventArgs e)
        {
            c.moveRight();
        }

        private void leftBox_Click(object sender, EventArgs e)
        {
            c.moveLeft();
        }

        private void processBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            c.moveRight();
        }

        private void selectedBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            c.moveLeft();
        }

        private void FullscreenLock_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000);
                this.ShowInTaskbar = false;
                this.Hide();
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this.ShowInTaskbar = true;
            }
        }

        private bool ForceClose = false;

        private void FullscreenLock_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ForceClose && e.CloseReason == CloseReason.UserClosing && c.MinimizeToSystemTray)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ForceClose = true;
            Close();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                }
                else
                {
                    this.WindowState = FormWindowState.Minimized;
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingsForm.LoadSettings();

            var dialog = settingsForm.ShowDialog(this);

            if (dialog == DialogResult.OK)
            {
                c.SaveSettings();
            } 
        }

        private void selectExeButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.CurrentDirectory;
                openFileDialog.Filter = "exe files (*.exe)|*.exe|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    //Get the path of specified file
                    var filePath = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                    c.AddExe(filePath);
                }
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            c.delSelected();
            removeButton.Enabled = false;
        }

        public void SetOffsetsForApp()
        {
            var selected = selectedBox.SelectedIndex != -1 ? (ProcessModel)selectedBox.SelectedItem : (processBox.SelectedIndex != -1 ? (ProcessModel)processBox.SelectedItem : null);

            if (selected != null)
            {
                if (c.appOffsets.ContainsKey(selected.ProcessName))
                {
                    var offsets = c.appOffsets[selected.ProcessName];

                    xInput.Text = offsets.X.ToString();
                    yInput.Text = offsets.Y.ToString();
                    wInput.Text = offsets.W.ToString();
                    hInput.Text = offsets.H.ToString();
                }
                else
                {
                    xInput.Text = "0";
                    yInput.Text = "0";
                    wInput.Text = "0";
                    hInput.Text = "0";
                }
            }
            else
            {
                xInput.Text = "0";
                yInput.Text = "0";
                wInput.Text = "0";
                hInput.Text = "0";
            }
        }

        private void processBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (processBox.SelectedIndex != -1)
            {
                selectedBox.SelectedIndex = -1;

                if (((ProcessModel)processBox.SelectedItem).CreatedFromExe)
                {
                    removeButton.Enabled = true;
                }
                else
                {
                    removeButton.Enabled = false;
                }
                SetOffsetsForApp();
            }
            else
            {
                SetOffsetsForApp();
            }
        }

        private void selectedBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (selectedBox.SelectedIndex != -1)
            {
                processBox.SelectedIndex = -1;

                if (((ProcessModel)selectedBox.SelectedItem).CreatedFromExe)
                {
                    removeButton.Enabled = true;
                }
                else
                {
                    removeButton.Enabled = false;
                }
                SetOffsetsForApp();
            }
            else
            {
                SetOffsetsForApp();
            }
        }

        private Dictionary<TextBox, string> inputValues = new Dictionary<TextBox, string>();

        private void ChangeOffset(int? x = null, int? y = null, int? w = null, int? h = null)
        {
            var selected = selectedBox.SelectedIndex != -1 ? (ProcessModel)selectedBox.SelectedItem : (processBox.SelectedIndex != -1 ? (ProcessModel)processBox.SelectedItem : null);
            if (selected != null)
            {
                if (x != null || y != null || w != null || h != null)
                {
                    if (!c.appOffsets.ContainsKey(selected.ProcessName))
                    {
                        c.appOffsets.Add(selected.ProcessName, new OffsetParameters());
                    }

                    if (x != null)
                    {
                        c.appOffsets[selected.ProcessName].X = x.Value;
                    }
                    if (y != null)
                    {
                        c.appOffsets[selected.ProcessName].Y = y.Value;
                    }
                    if (w != null)
                    {
                        c.appOffsets[selected.ProcessName].W = w.Value;
                    }
                    if (h != null)
                    {
                        c.appOffsets[selected.ProcessName].H = h.Value;
                    }

                    if ((x != 0 && x != null) || 
                        (y != 0 && y != null) || 
                        (w != 0 && w != null) || 
                        (h != 0 && h != null))
                    {
                        c.SaveAppSettings();
                    }
                    else if (x == 0 && y == 0 && w == 0 && h == 0)
                    {
                        c.appOffsets.Remove(selected.ProcessName);
                    }
                }
            }
        }

        private int? GetIntInputValue(TextBox box)
        {
            int val;
            string sVal = box.Text;
            if (!int.TryParse(sVal, out val))
            {
                if (sVal == "-")
                {
                    val = 0;
                }
                else if (inputValues.ContainsKey(box))
                {
                    box.Text = inputValues[box];
                }
                else
                {
                    return null;
                }
            }
            if (!inputValues.ContainsKey(box))
            {
                inputValues.Add(box, box.Text);
            }
            else
            {
                inputValues[box] = box.Text;
            }
            return val;
        }

        private void xInput_TextChanged(object sender, EventArgs e)
        {
            ChangeOffset(x : GetIntInputValue((TextBox)sender));
        }

        private void yInput_TextChanged(object sender, EventArgs e)
        {
            ChangeOffset(y: GetIntInputValue((TextBox)sender));
        }

        private void wInput_TextChanged(object sender, EventArgs e)
        {
            ChangeOffset(w: GetIntInputValue((TextBox)sender));
        }

        private void hInput_TextChanged(object sender, EventArgs e)
        {
            ChangeOffset(h: GetIntInputValue((TextBox)sender));
        }
    }
}
