using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class ClientEvent : MessageBase
    {
        public ClientEventEnum EventType;

        public ClientEvent(ClientEventEnum aEventType) : base("ClientEvent")
        {
            EventType = aEventType;
        }
        public static ClientEvent CreateFromJson(string aActionJson)
        {
            return MessageBase.CreateFromJson<ClientEvent>(aActionJson);
        }
    }
}
