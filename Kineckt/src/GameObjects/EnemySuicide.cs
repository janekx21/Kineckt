using System.Collections.Generic;
using Kineckt.Engine;
using Kineckt.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt.GameObjects {
    public class EnemySuicide : ModelRenderer {
        private float speed = 2;
        private static Model model;
        private static Texture texture;

        private static readonly Vector3 MinPosition = new Vector3(-30, -200, -20);
        private static readonly Vector3 MaxPosition = new Vector3(30, 200, 30);
        private readonly Scene _scene;
        private readonly Vector2 _vel = Vector2.Zero;
        private float _spawnTimer = 5;

        public EnemySuicide(GraphicsDevice graphicsDevice, RenderTarget2D shadow, Scene scene) : base("EnemySuicide",
            graphicsDevice, shadow) {
            Texture = texture;
            Model = model;
            Rectangle.Size = new Vector2(4, 4);
            _scene = scene;
        }

        public static void LoadContent(ContentManager content) {
            model = content.Load<Model>("models/suicideEnemy");
            texture = content.Load<Texture2D>("images/collection");
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            _spawnTimer = System.Math.Max(_spawnTimer, 0);

            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            Position += new Vector3(0, 0, 35) * deltaTime * speed;

            Position += Vector3.Backward * deltaTime * speed * .1f;

            var y = (float) System.Math.Sin(TimeAlive * 4.2f) * .1f;
            Position = new Vector3(Position.X, y, Position.Z) +
                       (float) System.Math.Pow(_spawnTimer / 5, 6) * Vector3.Up * 30;

            Rotation = Quaternion.CreateFromAxisAngle(Vector3.Forward, _vel.X * .4f) *
                       Quaternion.CreateFromAxisAngle(Vector3.Up, _vel.X * .8f);
            _spawnTimer -= deltaTime * speed;

            Debug.WiredCube(Position, Rotation, new Vector3(Rectangle.Size.X, 2f, Rectangle.Size.Y), Color.Red);

            foreach (var gameObject in new List<GameObject>(_scene.GameObjects)) {
                if (!(gameObject is EnemySuicide es)) continue;
                if (es.Position.Z > 50) {
                    _scene.Destroy(es);
                }
            }
        }


        public override void OnDie() {
            base.OnDie();
            var p = _scene.GetFirstOrNull<Player>();
            if (p != null) {
                p.Score += 12;
            }
        }
    }
}