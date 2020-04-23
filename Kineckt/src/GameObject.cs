using Microsoft.Xna.Framework;

namespace Kineckt {
    public class GameObject {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
    }
}