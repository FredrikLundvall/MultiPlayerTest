using System;
using System.Collections.Generic;
using System.Text;


namespace BlowtorchesAndGunpowder
{
    public class GameObject
    {
        public float fPositionX = 0;
        public float fPositionY = 0;
        public float fDirection = 1.570796326794896619f;

        public GameObject(float aPositionX, float aPositionY, float aDirection)
        {
            fPositionX = aPositionX;
            fPositionY = aPositionY;
            fDirection = aDirection;
        }
    }
}
