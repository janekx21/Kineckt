using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt {
    public class Camera : GameObject {
        private const float NearClipPlane = .1f;

        private const float FarClipPlane = 1000;

        private readonly float _fieldOfView = MathHelper.ToRadians(80f);
        private readonly GraphicsDevice _graphicsDevice;
        public Vector3 LookTarget = Vector3.Zero;

        public Camera(GraphicsDevice graphicsDevice) : base("Camera") {
            _graphicsDevice = graphicsDevice;
        }

        private float AspectRatio => _graphicsDevice.Viewport.Width / (float) _graphicsDevice.Viewport.Height;

        public Matrix GetViewMatrix() {
            return Matrix.CreateLookAt(Position, LookTarget, Vector3.Up);
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