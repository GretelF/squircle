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
        public scShape shape;
        public object userData;
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
    }

    public class scBody
    {
        public IList<scBodyPart> bodyParts;
        public scBodyType bodyType;
        public scTransform transform;

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
