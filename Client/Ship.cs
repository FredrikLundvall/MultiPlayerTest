using System;
using System.Drawing;

namespace BlowtorchesAndGunpowder
{
    public class Ship : RendereableObject
    {
        public Ship()
        {
            _position.X = 320;
            _position.Y = 320;
            _localPoints = new PointF[4] { new Point(-4, 5), new Point(4, 5), new Point(0, -5), new Point(-4, 5) };
        }

        public int GetBulletDelay()
        {
            return 110;
        }
    }
}
