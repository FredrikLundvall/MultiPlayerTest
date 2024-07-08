using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public abstract class MessageBase
    {
        public string MessageClass;
        public bool FromServer;

        public MessageBase(string aMessageClass, bool aFromServer)
        {
            MessageClass = aMessageClass;
            FromServer = aFromServer;
        }

        public string GetAsJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static T CreateFromJson<T>(string aActionJson)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(aActionJson);
        }
    }
}
