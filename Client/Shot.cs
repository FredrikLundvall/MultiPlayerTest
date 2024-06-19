using System;
using System.Drawing;

namespace BlowtorchesAndGunpowder
{
    public class Shot : RendereableObject
    {
        public Shot(TimeSpan totalElapsedTime, PointF position, float direction, PointF startSpeed)
        {
            _removeTime = totalElapsedTime + new TimeSpan(0, 0, 0, 0, 11700);
;
            _thrustorsForce = 12990f;

            _position.X = position.X;
            _position.Y = position.Y;
            _direction = direction;
            _speedVector.X = startSpeed.X;//TODO: Denna bör inte vara lika hela tiden, utan räknas ut efter tids-spannet
            _speedVector.Y = startSpeed.Y;
            _localPoints = new PointF[2] { new Point(0, -5), new Point(0, -8) };
            EngageForwardThrustors(new TimeSpan(0, 0, 0, 0, 20));
        }
        public bool IsTimeToRemove(TimeSpan totalElapsedTime)
        {
            return totalElapsedTime > _removeTime;
        }
    }
}
