using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    public class scPhysicsWorld
    {
        public IList<scBody> bodies;

        public scPhysicsWorld()
        {
            bodies = new List<scBody>();
        }

        public scBody createBody(scBodyDescription description, IList<scBodyPartDescription> bodypartDescriptions)
        {
            var body = new scBody();
            body.transform = description.transform;
            body.bodyType = description.bodyType;

            foreach(var bodyPartDescription in bodypartDescriptions)
            {
                var bodyPart = new scBodyPart();
                bodyPart.shape = bodyPartDescription.shape;
                bodyPart.userData = bodyPartDescription.userData;

                body.bodyParts.Add(bodyPart);
            }

            bodies.Add(body);
            return body;
        }

        public bool removeBody(scBody body)
        {
            return bodies.Remove(body);
        }



    }
}
