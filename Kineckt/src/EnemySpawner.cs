using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt {
    public class EnemySpawner : GameObject {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly RenderTarget2D _shadow;
        private readonly Scene _scene;
        private float _timer = 0;
        private float _timer2 = 0;
        private float halt = 3;
        Random _rnd = new Random();

        public EnemySpawner(GraphicsDevice graphicsDevice, RenderTarget2D shadow, Scene scene) : base("Enemy Spawner") {
            _graphicsDevice = graphicsDevice;
            _shadow = shadow;
            _scene = scene;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if (_scene.GameObjects.Find(o => o is Player) != null) {
                if (_timer <= 0) {
                    float h = (float) _rnd.NextDouble() * 2 - 1;
                    _scene.Spawn(new Enemy(_graphicsDevice, _shadow) {
                        Position = Position + Vector3.Right * h * 30 + Vector3.Up * 30
                    });
                    _timer = halt;
                }

                if (_timer2 <= 0) {
                    halt *= .95f;
                    _timer2 = 1;
                }

                _timer -= (float) gameTime.ElapsedGameTime.TotalSeconds;
                _timer2 -= (float) gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}