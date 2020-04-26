using Microsoft.Xna.Framework;

namespace Kineckt {
    public abstract class GameObject {
        protected GameObject(string name) {
            Name = name;
        }

        public Vector3 Position { get; set; } = Vector3.Zero;
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public string Name { get; }

        public virtual void Update(GameTime gameTime) {
            // do nothing
            // aka freeze
        }

        public virtual void DrawShadow(Scene scene) {
            // do nothing
            // aka i am invisible
        }

        public virtual void Draw(Scene scene) {
            // do nothing
            // aka i am invisible
        }
    }
}