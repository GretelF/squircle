﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    public class scPhysicsWorld
    {
        public IList<scBody> bodies;
        public DRectangle worldBounds;
        public DRectangle viewBounds;
        public Vector2 gravity = new Vector2(0, -9.81f);

        public scPhysicsWorld()
        {
            bodies = new List<scBody>();
        }

        public scBody createBody(scBodyDescription description, IList<scBodyPartDescription> bodyPartDescriptions)
        {
            var body = new scBody();
            body.transform = description.transform;
            body.bodyType = description.bodyType;

            foreach (var bodyPartDescription in bodyPartDescriptions)
            {
                var bodyPart = new scBodyPart();
                bodyPart.shape = bodyPartDescription.shape;
                bodyPart.userData = bodyPartDescription.userData;

                body.bodyParts.Add(bodyPart);
            }

            bodies.Add(body);
            return body;
        }

        public scBody createBody(scBodyDescription description, scBodyPartDescription bodyPartDescription)
        {
            var parts = new List<scBodyPartDescription>();
            parts.Add(bodyPartDescription);
            return createBody(description, parts);
        }

        public bool removeBody(scBody body)
        {
            return bodies.Remove(body);
        }

        public void simulate(GameTime gameTime)
        {
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
            var dynamicBodies = bodies.Where(b => b.owner != null);

            foreach (var body in dynamicBodies)
            {
                var newTransform = body.transform;
                // TODO: apply linear damping
                body.linearVelocity += gravity * dt;
                newTransform.position += body.linearVelocity * dt;
                // TODO: apply rotation

                //TODO: consider to let body apply it itself.
                body.transform = newTransform;
            }
        }
    }
}
