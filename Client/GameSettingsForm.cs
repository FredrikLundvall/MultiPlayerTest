using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BlowtorchesAndGunpowder
{
    public partial class GameSettingsForm : Form
    {
        private Settings fSettings;
        public GameSettingsForm(Settings aSettings)
        {
            fSettings = aSettings;
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            serverIpTxt.Text = fSettings.fServerIp;
            serverPortTxt.Text = fSettings.fServerPort.ToString();
            clientPortTxt.Text = fSettings.fClientPort.ToString();
            myUsernameTxt.Text = fSettings.fUserName;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            int serverPort = 0;
            int.TryParse(serverPortTxt.Text, out serverPort);
            int clientPort = 0;
            int.TryParse(clientPortTxt.Text, out clientPort);
            fSettings = new Settings(myUsernameTxt.Text, serverIpTxt.Text, serverPort, clientPort);
        }
        public Settings GetSettings()
        {
            return fSettings;
        }
    }
}
