using Kineckt.Engine;
using Kineckt.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt.GameObjects {
    public class Particle : ModelRenderer {
        private static Model model;

        private readonly float _liveTime = 1f;
        private readonly Scene _scene;
        private Vector3 _vel;

        public Particle(string name, GraphicsDevice graphicsDevice, RenderTarget2D shadowMap, Scene scene) : base(name,
            graphicsDevice, shadowMap) {
            _scene = scene;
            Model = model;
            var random = new Vector3(Random(), Random(), Random()) * 2 - Vector3.One;
            _vel = random * 20 + Vector3.Up * 30;
            Rotation = Quaternion.CreateFromYawPitchRoll(Random(), Random(), Random());
        }

        private float Random() {
            return (float) Kineckt.Rnd.NextDouble();
        }


        public static void LoadContent(ContentManager content) {
            model = content.Load<Model>("models/particle");
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            _vel += Vector3.Down * (float) gameTime.ElapsedGameTime.TotalSeconds * 100;
            Position += _vel * (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (TimeAlive > _liveTime) _scene.Destroy(this);

            Scale = (1 - TimeAlive / _liveTime) * 2f;
        }
    }
}