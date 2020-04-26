using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt {
    public class Bullet : ModelRenderer {
        private static Model model;
        private static Texture texture;
        private const float Speed = 100;

        public Bullet(GraphicsDevice graphicsDevice, RenderTarget2D shadowMap) :
            base("Bullet", graphicsDevice, shadowMap) {
            Texture = texture;
            Model = model;
        }

        public static void LoadContent(ContentManager content) {
            model = content.Load<Model>("models/bullet");
            texture = content.Load<Texture2D>("images/collection");
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            Position += Vector3.Forward * deltaTime * Speed;
            Rotation = Quaternion.CreateFromAxisAngle(Vector3.Forward, TimeAlive * 12);
        }
    }
}