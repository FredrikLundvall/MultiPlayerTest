using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BlowtorchesAndGunpowder
{
    public class RenderUtil
    {
        static public readonly PointF[] ShipLocalPoints = new PointF[4] { new Point(-4, 5), new Point(4, 5), new Point(0, -5), new Point(-4, 5) };

        static public PointF[] GetWorldPoints(PointF[] localPoints, float positionX, float positionY, float direction)
        {
            Matrix myMatrix = new Matrix();
            PointF[] worldPoints = new PointF[localPoints.Length];
            localPoints.CopyTo(worldPoints, 0);
            myMatrix.Reset();
            myMatrix.Rotate(450 - direction * 57.29577951f);
            myMatrix.Translate(positionX, positionY, MatrixOrder.Append);
            myMatrix.TransformPoints(worldPoints);
            return worldPoints;
        }
    }
}
