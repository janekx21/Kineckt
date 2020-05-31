using Kineckt.Graphics;
using Kineckt.World;
using Microsoft.Xna.Framework;

namespace Kineckt.Math {
    public static class Collision {
        public static bool Intersect(GameObject a, GameObject b) {
            Debug.WiredCube(a.Position, Quaternion.Identity, new Vector3(a.Rectangle.Size.X, .01f, a.Rectangle.Size.Y),
                Color.Orange);
            Debug.WiredCube(b.Position, Quaternion.Identity, new Vector3(b.Rectangle.Size.X, .01f, b.Rectangle.Size.Y),
                Color.Blue);

            var x = new Rect(a.Rectangle.Position - a.Rectangle.Size * .5f, a.Rectangle.Size);
            var y = new Rect(b.Rectangle.Position - b.Rectangle.Size * .5f, b.Rectangle.Size);

            return Rect.Intersect(x, y);
        }
    }
}