using System;
using System.Collections.Generic;
using System.Reflection;
using Kineckt.GameObjects;
using Kineckt.Graphics;
using Kineckt.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plane = Kineckt.GameObjects.Plane;

namespace Kineckt {
    public class Kineckt : Game {
        public static readonly Random Rnd = new Random(Environment.TickCount);
        private Texture2D _button;
        private Rectangle _buttonRectangle;
        private Rectangle _button2Rectangle;
        private Debug _debug;
        private DepthStencilState _depth;
        private SpriteFont _font;
        private RenderTarget2D _mainBuffer;
        private Effect _postEffect;

        private Scene _scene;

        private RenderTarget2D _shadowMapRenderTarget;
        private SpriteBatch _spriteBatch;

        public Kineckt() {
            new GraphicsDeviceManager(this) {GraphicsProfile = GraphicsProfile.HiDef};
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
                _buttonRectangle = new Rectangle(20, GraphicsDevice.Viewport.Height - 20 - _button.Height,
                    _button.Width,
                    _button.Height);
                
                _button2Rectangle = new Rectangle(20 + _buttonRectangle.Width + 20, GraphicsDevice.Viewport.Height - 20 - _button.Height,
                    _button.Width,
                    _button.Height);
            };

            _scene = new Scene();

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            DefaultShadowMapEffect = Content.Load<Effect>("shaders/ShadowMapsGenerate");

            DefaultEffect = Content.Load<Effect>("shaders/Diffuse");
            _postEffect = Content.Load<Effect>("shaders/Post");

            _button = Content.Load<Texture2D>("images/green_button00");
            _font = Content.Load<SpriteFont>("fonts/File");
            
            _buttonRectangle = new Rectangle(20, GraphicsDevice.Viewport.Height - 20 - _button.Height, _button.Width,
                _button.Height);

            _button2Rectangle = new Rectangle(20 + _buttonRectangle.Width + 20, GraphicsDevice.Viewport.Height - 20 - _button.Height, _button.Width,
                _button.Height);

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

            ResetGame();
            _debug = new Debug(GraphicsDevice);
        }

        void ResetGame() {
            _scene.Reset();
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
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_scene.GameObjects.Find(o => o is Player) == null) {
                var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
                dt *= .3f;
                gameTime.ElapsedGameTime = TimeSpan.FromSeconds(dt);
            }

            foreach (var go in new List<GameObject>(_scene.GameObjects)) go.Update(gameTime);

            var state = Mouse.GetState();
            if (state.LeftButton == ButtonState.Pressed) {
                if (_buttonRectangle.Contains(state.Position)) {
                    Exit();
                }

                if (_button2Rectangle.Contains(state.Position)) {
                    ResetGame();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            DrawShadows(gameTime);

            DrawModels(gameTime);

            DrawUi(gameTime);

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
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.DepthStencilState = _depth;
            GraphicsDevice.RasterizerState = new RasterizerState {CullMode = CullMode.CullCounterClockwiseFace};

            foreach (var go in new List<GameObject>(_scene.GameObjects)) go.DrawShadow(_scene);
        }

        private void DrawModels(GameTime gameTime) {
            GraphicsDevice.SetRenderTarget(_mainBuffer);
            GraphicsDevice.Clear(Color.Black); // you can see this color while resetting
            GraphicsDevice.DepthStencilState = _depth;
            GraphicsDevice.RasterizerState = new RasterizerState {CullMode = CullMode.CullCounterClockwiseFace};

            foreach (var go in new List<GameObject>(_scene.GameObjects)) go.Draw(_scene);
            _debug.DrawDebug(_scene);
        }

        private void DrawUi(GameTime gameTime) {
            GraphicsDevice.SetRenderTarget(null);

            // Draw main buffer back to screen
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, RasterizerState.CullNone, _postEffect);
            _spriteBatch.Draw(_mainBuffer,
                new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, RasterizerState.CullNone);


            { // draw exit button
                _spriteBatch.Draw(_button, _buttonRectangle.Location.ToVector2());
                var size = _font.MeasureString("Exit");
                _spriteBatch.DrawString(_font, "Exit", _buttonRectangle.Center.ToVector2() - size * .5f, Color.White);
            }

            { // draw reset button
                const string text = "Reset";
                _spriteBatch.Draw(_button, _button2Rectangle.Location.ToVector2());
                var size = _font.MeasureString(text);
                _spriteBatch.DrawString(_font, text, _button2Rectangle.Center.ToVector2() - size * .5f, Color.White);
            }

            var player = _scene.GetFirstOrNull<Player>();
            if (player != null) {
                var text = $"Score: {player.Score}";
                var textSize = _font.MeasureString(text);
                _spriteBatch.DrawString(_font, text,
                    GraphicsDevice.Viewport.Bounds.Size.ToVector2() - textSize - Vector2.One * 20, Color.White);

                var energy = player.Energy;
                var text2 = $"Energy: {energy:000}";
                if (energy <= 0) text2 = "Energy: empty";

                var textSize2 = _font.MeasureString(text2);
                _spriteBatch.DrawString(_font, text2,
                    GraphicsDevice.Viewport.Bounds.Size.ToVector2() - textSize2 - Vector2.One * 20 -
                    Vector2.UnitY * textSize.Y, Color.White);
            }

            _spriteBatch.End();
        }
    }
}