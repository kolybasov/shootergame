// Projectile.cs
//Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Shooter
{
    class Projectile
    {
        // Текстура
        public Texture2D Texture;
        // Позиція
        public Vector2 Position;
        // Стан
        public bool Active;
        // Пошкодження
        public int Damage;
        // Видима зона
        Viewport viewport;
        // Ширина
        public int Width
        {
            get { return Texture.Width; }
        }
        // Висота
        public int Height
        {
            get { return Texture.Height; }
        }
        // Швидкість руху
        float projectileMoveSpeed;

        public void Initialize(Viewport viewport, Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            this.viewport = viewport;
            // Стан - активний
            Active = true;
            // Пошкодження
            Damage = 5;
            // Швидкість руху
            projectileMoveSpeed = 20f;
        }

        public void Update()
        {
            // рух лазеру
            Position.X += projectileMoveSpeed;
            // Видалення лазеру за межами екрану
            if (Position.X + Texture.Width / 2 > viewport.Width)
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f,
            new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}
