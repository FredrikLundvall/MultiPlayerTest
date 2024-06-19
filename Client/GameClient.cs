using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BlowtorchesAndGunpowder
{
    internal class GameClient
    {
        const int UDP_SERVER_PORT = 11000;
        const int UDP_CLIENT_PORT = 11001;
        const string SERVER_IP = "127.0.0.1";
        UdpClient fUdpSender = new UdpClient();
        IPEndPoint fServerEndPoint = new IPEndPoint(IPAddress.Parse(SERVER_IP), UDP_SERVER_PORT);
        private TextLog fTextLog = new TextLog();
        public GameClient()
        {
            //fUdpSender.ExclusiveAddressUse = false;
            //fUdpSender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //fUdpSender.Client.Bind(fLocalEndPoint);
            //fUdpReceiver.ExclusiveAddressUse = false;
            //fUdpReceiver.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //fUdpReceiver.Client.Bind(fLocalEndPoint);
        }

        private bool fStarted = false;
        public void Start()
        {
            fStarted = true;
            using (var udpClient = new UdpClient(UDP_CLIENT_PORT, AddressFamily.InterNetwork))
            {
                //var remoteEndPoint = new IPEndPoint(IPAddress.Parse(SERVER_IP), UDP_CLIENT_PORT);
                var remoteEndPoint = default(IPEndPoint);//new IPEndPoint(IPAddress.Any, UDP_CLIENT_PORT);
                //udpClient.ExclusiveAddressUse = false;
                //udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                //udpClient.Client.Bind(fLocalEndPoint);
                fTextLog.AddLog(String.Format("Listening for udp {0}", udpClient.Client.LocalEndPoint.ToString()));
                string allDatagrams = "";
                while (fStarted)
                {
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    var receivedResults = udpClient.Receive(ref remoteEndPoint);
                    if (remoteEndPoint.ToString() == fServerEndPoint.ToString())
                    {
                        var datagram = Encoding.ASCII.GetString(receivedResults);
                        fTextLog.AddLog(String.Format("Receiving udp from {0} - {1}", remoteEndPoint.ToString(), datagram));
                        allDatagrams += datagram;
                        if (datagram.EndsWith("\"FromServer\":true}"))
                        {
                            var gameState = GameState.CreateFromJson(datagram);
                            if (gameState.PlayerShoot[0])
                                fTextLog.AddLog(String.Format("Shooting"));
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
            byte[] datagram = Encoding.ASCII.GetBytes(aMessage);
            fUdpSender.Send(datagram, datagram.Length, fServerEndPoint);
            fTextLog.AddLog(String.Format("Sending data to {0} - {1}", fServerEndPoint.ToString(), aMessage));
        }
        public string PullLog()
        {
            return fTextLog.PullLog();
        }
        public void Close()
        {
            Stop();
            fUdpSender.Close();
        }
    }
}
