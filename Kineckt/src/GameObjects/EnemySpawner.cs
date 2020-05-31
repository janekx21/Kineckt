using System;
using Kineckt.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt.GameObjects {
    public class EnemySpawner : GameObject {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Random _rnd = new Random();
        private readonly Scene _scene;
        private readonly RenderTarget2D _shadow;
        private float _halt = 3;
        private float _timer = 4;
        private float _timer2;
        private float _speed = 0;


        public EnemySpawner(GraphicsDevice graphicsDevice, RenderTarget2D shadow, Scene scene) : base("Enemy Spawner") {
            _graphicsDevice = graphicsDevice;
            _shadow = shadow;
            _scene = scene;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (_scene.GameObjects.Find(o => o is Player) != null) {
                if (_timer <= 0) {
                    var h = (float) _rnd.NextDouble() * 2 - 1;
                    _scene.Spawn(new Enemy(_graphicsDevice, _shadow, _scene, _speed) {
                        Position = Position + Vector3.Right * h * 30 + Vector3.Up * 30
                    });
                    _scene.Spawn(new EnemySuicide(_graphicsDevice, _shadow, _scene) {
                        Position = new Vector3(0, 0, -100) + Vector3.Right * h * 30 + Vector3.Up * 30
                    });

                    _timer = _halt;
                    _speed += .1f;
                }

                if (_timer2 <= 0) {
                    _halt *= .95f;
                    _timer2 = 1;
                }

                _timer -= (float) gameTime.ElapsedGameTime.TotalSeconds;
                _timer2 -= (float) gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}