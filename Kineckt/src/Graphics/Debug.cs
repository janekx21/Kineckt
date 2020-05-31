using System.Collections.Generic;
using Kineckt.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kineckt.Graphics {
    public class Debug {
        private static Effect unlitEffect;
        private static Model wireframeCube;

        private static readonly List<Cube> CubeList = new List<Cube>();

        private readonly GraphicsDevice _gd;

        public Debug(GraphicsDevice gd) {
            _gd = gd;
        }

        public static void LoadContent(ContentManager content) {
            wireframeCube = content.Load<Model>("models/wireframeCube");
            unlitEffect = content.Load<Effect>("shaders/Unlit");
        }

        public void DrawDebug(Scene scene) {
            foreach (var cube in CubeList) DrawWireframeCube(scene, cube.Pos, cube.Rot, cube.Scale, cube.Color);

            CubeList.Clear();
        }

        public static void WiredCube(Vector3 pos, Quaternion rot, Vector3 scale, Color color) {
#if DEBUG
            CubeList.Add(new Cube(pos, rot, scale, color));
#endif
        }

        private void DrawWireframeCube(Scene scene, Vector3 pos, Quaternion rot, Vector3 scale, Color color) {
            if (wireframeCube != null)
                foreach (var mesh in wireframeCube.Meshes) {
                    foreach (var meshPart in mesh.MeshParts) {
                        var matrix = Matrix.CreateScale(scale)
                                     * Matrix.CreateFromQuaternion(rot)
                                     * Matrix.CreateTranslation(pos);
                        var modelMatrix = mesh.ParentBone.Transform * matrix;

                        unlitEffect.Parameters["World"].SetValue(modelMatrix);
                        unlitEffect.Parameters["View"].SetValue(scene.Camera.GetViewMatrix());
                        unlitEffect.Parameters["Projection"].SetValue(scene.Camera.GetProjectionMatrix());
                        unlitEffect.Parameters["Color"].SetValue(color.ToVector4());
                        meshPart.Effect = unlitEffect;

                        _gd.SetVertexBuffer(meshPart.VertexBuffer);
                        _gd.Indices = meshPart.IndexBuffer;
                        var primitiveCount = meshPart.PrimitiveCount;
                        var vertexOffset = meshPart.VertexOffset;
                        var startIndex = meshPart.StartIndex;

                        // _gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexOffset, startIndex,
                        //primitiveCount);
                    }

                    mesh.Draw();
                }
        }

        private struct Cube {
            public readonly Vector3 Pos;
            public readonly Vector3 Scale;
            public readonly Quaternion Rot;
            public readonly Color Color;

            public Cube(Vector3 pos, Quaternion rot, Vector3 scale, Color color) {
                Scale = scale;
                Rot = rot;
                Pos = pos;
                Color = color;
            }
        }

        public class PlaceHolder : GameObject {
        }
    }
}