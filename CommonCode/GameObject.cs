using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace BlowtorchesAndGunpowder
{
    public class GameObject
    {
        public float fPositionX = 0;
        public float fPositionY = 0;
        public float fDirection = 1.570796326794896619f;
        public float fSpeedVectorX = 0;
        public float fSpeedVectorY = 0;

        public GameObject(float aPositionX, float aPositionY, float aDirection, float aSpeedvectorX, float aSpeedvectorY)
        {
            fPositionX = aPositionX;
            fPositionY = aPositionY;
            fDirection = aDirection;
            fSpeedVectorX = aSpeedvectorX;
            fSpeedVectorY = aSpeedvectorY;
        }
        public void RotateLeft(TimeSpan aTimeElapsed)
        {
            fDirection += (float)(MovementUtil.ROTATION_FORCE * aTimeElapsed.TotalSeconds);
        }
        public void RotateRight(TimeSpan aTimeElapsed)
        {
            fDirection -= (float)(MovementUtil.ROTATION_FORCE * aTimeElapsed.TotalSeconds);
        }
        public void EngageForwardThrustors(TimeSpan aTimeElapsed)
        {
            fSpeedVectorX += (float)(Math.Cos(fDirection) * MovementUtil.THRUSTORS_FORCE * aTimeElapsed.TotalSeconds);
            fSpeedVectorY -= (float)(Math.Sin(fDirection) * MovementUtil.THRUSTORS_FORCE * aTimeElapsed.TotalSeconds);
        }
        public void CalcNewPosition(TimeSpan aTimeElapsed, RectangleF aBounds)
        {
            fPositionY += (float)(fSpeedVectorY * aTimeElapsed.TotalSeconds);
            fPositionX += (float)(fSpeedVectorX * aTimeElapsed.TotalSeconds);
            CheckBounds(aBounds);
        }
        private void CheckBounds(RectangleF aBounds)
        {
            if (fPositionX < aBounds.Left)
            {
                BounceX(aBounds.Left - fPositionX);
            }
            else if (fPositionX > aBounds.Right)
            {
                BounceX(aBounds.Right - fPositionX);
            }

            if (fPositionY < aBounds.Top)
            {
                BounceY(aBounds.Top - fPositionY);
            }
            else if (fPositionY > aBounds.Bottom)
            {
                BounceY(aBounds.Bottom - fPositionY);
            }
        }
        private void BounceX(float aDeltaX)
        {
            fPositionX += aDeltaX * 2;
            fSpeedVectorX = -fSpeedVectorX;
            fDirection = FlipX(fDirection);
        }

        private void BounceY(float aDeltaY)
        {
            fPositionY += aDeltaY * 2;
            fSpeedVectorY = -fSpeedVectorY;
            fDirection = FlipY(fDirection);
        }
        private float FlipX(float aAngle)
        {
            aAngle = (float)Math.PI - aAngle;
            return aAngle;
        }
        private float FlipY(float aAngle)
        {
            aAngle = (float)Math.PI * 2 - aAngle;
            return aAngle;
        }
    }
}
