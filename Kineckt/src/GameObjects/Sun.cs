using Kineckt.World;
using Microsoft.Xna.Framework;

namespace Kineckt.GameObjects {
    public class Sun : GameObject {
        public Sun() : base("Sun") {
        }

        public Matrix GetLightViewProjection() {
            var lightView = Matrix.CreateLookAt(Position,
                Vector3.Zero,
                Vector3.Up);

            var lightProjection = Matrix.CreateOrthographic(250, 250, 10, 500);

            return lightView * lightProjection;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            Position = Vector3.Transform(Position,
                Matrix.CreateFromAxisAngle(Vector3.Up, (float) gameTime.ElapsedGameTime.TotalSeconds * .1f));
        }
    }
}