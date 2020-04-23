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
        private Model _plane;
        private Scene _scene;

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

            _scene = new Scene();
            _scene.Spawn(new Sun());
            _scene.Spawn(new Camera(GraphicsDevice));

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _model = Content.Load<Model>("models/starshipOmega");
            _plane = Content.Load<Model>("models/plane");
            _texture = Content.Load<Texture2D>("images/starshipOmegaPaintOver");
            var krakula = Content.Load<Texture2D>("images/krakula-xl");
            _shadowMapGenerate = Content.Load<Effect>("shaders/ShadowMapsGenerate");
            _diffuse = Content.Load<Effect>("shaders/Diffuse");
            
            _scene.Spawn(new ModelRenderer("foo", GraphicsDevice, _shadowMapRenderTarget) {
                Model = _model, Effect = _diffuse, Texture = _texture, ShadowMapEffect = _shadowMapGenerate
            });
            _scene.Spawn(new ModelRenderer("foo", GraphicsDevice, _shadowMapRenderTarget) {
                Model = _plane, Effect = _diffuse, Texture = krakula, ShadowMapEffect = _shadowMapGenerate
            });
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
            _scene.Camera.Position = Vector3.Transform(Vector3.Backward * 8, rotation);
            _scene.Sun.Position = new Vector3(2, 8, 2);

            GraphicsDevice.SetRenderTarget(_shadowMapRenderTarget);
            GraphicsDevice.Clear(Color.Red);
            GraphicsDevice.DepthStencilState = _depth;
            // DrawShadowMap(_model, _scene.Sun);
            foreach (var go in _scene.GameObjects) {
                go.DrawShadow(_scene);
            }

            GraphicsDevice.SetRenderTarget(_mainBuffer);
            GraphicsDevice.Clear(Color.Aqua);
            GraphicsDevice.DepthStencilState = _depth;

            foreach (var go in _scene.GameObjects) {
                go.Draw(_scene);
            }
            // DrawModel(_model, _scene.Camera, _scene.Sun);
            // DrawModel(_plane, _scene.Camera, _scene.Sun);
            /*
            foreach (var mesh in _model.Meshes) {
                foreach (var meshPart in mesh.MeshParts) {
                    var modelMatrix = mesh.ParentBone.Transform;

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
            */


            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(0, BlendState.Opaque, SamplerState.AnisotropicClamp);
            _spriteBatch.Draw(_mainBuffer,
                new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.Draw(_shadowMapRenderTarget, new Rectangle(0, 0, 200, 200), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /*
        private void DrawModel(ModelRenderer modelRenderer, Camera camera, Sun sun) {
            foreach (var mesh in modelRenderer.Meshes) {
                foreach (var meshPart in mesh.MeshParts) {
                    Matrix modelMatrix = mesh.ParentBone.Transform;

                    _diffuse.Parameters["World"].SetValue(modelMatrix);
                    _diffuse.Parameters["View"].SetValue(camera.GetViewMatrix());
                    _diffuse.Parameters["Projection"].SetValue(camera.GetProjectionMatrix());

                    _diffuse.Parameters["Alberto"].SetValue(_texture);
                    _diffuse.Parameters["Shadow"].SetValue(_shadowMapRenderTarget);
                    _diffuse.Parameters["ShadowProjection"].SetValue(modelMatrix * sun.GetLightViewProjection());
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
        }

        private void DrawShadowMap(ModelRenderer modelRenderer, Sun sun) {
            foreach (var mesh in modelRenderer.Meshes) {
                foreach (var meshPart in mesh.MeshParts) {
                    Matrix modelMatrix = mesh.ParentBone.Transform;

                    _shadowMapGenerate.Parameters["LightViewProj"]
                        .SetValue(modelMatrix * sun.GetLightViewProjection());

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
        */
    }
}