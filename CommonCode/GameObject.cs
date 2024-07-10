using System;
using System.Collections.Generic;
using System.Text;


namespace BlowtorchesAndGunpowder
{
    public class GameObject
    {
        public float PositionX = 0;
        public float PositionY = 0;
        public float Direction = 1.570796326794896619f;

        public GameObject(float aPositionX, float aPositionY, float aDirection)
        {
            PositionX = aPositionX;
            PositionY = aPositionY;
            Direction = aDirection;
        }
    }
}
