using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class ClientEvent : MessageBase
    {
        public ClientEventEnum fClientEventType;
        public string fValue;

        public ClientEvent(ClientEventEnum aEventType, string aValue) : base("ClientEvent")
        {
            fClientEventType = aEventType;
            fValue = aValue;
        }
        public static ClientEvent CreateFromJson(string aActionJson)
        {
            return MessageBase.CreateFromJson<ClientEvent>(aActionJson);
        }
    }
}
