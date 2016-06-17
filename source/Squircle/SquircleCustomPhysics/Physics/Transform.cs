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

    /// <summary>
    /// Defines an angle in a unit-agnostic way. The user has to specifically state the unit when retrieving the float value.
    /// 
    /// Right now only degrees and radians are supported.
    /// 
    /// Note that the value stored is always normalized, i.e. the radians value is always greater than or equal to zero and less than or equal to two pi.
    /// </summary>
    public struct scAngle
    {
        static public readonly float Pi     = (float)Math.PI;
        static public readonly float HalfPi = 0.5f * Pi;
        static public readonly float TwoPi  = 2.0f * Pi;

        static public readonly float RadiansToDegreesFactor = 180.0f / Pi;
        static public readonly float DegreesToRadiansFactor = Pi / 180.0f;


        float _radians;

        public float radians
        {
            get
            {
                return _radians;
            }
            set
            {
                _radians = value;

                while (_radians < 0.0f)
                    _radians += TwoPi;
                while (_radians > TwoPi)
                    _radians -= TwoPi;
            }
        }

        public float degrees
        {
            get { return radians * RadiansToDegreesFactor; }
            set { radians = DegreesToRadiansFactor * value; }
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

        // Add one angle to another.
        public static scAngle operator +(scAngle angleA, scAngle angleB)
        {
            return FromRadians(angleA.radians + angleB.radians);
        }

        // Subtract one angle from another.
        public static scAngle operator -(scAngle angleA, scAngle angleB)
        {
            return FromRadians(angleA.radians - angleB.radians);
        }

        // Negate an angle.
        public static scAngle operator -(scAngle angle)
        {
            return FromRadians(-angle.radians);
        }

        // Scale an angle.
        public static scAngle operator *(scAngle angle, float scale)
        {
            return FromRadians(angle.radians * scale);
        }

        // Scale an angle.
        public static scAngle operator *(float scale, scAngle angle)
        {
            return angle * scale;
        }

        // Scale an angle.
        public static scAngle operator /(scAngle angle, float scale)
        {
            return FromRadians(angle.radians / scale);
        }

        #endregion Operator overloads
    }

    public static class scTransformUtils
    {
        public static Vector2 applyTransform(scTransform transform, Vector2 vector)
        {
            return transform.position + vector.Rotate(transform.rotation.radians);
        }
    }
}
