using Microsoft.Xna.Framework;

namespace Kineckt.Math {
    public class Rect {
        public Vector2 Position;
        public Vector2 Size;

        public Rect(Vector2 position, Vector2 size) {
            Position = position;
            Size = size;
        }

        public static bool Intersect(Rect a, Rect b) {
            return a.Position.X < b.Position.X + b.Size.X && a.Position.X + a.Size.X > b.Position.X &&
                   a.Position.Y < b.Position.Y + b.Size.Y && a.Position.Y + a.Size.Y > b.Position.Y;
        }
    }
}