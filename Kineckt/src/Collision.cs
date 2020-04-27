using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kineckt {
    public static class Collision {
        public static bool Intersect(GameObject a, GameObject b) {
            Debug.WiredCube(a.Position, Quaternion.Identity, new Vector3(a.rectangle.Size.X, .01f, a.rectangle.Size.Y),
                Color.Orange);
            Debug.WiredCube(b.Position, Quaternion.Identity, new Vector3(b.rectangle.Size.X, .01f, b.rectangle.Size.Y),
                Color.Blue);

            var x = new Rect(a.rectangle.Position - a.rectangle.Size * .5f, a.rectangle.Size);
            var y = new Rect(b.rectangle.Position - b.rectangle.Size * .5f, b.rectangle.Size);

            return Rect.Intersect(x, y);
        }
    }
}