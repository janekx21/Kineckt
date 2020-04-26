using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt {
    public class Plane : ModelRenderer {
        private static Model model;
        private static Texture texture;
        public Plane(GraphicsDevice graphicsDevice, RenderTarget2D shadowMap) : base("Plane", graphicsDevice, shadowMap) {
            Texture = texture;
            Model = model;
        }
        
        public static void LoadContent(ContentManager content) {
            model = content.Load<Model>("models/plane");
            texture = content.Load<Texture2D>("images/collection");
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            
            // Position += Vector3.Forward * deltaTime * 100;
        }
    }
}