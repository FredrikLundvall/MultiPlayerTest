using System;
using System.Drawing;

namespace BlowtorchesAndGunpowder
{
    public class Ship : RendereableObject
    {
        public Ship()
        {
            fPosition.X = 320;
            fPosition.Y = 320;
            fLocalPoints = new PointF[4] { new Point(-4, 5), new Point(4, 5), new Point(0, -5), new Point(-4, 5) };
        }

        public int GetBulletDelay()
        {
            return 110;
        }
    }
}
