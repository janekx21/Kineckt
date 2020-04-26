using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt {
    public class EnemySpawner : GameObject {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly RenderTarget2D _shadow;
        private readonly Scene _scene;
        private float _timer = 0;
        Random _rnd = new Random();

        public EnemySpawner(GraphicsDevice graphicsDevice, RenderTarget2D shadow, Scene scene) : base("Enemy Spawner") {
            _graphicsDevice = graphicsDevice;
            _shadow = shadow;
            _scene = scene;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (_timer <= 0) {
                float h = (float) _rnd.NextDouble() * 2 - 1;
                _scene.Spawn(new Enemy(_graphicsDevice, _shadow) {
                    Position = Position + Vector3.Right * h * 30
                });
                _timer = 1f;
            }

            _timer -= (float) gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}