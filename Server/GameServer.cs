using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BlowtorchesAndGunpowder
{
    internal class GameServer
    {
        public const int GAME_STATE_UPDATE_ELAPSE_TIME = 42;
        public const int UDP_RECEIVE_TIMEOUT = 5000;
        const int UDP_SERVER_PORT = 11000;
        //const int UDP_CLIENT_PORT = 11001;
        private RectangleF fGameBounds = new RectangleF(0,0,1920,1080);
        private Stopwatch fStopWatch = new Stopwatch();
        private TimeSpan fLastCheckTime;
        private TimeSpan fLastStateUpdateTime;
        private bool fStarted = false;
        private int fMaxClientIndex = MessageBase.NOT_JOINED_CLIENT_INDEX;
        private IPEndPoint fLocalEndPoint = new IPEndPoint(IPAddress.Loopback, UDP_SERVER_PORT);
        private Dictionary<int, Client> fClientList = new Dictionary<int, Client>();
        private GameState fGameState = new GameState();
        private List<GameObject> fGameObjectRemoveList = new List<GameObject>();

        public void Start()
        {
            fStarted = true; 
            fStopWatch.Start();
            fLastCheckTime = fStopWatch.Elapsed;
            fLastStateUpdateTime = fLastCheckTime;
            //Asynchronicly run the listening loop
            Task.Run(() => ListeningLoop());
            LogToConsole("Game loop", "");
            while (fStarted)
            {
                try
                {
                    TimeSpan currentTime = fStopWatch.Elapsed;
                    UpdateAllClientsState(currentTime - fLastCheckTime);
                    fLastCheckTime = currentTime;
                    if ((currentTime - fLastStateUpdateTime).Milliseconds > GAME_STATE_UPDATE_ELAPSE_TIME)
                    {
                        fLastStateUpdateTime = currentTime;
                        SendMessageToAllClients(fGameState.GetAsJson());
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(2);
                    }
                }
                catch (Exception e)
                {
                    LogToConsole("Exception in game loop {0}", e.Message);
                }
            }

        }
        private void ListeningLoop()
        {
            using (var udpClient = new UdpClient())
            {
                var remoteEndPoint = new IPEndPoint(IPAddress.Any, UDP_SERVER_PORT);
                udpClient.ExclusiveAddressUse = false;
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.Bind(fLocalEndPoint);
                udpClient.Client.ReceiveTimeout = UDP_RECEIVE_TIMEOUT;
                LogToConsole("Listening for udp {0}", remoteEndPoint.ToString());
                while (fStarted)
                {
                    try
                    {
                        //IPEndPoint object will allow us to read datagrams sent from any source.
                        var receivedResults = udpClient.Receive(ref remoteEndPoint);

                        var datagram = Encoding.ASCII.GetString(receivedResults);
                        //LogToConsole("Receiving udp from {0} - {1}", remoteEndPoint.ToString(), datagram);
                        InterpretIncommingMessage(remoteEndPoint, datagram);
                    }
                    catch (SocketException e)
                    {
                        if (e.ErrorCode == 10060)
                        {
                            LogToConsole("{0}", "Timeout");
                        }
                    }
                    catch (Exception e)
                    {
                        LogToConsole("Exception when receiving {0}", e.Message);
                    }
                }
            }
        }

        private void InterpretIncommingMessage(IPEndPoint aRemoteEndPoint, string aDatagram)
        {
            if (aDatagram.EndsWith("\"fMessageClass\":\"ClientEvent\"}"))
            {
                var clientEvent = ClientEvent.CreateFromJson(aDatagram);
                if (clientEvent.fClientEventType == ClientEventEnum.Joining)
                {
                    int clientIndex = clientEvent.fClientIndex;
                    if (!fClientList.ContainsKey(clientIndex))
                    {
                        fMaxClientIndex += 1;
                        clientIndex = fMaxClientIndex;
                        var clientIpEndPoint = new IPEndPoint(aRemoteEndPoint.Address, clientEvent.fClientPort); //Get this from the client when it joins
                        fClientList.Add(clientIndex, new Client(clientIndex, clientIpEndPoint, clientEvent.fValue));
                        LogToConsole("Adding new client {0} as index {1} with username {2}", aRemoteEndPoint.Address.ToString(), clientIndex, clientEvent.fValue);
                        var serverAcceptingEvent = new ServerEvent(clientIndex, ServerEventEnum.Accepting, clientEvent.fValue);
                        SendMessage(clientIpEndPoint, serverAcceptingEvent.GetAsJson());
                        if (!fGameState.fPlayerShipList.Any(p => p.fClientIndex == clientIndex))
                            fGameState.fPlayerShipList.Add(new GameObject(clientIndex, (float) fStopWatch.Elapsed.TotalSeconds, 320, 320, 1.570796326794896619f, 0, 0));

                        var serverEnteringEvent = new ServerEvent(clientIndex, ServerEventEnum.Entering, clientEvent.fValue);
                        SendMessageToAllClients(serverEnteringEvent.GetAsJson());
                    }
                }
                else if(clientEvent.fClientEventType == ClientEventEnum.Leaving)
                {
                    int clientIndex = clientEvent.fClientIndex;
                    var client = fClientList[clientIndex];
                    if(client != null)
                    {
                        LogToConsole("Removing client as index {1} with username {2}", aRemoteEndPoint.Address.ToString(), clientIndex, clientEvent.fValue);
                        var serverEvent = new ServerEvent(clientIndex, ServerEventEnum.Rejecting, clientEvent.fValue);
                        SendMessage(client.fEndPoint, serverEvent.GetAsJson());
                        fClientList.Remove(clientIndex);
                        var playerShip = fGameState.fPlayerShipList.FirstOrDefault(p => p.fClientIndex == clientIndex);
                        if (playerShip != null)
                            fGameState.fPlayerShipList.Remove(playerShip);

                        var serverExitingEvent = new ServerEvent(clientIndex, ServerEventEnum.Exiting, clientEvent.fValue);
                        SendMessageToAllClients(serverExitingEvent.GetAsJson());
                    }
                }
            }
            else if (aDatagram.EndsWith("\"fMessageClass\":\"ClientAction\"}"))
            {
                var clientAction = ClientAction.CreateFromJson(aDatagram);
                int clientIndex = clientAction.fClientIndex;
                if (!fClientList.ContainsKey(clientIndex))
                    return;
                var client = fClientList[clientIndex];
                if (client != null)
                {
                    client.fCurrentAction = clientAction;
                }
            }
        }

        private void UpdateAllClientsState(TimeSpan aTimeElapsedFromLast)
        {
            foreach (var playerShip in fGameState.fPlayerShipList)
            {
                if (!fClientList.ContainsKey(playerShip.fClientIndex))
                    continue;
                var client = fClientList[playerShip.fClientIndex];
                if (client.fCurrentAction.fRotationType == RotationEnum.Left)
                {
                    playerShip.RotateLeft(aTimeElapsedFromLast);
                }
                else if (client.fCurrentAction.fRotationType == RotationEnum.Right)
                {
                    playerShip.RotateRight(aTimeElapsedFromLast);
                }
                if (client.fCurrentAction.fIsThrusting)
                {
                    playerShip.EngageForwardThrustors(aTimeElapsedFromLast, MovementUtil.SHIP_THRUSTORS_FORCE, MovementUtil.SHIP_MAX_SPEED);
                }
                if (client.fCurrentAction.fIsShooting)
                {
                    if ((float)fStopWatch.Elapsed.TotalSeconds - client.fLastShotSecond > MovementUtil.SHIP_BULLET_DELAY)
                    {
                        client.fLastShotSecond = (float)fStopWatch.Elapsed.TotalSeconds;
                        var playerShot = new GameObject(client.fIndex, (float)fStopWatch.Elapsed.TotalSeconds, playerShip.fPositionX, playerShip.fPositionY, playerShip.fDirection, playerShip.fSpeedVectorX, playerShip.fSpeedVectorY);
                        fGameState.fPlayerShotList.Add(playerShot);
                        playerShot.EngageForwardThrustors(new TimeSpan(0, 0, 0, 0, 20), MovementUtil.SHOT_THRUSTORS_FORCE);
                    }
                }
                playerShip.CalcNewPosition(aTimeElapsedFromLast, fGameBounds, false);
            }
            foreach(var playerShot in fGameState.fPlayerShotList)
            {
                if (fStopWatch.Elapsed.TotalSeconds - playerShot.fSpawnAtSecond > MovementUtil.SHOT_ALIVE_SECONDS)
                {
                    fGameObjectRemoveList.Add(playerShot);
                }
                else
                {
                    playerShot.CalcNewPosition(aTimeElapsedFromLast, fGameBounds, true);
                }
            }
            foreach (var playerShot in fGameObjectRemoveList)
            {
                fGameState.fPlayerShotList.Remove(playerShot);
            }
            fGameObjectRemoveList.Clear();
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
            foreach(var client in fClientList.Values)
            {
                udpSender.Send(datagram, datagram.Length, client.fEndPoint);
                //LogToConsole("Sending data to {0} - {1}", client.fEndPoint.ToString(), aMessage);
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
        private void LogToConsole(string aFormat, object aArg0)
        {
            LogToConsole(aFormat, aArg0, null);
        }
        private void LogToConsole(string aFormat, object aArg0, object aArg1)
        {
            LogToConsole(aFormat, aArg0, aArg1, null);
        }
        private void LogToConsole(string aFormat, object aArg0, object aArg1, object aArg2)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " " + aFormat, aArg0, aArg1, aArg2);
        }
    }
}
