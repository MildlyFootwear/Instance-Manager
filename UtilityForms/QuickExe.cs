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
    public partial class QuickExe : Form
    {
        public QuickExe()
        {
            InitializeComponent();
        }

        private void GamepadButton(object sender, EventArgs e)
        {
            if (gamepad.B_up)
            {
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

        private void QuickExe_Load(object sender, EventArgs e)
        {
            this.Text = ToolName + " - " + Settings.Default.ActiveProfile + " - Select Exe";
            foreach (string str in ProfileExes)
            {
                string[] split = str.Split("|");
                if (str.IndexOf("|") != -1)
                {
                    comboBox1.Items.Add(Path.GetFileName(split[0]) + " " + split[1]);
                }
                else comboBox1.Items.Add(Path.GetFileName(split[0]));
            }

            comboBox1.SelectedIndex = 0;

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
            SelectedExe = ProfileExes[comboBox1.SelectedIndex];
            if (gamepad != null)
            {
                WriteLineIfDebug("Gamepad valid, removing events.");
                gamepad.StateChanged -= GamepadButton;
                X.StopPolling();
            }
            this.FormClosing -= QuickExe_FormClosing;
            this.Close();
        }

        private void QuickExe_FormClosing(object sender, FormClosingEventArgs e)
        {
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
