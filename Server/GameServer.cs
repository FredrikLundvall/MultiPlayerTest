using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BlowtorchesAndGunpowder
{
    internal class GameServer
    {
        const int UDP_SERVER_PORT = 11000;
        const int UDP_CLIENT_PORT = 11001;
        private bool fStarted = false;
        private IPEndPoint fLocalEndPoint = new IPEndPoint(IPAddress.Loopback, UDP_SERVER_PORT);
        private List<IPEndPoint> fAllClientEndpoints = new List<IPEndPoint>();

        public void Start()
        {
            fStarted = true;
            using (var udpClient = new UdpClient())
            {
                var remoteEndPoint = new IPEndPoint(IPAddress.Any, UDP_SERVER_PORT);
                udpClient.ExclusiveAddressUse = false;
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.Bind(fLocalEndPoint);
                LogToConsole("Listening for udp {0}", remoteEndPoint.ToString());
                string allDatagrams = "";
                while (fStarted)
                {
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    var receivedResults = udpClient.Receive(ref remoteEndPoint);
                    if(!fAllClientEndpoints.Any(s => s.Address.ToString() == remoteEndPoint.Address.ToString())) 
                    {
                        fAllClientEndpoints.Add(new IPEndPoint(remoteEndPoint.Address, UDP_CLIENT_PORT));
                        LogToConsole("Adding new client {0}", remoteEndPoint.Address.ToString());
                    }
                    var datagram = Encoding.ASCII.GetString(receivedResults);
                    LogToConsole("Receiving udp from {0} - {1}", remoteEndPoint.ToString(), datagram);
                    allDatagrams += datagram;
                    if (datagram.EndsWith("\"FromServer\":false}"))
                    {
                        var clientAction = ClientAction.CreateFromJson(datagram);
                        if (clientAction.PlayerShooting)
                        {
                            var gameState = new GameState();
                            gameState.PlayerShoot.Add(0, true);
                            SendMessage(gameState.GetAsJson());
                        }
                    }
                }
            }
        }
        public void Stop()
        {
            fStarted = false;
        }
        public void SendMessage(String aMessage)
        {
            UdpClient udpSender = new UdpClient();
            udpSender.ExclusiveAddressUse = false;
            udpSender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpSender.Client.Bind(fLocalEndPoint);
            byte[] datagram = Encoding.ASCII.GetBytes(aMessage);
            foreach(var endpoint in fAllClientEndpoints)
            {
                udpSender.Send(datagram, datagram.Length, endpoint);
                LogToConsole("Sending data to {0} - {1}", endpoint.ToString(), aMessage);
            }
        }
        private void LogToConsole(string format, object arg0)
        {
            LogToConsole(format, arg0, null);
        }
        private void LogToConsole(string format, object arg0, object arg1)
        {
            Console.WriteLine("{0} " + format, DateTime.Now.ToString("HH:mm:ss"), arg0, arg1);
        }
    }
}
