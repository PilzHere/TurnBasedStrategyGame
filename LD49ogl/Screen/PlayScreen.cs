using System;
using MGLib;
using MGLib.AssetManager;
using MGLib.Entity.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace LD49ogl.Screen
{
    public class PlayScreen : MGLib.Screen.Screen
    {
        private GraphicsDevice _graphicsDevice;
        
        public PlayScreen(Game1 game, AssetManager assMan, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) : base(game, assMan, spriteBatch)
        {
            _graphicsDevice = graphicsDevice;
            
            Entities.Add(new Enemy(0, 0));
        }

        public override void Input(GameTime gt, float dt)
        {
            //Console.WriteLine("PlayScreen Input");
            
            if (KeyboardExtended.GetState().IsKeyDown(Keys.T))
            {
                Game.RemoveFirstScreen();
            }
        }

        public override void Tick(GameTime gt, float dt)
        {
            //Console.WriteLine("PlayScreen Tick");
            
            RemoveDestroyedEntities();

            UpdateAllEntities(dt);
            
            //if (Entities.Count > 0)
            //    Entities[0].Destroy();
        }

        public override void Draw(float dt)
        {
            //Console.WriteLine("PlayScreen Draw");
            
            _graphicsDevice.Clear(Color.CornflowerBlue);

            DrawAllEntities(dt);
        }
        
        public override void Destroy()
        {
            // remove shit here
            
            base.Destroy();
        }
    }
}