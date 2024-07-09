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
                while (fStarted)
                {
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    var receivedResults = udpClient.Receive(ref remoteEndPoint);
                    var datagram = Encoding.ASCII.GetString(receivedResults);
                    LogToConsole("Receiving udp from {0} - {1}", remoteEndPoint.ToString(), datagram);
                    InterpretIncommingMessage(remoteEndPoint, datagram);
                }
            }
        }

        private void InterpretIncommingMessage(IPEndPoint aRemoteEndPoint, string aDatagram)
        {
            if (aDatagram.EndsWith("\"MessageClass\":\"ClientEvent\"}"))
            {
                var clientEvent = ClientEvent.CreateFromJson(aDatagram);
                if (clientEvent.EventType == ClientEventEnum.Joining)
                {
                    if (!fAllClientEndpoints.Any(s => s.Address.ToString() == aRemoteEndPoint.Address.ToString()))
                    {
                        var ipEndPoint = new IPEndPoint(aRemoteEndPoint.Address, UDP_CLIENT_PORT);
                        fAllClientEndpoints.Add(ipEndPoint);
                        LogToConsole("Adding new client {0}", aRemoteEndPoint.Address.ToString());
                        var serverEvent = new ServerEvent(ServerEventEnum.Admitting);
                        SendMessage(ipEndPoint, serverEvent.GetAsJson());
                    }
                }
            }
            else if (aDatagram.EndsWith("\"MessageClass\":\"ClientAction\"}"))
            {
                var clientAction = ClientAction.CreateFromJson(aDatagram);
                if (clientAction.IsShooting)
                {
                    var gameState = new GameState();
                    gameState.PlayerShoot.Add(0, true);
                    SendMessageToAllClients(gameState.GetAsJson());
                }
            }
        }

        public void Stop()
        {
            fStarted = false;
        }
        public void SendMessageToAllClients(String aMessage)
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
        public void SendMessage(IPEndPoint aIpEndPoint, String aMessage)
        {
            UdpClient udpSender = new UdpClient();
            udpSender.ExclusiveAddressUse = false;
            udpSender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpSender.Client.Bind(fLocalEndPoint);
            byte[] datagram = Encoding.ASCII.GetBytes(aMessage);
            udpSender.Send(datagram, datagram.Length, aIpEndPoint);
            LogToConsole("Sending data to {0} - {1}", aIpEndPoint.ToString(), aMessage);
        }
        private void LogToConsole(string format, object arg0)
        {
            LogToConsole(format, arg0, null);
        }
        private void LogToConsole(string format, object arg0, object arg1)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " " + format, arg0, arg1);
        }
    }
}
