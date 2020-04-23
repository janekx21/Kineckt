using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt {
    public class ModelRenderer : GameObject {
        public Model Model { get; set; } = null;
        public Effect Effect { get; set; } = null;
        public Effect ShadowMapEffect { get; set; } = null;
        public Texture Texture { get; set; } = null;
        
        private readonly GraphicsDevice _graphicsDevice;
        private readonly RenderTarget2D _shadowMap;
        public ModelRenderer(string name, GraphicsDevice graphicsDevice, RenderTarget2D shadowMap) : base(name) {
            _graphicsDevice = graphicsDevice;
            _shadowMap = shadowMap;
        }

        public override void DrawShadow(Scene scene) {
            base.DrawShadow(scene);
            foreach (var mesh in Model.Meshes) {
                foreach (var meshPart in mesh.MeshParts) {
                    Matrix modelMatrix = mesh.ParentBone.Transform;

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
        }

        public override void Draw(Scene scene) {
            foreach (var mesh in Model.Meshes) {
                foreach (var meshPart in mesh.MeshParts) {
                    Matrix modelMatrix = mesh.ParentBone.Transform;

                    Effect.Parameters["World"].SetValue(modelMatrix);
                    Effect.Parameters["View"].SetValue(scene.Camera.GetViewMatrix());
                    Effect.Parameters["Projection"].SetValue(scene.Camera.GetProjectionMatrix());

                    Effect.Parameters["Alberto"].SetValue(Texture);
                    Effect.Parameters["Shadow"].SetValue(_shadowMap);
                    Effect.Parameters["ShadowProjection"].SetValue(modelMatrix * scene.Sun.GetLightViewProjection());
                    Effect.CurrentTechnique.Passes[0].Apply();

                    _graphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                    _graphicsDevice.Indices = meshPart.IndexBuffer;
                    var primitiveCount = meshPart.PrimitiveCount;
                    var vertexOffset = meshPart.VertexOffset;
                    var startIndex = meshPart.StartIndex;

                    _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexOffset, startIndex,
                        primitiveCount);
                }
            }
        }
    }
}