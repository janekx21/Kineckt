using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kineckt
{
    public static class Collision
    {
        public static bool Intersect(GameObject a, GameObject b) {
            var x = a.rectangle;
            x.Position -= x.Size * .5f;
            var y = b.rectangle;
            y.Position -= y.Size * .5f;
            
            return Rect.Intersect(x, y);
        }
    }
}
