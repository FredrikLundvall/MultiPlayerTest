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
        public List<GameObject> fPlayerShipList = new List<GameObject>();
        public List<GameObject> fPlayerShotList = new List<GameObject>();
        public GameState() : base("GameState")
        {
        }
        public static GameState CreateFromJson(string aActionJson)
        {
            return MessageBase.CreateFromJson<GameState>(aActionJson);
        }
    }
}
