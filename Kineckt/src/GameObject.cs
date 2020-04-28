using Microsoft.Xna.Framework;

namespace Kineckt {
    public abstract class GameObject {
        protected GameObject(string name) {
            Name = name;
        }

        public Rect rectangle { get; set; } = new Rect(new Vector2(0, 0), new Vector2(0, 0));
        
        private Vector3 _position = Vector3.Zero;
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value;
                rectangle.Position = new Vector2(value.X, value.Z);
            }
        }

        public float Scale { get; set; } = 1;

        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public string Name { get; }

        protected float TimeAlive = 0;

        protected GameObject()
        {
            // throw new System.NotImplementedException();
        }

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