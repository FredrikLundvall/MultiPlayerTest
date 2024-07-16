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

        public ClientEvent(int aClientIndex, ClientEventEnum aEventType, string aValue) : base("ClientEvent")
        {
            fClientIndex = aClientIndex;
            fClientEventType = aEventType;
            fValue = aValue;
        }
        public static ClientEvent CreateFromJson(string aActionJson)
        {
            return MessageBase.CreateFromJson<ClientEvent>(aActionJson);
        }
    }
}
