﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace BlowtorchesAndGunpowder
{
    internal class GameServer
    {
        public const int GAME_STATE_UPDATE_ELAPSE_TIME = 32;
        public const int UDP_RECEIVE_TIMEOUT = 5000;
        const int UDP_SERVER_PORT = 11000;
        const int UDP_CLIENT_PORT = 11001;
        private Stopwatch fStopWatch = new Stopwatch();
        private TimeSpan fTotalTimeElapsed;
        private TimeSpan fTotalTimeElapsedWhenLastStateUpdate;
        private bool fStarted = false;
        private int fMaxClientIndex = MessageBase.NOT_JOINED_CLIENT_INDEX;
        private IPEndPoint fLocalEndPoint = new IPEndPoint(IPAddress.Loopback, UDP_SERVER_PORT);
        private Dictionary<int, Client> fClientList = new Dictionary<int, Client>();
        private GameState fGameState = new GameState();

        public void Start()
        {
            fStarted = true;
            fStopWatch.Start();
            fTotalTimeElapsed = fStopWatch.Elapsed;
            fTotalTimeElapsedWhenLastStateUpdate = fTotalTimeElapsed;
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
                        TimeSpan timeElapsedNow = fStopWatch.Elapsed;
                        TimeSpan timeElapsedFromLast = timeElapsedNow - fTotalTimeElapsed;
                        fTotalTimeElapsed = timeElapsedNow;

                        var datagram = Encoding.ASCII.GetString(receivedResults);
                        LogToConsole("Receiving udp from {0} - {1}", remoteEndPoint.ToString(), datagram);
                        InterpretIncommingMessage(timeElapsedFromLast, remoteEndPoint, datagram);
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

        private void InterpretIncommingMessage(TimeSpan aTimeElapsed, IPEndPoint aRemoteEndPoint, string aDatagram)
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
                        var clientIpEndPoint = new IPEndPoint(aRemoteEndPoint.Address, UDP_CLIENT_PORT);
                        fClientList.Add(clientIndex, new Client(clientIndex, clientIpEndPoint, clientEvent.fValue));
                        LogToConsole("Adding new client {0} as index {1} with username {2}", aRemoteEndPoint.Address.ToString(), clientIndex, clientEvent.fValue);
                        var serverAcceptingEvent = new ServerEvent(clientIndex, ServerEventEnum.Accepting, clientEvent.fValue);
                        SendMessage(clientIpEndPoint, serverAcceptingEvent.GetAsJson());
                        if (!fGameState.fPlayerShip.ContainsKey(clientIndex))
                            fGameState.fPlayerShip.Add(clientIndex, new GameObject(320, 320, 0, 0, 0));

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
                        if (fGameState.fPlayerShip.ContainsKey(clientIndex))
                            fGameState.fPlayerShip.Remove(clientIndex);

                        var serverExitingEvent = new ServerEvent(clientIndex, ServerEventEnum.Exiting, clientEvent.fValue);
                        SendMessageToAllClients(serverExitingEvent.GetAsJson());
                    }
                }
            }
            else if (aDatagram.EndsWith("\"fMessageClass\":\"ClientAction\"}"))
            {
                var clientAction = ClientAction.CreateFromJson(aDatagram);
                int clientIndex = clientAction.fClientIndex;
                var playerShip = fGameState.fPlayerShip[clientIndex];
                if (clientAction.fRotationType == RotationEnum.Left)
                {
                    playerShip.RotateLeft(aTimeElapsed);
                }
                else if (clientAction.fRotationType == RotationEnum.Right)
                {
                    playerShip.RotateRight(aTimeElapsed);
                }
                if (clientAction.fIsThrusting)
                {
                    playerShip.EngageForwardThrustors(aTimeElapsed);
                }
                if (clientAction.fIsShooting)
                {
                    if(!fGameState.fPlayerShoot.ContainsKey(0))
                        fGameState.fPlayerShoot.Add(0, true);
                }
                SendUpdateState();
            }
        }
        public void SendUpdateState()
        {
            fTotalTimeElapsedWhenLastStateUpdate = fTotalTimeElapsed;
            SendMessageToAllClients(fGameState.GetAsJson());
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
                LogToConsole("Sending data to {0} - {1}", client.fEndPoint.ToString(), aMessage);
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
