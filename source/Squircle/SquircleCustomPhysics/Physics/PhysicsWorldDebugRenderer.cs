using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    public class scPhysicsWorldDebugRenderer
    {
        public scPhysicsWorld world;

        public void Draw(SpriteBatch spriteBatch)
        {
            if (world == null)
            {
                return;
            }
            
            foreach (var body in world.bodies)
            {
                var boundingBox = body.calculateBoundingBox();
                spriteBatch.DrawRectangle(scBoundingUtils.toXNARectangle(boundingBox), Color.Beige);
            }

        }
    }
}
