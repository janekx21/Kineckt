using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt {
    public class Particle : ModelRenderer {
        private static Model model;
        private Scene _scene;
        private Vector3 vel;

        public Particle(string name, GraphicsDevice graphicsDevice, RenderTarget2D shadowMap, Scene scene) : base(name, graphicsDevice, shadowMap) {
            _scene = scene;
            Model = model;
            var random = new Vector3(Random(), Random(), Random()) * 2 - Vector3.One;
            vel = random * 20 + Vector3.Up * 30;
        }

        private float Random() {
            return (float) Kineckt.Rnd.NextDouble();
        }
        
        
        public static void LoadContent(ContentManager content)
        {
            model = content.Load<Model>("models/particle");
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            vel += Vector3.Down * (float) gameTime.ElapsedGameTime.TotalSeconds * 100;
            Position += vel * (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (TimeAlive > 2f) {
                _scene.Destroy(this);
            }
        }
    }
}