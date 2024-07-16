using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class Settings
    {
        public readonly string fUserName;
        public readonly string fServerIp;
        public readonly int fServerPort;
        public readonly int fClientPort;
        public Settings(string aUserName, string aServerIp, int aServerPort, int aClientPort)
        {
            fUserName = aUserName;
            fServerIp = aServerIp;
            fServerPort = aServerPort;
            fClientPort = aClientPort;
        }
    }
}
