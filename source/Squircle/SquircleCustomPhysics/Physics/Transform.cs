using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    public struct scTransform
    {
        public Vector2 position;
        public scAngle rotation;
    }

    public struct scAngle
    {
        static public readonly float Pi     = (float)Math.PI;
        static public readonly float HalfPi = 0.5f * Pi;
        static public readonly float TwoPi  = 2.0f * Pi;

        static public readonly float RadiansToDegreesFactor = 180.0f / Pi;
        static public readonly float DegreesToRadiansFactor = Pi / 180.0f;


        public float radians;
        public float degrees
        {
            get { return radians * RadiansToDegreesFactor; }
            set { radians = DegreesToRadiansFactor * value; }
        }

        /// <summary>
        /// Return an scAngle in a normalized range of 0-2pi (or 0-360 degrees).
        /// </summary>
        /// <returns></returns>
        public scAngle getNormalized()
        {
            var angle = scAngle.FromRadians(radians);

            while (angle.radians < 0.0f)
                angle.radians += TwoPi;
            while (angle.radians > TwoPi)
                angle.radians -= TwoPi;

            return angle;
        }

        #region Factory methods to explicitly create a rotation from either degrees or radians

        static public scAngle FromDegrees(float degrees)
        {
            var rotation = new scAngle();
            rotation.degrees = degrees;
            return rotation;
        }

        static public scAngle FromRadians(float radians)
        {
            var rotation = new scAngle();
            rotation.radians = radians;
            return rotation;
        }

        #endregion

        #region Operator overloads

        public static scAngle operator +(scAngle angleA, scAngle angleB)
        {
            return scAngle.FromRadians(angleA.radians + angleB.radians).getNormalized();
        }

        public static scAngle operator -(scAngle angleA, scAngle angleB)
        {
            return scAngle.FromRadians(angleA.radians - angleB.radians).getNormalized();
        }

        #endregion Operator overloads
    }
}
