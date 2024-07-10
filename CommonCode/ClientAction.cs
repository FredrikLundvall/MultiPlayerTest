using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class ClientAction : MessageBase
    {
        public RotationEnum fRotationType = RotationEnum.None;
        public bool fIsThrusting;
        public bool fIsShooting;

        public ClientAction(RotationEnum aRotationType, bool aIsThrusting, bool aIsShooting) : base("ClientAction")
        {
            fRotationType = aRotationType;
            fIsThrusting = aIsThrusting;
            fIsShooting = aIsShooting;
        }
        public static ClientAction CreateFromJson(string aActionJson)
        {
            return MessageBase.CreateFromJson<ClientAction>(aActionJson);
        }
    }
}
