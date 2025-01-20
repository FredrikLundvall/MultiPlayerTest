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
        public const int UDP_RECEIVE_TIMEOUT = 5000;
        public const int UDP_SERVER_PORT = 11000;
        public const string SERVER_IP = "127.0.0.1"; //192.168.3.17 127.0.0.1
        public const int UDP_CLIENT_PORT = 11001;
        UdpClient fUdpSender = new UdpClient();
        IPEndPoint fServerEndPoint = null;
        private TextLog fTextLog = new TextLog();
        private int fClientIndex = MessageBase.NOT_JOINED_CLIENT_INDEX;
        private Dictionary<int, string> fClientNames = new Dictionary<int, string>();
        private GameState fGameState = new GameState();
        Settings fSettings = null;

        public GameClient(Settings aSettings)
        {
            fSettings = aSettings;
            fServerEndPoint = new IPEndPoint(IPAddress.Parse(fSettings.fServerIp), fSettings.fServerPort);
        }
        private bool fStarted = false;
        public void Start()
        {
            fStarted = true;
            using (var udpClient = new UdpClient(fSettings.fClientPort, AddressFamily.InterNetwork))
            {
                udpClient.Client.ReceiveTimeout = UDP_RECEIVE_TIMEOUT;
                var remoteEndPoint = default(IPEndPoint);
                fTextLog.AddLog(String.Format("Listening for udp {0}", udpClient.Client.LocalEndPoint.ToString()));
                while (fStarted)
                {
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    try
                    {
                        var receivedResults = udpClient.Receive(ref remoteEndPoint);
                        if (remoteEndPoint.ToString() == fServerEndPoint.ToString())
                        {
                            var datagram = Encoding.ASCII.GetString(receivedResults);
                            //fTextLog.AddLog(String.Format("Receiving udp from {0} - {1}", remoteEndPoint.ToString(), datagram));
                            InterpretIncommingMessage(remoteEndPoint, datagram);
                        }
                    }
                    catch (SocketException e)
                    {
                        if (e.ErrorCode == 10060)
                        {
                            fTextLog.AddLog("Timeout");
                        }
                    }
                    catch (Exception e)
                    {
                        fTextLog.AddLog(String.Format("Exception when receiving {0}", e.Message));
                    }
                }
            }
        }
        private void InterpretIncommingMessage(IPEndPoint aRemoteEndPoint, string aDatagram)
        {
            if (aDatagram.EndsWith("\"fMessageClass\":\"GameState\"}"))
            {
                fGameState = GameState.CreateFromJson(aDatagram);
            }
            else if (aDatagram.EndsWith("\"fMessageClass\":\"ServerEvent\"}"))
            {
                var serverEvent = ServerEvent.CreateFromJson(aDatagram);
                if (serverEvent.fServerEventType == ServerEventEnum.Accepting)
                {
                    fClientIndex = serverEvent.fClientIndex;
                    fClientNames[serverEvent.fClientIndex] = serverEvent.fValue;
                    fTextLog.AddLog(String.Format("Admitted as index {0} name {1}", fClientIndex, serverEvent.fValue));
                }
                else if(serverEvent.fServerEventType == ServerEventEnum.Rejecting)
                {
                    fTextLog.AddLog(String.Format("Rejected as index {0} own index {1} name {2}", serverEvent.fClientIndex, fClientIndex, serverEvent.fValue));
                    fClientNames.Remove(serverEvent.fClientIndex);
                    fClientIndex = MessageBase.NOT_JOINED_CLIENT_INDEX;
                }
                else if (serverEvent.fServerEventType == ServerEventEnum.Entering)
                {
                    fClientNames[serverEvent.fClientIndex] = serverEvent.fValue;
                    fTextLog.AddLog(String.Format("Player entering index {0} name {1}", serverEvent.fClientIndex, serverEvent.fValue));
                }
                else if (serverEvent.fServerEventType == ServerEventEnum.Exiting)
                {
                    fClientNames.Remove(serverEvent.fClientIndex);
                    fTextLog.AddLog(String.Format("Player exiting index {0} name {1}", serverEvent.fClientIndex, serverEvent.fValue));
                }
            }
        }
        public void Stop()
        {
            fStarted = false;
            fTextLog.AddLog("Stopped listening");
        }
        public void SendMessage(String aMessage)
        {
            byte[] datagram = Encoding.ASCII.GetBytes(aMessage);
            fUdpSender.Send(datagram, datagram.Length, fServerEndPoint);
            //fTextLog.AddLog(String.Format("Sending data to {0} - {1}", fServerEndPoint.ToString(), aMessage));
        }
        public GameState GetGameState()
        {
            return fGameState;
        }
        public int GetClientIndex()
        {
            return fClientIndex;
        }
        public string[] GetLog()
        {
            return fTextLog.GetLog();
        }
        public void Close()
        {
            Stop();
            fUdpSender.Close();
        }
        public void ChangeSetting(Settings aSettings)
        {
            fSettings = aSettings;
            fServerEndPoint = new IPEndPoint(IPAddress.Parse(fSettings.fServerIp), fSettings.fServerPort);
        }
    }
}
