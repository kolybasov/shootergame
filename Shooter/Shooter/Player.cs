using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Shooter
{
    class Player
    {
        // Анімація гравця
        public Animation PlayerAnimation;
        // Позиція
        public Vector2 Position;
        // Стан
        public bool Active;
        // Здоров'я
        public int Health;
        // Ширина текстури
        public int Width
        {
            get { return PlayerAnimation.FrameWidth; }
        }
        // Висота текстури
        public int Height
        {
            get { return PlayerAnimation.FrameHeight; }
        }
        // Ініціалізація
        public void Initialize(Animation animation, Vector2 position)
        {
            PlayerAnimation = animation;
            // Стартова позиція
            Position = position;
            // Стан - активний
            Active = true;
            // Здоров'я 100
            Health = 100;
        }
        // Оновлення анімації гравця
        public void Update(GameTime gameTime)
        {
            PlayerAnimation.Position = Position;
            PlayerAnimation.Update(gameTime);
        }
        // Малювання гравця
        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerAnimation.Draw(spriteBatch);
        }

    }
}
