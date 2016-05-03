using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    class scPhysicsWorld
    {
        IList<scBody> bodies;

        scPhysicsWorld()
        {
            bodies = new List<scBody>();
        }

        scBody createBody(scBodyDescription description)
        {
            var result = new scBody();
            result.position = description.position;
            result.bodyType = description.bodyType;

            foreach(var bodyPartDescription in description.bodypartDescriptions)
            {
                var bodyPart = new scBodyPart();
                bodyPart.shape = bodyPartDescription.shape;
                bodyPart.userData = bodyPartDescription.userData;
                result.bodyParts.Add(bodyPart);
            }

            bodies.Add(result);
            return result;
        }

        bool removeBody(scBody body)
        {
            return bodies.Remove(body);
        }
    }
}
