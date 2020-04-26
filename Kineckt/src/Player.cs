using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kineckt {
    public class Player : ModelRenderer {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly RenderTarget2D _shadow;
        private static Model model;
        private static Texture texture;
        
        private const float Speed = 80;
        private const float Acceleration = 1200;
        private const float Friction = 600;
        private static readonly Vector3 MinPosition = new Vector3(-40, -200, -20);
        private static readonly Vector3 MaxPosition = new Vector3(40, 200, 30);
        
        private Vector2 _vel = Vector2.Zero;

        public Player(GraphicsDevice graphicsDevice, RenderTarget2D shadow) : base("Player", graphicsDevice, shadow) {
            _graphicsDevice = graphicsDevice;
            _shadow = shadow;
            Texture = texture;
            Model = model;
        }

        public Scene Scene { get; set; } = null;
        private float shootTimer = 0;

        public static void LoadContent(ContentManager content) {
            model = content.Load<Model>("models/starshipOmega");
            texture = content.Load<Texture2D>("images/starshipOmegaPaintOver");
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            Scene.Camera.Position = Vector3.Lerp(Position + Vector3.Backward * 2 + Vector3.Up * 20,
                Vector3.Up * 28 + Vector3.Backward * 20, .85f);
            Scene.Camera.LookTarget = Vector3.Lerp(Position, Vector3.Backward * 10, .85f);

            var state = Keyboard.GetState();
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;


            if (state.IsKeyDown(Keys.A) && shootTimer <= 0) {
                Scene.Spawn(new Bullet(_graphicsDevice, _shadow, Scene) {
                    Position = Position + Vector3.Forward*8
                });
                Scene.Camera.Shake(.01f);
                shootTimer = .15f;
            }

            shootTimer -= deltaTime;

            var mov = Vector2.Zero;
            if (state.IsKeyDown(Keys.Left)) mov.X -= 1;
            if (state.IsKeyDown(Keys.Right)) mov.X += 1;
            if (state.IsKeyDown(Keys.Up)) mov.Y -= 1;
            if (state.IsKeyDown(Keys.Down)) mov.Y += 1;
            if (mov.Length() > 1) mov.Normalize();

            _vel = _vel.MoveTowards(Vector2.Zero, deltaTime * Friction);

            // accelerate player
            _vel += mov * deltaTime * Acceleration;
            
            // there is a max player speed
            // this is a "clamp magnitude"
            // TODO move into extension function
            if (_vel.Length() > Speed) {
                _vel.Normalize();
                _vel *= Speed;
            }

            var y = (float) Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4.2f);
            Position += new Vector3(_vel.X, y, _vel.Y) * deltaTime;

            // play area borders
            Position = Vector3.Clamp(Position, MinPosition, MaxPosition);
            // makes player tilt in move direction
            Rotation = Quaternion.CreateFromAxisAngle(Vector3.Forward, _vel.X * .005f);
            Rotation *= Quaternion.CreateFromAxisAngle(Vector3.Right, _vel.Y * .005f);
        }
    }
}