using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace Shooter
{
    class Options
    {
        // Фон
        Texture2D Texture;
        // Позиція
        Rectangle Position;
        // Вказівник
        public int pointer;
        // Колір виділення
        public Color color = new Color(0,0,0);
        // Ініціалізація
        public void Initialize(ContentManager content, GraphicsDevice graphics)
        {
            Texture = content.Load<Texture2D>("mainMenu");
            Position = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
            pointer = 1;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

    }
}
