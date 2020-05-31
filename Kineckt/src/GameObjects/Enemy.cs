using System.Collections.Generic;
using Kineckt.Engine;
using Kineckt.Graphics;
using Kineckt.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt.GameObjects {
    public class Enemy : ModelRenderer {
        private float speed = 5;
        private static Model model;
        private static Texture texture;

        private static readonly Vector3 MinPosition = new Vector3(-30, -200, -20);
        private static readonly Vector3 MaxPosition = new Vector3(30, 200, 30);
        private readonly Scene _scene;

        private float _spawnTimer = 5;

        private State _state = State.Left;
        private float _timer;
        private Vector2 _vel = Vector2.Zero;

        public Enemy(GraphicsDevice graphicsDevice, RenderTarget2D shadow, Scene scene, float speedAddition) : base(
            "Enemy", graphicsDevice,
            shadow) {
            Texture = texture;
            Model = model;
            Rectangle.Size = new Vector2(4, 4);
            _scene = scene;
            speed += speedAddition;

            if (Kineckt.Rnd.NextDouble() > .5) _state = State.Right;
        }

        public static void LoadContent(ContentManager content) {
            model = content.Load<Model>("models/baseEnemy");
            texture = content.Load<Texture2D>("images/collection");
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            _spawnTimer = System.Math.Max(_spawnTimer, 0);

            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer <= 0) {
                _timer = 1;
                if (Kineckt.Rnd.NextDouble() > .8) {
                    _state = State.Left;
                }
                else if (Kineckt.Rnd.NextDouble() > .8) {
                    _state = State.Right;
                }
                else if (Kineckt.Rnd.NextDouble() > .5) {
                    _state = State.Forward;
                    _vel += Vector2.UnitY * 2;
                }
            }

            if (_state == State.Left) {
                _vel = _vel.MoveTowards(-Vector2.UnitX, deltaTime * 5f);
                if (Position.X < MinPosition.X) _state = State.Right;
            }

            else if (_state == State.Right) {
                _vel = _vel.MoveTowards(Vector2.UnitX, deltaTime * 5f);

                if (Position.X > MaxPosition.X) _state = State.Left;
            }
            else if (_state == State.Forward) {
                if (Position.X > MaxPosition.X) _state = State.Left;

                if (Position.X < MinPosition.X) _state = State.Right;

                _vel = _vel.MoveTowards(Vector2.Zero, deltaTime * 5f);
            }

            Position += new Vector3(_vel.X, 0, _vel.Y) * deltaTime * speed;

            Position += Vector3.Backward * deltaTime * speed * .1f;

            var y = (float) System.Math.Sin(TimeAlive * 4.2f) * .1f;
            Position = new Vector3(Position.X, y, Position.Z) +
                       (float) System.Math.Pow(_spawnTimer / 5, 6) * Vector3.Up * 30;
            // Rectangle.Position.X = Position.X;
            // Rectangle.Position.Y = Position.Z;

            Rotation = Quaternion.CreateFromAxisAngle(Vector3.Forward, _vel.X * .4f) *
                       Quaternion.CreateFromAxisAngle(Vector3.Up, _vel.X * .8f);
            _spawnTimer -= deltaTime * speed;
            _timer -= deltaTime;

            Debug.WiredCube(Position, Rotation, new Vector3(Rectangle.Size.X, 2f, Rectangle.Size.Y), Color.Red);

            foreach (var gameObject in new List<GameObject>(_scene.GameObjects))
                if (gameObject is Enemy e)
                    if (e.Position.Z > 50)
                        _scene.Destroy(e);
        }


        public override void OnDie() {
            base.OnDie();
            var player = _scene.GetFirstOrNull<Player>();
            if (player != null) {
                player.Score += 12;
            }
        }

        private enum State {
            Left,
            Right,
            Forward
        }
    }
}