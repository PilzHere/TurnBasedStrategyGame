using System;
using MGLib;
using MGLib.AssetManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;

namespace LD49ogl.Screen
{
    public class MainMenuScreen : MGLib.Screen.Screen
    {
        private GraphicsDevice _graphicsDevice;

        // test
        private Texture2D bg01, txtTank;
        private Sprite _spriteBg01, _spriteTank;

        private TiledMap _tiledMap;
        private TiledMapRenderer _tiledMapRenderer;

        private OrthographicCamera _camera;

        public MainMenuScreen(Game1 game, AssetManager assMan, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) :
            base(game, assMan, spriteBatch)
        {
            _graphicsDevice = graphicsDevice;

            bg01 = assMan.LoadAsset<Texture2D>("Textures/bg01");
            txtTank = assMan.LoadAsset<Texture2D>("Textures/tank01");
            _spriteBg01 = new Sprite(bg01);
            _spriteTank = new Sprite(txtTank);

            // Tiled maps render from top --> bottom!
            _tiledMap = AssMan.LoadAsset<TiledMap>("Maps/map01");

            _tiledMapRenderer = new TiledMapRenderer(_graphicsDevice, _tiledMap);

            var viewportAdapter = new BoxingViewportAdapter(game.Window, _graphicsDevice, 240, 160);
            _camera = new OrthographicCamera(viewportAdapter);
        }

        private int camSpeed = 100;
        private Vector2 camPos;
        private Vector2 tankPos;

        private bool _upKeyReleased, _downKeyReleased, _leftKeyReleased, _rightKeyReleased;

        private Keys _newScreenKey = Keys.R;
        
        // Arrow keys are not checked in WasJustReleased/WasJustPressed!
        private Keys _moveTankUpKey = Keys.Up,
            _moveTankDownKey = Keys.Down,
            _moveTankLeftKey = Keys.Left,
            _moveTankRightKey = Keys.Right;

        public override void Input(GameTime gt, float dt)
        {
            const int oneStep = 16;
            
            float camPosNewY = 0;
            float camPosNewX = 0;

            if (KeyboardExtended.GetState().IsKeyDown(_newScreenKey))
            {
                // BUG: Should actually be executed beginning of next tick. SCENE_MANAGER? 
                Game.AddScreen(new PlayScreen(Game, AssMan, SpriteBatch, _graphicsDevice));
            }

            // TANK
            if (KeyboardExtended.GetState().IsKeyUp(_moveTankUpKey))
            {
                _upKeyReleased = true;
            }

            if (KeyboardExtended.GetState().IsKeyDown(_moveTankUpKey))
            {
                if (_upKeyReleased)
                {
                    tankPos.Y -= oneStep;
                    _upKeyReleased = false;
                }
            }

            if (KeyboardExtended.GetState().IsKeyUp(_moveTankDownKey))
            {
                _downKeyReleased = true;
            }

            if (KeyboardExtended.GetState().IsKeyDown(_moveTankDownKey))
            {
                if (_downKeyReleased)
                {
                    tankPos.Y += oneStep;
                    _downKeyReleased = false;
                }
            }

            if (KeyboardExtended.GetState().IsKeyUp(_moveTankLeftKey))
            {
                _leftKeyReleased = true;
            }

            if (KeyboardExtended.GetState().IsKeyDown(_moveTankLeftKey))
            {
                if (_leftKeyReleased)
                {
                    tankPos.X -= oneStep;
                    _leftKeyReleased = false;
                }
            }

            if (KeyboardExtended.GetState().IsKeyUp(_moveTankRightKey))
            {
                _rightKeyReleased = true;
            }

            if (KeyboardExtended.GetState().IsKeyDown(_moveTankRightKey))
            {
                if (_rightKeyReleased)
                {
                    tankPos.X += oneStep;
                    _rightKeyReleased = false;
                }
            }

            //CAM
            if (KeyboardExtended.GetState().IsKeyDown(Keys.W))
            {
                camPosNewY -= camSpeed * dt; // north
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.S))
            {
                camPosNewY += camSpeed * dt; // south
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.A))
            {
                camPosNewX -= camSpeed * dt; // west
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.D))
            {
                camPosNewX += camSpeed * dt; // east
            }

            /*if (camPos != Vector2.Zero)
            {
                camPos.Normalize();
            }*/

            // Move camera in INTs for pixel-perfect rendering!
            camPos.Y += (int)camPosNewY;
            camPos.X += (int)camPosNewX;

            //Console.WriteLine("CamPos: " + camPos);
        }

        public override void Tick(GameTime gt, float dt)
        {
            RemoveDestroyedEntities();

            _camera.LookAt(camPos);

            _tiledMapRenderer.Update(gt);

            UpdateAllEntities(dt);

            //if (Entities.Count > 0)
            //    Entities[0].Destroy();
        }

        private float _newPosX, _newPosY;
        private int _acc;

        public override void Draw(float dt)
        {
            _graphicsDevice.Clear(Color.HotPink);

            _tiledMapRenderer.Draw(_camera.GetViewMatrix()); // Keep out of spriteBatch

            _spriteBg01.Depth = 1.0f; // 1 back, 0 front.
            _spriteTank.Depth = 0.0f; // 1 back, 0 front.

            SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null,
                null, _camera.GetViewMatrix());

            const float radius = 100;
            _newPosX += (float)Math.Cos(MathHelper.ToRadians(_acc)) * radius * dt;
            _newPosY += (float)Math.Sin(MathHelper.ToRadians(_acc)) * radius * dt;

            _acc++;
            const int resetAngle = 360;
            if (_acc >= resetAngle)
                _acc -= resetAngle;

            //Console.WriteLine(_acc);
            //Console.WriteLine(newPosX);

            // test
            //_sprite.Draw(SpriteBatch, new Vector2((int)_newPosX, (int)_newPosY), 0, new Vector2(1, 1));
            var tankPosInt = new Vector2((int)tankPos.X, (int)tankPos.Y);
            _spriteTank.Draw(SpriteBatch, tankPosInt, 0, new Vector2(1, 1));

            DrawAllEntities(dt);

            SpriteBatch.End();
        }

        public override void Destroy()
        {
            // remove shit here

            base.Destroy();
        }
    }
}