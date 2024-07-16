using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public abstract class MessageBase
    {
        public const int NOT_JOINED_CLIENT_INDEX = -1;

        public string fMessageClass;
 
        public MessageBase(string aMessageClass)
        {
            fMessageClass = aMessageClass;
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
