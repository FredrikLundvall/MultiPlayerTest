using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace BlowtorchesAndGunpowder
{
    class WireframeGameServer
    {
        public static void Main(string[] args)
        {
            // Create a server and start listening for incoming connections
            var gameServer = new GameServer();
            Task.Run(() => gameServer.Start());

            Console.WriteLine("Press any key to stop server...");
            Console.ReadKey();
           //Stop the server at the end
            gameServer.Stop();
        }
    }
}
