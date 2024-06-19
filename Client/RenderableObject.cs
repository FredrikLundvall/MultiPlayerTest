using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BlowtorchesAndGunpowder
{
    public abstract class RendereableObject
    {
        protected float _thrustorsForce = 60f;
        protected float _rotationForce = 4f;
        protected TimeSpan _removeTime = new TimeSpan(0);

        protected PointF[] _localPoints;
        protected PointF _position = new PointF(0, 0);
        protected float _direction = 1.570796326794896619f;
        protected PointF _speedVector = new PointF(0, 0);
        protected Matrix _myMatrix = new Matrix();


        public float GetDirection()
        {
            return _direction;
        }

        public PointF GetPosition()
        {
            return _position;
        }

        public PointF GetSpeedVector()
        {
            return _speedVector;
        }

        public PointF[] GetWorldPoints()
        {
            PointF[] worldPoints = new PointF[_localPoints.Length];
            _localPoints.CopyTo(worldPoints, 0);
            _myMatrix.Reset();
            _myMatrix.Rotate(450 - _direction * 57.29577951f);
            _myMatrix.Translate(_position.X, _position.Y, MatrixOrder.Append);
            _myMatrix.TransformPoints(worldPoints);
            return worldPoints;
        }

        public void RotateLeft(TimeSpan aTimeElapsed)
        {
            _direction += (float)(_rotationForce * aTimeElapsed.TotalSeconds);
        }

        public void RotateRight(TimeSpan aTimeElapsed)
        {
            _direction -= (float)(_rotationForce * aTimeElapsed.TotalSeconds);
        }

        public void EngageForwardThrustors(TimeSpan aTimeElapsed)
        {
            _speedVector.X += (float)(Math.Cos(_direction) * _thrustorsForce * aTimeElapsed.TotalSeconds);
            _speedVector.Y -= (float)(Math.Sin(_direction) * _thrustorsForce * aTimeElapsed.TotalSeconds);
        }

        public void CalcNewPosition(TimeSpan aTimeElapsed, RectangleF bounds)
        {
            _position.Y += (float)(_speedVector.Y * aTimeElapsed.TotalSeconds);
            _position.X += (float)(_speedVector.X * aTimeElapsed.TotalSeconds);
            CheckBounds(bounds);
        }

        private void CheckBounds(RectangleF bounds)
        {
            if (_position.X < bounds.Left)
            {
                BounceX(bounds.Left - _position.X);
            }
            else if (_position.X > bounds.Right)
            {
                BounceX(bounds.Right - _position.X);
            }

            if (_position.Y < bounds.Top)
            {
                BounceY(bounds.Top - _position.Y);
            }
            else if (_position.Y > bounds.Bottom)
            {
                BounceY(bounds.Bottom - _position.Y);
            }
        }
        private void BounceX(float deltaX)
        {
            _position.X += deltaX * 2;
            _speedVector.X = -_speedVector.X;
            _direction = FlipX(_direction);
        }

        private void BounceY(float deltaY)
        {
            _position.Y += deltaY * 2;
            _speedVector.Y = -_speedVector.Y;
            _direction = FlipY(_direction);
        }

        private float FlipX(float angle)
        {
            angle = (float)Math.PI - angle;
            return angle;
        }

        private float FlipY(float angle)
        {
            angle = (float)Math.PI * 2 - angle;
            return angle;
        }

    }
}
