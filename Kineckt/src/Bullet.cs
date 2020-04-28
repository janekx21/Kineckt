using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt {
    public class Bullet : ModelRenderer {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly RenderTarget2D _shadowMap;
        private readonly Scene _scene;
        private static Model model;
        private static Texture texture;
        private const float Speed = 100;

        public Bullet(GraphicsDevice graphicsDevice, RenderTarget2D shadowMap, Scene scene) :
            base("Bullet", graphicsDevice, shadowMap) {
            _graphicsDevice = graphicsDevice;
            _shadowMap = shadowMap;
            _scene = scene;
            Texture = texture;
            Model = model;
            rectangle.Size = new Vector2(2, 4);
        }

        public static void LoadContent(ContentManager content) {
            model = content.Load<Model>("models/bullet");
            texture = content.Load<Texture2D>("images/collection");
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            Position += Vector3.Forward * deltaTime * Speed;
            Rotation = Quaternion.CreateFromAxisAngle(Vector3.Forward, TimeAlive * 12);

            rectangle.Position.X = Position.X;
            rectangle.Position.Y = Position.Z;

            foreach (var gameObject in new List<GameObject>(_scene.GameObjects)) {
                if (gameObject is Enemy e) {
                    if (Collision.Intersect(e, this)) {
                        _scene.Destroy(e);
                        _scene.Destroy(this);
                        for (var i = 0; i < 50; i++) {
                            _scene.Spawn(new Particle("Spawn Particle", _graphicsDevice, _shadowMap, _scene) {
                                Position = e.Position
                            });
                        }
                        _scene.Camera.Shake(.02f);
                    }
                }
            }

            foreach (var gameObject in new List<GameObject>(_scene.GameObjects)) {
                if (gameObject is EnemySuicide es) {
                    if (Collision.Intersect(es, this)) {
                        _scene.Destroy(es);
                        _scene.Destroy(this);
                        for (var i = 0; i < 50; i++)
                        {
                            _scene.Spawn(new Particle("Spawn Particle", _graphicsDevice, _shadowMap, _scene) {
                                Position = es.Position
                            });
                        }
                        _scene.Camera.Shake(.02f);
                    }
                }
            }

            if (Position.Z < -50) {
                _scene.Destroy(this);
            }
            Debug.WiredCube(Position, Rotation, new Vector3(rectangle.Size.X, 1f,rectangle.Size.Y), Color.Green);
        }
    }
}