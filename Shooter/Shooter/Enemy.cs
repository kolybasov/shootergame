using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Shooter
{

    class Enemy
    {
        // Анімація
        public Animation EnemyAnimation;
        // Позиція
        public Vector2 Position;
        // Стан
        public bool Active;
        // Здоров'я
        public int Health;
        // Пошкодження
        public int Damage;
        // Кількість очок 
        public int Value;
        // Ширина 
        public int Width
        {
            get { return EnemyAnimation.FrameWidth; }
        }
        // Висота
        public int Height
        {
            get { return EnemyAnimation.FrameHeight; }
        }
        // Швидкість руху по осі Х
        float enemyMoveSpeed;
        // Швидкість руху по осі Y
        float enemyMoveSpeedY;

        public void Initialize(Animation animation, Vector2 position, int health, float moveSpeed)
        {
            // Завантаження текстури
            EnemyAnimation = animation;
            // Стартова позиція
            Position = position;
            // Стан - активний
            Active = true;
            // Здоров'я
            Health = health;
            // Пошкодження
            Damage = 10;
            // Швидкість
            enemyMoveSpeed = moveSpeed;
            // Кількість очок
            Value = 100;
            // Швидкість руху по вертикалі
            Random rnd = new Random();
            enemyMoveSpeedY = (float)(rnd.Next(-2, 2) * 0.4);
        }

        public void Update(GameTime gameTime)
        {
            // Рух
            Position.X -= enemyMoveSpeed;
            Position.Y += enemyMoveSpeedY;
            // Позиція
            EnemyAnimation.Position = Position;
            // Анімація
            EnemyAnimation.Update(gameTime);
            // Видалення ворога, якщо він вилетів за межі екрану, або здоров'я дорівнює 0
            if (Position.X < -Width || Health <= 0 || Position.Y < -Height)
            {
                Active = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            EnemyAnimation.Draw(spriteBatch);
        }

    }
}
