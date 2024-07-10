using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class ServerEvent : MessageBase
    {
        public ServerEventEnum fServerEventType;
        public string Value;

        public ServerEvent(ServerEventEnum aEventType, string aValue) : base("ServerEvent")
        {
            fServerEventType = aEventType;
            Value = aValue;
        }
        public static ServerEvent CreateFromJson(string aActionJson)
        {
            return MessageBase.CreateFromJson<ServerEvent>(aActionJson);
        }
    }
}
