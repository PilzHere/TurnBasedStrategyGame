using System;
using System.Collections.Generic;
using System.Linq;
using LD49ogl.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Content;
using MonoGame.Extended.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using screen = MGLib.Screen.Screen;
using assMan = MGLib.AssetManager.AssetManager;

namespace MGLib
{
    /*
     * TODO: Make an Turn Based Strategy game? Theme is UNSTABLE.
     * Move around a tile-based map. Get into combat with others?
     * Advance Wars remake?
     * Resolution: GBA screen?
     */

    public class Game1 : Game
    {
        private readonly string GameTitle;
        private uint _ticks, _frames, _lastUps, _lastFps; // Actual ups, fps.

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private float _deltaTime, _fps;
        private double _countTicksAndFramesTimer, _runTime;
        private int _smoothedFps; // Not really useful.

        private double _windowCheckIfResizedTimer;

        private const int MaxWindowWidth = 7680, MaxWindowHeight = 4320; // Max 8k resolution.
        private const int MinWindowWidth = 240, MinWindowHeight = 160;
        private int windowScale = 4; // Only scales the window.
        private const int FboWidth = 240, FboHeight = 160;
        private int _windowedLastWidth, _windowedLastHeight;

        private RenderTarget2D _fbo;
        private Texture2D _fboTexture;

        private assMan _assMan;

        private Stack<screen> _gameScreens;

        //private TiledMap _tiledMap;
        //private TiledMapRenderer _tiledMapRenderer;

        public Game1(string gameTitle)
        {
            GameTitle = gameTitle;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;

            _graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.HardwareModeSwitch = false; // Faster on my machine (Linux).
            _graphics.PreferredBackBufferWidth = MinWindowWidth * windowScale;
            _graphics.PreferredBackBufferHeight = MinWindowHeight * windowScale;
            const int desiredMaxFps = 60; // Desired fps. Not ACTUAL FPS.
            TargetElapsedTime = TimeSpan.FromSeconds(1d / desiredMaxFps); // Cap max FPS.

            _graphics.ApplyChanges();

            _windowedLastWidth = Window.ClientBounds.Width;
            _windowedLastHeight = Window.ClientBounds.Height;

            base.Initialize();
        }

        private void UpdateWindowResize(GameTime gameTime)
        {
            _windowCheckIfResizedTimer += gameTime.ElapsedGameTime.TotalSeconds;
            var windowResizeTime = 0.2d;

            if (_windowCheckIfResizedTimer > windowResizeTime)
            {
                if (!_graphics.IsFullScreen)
                {
                    if (Window.ClientBounds.Width != _graphics.PreferredBackBufferWidth ||
                        Window.ClientBounds.Height != _graphics.PreferredBackBufferHeight)
                    {
                        _graphics.PreferredBackBufferWidth =
                            Math.Clamp(Window.ClientBounds.Width, MinWindowWidth, MaxWindowWidth);
                        _graphics.PreferredBackBufferHeight =
                            Math.Clamp(Window.ClientBounds.Height, MinWindowHeight, MaxWindowHeight);

                        _graphics.ApplyChanges();
                    }
                }

                _windowCheckIfResizedTimer = 0;
            }
        }

        private void UpdateWindowFullscreen()
        {
            _graphics.ToggleFullScreen();

            if (_graphics.IsFullScreen)
            {
                _windowedLastWidth = _graphics.PreferredBackBufferWidth;
                _windowedLastHeight = _graphics.PreferredBackBufferHeight;

                _graphics.PreferredBackBufferWidth =
                    Math.Clamp(Window.ClientBounds.Width, MinWindowWidth, MaxWindowWidth);
                _graphics.PreferredBackBufferHeight =
                    Math.Clamp(Window.ClientBounds.Height, MinWindowHeight, MaxWindowHeight);
            }
            else
            {
                _graphics.PreferredBackBufferWidth = _windowedLastWidth;
                _graphics.PreferredBackBufferHeight = _windowedLastHeight;
            }

            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _assMan = new assMan(Content);
            _assMan.LoadBasicAssets();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameScreens = new Stack<screen>();

            _fbo = new RenderTarget2D(_graphics.GraphicsDevice,
                FboWidth,
                FboHeight, false,
                SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);
            _fboTexture = _fbo;

            //_tiledMap = Content.Load<TiledMap>("Maps/map01");
            //_tiledMapRenderer = new TiledMapRenderer(_graphics.GraphicsDevice, _tiledMap);

            AddScreen(new MainMenuScreen(this, _assMan, _spriteBatch, GraphicsDevice));
        }

        public void AddScreen(screen newScreen)
        {
            _gameScreens.Push(newScreen);
        }

        public void RemoveFirstScreen()
        {
            _gameScreens.First().Destroy();
            _gameScreens.Pop();
        }

        private void RemoveAllScreens()
        {
            while (_gameScreens.Count != 0)
            {
                _gameScreens.First().Destroy();
                _gameScreens.Pop();
            }

            _gameScreens.Clear();
        }

        private void UpdateRunTime(GameTime gameTime)
        {
            _runTime += gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void CountTicksAndFramesPerSecond(GameTime gameTime)
        {
            _countTicksAndFramesTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (_countTicksAndFramesTimer > 1)
            {
                _lastUps = _ticks;
                _lastFps = _frames;

                _ticks = 0;
                _frames = 0;

                _countTicksAndFramesTimer--;
            }
        }

        private void CalculateDeltaTime(GameTime gameTime)
        {
            _deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _fps = 1 / _deltaTime;
            _smoothedFps = (int)Math.Round(_fps);
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateWindowResize(gameTime);

            UpdateRunTime(gameTime);
            CountTicksAndFramesPerSecond(gameTime);
            CalculateDeltaTime(gameTime);
            UpdateWindowTitle();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (KeyboardExtended.GetState().WasKeyJustDown(Keys.F))
            {
                UpdateWindowFullscreen();
            }

            _gameScreens.First().Input(gameTime, _deltaTime);
            _gameScreens.First().Tick(gameTime, _deltaTime);

            //_tiledMapRenderer.Update(gameTime);

            base.Update(gameTime);

            _ticks++;
        }

        private void UpdateWindowTitle()
        {
            var windowTitle = GameTitle + "    " + _lastFps + " FPS / " + _lastUps + " UPS " + _deltaTime +
                              " ms    Screens: " + _gameScreens.Count + "    Ents: " +
                              _gameScreens.First()._Entities.Count;
            Window.Title = windowTitle;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _graphics.GraphicsDevice.SetRenderTarget(_fbo); // render onto fbo
            _gameScreens.First().Draw(_deltaTime);
            _graphics.GraphicsDevice.SetRenderTarget(null); // stop fbo rendering

            // Draw the FBO onto screen
            _fboTexture = _fbo;
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            var fboScaleWidth = (float)_graphics.PreferredBackBufferWidth / FboWidth;
            var fboScaleHeight = (float)_graphics.PreferredBackBufferHeight / FboHeight;
            _spriteBatch.Draw(_fboTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero,
                new Vector2(fboScaleWidth, fboScaleHeight),
                SpriteEffects.None, 0);
            _spriteBatch.End();

            base.Draw(gameTime);

            _frames++;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            RemoveAllScreens();

            _spriteBatch.Dispose();

            _graphics.Dispose();

            base.OnExiting(sender, args);
        }
    }
}