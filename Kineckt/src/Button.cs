using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

//namespace Kineckt
//{
//    public class Button : ModelRenderer
//    {
//        //private SpriteBatch spriteBatch;

//        private Vector2 position = Vector2.Zero;
//        Rect rectangle = new Rect(new Vector2(0, 0), new Vector2(0.1f, 0.1f));
//        private Texture2D texture;

//        public Button(GraphicsDevice graphicsDevice, RenderTarget2D shadowMap, Scene scene) :
//            base("Button", graphicsDevice, shadowMap)
//        {
//            _scene = scene;
//            Texture = texture;
//            Model = model;
//            rectangle.Size = Vector2.One;
//        }

//        public override void Update(GameTime gameTime)
//        {
//            base.Update(gameTime);
//            MouseState state = Mouse.GetState();
//            position.X = state.X;
//            position.Y = state.Y;

//            Rect mouseRect = new Rect(new Vector2(position.X, position.Y), new Vector2(0.1f, 0.1f));

//            if ((state.LeftButton == ButtonState.Pressed) && Rect.Intersect(mouseRect, rectangle))
//            {
//                Exit();
//            }
//        }
//    }
//}