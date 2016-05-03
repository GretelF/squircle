using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    enum scBodyType
    {
        Static,
        Dynamic,
        Kinematic,
    }

    struct scBodyPartDescription
    {
        public scShape shape;
        public object userData;
    }

    class scBodyPart
    {
        public scShape shape;
        public object userData;
    }

    struct scBodyDescription
    {
        public IList<scBodyPartDescription> bodypartDescriptions;
        public scBodyType bodyType;
        public Vector2 position;
    }

    class scBody
    {
        public IList<scBodyPart> bodyParts;
        public scBodyType bodyType;
        public Vector2 position;

        public scBody()
        {
            bodyParts = new List<scBodyPart>();
        }
    }
}
