using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Kineckt {
    public class Enemy : ModelRenderer {
        private static float Speed = 5;
        private static Model model;
        private static Texture texture;
        Vector2 vel = Vector2.Zero;

        private static readonly Vector3 MinPosition = new Vector3(-20, -200, -20);
        private static readonly Vector3 MaxPosition = new Vector3(20, 200, 30);

        private float spawnTimer = 5;

        private enum State {
            Left,
            Right,
            Forward
        }

        private State _state = State.Left;
        private float _timer = 0;

        public Enemy(GraphicsDevice graphicsDevice, RenderTarget2D shadow) : base("Enemy", graphicsDevice, shadow) {
            Texture = texture;
            Model = model;
            rectangle.Size = new Vector2(4, 4);
            Speed += 1f;

            if (Kineckt.Rnd.NextDouble() > .5) {
                _state = State.Right;
            }
        }

        public static void LoadContent(ContentManager content) {
            model = content.Load<Model>("models/baseEnemy");
            texture = content.Load<Texture2D>("images/collection");
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            spawnTimer = Math.Max(spawnTimer, 0);

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
                    vel += Vector2.UnitY * 2;
                }
            }

            if (_state == State.Left) {
                vel = vel.MoveTowards(-Vector2.UnitX, deltaTime * 5f);
                if (Position.X < MinPosition.X) {
                    _state = State.Right;
                }
            }

            else if (_state == State.Right) {
                vel = vel.MoveTowards(Vector2.UnitX, deltaTime * 5f);

                if (Position.X > MaxPosition.X) {
                    _state = State.Left;
                }
            }
            else if (_state == State.Forward) {
                if (Position.X > MaxPosition.X) {
                    _state = State.Left;
                }

                if (Position.X < MinPosition.X) {
                    _state = State.Right;
                }

                vel = vel.MoveTowards(Vector2.Zero, deltaTime * 5f);
            }

            Position += new Vector3(vel.X, 0, vel.Y) * deltaTime * Speed;

            Position += Vector3.Backward * deltaTime * Speed * .1f;

            var y = (float) Math.Sin(TimeAlive * 4.2f) * .1f;
            Position = new Vector3(Position.X, y, Position.Z) +
                       (float) Math.Pow(spawnTimer / 5, 6) * Vector3.Up * 30;
            // rectangle.Position.X = Position.X;
            // rectangle.Position.Y = Position.Z;

            Rotation = Quaternion.CreateFromAxisAngle(Vector3.Forward, vel.X * .4f) *
                       Quaternion.CreateFromAxisAngle(Vector3.Up, vel.X * .8f);
            spawnTimer -= deltaTime * Speed;
            _timer -= deltaTime;

            Debug.WiredCube(Position, Rotation, new Vector3(rectangle.Size.X, 2f, rectangle.Size.Y), Color.Red);
        }


        public override void OnDie() {
            base.OnDie();
            Kineckt.score += 12;
        }
    }
}