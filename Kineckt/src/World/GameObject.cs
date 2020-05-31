using Kineckt.Math;
using Microsoft.Xna.Framework;

namespace Kineckt.World {
    public abstract class GameObject {
        private Vector3 _position = Vector3.Zero;

        protected float TimeAlive;

        protected GameObject(string name) {
        }

        protected GameObject() {
            // throw new System.NotImplementedException();
        }

        public Rect Rectangle { get; } = new Rect(new Vector2(0, 0), new Vector2(0, 0));

        public Vector3 Position {
            get => _position;
            set {
                _position = value;
                Rectangle.Position = new Vector2(value.X, value.Z);
            }
        }

        protected float Scale { get; set; } = 1;

        protected Quaternion Rotation { get; set; } = Quaternion.Identity;

        public virtual void Update(GameTime gameTime) {
            TimeAlive += (float) gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void DrawShadow(Scene scene) {
            // do nothing
            // aka i am invisible
        }

        public virtual void Draw(Scene scene) {
            // do nothing
            // aka i am invisible
        }

        public virtual void OnDie() {
            // empty
        }
    }
}