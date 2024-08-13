using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XInput.Wrapper;

namespace Instance_Manager.UtilityForms
{
    public partial class QuickProfile : Form
    {
        public QuickProfile()
        {
            InitializeComponent();
        }

        private void GamepadButton(object sender, EventArgs e)
        {
            if (gamepad.B_up) {
                Application.Exit();
            }
            if ((gamepad.Dpad_Up_up || gamepad.Dpad_Left_up) && comboBox1.SelectedIndex > 0)
            {
                comboBox1.SelectedIndex--;
            }
            if ((gamepad.Dpad_Down_up || gamepad.Dpad_Right_up) && comboBox1.SelectedIndex < Profiles.Count - 1)
            {
                comboBox1.SelectedIndex++;
            }
            if (gamepad.A_up)
            {
                button1.PerformClick();
            }
        }

        private void QuickProfile_Load(object sender, EventArgs e)
        {
            this.Text = "Select Profile";
            this.comboBox1.DataSource = Profiles;
            this.comboBox1.SelectedIndex = Profiles.IndexOf(Settings.Default.ActiveProfile);

            if (gamepad != null)
            {
                WriteLineIfDebug("Gamepad valid, setting up events.");
                gamepad.StateChanged += GamepadButton;
                X.StartPolling(gamepad);
            }

            void Close(object sender, EventArgs e)
            {
                this.Close();
            }

            Button cancel = new();
            cancel.Click += Close;
            this.CancelButton = cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetProfile( this.comboBox1.SelectedItem as string);
            this.FormClosing -= QuickProfile_FormClosing;
            if (gamepad != null)
            {
                WriteLineIfDebug("Gamepad valid, removing events.");
                gamepad.StateChanged -= GamepadButton;
                X.StopPolling();
            }
            this.Close();
        }

        private void QuickProfile_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteLineIfDebug("\nExecuting Method: QuickProfile_FormClosing");
            if (gamepad != null)
            {
                WriteLineIfDebug("Gamepad valid, removing events.");
                gamepad.StateChanged -= GamepadButton;
                X.StopPolling();
            }
            QuickLaunch = false;
            Application.Exit();
        }
    }
}
