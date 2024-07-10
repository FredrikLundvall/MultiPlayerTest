using System;
using System.Drawing;

namespace BlowtorchesAndGunpowder
{
    public class Shot : RendereableObject
    {
        public Shot(TimeSpan aTotalElapsedTime, PointF aPosition, float aDirection, PointF aStartSpeed)
        {
            fRemoveTime = aTotalElapsedTime + new TimeSpan(0, 0, 0, 0, 4000);
;
            fThrustorsForce = 30000f;

            fPosition.X = aPosition.X;
            fPosition.Y = aPosition.Y;
            fDirection = aDirection;
            fSpeedVector.X = aStartSpeed.X;//TODO: Denna bör inte vara lika hela tiden, utan räknas ut efter tids-spannet
            fSpeedVector.Y = aStartSpeed.Y;
            fLocalPoints = new PointF[2] { new Point(0, -5), new Point(0, -8) };
            EngageForwardThrustors(new TimeSpan(0, 0, 0, 0, 20));
        }
        public bool IsTimeToRemove(TimeSpan aTotalElapsedTime)
        {
            return aTotalElapsedTime > fRemoveTime;
        }
    }
}
