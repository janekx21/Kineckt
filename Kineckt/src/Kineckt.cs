using System;
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
        public static readonly Random Rnd = new Random(Environment.TickCount);
        private Texture2D button;
        private Rectangle buttonRectangle;
        private SpriteFont font;

        public static int score = 0;
        public static float energy = 100;
        private Debug _debug;
        private Effect _postEffect;

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
                buttonRectangle = new Rectangle(20, GraphicsDevice.Viewport.Height - 20 - button.Height, button.Width,
                    button.Height);
            };

            _scene = new Scene();

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            DefaultShadowMapEffect = Content.Load<Effect>("shaders/ShadowMapsGenerate");

            DefaultEffect = Content.Load<Effect>("shaders/Diffuse");
            _postEffect = Content.Load<Effect>("shaders/Post");

            button = Content.Load<Texture2D>("images/green_button00");
            font = Content.Load<SpriteFont>("fonts/File");
            buttonRectangle = new Rectangle(20, GraphicsDevice.Viewport.Height - 20 - button.Height, button.Width,
                button.Height);


            // load content via reflection
            var subclassTypes = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var item in subclassTypes) {
                var funk = item.GetMethod("LoadContent", new[] {typeof(ContentManager)});
                try {
                    if (funk != null) funk.Invoke(null, new object[] {Content});
                }
                catch (Exception e) {
                    Console.WriteLine(item);
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        protected override void UnloadContent() {
        }

        protected override void BeginRun() {
            base.BeginRun();
            _scene.Spawn(new Sun {
                Position = new Vector3(200, 200, -50)
            });
            _scene.Spawn(new Camera(GraphicsDevice));

            _scene.Spawn(new Player(GraphicsDevice, _shadowMapRenderTarget) {
                Scene = _scene
            });
            _scene.Spawn(new Plane(GraphicsDevice, _shadowMapRenderTarget));
            _scene.Spawn(new EnemySpawner(GraphicsDevice, _shadowMapRenderTarget, _scene) {
                Position = new Vector3(0, 0, -20f)
            });

            _debug = new Debug(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_scene.GameObjects.Find(o => o is Player) == null) {
                float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
                dt *= .3f;
                gameTime.ElapsedGameTime = TimeSpan.FromSeconds(dt);
            }


            energy = (float) Math.Min(energy + gameTime.ElapsedGameTime.TotalSeconds * 50, 100);

            foreach (var go in new List<GameObject>(_scene.GameObjects)) go.Update(gameTime);

            var state = Mouse.GetState();
            if (state.LeftButton == ButtonState.Pressed) {
                if (buttonRectangle.Contains(state.Position)) {
                    Exit();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            DrawShadows(gameTime);

            DrawModels(gameTime);

            _debug.DrawDebug(_scene);

            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, RasterizerState.CullNone, _postEffect);

            _spriteBatch.Draw(_mainBuffer,
                new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.Draw(button, buttonRectangle.Location.ToVector2());
            var size = font.MeasureString("Exit");
            _spriteBatch.DrawString(font, "Exit", buttonRectangle.Center.ToVector2() - size * .5f, Color.White);

            var text = $"Score: {score}";
            var textSize = font.MeasureString(text);
            _spriteBatch.DrawString(font, text,
                GraphicsDevice.Viewport.Bounds.Size.ToVector2() - textSize - Vector2.One * 20, Color.White);

            var text2 = $"Energy: {energy:000}";
            if (energy <= 0) {
                text2 = $"Energy: ---";
            }

            var textSize2 = font.MeasureString(text2);
            _spriteBatch.DrawString(font, text2,
                GraphicsDevice.Viewport.Bounds.Size.ToVector2() - textSize2 - Vector2.One * 20 -
                Vector2.UnitY * textSize.Y, Color.White);

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