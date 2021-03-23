using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace FullscreenLock
{
    public partial class SettingsForm : Form
    {
        private Checker c;

        public SettingsForm()
        {
            InitializeComponent();
        }
        public SettingsForm(Checker c)
        {
            InitializeComponent();

            this.c = c;

            LoadSettings();
        }

        public void LoadSettings()
        {
            startMinimizedCheckbox.Checked = c.StartMinimized;
            minimizeCheckbox.Checked = c.MinimizeToSystemTray;
            shiftOverrideCheckbox.Checked = c.ShiftOverrideEnabled;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            c.StartMinimized = startMinimizedCheckbox.Checked;
            c.MinimizeToSystemTray = minimizeCheckbox.Checked;
            c.ShiftOverrideEnabled = shiftOverrideCheckbox.Checked;
            Close();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
