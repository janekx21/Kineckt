using Microsoft.Xna.Framework;

namespace Kineckt {
    public class Sun : GameObject {
        public Sun() : base("Sun") {
        }

        public Matrix GetLightViewProjection() {
            Matrix lightView = Matrix.CreateLookAt(Position,
                Vector3.Zero,
                Vector3.Up);

            Matrix lightProjection = Matrix.CreateOrthographic(180, 180, 10, 180);

            return lightView * lightProjection;
        }
    }
}