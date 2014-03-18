// Animation.cs
//Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class Animation
    {
        // Текстури для анімації
        Texture2D spriteStrip;
        // Шкала
        float scale;
        // Останнє оновлення
        int elapsedTime;
        // Час оновлення
        int frameTime;
        // Кількість кадрів
        int frameCount;
        // Індекс поточного кадру
        int currentFrame;
        // Колір
        Color color;
        // Площа текстури, яку хочемо показати
        Rectangle sourceRect = new Rectangle();
        // Координати місця виведення анімації
        Rectangle destinationRect = new Rectangle();
        // Ширина кадру
        public int FrameWidth;
        // Висота кадру
        public int FrameHeight;
        // Стан анімації
        public bool Active;
        // Зациклення
        public bool Looping;
        // Ширина отриманого кадру
        public Vector2 Position;


        public void Initialize(Texture2D texture, Vector2 position,
            int frameWidth, int frameHeight, int frameCount,
            int frametime, Color color, float scale, bool looping)
        {
            this.color = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.scale = scale;
            Looping = looping;
            Position = position;
            spriteStrip = texture;
            // Обнулення часу
            elapsedTime = 0;
            currentFrame = 0;
            // Активація анімації
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            // Не оовлювати гру, коли анімація зупинена
            if (Active == false)
                return;
            // Оновлення пройденого часу
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            // Перемикання кадрів
            if (elapsedTime > frameTime)
            {
                // Рух до наступного кадру
                currentFrame++;
                // Якщо кадри закінчились, обнулити
                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    // Якщо анімація не циклічна, зупинити
                    if (Looping == false)
                        Active = false;
                }
                // Обнулення пройденого часу
                elapsedTime = 0;
            }
            // Вибір правильного кадру з текстури
            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
            destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,
            (int)Position.Y - (int)(FrameHeight * scale) / 2,
            (int)(FrameWidth * scale),
            (int)(FrameHeight * scale));
        }
        // Малювання анімації
        public void Draw(SpriteBatch spriteBatch)
        {
            // Малювати анімацію, лише коли активна
            if (Active)
            {
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
            }
        }

    }
}
