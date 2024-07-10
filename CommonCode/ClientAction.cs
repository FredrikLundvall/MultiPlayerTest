using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class ClientAction : MessageBase
    {
        public bool IsShooting;
        public RotationEnum fRotationType = RotationEnum.None;

        public ClientAction(bool aIsShooting, RotationEnum aRotationType) : base("ClientAction")
        {
            IsShooting = aIsShooting;
            fRotationType = aRotationType;
        }
        public static ClientAction CreateFromJson(string aActionJson)
        {
            return MessageBase.CreateFromJson<ClientAction>(aActionJson);
        }
    }
}
