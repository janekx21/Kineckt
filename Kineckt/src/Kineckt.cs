﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kineckt {
    public class Kineckt : Game {
        public static Effect DefaultEffect { get; private set; } = null;
        public static Effect DefaultShadowMapEffect { get; private set; } = null;
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private RenderTarget2D _shadowMapRenderTarget;
        private RenderTarget2D _mainBuffer;
        
        private DepthStencilState _depth;
        
        private Scene _scene;
        private ModelRenderer _ship;

        public Kineckt() {
            _graphics = new GraphicsDeviceManager(this) {GraphicsProfile = GraphicsProfile.HiDef};
            Content.RootDirectory = "Content";
        }

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
            _scene.Spawn(new Sun() {
                Position = new Vector3(2, 8, 2)
            });
            _scene.Spawn(new Camera(GraphicsDevice));

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var model = Content.Load<Model>("models/starshipOmega");
            var plane = Content.Load<Model>("models/plane");
            var texture = Content.Load<Texture2D>("images/starshipOmegaPaintOver");
            var krakula = Content.Load<Texture2D>("images/krakula-xl");
            DefaultShadowMapEffect = Content.Load<Effect>("shaders/ShadowMapsGenerate");

            DefaultEffect = Content.Load<Effect>("shaders/Diffuse");

            _ship = new ModelRenderer("Ship", GraphicsDevice, _shadowMapRenderTarget) {
                Model = model, Texture = texture
            };
            _scene.Spawn(_ship);
            _scene.Spawn(new ModelRenderer("Plane", GraphicsDevice, _shadowMapRenderTarget) {
                Model = plane, Texture = krakula
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
            // Turn camera
            var rotation = Matrix.CreateRotationY((float) gameTime.TotalGameTime.TotalSeconds);
            _scene.Camera.Position = Vector3.Transform(Vector3.Backward * 8, rotation);
            _ship.Rotation =
                Quaternion.CreateFromAxisAngle(Vector3.Up, (float) gameTime.TotalGameTime.TotalSeconds * -.3f);

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

        void SetupBuffers() {
            _shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice,
                2048, 2048, false, SurfaceFormat.Single,
                DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);

            _mainBuffer = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
            
            _depth = new DepthStencilState
                {DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Less};
        }

        void DrawShadows(GameTime gameTime) {
            GraphicsDevice.SetRenderTarget(_shadowMapRenderTarget);
            GraphicsDevice.Clear(Color.Red);
            GraphicsDevice.DepthStencilState = _depth;

            foreach (var go in _scene.GameObjects) {
                go.DrawShadow(_scene);
            }
        }

        void DrawModels(GameTime gameTime) {
            GraphicsDevice.SetRenderTarget(_mainBuffer);
            GraphicsDevice.Clear(Color.Aqua);
            GraphicsDevice.DepthStencilState = _depth;

            foreach (var go in _scene.GameObjects) {
                go.Draw(_scene);
            }
        }
    }
}