using Microsoft.Xna.Framework;

namespace Kineckt {
    public class Sun : GameObject {
        public Matrix GetLightViewProjection() {
            Matrix lightView = Matrix.CreateLookAt(Position,
                Vector3.Zero,
                Vector3.Up);

            Matrix lightProjection = Matrix.CreateOrthographic(10, 10, 1, 15);

            return lightView * lightProjection;
        }
    }
}