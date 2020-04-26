using Microsoft.Xna.Framework;

namespace Kineckt
{
    public struct Rect
    {
        public Vector2 Position;
        public Vector2 Size;

        public Rect(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public static bool intersect(Rect a, Rect b)
        {
            
            //float posPosXa = a.Position.X + a.Size.X;
            //float negPosXa = a.Position.X - a.Size.X;
            //float posPosYa = a.Position.Y + a.Size.Y;
            //float negPosYa = a.Position.Y - a.Size.Y;

            //float posPosXb = a.Position.X + a.Size.X;
            //float negPosXb = a.Position.X - a.Size.X;
            //float posPosYb = a.Position.Y + a.Size.Y;
            //float negPosYb = a.Position.Y - a.Size.Y;

            return a.Position.X < b.Position.X + b.Size.X && a.Position.X + a.Size.X > b.Position.X &&
                   a.Position.Y < b.Position.Y + b.Size.Y && a.Position.Y + a.Size.Y > b.Position.Y;

            // Vector2 posX = Vector2()
        }
    }
}