using Kineckt.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt.Graphics {
    public class ModelRenderer : GameObject {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly RenderTarget2D _shadowMap;

        protected ModelRenderer(string name, GraphicsDevice graphicsDevice, RenderTarget2D shadowMap) : base(name) {
            _graphicsDevice = graphicsDevice;
            _shadowMap = shadowMap;
            var white = new Texture2D(graphicsDevice, 1, 1);
            white.SetData(new[] {Color.White});
            Ao = white;
            Texture = white;
        }

        protected Model Model { get; set; } = null;
        private Effect Effect { get; } = Kineckt.DefaultEffect;
        private Effect ShadowMapEffect { get; } = Kineckt.DefaultShadowMapEffect;
        protected Texture Texture { get; set; }
        private Texture Ao { get; }

        public override void DrawShadow(Scene scene) {
            if (Model == null) return;
            foreach (var mesh in Model.Meshes)
            foreach (var meshPart in mesh.MeshParts) {
                var matrix = Matrix.CreateScale(Scale)
                             * Matrix.CreateFromQuaternion(Rotation)
                             * Matrix.CreateTranslation(Position);
                var modelMatrix = mesh.ParentBone.Transform * matrix;


                ShadowMapEffect.Parameters["LightViewProj"]
                    .SetValue(modelMatrix * scene.Sun.GetLightViewProjection());

                ShadowMapEffect.CurrentTechnique.Passes[0].Apply();

                _graphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                _graphicsDevice.Indices = meshPart.IndexBuffer;
                var primitiveCount = meshPart.PrimitiveCount;
                var vertexOffset = meshPart.VertexOffset;
                var startIndex = meshPart.StartIndex;

                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexOffset, startIndex,
                    primitiveCount);
            }
        }

        public override void Draw(Scene scene) {
            if (Model == null) return;
            foreach (var mesh in Model.Meshes) {
                foreach (var meshPart in mesh.MeshParts) {
                    var matrix = Matrix.CreateScale(Scale)
                                 * Matrix.CreateFromQuaternion(Rotation)
                                 * Matrix.CreateTranslation(Position);
                    var modelMatrix = mesh.ParentBone.Transform * matrix;
                    meshPart.Effect = Effect;

                    // Matrices
                    Effect.Parameters["World"].SetValue(modelMatrix);
                    Effect.Parameters["View"].SetValue(scene.Camera.GetViewMatrix());
                    Effect.Parameters["Projection"].SetValue(scene.Camera.GetProjectionMatrix());

                    // Textures
                    if (Texture != null) Effect.Parameters["Alberto"].SetValue(Texture);
                    if (Ao != null) Effect.Parameters["AmbientOcclusion"].SetValue(Ao);

                    // Shadow Stuff
                    Effect.Parameters["Shadow"].SetValue(_shadowMap);
                    Effect.Parameters["ShadowProjection"]
                        .SetValue(modelMatrix * scene.Sun.GetLightViewProjection());

                    // Util
                    Effect.Parameters["LightDirection"].SetValue(-scene.Sun.Position);
                    Effect.Parameters["CameraPosition"].SetValue(scene.Camera.Position);
                }

                mesh.Draw();
            }
        }
    }
}