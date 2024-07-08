using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowtorchesAndGunpowder
{
    public class GameState : MessageBase
    {
        public Dictionary<int,bool> PlayerShoot = new Dictionary<int, bool>();
        public GameState() : base("GameState", true)
        {
        }
        public static GameState CreateFromJson(string aActionJson)
        {
            return MessageBase.CreateFromJson<GameState>(aActionJson);
        }
    }
}
