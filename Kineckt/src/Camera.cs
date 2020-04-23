using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt {
    public class Camera : GameObject {
        private readonly GraphicsDevice _graphicsDevice;
        
        private const float NearClipPlane = .1f;

        private const float FarClipPlane = 200;

        private readonly float _fieldOfView = MathHelper.ToRadians(80f);
        
        private float AspectRatio => _graphicsDevice.Viewport.Width / (float) _graphicsDevice.Viewport.Height;

        public Camera(GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;
        }

        public Matrix GetViewMatrix() {
            return Matrix.CreateLookAt(Position, Vector3.Zero, Vector3.Up);
        }

        public Matrix GetProjectionMatrix() {
            return Matrix.CreatePerspectiveFieldOfView(
                _fieldOfView,
                AspectRatio,
                NearClipPlane,
                FarClipPlane
            );
        }
    }
}