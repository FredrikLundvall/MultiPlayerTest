using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class ClientAction : MessageBase
    {
        public bool PlayerShooting;

        public ClientAction(bool aPlayerShooting) : base(false)
        {
            PlayerShooting = aPlayerShooting;
        }
        public static ClientAction CreateFromJson(string aActionJson)
        {
            return MessageBase.CreateFromJson<ClientAction>(aActionJson);
        }
    }
}
