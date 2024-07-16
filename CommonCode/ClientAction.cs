using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class ClientAction : MessageBase
    {
        public int fClientIndex = MessageBase.NOT_JOINED_CLIENT_INDEX;
        public RotationEnum fRotationType = RotationEnum.None;
        public bool fIsThrusting;
        public bool fIsShooting;

        public ClientAction(int aClientIndex, RotationEnum aRotationType, bool aIsThrusting, bool aIsShooting) : base("ClientAction")
        {
            fClientIndex = aClientIndex;
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
