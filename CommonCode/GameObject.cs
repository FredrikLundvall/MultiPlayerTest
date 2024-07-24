using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace BlowtorchesAndGunpowder
{
    public class GameObject
    {
        public int fClientIndex = MessageBase.NOT_JOINED_CLIENT_INDEX;
        public float fSpawnAtSecond = 0;
        public float fPositionX = 0;
        public float fPositionY = 0;
        public float fDirection = 1.570796326794896619f;
        public float fSpeedVectorX = 0;
        public float fSpeedVectorY = 0;

        public GameObject(int aClientIndex, float aSpawnAtSecond, float aPositionX, float aPositionY, float aDirection, float aSpeedvectorX, float aSpeedvectorY)
        {
            fClientIndex = aClientIndex;
            fSpawnAtSecond = aSpawnAtSecond;
            fPositionX = aPositionX;
            fPositionY = aPositionY;
            fDirection = aDirection;
            fSpeedVectorX = aSpeedvectorX;
            fSpeedVectorY = aSpeedvectorY;
        }
        public void RotateLeft(TimeSpan aTimeElapsed)
        {
            fDirection += (float)(MovementUtil.SHIP_ROTATION_FORCE * aTimeElapsed.TotalSeconds);
        }
        public void RotateRight(TimeSpan aTimeElapsed)
        {
            fDirection -= (float)(MovementUtil.SHIP_ROTATION_FORCE * aTimeElapsed.TotalSeconds);
        }
        public void EngageForwardThrustors(TimeSpan aTimeElapsed, float aThrustorsForce, float aSpeedLimit = float.MaxValue)
        {
            float speedVectorX = fSpeedVectorX + (float)(Math.Cos(fDirection) * aThrustorsForce * aTimeElapsed.TotalSeconds);
            float speedVectorY = fSpeedVectorY - (float)(Math.Sin(fDirection) * aThrustorsForce * aTimeElapsed.TotalSeconds);
            float speed = (float) Math.Sqrt(speedVectorX * speedVectorX + speedVectorY * speedVectorY);
            if(speed > aSpeedLimit)
            {
                return;
            }
            fSpeedVectorX = speedVectorX;
            fSpeedVectorY = speedVectorY;
        }
        public void CalcNewPosition(TimeSpan aTimeElapsed, RectangleF aBounds, bool aFlipWhenBounce = true)
        {
            fPositionY += (float)(fSpeedVectorY * aTimeElapsed.TotalSeconds);
            fPositionX += (float)(fSpeedVectorX * aTimeElapsed.TotalSeconds);
            CheckBounds(aBounds, aFlipWhenBounce);
        }
        private void CheckBounds(RectangleF aBounds, bool aFlipWhenBounce)
        {
            if (fPositionX < aBounds.Left)
            {
                BounceX(aBounds.Left - fPositionX, aFlipWhenBounce);
            }
            else if (fPositionX > aBounds.Right)
            {
                BounceX(aBounds.Right - fPositionX, aFlipWhenBounce);
            }

            if (fPositionY < aBounds.Top)
            {
                BounceY(aBounds.Top - fPositionY, aFlipWhenBounce);
            }
            else if (fPositionY > aBounds.Bottom)
            {
                BounceY(aBounds.Bottom - fPositionY, aFlipWhenBounce);
            }
        }
        private void BounceX(float aDeltaX, bool aFlipWhenBounce)
        {
            fPositionX += aDeltaX * 2;
            fSpeedVectorX = -fSpeedVectorX;
            if(aFlipWhenBounce)
                fDirection = FlipX(fDirection);
        }
        private void BounceY(float aDeltaY, bool aFlipWhenBounce)
        {
            fPositionY += aDeltaY * 2;
            fSpeedVectorY = -fSpeedVectorY;
            if (aFlipWhenBounce)
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
