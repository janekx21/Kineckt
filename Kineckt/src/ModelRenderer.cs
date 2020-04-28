using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt {
    public class ModelRenderer : GameObject {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly RenderTarget2D _shadowMap;

        public ModelRenderer(string name, GraphicsDevice graphicsDevice, RenderTarget2D shadowMap) : base(name) {
            _graphicsDevice = graphicsDevice;
            _shadowMap = shadowMap;
            var white = new Texture2D(graphicsDevice, 1, 1);
            white.SetData(new[] {Color.White});
            AO = white;
            Texture = white;
            var up = new Texture2D(graphicsDevice, 1, 1);
            up.SetData(new[] {new Color(Vector3.One * .5f)});
            NormalMap = up;
        }

        public Model Model { get; set; } = null;
        private Effect Effect { get; } = Kineckt.DefaultEffect;
        private Effect ShadowMapEffect { get; } = Kineckt.DefaultShadowMapEffect;
        public Texture Texture { get; set; }
        public Texture AO { get; set; }
        public Texture NormalMap { get; set; }

        public override void DrawShadow(Scene scene) {
            if (Model != null) {
                foreach (var mesh in Model.Meshes) {
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
            }
        }

        public override void Draw(Scene scene) {
            if (Model != null) {
                foreach (var mesh in Model.Meshes) {
                    foreach (var meshPart in mesh.MeshParts) {
                        var matrix = Matrix.CreateScale(Scale)
                                     * Matrix.CreateFromQuaternion(Rotation)
                                     * Matrix.CreateTranslation(Position);
                        var modelMatrix = mesh.ParentBone.Transform * matrix;
                        meshPart.Effect = Effect;

                        Effect.Parameters["World"].SetValue(modelMatrix);
                        Effect.Parameters["View"].SetValue(scene.Camera.GetViewMatrix());
                        Effect.Parameters["Projection"].SetValue(scene.Camera.GetProjectionMatrix());

                        if (Texture != null) Effect.Parameters["Alberto"].SetValue(Texture);
                        if (AO != null) Effect.Parameters["AmbientOcclusion"].SetValue(AO);
                        if (NormalMap != null) Effect.Parameters["NormalMap"].SetValue(NormalMap);

                        Effect.Parameters["Shadow"].SetValue(_shadowMap);
                        Effect.Parameters["ShadowProjection"]
                            .SetValue(modelMatrix * scene.Sun.GetLightViewProjection());

                        Effect.Parameters["LightDirection"].SetValue(-scene.Sun.Position);
                        Effect.Parameters["CameraPosition"].SetValue(scene.Camera.Position);
                    }

                    mesh.Draw();
                }
            }
        }
    }
}