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
        private Settings _settings;
        public GameSettingsForm(Settings aSettings)
        {
            _settings = aSettings;
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            serverIpTxt.Text = _settings.fServerIp;
            serverPortTxt.Text = _settings.fServerPort.ToString();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            int port = 0;
            int.TryParse(serverPortTxt.Text, out port);
            _settings = new Settings(serverIpTxt.Text, port);
        }
        public Settings GetSettings()
        {
            return _settings;
        }
    }
}
