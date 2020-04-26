using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt {
    public class Camera : GameObject {
        private const float NearClipPlane = .1f;

        private const float FarClipPlane = 1000;

        private readonly float _fieldOfView = MathHelper.ToRadians(80f);
        private readonly GraphicsDevice _graphicsDevice;
        public Vector3 LookTarget = Vector3.Zero;

        private Vector3 _shakeVelocity = Vector3.Zero;

        private readonly Random _rnd = new Random();

        public Camera(GraphicsDevice graphicsDevice) : base("Camera") {
            _graphicsDevice = graphicsDevice;
        }

        private float AspectRatio => _graphicsDevice.Viewport.Width / (float) _graphicsDevice.Viewport.Height;

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            var speed = _shakeVelocity.Length() * 10;
            _shakeVelocity = _shakeVelocity.moveTowards(Vector3.Zero, (float) gameTime.ElapsedGameTime.TotalSeconds * speed);
        }

        public Matrix GetViewMatrix() {
            var shake = _shakeVelocity;
            var shakeMatrix = Matrix.CreateFromYawPitchRoll(shake.X, shake.Y, shake.Z);
            return Matrix.CreateLookAt(Position, LookTarget, Vector3.Up) * shakeMatrix;
        }

        private float Random() {
            return (float) _rnd.NextDouble();
        }

        public Matrix GetProjectionMatrix() {
            return Matrix.CreatePerspectiveFieldOfView(
                _fieldOfView,
                AspectRatio,
                NearClipPlane,
                FarClipPlane
            );
        }

        public void Shake(float intensity) {
            var shakeAdd = new Vector3(Random(), Random(), Random()) * 2 - Vector3.One;
            _shakeVelocity += shakeAdd * intensity;
        }
    }
}