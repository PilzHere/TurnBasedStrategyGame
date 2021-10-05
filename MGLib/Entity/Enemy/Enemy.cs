using System;
using Microsoft.Xna.Framework;

namespace MGLib.Entity.Enemy
{
    public class Enemy : Entity
    {
        private Vector2 _position;

        public Enemy()
        {
            
        }

        public Enemy(Vector2 position) : this()
        {
            _position = position;
        }
        
        public Enemy(float x, float y) : this()
        {
            _position = new Vector2(x, y);
        }

        public override void Tick(float dt)
        {
            Console.WriteLine("Enemy Tick");
        }

        public override void Draw(float dt)
        {
            Console.WriteLine("Enemy Draw");
        }

        public override void Destroy()
        {
            Console.WriteLine("Enemy Destroy");

            base.Destroy();
        }
    }
}