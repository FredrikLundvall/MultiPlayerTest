using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class ServerEvent : MessageBase
    {
        public ServerEventEnum EventType;

        public ServerEvent(ServerEventEnum aEventType) : base("ServerEvent")
        {
            EventType = aEventType;
        }
        public static ServerEvent CreateFromJson(string aActionJson)
        {
            return MessageBase.CreateFromJson<ServerEvent>(aActionJson);
        }
    }
}
