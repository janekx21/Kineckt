using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kineckt {
    public class Kineckt : Game {
        private readonly GraphicsDeviceManager _graphics;

        private DepthStencilState _depth;
        private RenderTarget2D _mainBuffer;

        private Scene _scene;

        private RenderTarget2D _shadowMapRenderTarget;
        private SpriteBatch _spriteBatch;

        public Kineckt() {
            _graphics = new GraphicsDeviceManager(this) {GraphicsProfile = GraphicsProfile.HiDef};
            Content.RootDirectory = "Content";
        }

        public static Effect DefaultEffect { get; private set; }
        public static Effect DefaultShadowMapEffect { get; private set; }

        protected override void Initialize() {
            Window.AllowUserResizing = true;

            IsMouseVisible = true;

            SetupBuffers();

            Window.ClientSizeChanged += (sender, args) => {
                _mainBuffer = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color,
                    DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
            };

            _scene = new Scene();

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            DefaultShadowMapEffect = Content.Load<Effect>("shaders/ShadowMapsGenerate");

            DefaultEffect = Content.Load<Effect>("shaders/Diffuse");


            // load content via reflection
            var subclassTypes = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var item in subclassTypes) {
                var funk = item.GetMethod("LoadContent", new[] {typeof(ContentManager)});
                if (funk != null) funk.Invoke(null, new object[] {Content});
            }
        }

        protected override void UnloadContent() {
        }

        protected override void BeginRun() {
            base.BeginRun();
            _scene.Spawn(new Sun {
                Position = new Vector3(28, 20, 2)
            });
            _scene.Spawn(new Camera(GraphicsDevice));

            _scene.Spawn(new Player(GraphicsDevice, _shadowMapRenderTarget) {
                Scene = _scene
            });
            _scene.Spawn(new Plane(GraphicsDevice, _shadowMapRenderTarget));
            _scene.Spawn(new Enemy(GraphicsDevice, _shadowMapRenderTarget)
            {
                Position = new Vector3(0, 0, 0)
            });

        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            foreach (var go in new List<GameObject>(_scene.GameObjects)) go.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            // Turn camera
            // var rotation = Matrix.CreateRotationY((float) gameTime.TotalGameTime.TotalSeconds);
            // _scene.Camera.Position = Vector3.Transform(Vector3.Backward * 8, rotation);
            /*
            _ship.Rotation =
                Quaternion.CreateFromAxisAngle(Vector3.Up, (float) gameTime.TotalGameTime.TotalSeconds * -.3f);
                */


            DrawShadows(gameTime);

            DrawModels(gameTime);

            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(0, BlendState.Opaque, SamplerState.AnisotropicClamp);
            _spriteBatch.Draw(_mainBuffer,
                new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.Draw(_shadowMapRenderTarget, new Rectangle(0, 0, 200, 200), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SetupBuffers() {
            _shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice,
                2048, 2048, false, SurfaceFormat.Single,
                DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);

            _mainBuffer = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);

            _depth = new DepthStencilState
                {DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Less};
        }

        private void DrawShadows(GameTime gameTime) {
            GraphicsDevice.SetRenderTarget(_shadowMapRenderTarget);
            GraphicsDevice.Clear(Color.Red);
            GraphicsDevice.DepthStencilState = _depth;
            GraphicsDevice.RasterizerState = new RasterizerState {CullMode = CullMode.CullCounterClockwiseFace};

            foreach (var go in new List<GameObject>(_scene.GameObjects)) go.DrawShadow(_scene);
        }

        private void DrawModels(GameTime gameTime) {
            GraphicsDevice.SetRenderTarget(_mainBuffer);
            GraphicsDevice.Clear(Color.Aqua);
            GraphicsDevice.DepthStencilState = _depth;
            GraphicsDevice.RasterizerState = new RasterizerState {CullMode = CullMode.CullCounterClockwiseFace};

            foreach (var go in new List<GameObject>(_scene.GameObjects)) go.Draw(_scene);
        }
    }
}