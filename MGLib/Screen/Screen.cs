using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using assMan = MGLib.AssetManager.AssetManager;

namespace MGLib.Screen
{
    public abstract class Screen
    {
        protected Game1 Game;
        protected assMan AssMan;
        protected SpriteBatch SpriteBatch;
        protected List<Entity.Entity> Entities;

        public Screen(Game1 game, assMan assMan, SpriteBatch spriteBatch)
        {
            Game = game;
            AssMan = assMan;
            SpriteBatch = spriteBatch;

            Entities = new List<Entity.Entity>();
        }

        public abstract void Input(GameTime gt, float dt);
        public abstract void Tick(GameTime gt, float dt);
        public abstract void Draw(float dt);

        protected void UpdateAllEntities(float dt)
        {
            foreach (var entity in Entities)
            {
                entity.Tick(dt);
            }
        }

        protected void DrawAllEntities(float dt)
        {
            foreach (var entity in Entities)
            {
                entity.Draw(dt);
            }
        }

        protected void RemoveAllEntities()
        {
            Entities.Clear();
        }

        protected void RemoveDestroyedEntities()
        {
            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                var ent = Entities[i];
                if (ent._ToDestroy)
                {
                    var destroyedId = ent._Id;
                    Entities.RemoveAt(i);

                    Console.WriteLine("Destroyed entity: " + destroyedId);
                }
            }
        }

        public virtual void Destroy()
        {
            RemoveAllEntities();
        }

        public List<Entity.Entity> _Entities => Entities;
    }
}