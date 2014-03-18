using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace Shooter
{
    class MainMenu
    {
        // Фон
        Texture2D Texture;
        // Позиція
        Rectangle Position;
        // Музика в меню
        public Song MenuMusic;
        // Колір
        Color Color;
        // Вказівник
        public int pointer;
        // Пункти меню
        String menuNew = "New game";
        String menuOptions = "Options";
        String menuExit = "Exit";
        // Координати пунктів
        Vector2 stringWidth1, stringWidth2, stringWidth3;

        public void Initialize(ContentManager content, GraphicsDevice graphics, SpriteFont font)
        {
            Texture = content.Load<Texture2D>("mainMenu");
            Position = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
            MenuMusic = content.Load<Song>("Sound/menuMusic");
            Color = new Color(0,0,0);
            stringWidth1 = font.MeasureString(menuNew);
            stringWidth2 = font.MeasureString(menuOptions);
            stringWidth3 = font.MeasureString(menuExit);
            pointer = 1;
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphics)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
            // Нова гра
            if(pointer == 1)
                spriteBatch.DrawString(font, menuNew, new Vector2(graphics.Viewport.TitleSafeArea.Width / 2 - stringWidth1.X /2, 220), Color);
            else
                spriteBatch.DrawString(font, menuNew, new Vector2(graphics.Viewport.TitleSafeArea.Width / 2 - stringWidth1.X / 2, 220), Color.White);
            // Налаштування
            if(pointer == 2)
                spriteBatch.DrawString(font, menuOptions, new Vector2(graphics.Viewport.TitleSafeArea.Width / 2 - stringWidth2.X / 2, 265), Color);
            else
                spriteBatch.DrawString(font, menuOptions, new Vector2(graphics.Viewport.TitleSafeArea.Width / 2 - stringWidth2.X / 2, 265), Color.White);
            // Вихід
            if(pointer == 3)
                spriteBatch.DrawString(font, menuExit, new Vector2(graphics.Viewport.TitleSafeArea.Width / 2 - stringWidth3.X / 2, 310), Color);
            else
                spriteBatch.DrawString(font, menuExit, new Vector2(graphics.Viewport.TitleSafeArea.Width / 2 - stringWidth3.X / 2, 310), Color.White);
        }
    }
}
