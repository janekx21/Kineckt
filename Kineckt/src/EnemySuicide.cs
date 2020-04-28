using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt
{
    public class EnemySuicide : ModelRenderer
    {
        private static float Speed = 2;
        private static Model model;
        private static Texture texture;
        Vector2 vel = Vector2.Zero;
        private static int z = 1;
        private readonly Scene _scene;
        private float spawnTimer = 5;
        private float _timer = 0;

        private static readonly Vector3 MinPosition = new Vector3(-30, -200, -20);
        private static readonly Vector3 MaxPosition = new Vector3(30, 200, 30);

        public EnemySuicide(GraphicsDevice graphicsDevice, RenderTarget2D shadow, Scene scene) : base("EnemySuicide", graphicsDevice, shadow)
        {
            Texture = texture;
            Model = model;
            rectangle.Size = new Vector2(4, 4);
            //Speed += 1f;
            _scene = scene;
        }

        public static void LoadContent(ContentManager content)
        {
            model = content.Load<Model>("models/baseEnemy");
            texture = content.Load<Texture2D>("images/collection");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            spawnTimer = Math.Max(spawnTimer, 0);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            Position += new Vector3(0, 0, 35) * deltaTime * Speed;

            Position += Vector3.Backward * deltaTime * Speed * .1f;

            var y = (float)Math.Sin(TimeAlive * 4.2f) * .1f;
            Position = new Vector3(Position.X, y, Position.Z) +
                       (float)Math.Pow(spawnTimer / 5, 6) * Vector3.Up * 30;
            // rectangle.Position.X = Position.X;
            // rectangle.Position.Y = Position.Z;

            Rotation = Quaternion.CreateFromAxisAngle(Vector3.Forward, vel.X * .4f) *
                       Quaternion.CreateFromAxisAngle(Vector3.Up, vel.X * .8f);
            spawnTimer -= deltaTime * Speed;
            _timer -= deltaTime;

            Debug.WiredCube(Position, Rotation, new Vector3(rectangle.Size.X, 2f, rectangle.Size.Y), Color.Red);

            foreach (var gameObject in new List<GameObject>(_scene.GameObjects))
            {
                if (gameObject is EnemySuicide es)
                {
                    if (es.Position.Z > 50)
                    {
                        _scene.Destroy(es);
                    }
                }
            }
        }




        public override void OnDie()
        {
            base.OnDie();
            Kineckt.score += 12;
        }


    }
}