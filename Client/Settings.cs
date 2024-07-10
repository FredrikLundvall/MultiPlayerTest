using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class Settings
    {
        public readonly string fServerIp;
        public readonly int fServerPort;
        public Settings(string aServerIp, int aServerPort)
        {
            fServerIp = aServerIp;
            fServerPort = aServerPort;
        }
    }
}
