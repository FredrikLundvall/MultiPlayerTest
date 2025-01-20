using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class ClientEvent : MessageBase
    {
        public int fClientIndex = MessageBase.NOT_JOINED_CLIENT_INDEX;
        public ClientEventEnum fClientEventType;
        public string fValue;
        public int fClientPort;

        public ClientEvent(int aClientIndex, ClientEventEnum aEventType, string aValue, int aClientPort) : base("ClientEvent")
        {
            fClientIndex = aClientIndex;
            fClientEventType = aEventType;
            fValue = aValue;
            fClientPort = aClientPort;
        }
        public static ClientEvent CreateFromJson(string aActionJson)
        {
            return MessageBase.CreateFromJson<ClientEvent>(aActionJson);
        }
    }
}
