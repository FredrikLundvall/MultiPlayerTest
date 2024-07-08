using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class ClientAction : MessageBase
    {
        public bool IsShooting;

        public ClientAction(bool aIsShooting) : base("ClientAction", false)
        {
            IsShooting = aIsShooting;
        }
        public static ClientAction CreateFromJson(string aActionJson)
        {
            return MessageBase.CreateFromJson<ClientAction>(aActionJson);
        }
    }
}
