using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class ServerEvent : MessageBase
    {
        public int fClientIndex = MessageBase.NOT_JOINED_CLIENT_INDEX;
        public ServerEventEnum fServerEventType;
        public string fValue;

        public ServerEvent(int aClientIndex, ServerEventEnum aEventType, string aValue) : base("ServerEvent")
        {
            fClientIndex = aClientIndex;
            fServerEventType = aEventType;
            fValue = aValue;
        }
        public static ServerEvent CreateFromJson(string aActionJson)
        {
            return MessageBase.CreateFromJson<ServerEvent>(aActionJson);
        }
    }
}
