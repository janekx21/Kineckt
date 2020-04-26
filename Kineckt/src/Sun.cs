using Microsoft.Xna.Framework;

namespace Kineckt {
    public class Sun : GameObject {
        public Sun() : base("Sun") {
        }

        public Matrix GetLightViewProjection() {
            var lightView = Matrix.CreateLookAt(Position,
                Vector3.Zero,
                Vector3.Up);

            var lightProjection = Matrix.CreateOrthographic(180, 180, 10, 180);

            return lightView * lightProjection;
        }
    }
}