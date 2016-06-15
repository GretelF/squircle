using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    public class scPhysicsWorld
    {
        public Game game;
        public IList<scBody> bodies;
        public DRectangle worldBounds;
        public scBoundingBox viewBounds;
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

        public void simulate(GameTime gameTime, Square square, Circle circle)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var oldSquareTransform = square.Body.transform;
            var oldCircleTransform = circle.Body.transform;

            simulateMovement(square.Body, dt);
            simulateMovement(circle.Body, dt);

            moveIntoViewBounds(square.Body);
            moveIntoViewBounds(circle.Body);

            var otherBodies = bodies.Where(b => b != square.Body && b != circle.Body);

            {
                var boundingBox = square.Body.calculateBoundingBox();
                foreach (var otherBody in otherBodies)
                {
                    var otherBoundingBox = otherBody.calculateBoundingBox();
                    if (scBoundingUtils.overlaps(boundingBox, otherBoundingBox))
                    {
                        //TODO intersection detection

                    }
                }

            }
            
            {
                var boundingBox = circle.Body.calculateBoundingBox();
                foreach (var otherBody in otherBodies)
                {
                    var otherBoundingBox = otherBody.calculateBoundingBox();
                    if (scBoundingUtils.overlaps(boundingBox, otherBoundingBox))
                    {
                        //TODO intersection detection
                    }
                }

            }


            detectAndResolveIntersection(square.Body, dt);
            detectAndResolveIntersection(circle.Body, dt);

        }

        public void moveIntoViewBounds(scBody body)
        {
            var viewBoundsBoundingBox = scBoundingUtils.createFromBoundingVertices((Vector2)viewBounds.upperLeft, (Vector2)viewBounds.lowerRight);

            var boundingBox = body.calculateBoundingBox();
            if (!viewBoundsBoundingBox.contains(boundingBox))
            {
                if (boundingBox.leftBorder < viewBoundsBoundingBox.leftBorder)
                {
                    body.transform.position.X = viewBoundsBoundingBox.leftBorder + boundingBox.halfExtents.X;
                }

                if (boundingBox.rightBorder > viewBoundsBoundingBox.rightBorder)
                {
                    body.transform.position.X = viewBoundsBoundingBox.rightBorder - boundingBox.halfExtents.X;
                }

                if (boundingBox.upperBorder < viewBoundsBoundingBox.upperBorder)
                {
                    body.transform.position.Y = viewBoundsBoundingBox.upperBorder + boundingBox.halfExtents.Y;
                }

                if (boundingBox.lowerBorder > viewBoundsBoundingBox.lowerBorder)
                {
                    body.transform.position.Y = viewBoundsBoundingBox.lowerBorder - boundingBox.halfExtents.Y;
                }

                body.linearVelocity = Vector2.Zero;
            }
        }

        private void simulateMovement(scBody body, float dt)
        {
            var newTransform = body.transform;
            // TODO: apply linear damping
            body.linearVelocity += gravity * dt;
            newTransform.position += body.linearVelocity * dt;
            // TODO: apply rotation

            body.transform = newTransform;
        }

        private void detectAndResolveIntersection(scBody body, float dt)
        {

        }
    }
}

