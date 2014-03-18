// ParallaxingBackground.cs
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class ParallaxingBackground
    {
        // Фон
        Texture2D texture;
        // Масив позиція шарів паралаксу
        Vector2[] positions;
        // Швидкість руху фону
        int speed;
        // Ініціалізація
        public void Initialize(ContentManager content, String texturePath, int screenWidth, int speed)
        {
            // Завантаження текстури
            texture = content.Load<Texture2D>(texturePath);
            // Задання швидкості руху фону
            this.speed = speed;
            // Позиція фону
            positions = new Vector2[screenWidth / texture.Width + 1];
            // Позиціїї шарів
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = new Vector2(i * texture.Width, 0);
            }
        }

        public void Update()
        {
            // Оновлення позиції
            for (int i = 0; i < positions.Length; i++)
            {
                // Оновлення з заданою швидкістю
                positions[i].X += speed;
                // Рух у ліво
                if (speed <= 0)
                {
                    // Перевірка на вихід текстури за видиму область
                    if (positions[i].X <= -texture.Width)
                    {
                        positions[i].X = texture.Width * (positions.Length - 1);
                    }
                }
                // Рух вправо
                else
                {
                    // Перевірка на вихід текстури за видиму область
                    if (positions[i].X >= texture.Width * (positions.Length - 1))
                    {
                        positions[i].X = -texture.Width;
                    }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                spriteBatch.Draw(texture, positions[i], Color.White);
            }
        }
    }
}
