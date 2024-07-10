using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BlowtorchesAndGunpowder
{
    public abstract class RendereableObject
    {
        protected float fThrustorsForce = 60f;
        protected float fRotationForce = 4f;
        protected TimeSpan fRemoveTime = new TimeSpan(0);

        protected PointF[] fLocalPoints;
        protected PointF fPosition = new PointF(0, 0);
        protected float fDirection = 1.570796326794896619f;
        protected PointF fSpeedVector = new PointF(0, 0);
        protected Matrix fMyMatrix = new Matrix();


        public float GetDirection()
        {
            return fDirection;
        }

        public PointF GetPosition()
        {
            return fPosition;
        }

        public PointF GetSpeedVector()
        {
            return fSpeedVector;
        }

        public PointF[] GetWorldPoints()
        {
            PointF[] worldPoints = new PointF[fLocalPoints.Length];
            fLocalPoints.CopyTo(worldPoints, 0);
            fMyMatrix.Reset();
            fMyMatrix.Rotate(450 - fDirection * 57.29577951f);
            fMyMatrix.Translate(fPosition.X, fPosition.Y, MatrixOrder.Append);
            fMyMatrix.TransformPoints(worldPoints);
            return worldPoints;
        }

        public void RotateLeft(TimeSpan aTimeElapsed)
        {
            fDirection += (float)(fRotationForce * aTimeElapsed.TotalSeconds);
        }

        public void RotateRight(TimeSpan aTimeElapsed)
        {
            fDirection -= (float)(fRotationForce * aTimeElapsed.TotalSeconds);
        }

        public void EngageForwardThrustors(TimeSpan aTimeElapsed)
        {
            fSpeedVector.X += (float)(Math.Cos(fDirection) * fThrustorsForce * aTimeElapsed.TotalSeconds);
            fSpeedVector.Y -= (float)(Math.Sin(fDirection) * fThrustorsForce * aTimeElapsed.TotalSeconds);
        }

        public void CalcNewPosition(TimeSpan aTimeElapsed, RectangleF aBounds)
        {
            fPosition.Y += (float)(fSpeedVector.Y * aTimeElapsed.TotalSeconds);
            fPosition.X += (float)(fSpeedVector.X * aTimeElapsed.TotalSeconds);
            CheckBounds(aBounds);
        }

        private void CheckBounds(RectangleF aBounds)
        {
            if (fPosition.X < aBounds.Left)
            {
                BounceX(aBounds.Left - fPosition.X);
            }
            else if (fPosition.X > aBounds.Right)
            {
                BounceX(aBounds.Right - fPosition.X);
            }

            if (fPosition.Y < aBounds.Top)
            {
                BounceY(aBounds.Top - fPosition.Y);
            }
            else if (fPosition.Y > aBounds.Bottom)
            {
                BounceY(aBounds.Bottom - fPosition.Y);
            }
        }
        private void BounceX(float aDeltaX)
        {
            fPosition.X += aDeltaX * 2;
            fSpeedVector.X = -fSpeedVector.X;
            fDirection = FlipX(fDirection);
        }

        private void BounceY(float aDeltaY)
        {
            fPosition.Y += aDeltaY * 2;
            fSpeedVector.Y = -fSpeedVector.Y;
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
