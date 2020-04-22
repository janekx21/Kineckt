using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kineckt {
    public class Kineckt : Game {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Model _model;

        private const float NearClipPlane = .1f;

        private const float FarClipPlane = 200;

        private readonly float _fieldOfView = MathHelper.ToRadians(80f);

        private Texture2D _texture;
        private Effect _shadowMapGenerate;
        private RenderTarget2D _shadowMapRenderTarget;
        private Effect _diffuse;
        private RenderTarget2D _mainBuffer;
        private DepthStencilState _depth;

        private float AspectRatio => _graphics.PreferredBackBufferWidth / (float) _graphics.PreferredBackBufferHeight;

        public Kineckt() {
            _graphics = new GraphicsDeviceManager(this) {GraphicsProfile = GraphicsProfile.HiDef};
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            Window.AllowUserResizing = true;

            IsMouseVisible = true;

            _shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice,
                2048, 2048, false, SurfaceFormat.Single,
                DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
            _depth = new DepthStencilState
                {DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Less};

            _mainBuffer = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);


            Window.ClientSizeChanged += new EventHandler<EventArgs>((sender, args) => {
                _mainBuffer = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color,
                    DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
            });
            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _model = Content.Load<Model>("models/cube");
            _texture = Content.Load<Texture2D>("images/krakula-xl");
            _shadowMapGenerate = Content.Load<Effect>("shaders/ShadowMapsGenerate");
            _diffuse = Content.Load<Effect>("shaders/Diffuse");
        }

        protected override void UnloadContent() {
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            var rotation = Matrix.CreateRotationY((float) gameTime.TotalGameTime.TotalSeconds);
            var cameraPosition = Vector3.Transform(Vector3.Backward * 4, rotation);

            var cameraLookAtVector = Vector3.Zero;
            var cameraUpVector = Vector3.Up;

            var viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraLookAtVector, cameraUpVector);

            var projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                _fieldOfView,
                AspectRatio,
                NearClipPlane,
                FarClipPlane
            );

            Matrix lightView = Matrix.CreateLookAt(new Vector3(2, 8, 2),
                Vector3.Zero,
                Vector3.Up);

            Matrix lightProjection = Matrix.CreateOrthographic(10, 10, 1, 15);

            Matrix lightViewProjection = lightView * lightProjection;


            GraphicsDevice.SetRenderTarget(_shadowMapRenderTarget);
            GraphicsDevice.Clear(Color.Red);
            GraphicsDevice.DepthStencilState = _depth;
            DrawShadowMap(_model, Matrix.Identity, lightViewProjection);

            GraphicsDevice.SetRenderTarget(_mainBuffer);
            GraphicsDevice.Clear(Color.Aqua);
            GraphicsDevice.DepthStencilState = _depth;

            foreach (var mesh in _model.Meshes) {
                foreach (var meshPart in mesh.MeshParts) {
                    Matrix modelMatrix = mesh.ParentBone.Transform;

                    _diffuse.Parameters["World"].SetValue(modelMatrix);
                    _diffuse.Parameters["View"].SetValue(viewMatrix);
                    _diffuse.Parameters["Projection"].SetValue(projectionMatrix);

                    _diffuse.Parameters["Alberto"].SetValue(_texture);
                    _diffuse.Parameters["Shadow"].SetValue(_shadowMapRenderTarget);
                    _diffuse.Parameters["ShadowProjection"].SetValue(modelMatrix * lightViewProjection);
                    _diffuse.CurrentTechnique.Passes[0].Apply();

                    GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                    GraphicsDevice.Indices = meshPart.IndexBuffer;
                    var primitiveCount = meshPart.PrimitiveCount;
                    var vertexOffset = meshPart.VertexOffset;
                    var startIndex = meshPart.StartIndex;

                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexOffset, startIndex,
                        primitiveCount);
                }
            }

            base.Draw(gameTime);


            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(0, BlendState.Opaque, SamplerState.AnisotropicClamp);
            _spriteBatch.Draw(_mainBuffer,
                new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.Draw(_shadowMapRenderTarget, new Rectangle(0, 0, 200, 200), Color.White);
            _spriteBatch.End();
        }

        private void DrawShadowMap(Model model, Matrix modelWorldMatrix, Matrix lightViewProjection) {
            foreach (var mesh in model.Meshes) {
                foreach (var meshPart in mesh.MeshParts) {
                    Matrix modelMatrix = mesh.ParentBone.Transform;

                    _shadowMapGenerate.Parameters["LightViewProj"]
                        .SetValue(modelMatrix * modelWorldMatrix * lightViewProjection);

                    _shadowMapGenerate.CurrentTechnique.Passes[0].Apply();

                    GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                    GraphicsDevice.Indices = meshPart.IndexBuffer;
                    var primitiveCount = meshPart.PrimitiveCount;
                    var vertexOffset = meshPart.VertexOffset;
                    var startIndex = meshPart.StartIndex;

                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexOffset, startIndex,
                        primitiveCount);
                }
            }
        }
    }
}