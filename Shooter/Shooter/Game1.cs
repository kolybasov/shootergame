using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Shooter
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
/// <summary>
/// Розділ оголошення змінних
/// </summary>
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        // Створення об'єкту гравець
        Player player;
        // Визначення стану клавіатури
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        // Визначення стану геймпаду
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;
        // Швидкість гравця
        float playerMoveSpeed;
        // Оголошення текстури для статичного фону
        Texture2D mainBackground;
        // Шари паралаксу
        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;
        // Вороги
        Texture2D enemyTexture;
        List<Enemy> enemies;
        // Характеристики, з якими з'являтимуться вороги
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;
        TimeSpan enemyDificultTime;
        // Генератор випадкових чисел
        Random random;
        // Текстури лазерів
        Texture2D projectileTexture;
        List<Projectile> projectiles;
        // Характеристики трільби
        TimeSpan fireTime;
        TimeSpan previousFireTime;
        // Список текстур для вибуху
        Texture2D explosionTexture;
        List<Animation> explosions;
        // Звуки
        SoundEffect laserSound;
        SoundEffect explosionSound;
        Song gameplayMusic;
        Song sufferGram;
        // Основні ігрові характеристики
        int score;
        int enemyHealth = 10;
        float enemyMoveSpeed = 6f;
        // Оголошення шрифту
        SpriteFont font;
        // Ініціалізація станів гри
        enum GameState
        { 
            MainMenu,
            Playing,
            Pause,
            Options,
            GameOver
        }
        // Стартовий стан - меню
        GameState currentGameState = GameState.MainMenu;
        // Ігрові екрани
        MainMenu menu;
        Options options;
        TimeSpan menuTime;
        TimeSpan previousMenuTime;
        Texture2D gameOver;

/// <summary>
/// Стандартні методи
/// </summary>
        // Конструктор
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = true;
        }
        // Стандартний метод ініціалізації
        protected override void Initialize()
        {
            Window.Title = "РІПТ 31-К 2012 Басов М.А.";
            // Ініціалізація класу гравця
            player = new Player();
            // Задання швидкості гравця
            playerMoveSpeed = 8.0f;
            // Ініціалізація шарів паралаксу
            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();
            // Ініціалізація списку ворогів
            enemies = new List<Enemy>();
            // Обнулення попереднього народження ворогів
            previousSpawnTime = TimeSpan.Zero;
            // Час, через який народжуються вороги
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
            // Ініціалізація генератора випадкових чисел
            random = new Random();
            // Ініціалізація списку лазерів
            projectiles = new List<Projectile>();
            // Задання проміжку, через який можна стріляти
            fireTime = TimeSpan.FromSeconds(.15f);
            // Ініціалізація списку вибухів
            explosions = new List<Animation>();
            // Обнулення очок
            score = 0;
            // Ігрові екрани
            menu = new MainMenu();
            menuTime = TimeSpan.FromSeconds(.3f);
            options = new Options();

            base.Initialize();
        }
        // Стандартна функція завантаження контенету
        protected override void LoadContent()
        {
            // Створення нового об'єкту SpriteBatch, який відповідає за малювання текстур
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Завантаження ресурсів гравця
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);
            // Стартова позиція гравця
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + playerAnimation.FrameWidth / 2, GraphicsDevice.Viewport.TitleSafeArea.Y
            + GraphicsDevice.Viewport.TitleSafeArea.Height/2);
            player.Initialize(playerAnimation, playerPosition);
            // Завантаження текстур паралаксу
            bgLayer1.Initialize(Content, "bgLayer1", GraphicsDevice.Viewport.Width, -1);
            bgLayer2.Initialize(Content, "bgLayer2", GraphicsDevice.Viewport.Width, -2);
            // Статичний фон
            mainBackground = Content.Load<Texture2D>("mainbackground");
            // Текстури ворогів
            enemyTexture = Content.Load<Texture2D>("mineAnimation");
            // Текстури лазерів
            projectileTexture = Content.Load<Texture2D>("laser");
            // Вибухи
            explosionTexture = Content.Load<Texture2D>("explosion");
            // Завантаження звуків
            gameplayMusic = Content.Load<Song>("sound/gameMusic");
            laserSound = Content.Load<SoundEffect>("sound/laserFire");
            explosionSound = Content.Load<SoundEffect>("sound/explosion");
            sufferGram = Content.Load<Song>("sound/suffer");
            // Завантаження шрифту
            font = Content.Load<SpriteFont>("gameFont");
            // Ігрові екрани
            gameOver = Content.Load<Texture2D>("endMenu");
            options.Initialize(Content, GraphicsDevice);
            menu.Initialize(Content, GraphicsDevice, font);
            // Відтворення музики у головному меню
            PlayMusic(menu.MenuMusic);
        }
        // Стандартний метод вивантаження контенту
        protected override void UnloadContent()
        { }
        // Стандартний метод, що слідкує за оновленнями гри
        protected override void Update(GameTime gameTime)
        {
            // Збереження попереднього стану геймпаду/клавіатури
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;
            // Зчитування поточного стану геймпаду/клавіатури
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            // Пасхалка
            if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.RightControl) && currentKeyboardState.IsKeyDown(Keys.Space))
                PlayMusic(sufferGram);
            // Перемикання між станами гри
            switch (currentGameState)
            {
                // Головне меню
                case GameState.MainMenu:
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                    UpdateMenu(gameTime);
                    break;
                // Стан гри
                case GameState.Playing:
                    //Оновлення стану гравця
                    UpdatePlayer(gameTime);
                    // Оновлення шарів паралаксу
                    bgLayer1.Update();
                    bgLayer2.Update();
                    // Оновлення стану ворогів
                    UpdateEnemies(gameTime);
                    // Обробіток зіткнень
                    UpdateCollision();
                    // Оновлення стану лазерів 
                    UpdateProjectiles();
                    // Оновлення стану вибухів
                    UpdateExplosions(gameTime);
                    break;
                // Стан паузи
                case GameState.Pause:
                    UpdatePause(gameTime);
                    break;
                // Стан опцій
                case GameState.Options:
                    UpdateOptions(gameTime);
                    break;
                // Стан завершеної гри
                case GameState.GameOver:
                    UpdateGameOver(gameTime);
                    break;
            }
            base.Update(gameTime);
        }
        // Стандартний метод малювання
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            // Початок малювання
            spriteBatch.Begin();
            // Стани гри
            switch (currentGameState)
            {
                // Головне меню
                case GameState.MainMenu:
                    menu.Draw(spriteBatch, font, GraphicsDevice);
                    break;
                // Опції
                case GameState.Options:
                    options.Draw(spriteBatch);
                    if (options.pointer == 1)
                        spriteBatch.DrawString(font, "enemies speed: " + Math.Round(enemyMoveSpeed, 1), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("enemies speed: " + Math.Round(enemyMoveSpeed, 1)).X / 2, 220), options.color);
                    else
                        spriteBatch.DrawString(font, "enemies speed: " + Math.Round(enemyMoveSpeed, 1), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("enemies speed: " + Math.Round(enemyMoveSpeed, 1)).X / 2, 220), Color.White);
                    if (options.pointer == 2)
                        spriteBatch.DrawString(font, "enemies health: " + enemyHealth, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("enemies health: " + enemyHealth).X / 2, 265), options.color);
                    else
                        spriteBatch.DrawString(font, "enemies health: " + enemyHealth, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("enemies health: " + enemyHealth).X / 2, 265), Color.White);
                    spriteBatch.DrawString(font, "press escape(back) to back to main menu", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("press escape(back) to back to main menu").X / 2, 310), Color.White);
                    break;
                // Пауза
                case GameState.Pause:
                    spriteBatch.DrawString(font, "game paused", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("game paused").X / 2, 180), Color.White);
                    spriteBatch.DrawString(font, "press backscape(B) to back to main menu", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("press backscape(B) to back to main menu").X / 2, 225), Color.White);
                    spriteBatch.DrawString(font, "press enter(Start) to continue", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("press enter(Start) to continue").X / 2, 270), Color.White);
                    break;
                // Гра
                case GameState.Playing:
                    // Статичний фон
                    spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);
                    // Паралакс
                    bgLayer1.Draw(spriteBatch);
                    bgLayer2.Draw(spriteBatch);
                    // ГРавець
                    player.Draw(spriteBatch);
                    // Вороги
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].Draw(spriteBatch);
                    }
                    // Лазери
                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        projectiles[i].Draw(spriteBatch);
                    }
                    // Вибухи
                    for (int i = 0; i < explosions.Count; i++)
                    {
                        explosions[i].Draw(spriteBatch);
                    }
                    // Очки
                    spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 170, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
                    // Здоров'я
                    spriteBatch.DrawString(font, "health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
                    break;
                // Заевршення гри
                case GameState.GameOver:
                    spriteBatch.Draw(gameOver, Vector2.Zero, Color.White);
                    spriteBatch.DrawString(font, "your score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("your score: " + score).X / 2, 220), Color.White);
                    spriteBatch.DrawString(font, "press escape(back) to exit", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("press escape(back) to exit").X / 2, 265), Color.White);
                    spriteBatch.DrawString(font, "enter(start) to start new game", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("enter(start) to start new game").X / 2, 310), Color.White);
                    break;
            }
            // Завершення малювання
            spriteBatch.End();
            base.Draw(gameTime);
        }
/// <summary>
/// Методи, що використовуються для меню
/// </summary>
        // Метод для циклічного відтворення музики
        private void PlayMusic(Song song)
        {
            try
            {
                // Почати відтворення
                MediaPlayer.Play(song);
                // Відтворювати циклічно
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }
        // Обробка головного меню
        private void UpdateMenu(GameTime gameTime)
        {
            // Скидання усіх показників
            score = 0;
            player.Health = 100;
            player.Position.X = player.Width / 2;
            player.Position.Y = GraphicsDevice.Viewport.Height / 2 - player.Height / 2 + 35;
            // Зупинка ворогів
            for (int i = 0; i < enemies.Count - 1; i++)
            {
                enemies[i].Active = false;
            }
            // При натисненні клавіші вниз, перелистування пунктів меню вниз
            if ((currentKeyboardState.IsKeyDown(Keys.Down)
                || currentGamePadState.DPad.Down == ButtonState.Pressed
                || currentGamePadState.ThumbSticks.Left.Y < 0)
                && gameTime.TotalGameTime - previousMenuTime > menuTime)
            {
                previousMenuTime = gameTime.TotalGameTime;
                if (menu.pointer <= 2)
                    menu.pointer += 1;
                else
                    menu.pointer = 1;
            }
            // При натисненні клавіші вгору, перелистування пунктів меню вгору
            else if ((currentKeyboardState.IsKeyDown(Keys.Up)
                || currentGamePadState.DPad.Up == ButtonState.Pressed
                || currentGamePadState.ThumbSticks.Left.Y > 0)
                && gameTime.TotalGameTime - previousMenuTime > menuTime)
            {
                previousMenuTime = gameTime.TotalGameTime;
                if (menu.pointer > 1)
                    menu.pointer -= 1;
                else
                    menu.pointer = 3;
            }
            // Вибір пункту меню
            if (currentKeyboardState.IsKeyDown(Keys.Enter) || currentGamePadState.Buttons.A == ButtonState.Pressed)
            {
                switch (menu.pointer)
                {
                    case 1:
                        PlayMusic(gameplayMusic);
                        currentGameState = GameState.Playing;
                        break;
                    case 2:
                        currentGameState = GameState.Options;
                        break;
                    case 3:
                        this.Exit();
                        break;
                }
            }
        }
        // Стан завершеної гри
        private void UpdateGameOver(GameTime gameTime)
        {
            // Зупинка відтворення музики  
            MediaPlayer.Stop();
            // Зупинка гравця
            player.Active = false;
            // Зупинка ворогів
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Active = false;
            }
            // Зупинка лазерів
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Active = false;
            }
            // Зупинка вибухів
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                }
            }
            // Старт нової гри
            if (currentKeyboardState.IsKeyDown(Keys.Enter)
                || currentGamePadState.Buttons.Start == ButtonState.Pressed)
            {
                score = 0;
                player.Health = 100;
                PlayMusic(gameplayMusic);
                player.Position.X = player.Width / 2;
                player.Position.Y = GraphicsDevice.Viewport.Height / 2 - player.Height / 2 + 35;
                currentGameState = GameState.Playing;
            }
            // Вихід у головне меню
            if (currentKeyboardState.IsKeyDown(Keys.Escape)
                || currentGamePadState.Buttons.Back == ButtonState.Pressed
                || currentGamePadState.Buttons.B == ButtonState.Pressed)
            {
                PlayMusic(menu.MenuMusic);
                currentGameState = GameState.MainMenu;
            }
            enemyHealth = 10;
            enemyMoveSpeed = 6f;
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
        }
        // Стан паузи
        private void UpdatePause(GameTime gameTime)
        {
            // Пауза відтворення
            MediaPlayer.Pause();
            // Зупинка вібрації гейймпаду
            GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
            // Продовження гри
            if (currentKeyboardState.IsKeyDown(Keys.Enter) || currentGamePadState.Buttons.Start == ButtonState.Pressed)
            {
                MediaPlayer.Resume();
                currentGameState = GameState.Playing;
            }
            //Вихід у головне меню
            if (currentKeyboardState.IsKeyDown(Keys.Back) || currentGamePadState.Buttons.B == ButtonState.Pressed)
            {
                PlayMusic(menu.MenuMusic);
                currentGameState = GameState.MainMenu;
            }
        }
        // Стан налаштувань
        private void UpdateOptions(GameTime gameTime)
        {
            // Гортання списку налаштувань
            if ((currentKeyboardState.IsKeyDown(Keys.Down)
                || currentGamePadState.DPad.Down == ButtonState.Pressed
                || currentGamePadState.ThumbSticks.Left.Y < 0)
                && gameTime.TotalGameTime - previousMenuTime > menuTime)
            {
                previousMenuTime = gameTime.TotalGameTime;
                if (options.pointer <= 1)
                    options.pointer += 1;
                else
                    options.pointer = 1;
            }
            else if ((currentKeyboardState.IsKeyDown(Keys.Up)
                || currentGamePadState.DPad.Up == ButtonState.Pressed
                || currentGamePadState.ThumbSticks.Left.Y > 0)
                && gameTime.TotalGameTime - previousMenuTime > menuTime)
            {
                previousMenuTime = gameTime.TotalGameTime;
                if (options.pointer > 1)
                    options.pointer -= 1;
                else
                    options.pointer = 2;
            }
            // Зміна налаштувань(збільшення)
            if ((currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed || currentGamePadState.ThumbSticks.Left.X > 0)
                && gameTime.TotalGameTime - previousMenuTime > TimeSpan.FromSeconds(.2f))
            {
                previousMenuTime = gameTime.TotalGameTime;
                switch (options.pointer)
                {
                    // Швидкість руху ворогів
                    case 1:
                        if (enemyMoveSpeed <= 29)
                            enemyMoveSpeed += .3f;
                        break;
                    // Кількість здоров'я ворогів
                    case 2:
                        if (enemyHealth <= 45)
                            enemyHealth += 1;
                        break;
                }
            }
            // Зміна налаштувань(зменшення)
            if ((currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed || currentGamePadState.ThumbSticks.Left.X < 0)
                && gameTime.TotalGameTime - previousMenuTime > TimeSpan.FromSeconds(.2f))
            {
                previousMenuTime = gameTime.TotalGameTime;
                switch (options.pointer)
                {
                    case 1:
                        if (enemyMoveSpeed >= 2)
                            enemyMoveSpeed -= .3f;
                        break;

                    case 2:
                        if (enemyHealth >= 10)
                            enemyHealth -= 1;
                        break;
                }
            }
            // Повернення до головного меню
            if (currentKeyboardState.IsKeyDown(Keys.Escape) || currentGamePadState.Buttons.Back == ButtonState.Pressed
                || currentGamePadState.Buttons.B == ButtonState.Pressed)
            {
                previousMenuTime = gameTime.TotalGameTime;
                currentGameState = GameState.MainMenu;
            }
        }
/// <summary>
/// Методи, що використовуються під час гри
/// </summary>
        // Додавання анімації вибуху
        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 1f, false);
            explosions.Add(explosion);
        }
        // Додавання ворогів
        private void AddEnemy()
        {
            // Створення об'єкту анімації
            Animation enemyAnimation = new Animation();
            // Ініціалізація анімації ворогів
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);
            // Випадкова генерація позиції ворогів
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));
            // Створення об'єкту ворог
            Enemy enemy = new Enemy();
            // Ініціалізація ворогів
            enemy.Initialize(enemyAnimation, position, enemyHealth, enemyMoveSpeed);
            // Додавання ворога до списку активних, в даний момент, ворогів
            enemies.Add(enemy);
        }
        // Обробка поводження ворогів
        private void UpdateEnemies(GameTime gameTime)
        {
            // Збільшення складності гри з часом
            if (score >= 2000 && gameTime.TotalGameTime - enemyDificultTime > TimeSpan.FromSeconds(20f))
            {
                enemyDificultTime = gameTime.TotalGameTime;
                enemyHealth += 2;
                enemyMoveSpeed += .3f;
                enemySpawnTime -= TimeSpan.FromSeconds(.1f);
            }
            // Народження нового ворога через заданий час
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;
                // Додавання ворога
                AddEnemy();
            }
            // Слідкування за показниками ворогів
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);
                if (enemies[i].Active == false && currentGameState != GameState.Pause)
                {
                    // Якщо у ворога більше немає життів
                    if (enemies[i].Health <= 0)
                    {
                        // Додати вибух
                        AddExplosion(enemies[i].Position);
                        // Відтворити звук вибуху
                        explosionSound.Play();
                        // Начислити очки
                        score += enemies[i].Value;
                    }
                    // Видалити ворога
                    enemies.RemoveAt(i);
                }
            }
        }
        // Обробіток вибухів
        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                // Вібрація геймпаду під час вибуху
                GamePad.SetVibration(PlayerIndex.One,0.3f,0.3f);
                if (explosions[i].Active == false)
                {
                    //Видалення вибуху і зупинка вібрації
                    explosions.RemoveAt(i);
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                }
            }
        }
        // Додавання лазерів
        private void AddProjectile(Vector2 position)
        {
            Projectile projectile = new Projectile();
            projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
            projectiles.Add(projectile);
        }
        // Обробіток лазерів
        private void UpdateProjectiles()
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();
                // Видалення лазеру
                if (projectiles[i].Active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }
        // Обробіток стану гравця
        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);
            // Управління за допомогою стіків
            player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;
            // Управління клавітурою або геймпадом
            if (currentKeyboardState.IsKeyDown(Keys.Left) ||
            currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                player.Position.X -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right) ||
            currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                player.Position.X += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) ||
            currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player.Position.Y -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) ||
            currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player.Position.Y += playerMoveSpeed;
            }
            // Пауза
            if (currentKeyboardState.IsKeyDown(Keys.Escape) || currentGamePadState.Buttons.Back == ButtonState.Pressed)
                currentGameState = GameState.Pause;
            // Стрільба
            if ((currentKeyboardState.IsKeyDown(Keys.Space) || currentGamePadState.Triggers.Right > 0)
                && gameTime.TotalGameTime - previousFireTime > fireTime)
            {
                // Зкидання поточного часу
                previousFireTime = gameTime.TotalGameTime;
                // Додавання лазеру
                AddProjectile(player.Position + new Vector2(player.Width / 2, 0));
                // Відтворення звуку лазеру
                laserSound.Play();
            }
            // Обмеження руху гравця за рамки ігрового вікна
            player.Position.X = MathHelper.Clamp(player.Position.X, 0 +player.Width / 2, GraphicsDevice.Viewport.Width - player.Width / 2);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0 +player.Height / 2+35, GraphicsDevice.Viewport.Height - player.Height / 2);
            // Відстеження здоров'я гравця
            if (player.Health <= 0)
                currentGameState = GameState.GameOver;
        }
        // Обробіток зіткнень
        private void UpdateCollision()
        {
            // Використання прямокутників для визначення зіткнень
            Rectangle rectangle1;
            Rectangle rectangle2;
            // Створення прямокутника з позицією гравця
            rectangle1 = new Rectangle((int)player.Position.X,
            (int)player.Position.Y,
            player.Width,
            player.Height);
            // Зіткнення гравця і ворогів
            for (int i = 0; i < enemies.Count; i++)
            {
                // Створення прямокутника з позицією ворога
                rectangle2 = new Rectangle((int)enemies[i].Position.X,
                (int)enemies[i].Position.Y,
                enemies[i].Width,
                enemies[i].Height);
                // Обробіток зіткнення
                if (rectangle1.Intersects(rectangle2))
                {
                    // Зменшення життів гравця
                    player.Health -= enemies[i].Damage;
                    // Знищення ворога
                    enemies[i].Health = 0;
                }

            }
            // Зіткнення лазерів і ворогів
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // Позиція лазеру
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);
                    //Позиція ворога
                    rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
                    (int)enemies[j].Position.Y - enemies[j].Height / 2,
                    enemies[j].Width, enemies[j].Height);
                    // Обробіток зіткнення
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies[j].Health -= projectiles[i].Damage;
                        projectiles[i].Active = false;
                    }
                }
            }
        }
    }
}
