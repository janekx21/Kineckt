using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kineckt
{
    public static class Collision
    {
        public static bool Intersect(GameObject a, GameObject b)
        {
            return Rect.Intersect(a.rectangle, b.rectangle);
        }
    }
}
