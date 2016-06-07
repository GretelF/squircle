using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    public enum scBodyType
    {
        Static,
        Dynamic,
        Kinematic,
    }

    public struct scBodyPartDescription
    {
        public float restitution;
        public float friction;
        public float density;
        public scShape shape;
        public object userData;
        public bool isTrigger;
    }

    public class scBodyPart
    {
        public scShape shape;
        public object userData;
    }

    public struct scBodyDescription
    {
        public scBodyType bodyType;
        public scTransform transform;
        public float inertiaScale;
        public float linearDamping;
        public float angularDamping;

        public object userData;
    }

    public class scBody
    {
        public GameObject owner;

        public IList<scBodyPart> bodyParts;
        public scBodyType bodyType;
        public scTransform transform;
        public Vector2 linearVelocity;
        public float linearDamping;
        public float angularDamping;

        public object userData;

        public scBody()
        {
            bodyParts = new List<scBodyPart>();
        }

        public scBoundingBox calculateBoundingBox()
        {
            scBoundingBox boundingBox = bodyParts[0].shape.getBoundingBox(transform);
            foreach(var bodyPart in bodyParts.Skip(1))
            {
                boundingBox = scBoundingUtils.union(boundingBox, bodyPart.shape.getBoundingBox(transform));
            }
            return boundingBox;
        }
    }
}
