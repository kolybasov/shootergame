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
/// ����� ���������� ������
/// </summary>
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        // ��������� ��'���� �������
        Player player;
        // ���������� ����� ���������
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        // ���������� ����� ��������
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;
        // �������� ������
        float playerMoveSpeed;
        // ���������� �������� ��� ���������� ����
        Texture2D mainBackground;
        // ���� ���������
        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;
        // ������
        Texture2D enemyTexture;
        List<Enemy> enemies;
        // ��������������, � ����� �'������������ ������
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;
        TimeSpan enemyDificultTime;
        // ��������� ���������� �����
        Random random;
        // �������� ������
        Texture2D projectileTexture;
        List<Projectile> projectiles;
        // �������������� ������
        TimeSpan fireTime;
        TimeSpan previousFireTime;
        // ������ ������� ��� ������
        Texture2D explosionTexture;
        List<Animation> explosions;
        // �����
        SoundEffect laserSound;
        SoundEffect explosionSound;
        Song gameplayMusic;
        Song sufferGram;
        // ������ ����� ��������������
        int score;
        int enemyHealth = 10;
        float enemyMoveSpeed = 6f;
        // ���������� ������
        SpriteFont font;
        // ����������� ����� ���
        enum GameState
        { 
            MainMenu,
            Playing,
            Pause,
            Options,
            GameOver
        }
        // ��������� ���� - ����
        GameState currentGameState = GameState.MainMenu;
        // ����� ������
        MainMenu menu;
        Options options;
        TimeSpan menuTime;
        TimeSpan previousMenuTime;
        Texture2D gameOver;

/// <summary>
/// ��������� ������
/// </summary>
        // �����������
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = true;
        }
        // ����������� ����� �����������
        protected override void Initialize()
        {
            Window.Title = "в�� 31-� 2012 ����� �.�.";
            // ����������� ����� ������
            player = new Player();
            // ������� �������� ������
            playerMoveSpeed = 8.0f;
            // ����������� ���� ���������
            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();
            // ����������� ������ ������
            enemies = new List<Enemy>();
            // ��������� ������������ ���������� ������
            previousSpawnTime = TimeSpan.Zero;
            // ���, ����� ���� ������������ ������
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
            // ����������� ���������� ���������� �����
            random = new Random();
            // ����������� ������ ������
            projectiles = new List<Projectile>();
            // ������� �������, ����� ���� ����� �������
            fireTime = TimeSpan.FromSeconds(.15f);
            // ����������� ������ �������
            explosions = new List<Animation>();
            // ��������� ����
            score = 0;
            // ����� ������
            menu = new MainMenu();
            menuTime = TimeSpan.FromSeconds(.3f);
            options = new Options();

            base.Initialize();
        }
        // ���������� ������� ������������ ���������
        protected override void LoadContent()
        {
            // ��������� ������ ��'���� SpriteBatch, ���� ������� �� ��������� �������
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // ������������ ������� ������
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);
            // �������� ������� ������
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + playerAnimation.FrameWidth / 2, GraphicsDevice.Viewport.TitleSafeArea.Y
            + GraphicsDevice.Viewport.TitleSafeArea.Height/2);
            player.Initialize(playerAnimation, playerPosition);
            // ������������ ������� ���������
            bgLayer1.Initialize(Content, "bgLayer1", GraphicsDevice.Viewport.Width, -1);
            bgLayer2.Initialize(Content, "bgLayer2", GraphicsDevice.Viewport.Width, -2);
            // ��������� ���
            mainBackground = Content.Load<Texture2D>("mainbackground");
            // �������� ������
            enemyTexture = Content.Load<Texture2D>("mineAnimation");
            // �������� ������
            projectileTexture = Content.Load<Texture2D>("laser");
            // ������
            explosionTexture = Content.Load<Texture2D>("explosion");
            // ������������ �����
            gameplayMusic = Content.Load<Song>("sound/gameMusic");
            laserSound = Content.Load<SoundEffect>("sound/laserFire");
            explosionSound = Content.Load<SoundEffect>("sound/explosion");
            sufferGram = Content.Load<Song>("sound/suffer");
            // ������������ ������
            font = Content.Load<SpriteFont>("gameFont");
            // ����� ������
            gameOver = Content.Load<Texture2D>("endMenu");
            options.Initialize(Content, GraphicsDevice);
            menu.Initialize(Content, GraphicsDevice, font);
            // ³��������� ������ � ��������� ����
            PlayMusic(menu.MenuMusic);
        }
        // ����������� ����� ������������ ��������
        protected override void UnloadContent()
        { }
        // ����������� �����, �� ����� �� ����������� ���
        protected override void Update(GameTime gameTime)
        {
            // ���������� ������������ ����� ��������/���������
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;
            // ���������� ��������� ����� ��������/���������
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            // ��������
            if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.RightControl) && currentKeyboardState.IsKeyDown(Keys.Space))
                PlayMusic(sufferGram);
            // ����������� �� ������� ���
            switch (currentGameState)
            {
                // ������� ����
                case GameState.MainMenu:
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                    UpdateMenu(gameTime);
                    break;
                // ���� ���
                case GameState.Playing:
                    //��������� ����� ������
                    UpdatePlayer(gameTime);
                    // ��������� ���� ���������
                    bgLayer1.Update();
                    bgLayer2.Update();
                    // ��������� ����� ������
                    UpdateEnemies(gameTime);
                    // �������� �������
                    UpdateCollision();
                    // ��������� ����� ������ 
                    UpdateProjectiles();
                    // ��������� ����� �������
                    UpdateExplosions(gameTime);
                    break;
                // ���� �����
                case GameState.Pause:
                    UpdatePause(gameTime);
                    break;
                // ���� �����
                case GameState.Options:
                    UpdateOptions(gameTime);
                    break;
                // ���� ��������� ���
                case GameState.GameOver:
                    UpdateGameOver(gameTime);
                    break;
            }
            base.Update(gameTime);
        }
        // ����������� ����� ���������
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            // ������� ���������
            spriteBatch.Begin();
            // ����� ���
            switch (currentGameState)
            {
                // ������� ����
                case GameState.MainMenu:
                    menu.Draw(spriteBatch, font, GraphicsDevice);
                    break;
                // �����
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
                // �����
                case GameState.Pause:
                    spriteBatch.DrawString(font, "game paused", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("game paused").X / 2, 180), Color.White);
                    spriteBatch.DrawString(font, "press backscape(B) to back to main menu", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("press backscape(B) to back to main menu").X / 2, 225), Color.White);
                    spriteBatch.DrawString(font, "press enter(Start) to continue", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("press enter(Start) to continue").X / 2, 270), Color.White);
                    break;
                // ���
                case GameState.Playing:
                    // ��������� ���
                    spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);
                    // ��������
                    bgLayer1.Draw(spriteBatch);
                    bgLayer2.Draw(spriteBatch);
                    // �������
                    player.Draw(spriteBatch);
                    // ������
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].Draw(spriteBatch);
                    }
                    // ������
                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        projectiles[i].Draw(spriteBatch);
                    }
                    // ������
                    for (int i = 0; i < explosions.Count; i++)
                    {
                        explosions[i].Draw(spriteBatch);
                    }
                    // ����
                    spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 170, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
                    // ������'�
                    spriteBatch.DrawString(font, "health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
                    break;
                // ���������� ���
                case GameState.GameOver:
                    spriteBatch.Draw(gameOver, Vector2.Zero, Color.White);
                    spriteBatch.DrawString(font, "your score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("your score: " + score).X / 2, 220), Color.White);
                    spriteBatch.DrawString(font, "press escape(back) to exit", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("press escape(back) to exit").X / 2, 265), Color.White);
                    spriteBatch.DrawString(font, "enter(start) to start new game", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - font.MeasureString("enter(start) to start new game").X / 2, 310), Color.White);
                    break;
            }
            // ���������� ���������
            spriteBatch.End();
            base.Draw(gameTime);
        }
/// <summary>
/// ������, �� ���������������� ��� ����
/// </summary>
        // ����� ��� ��������� ���������� ������
        private void PlayMusic(Song song)
        {
            try
            {
                // ������ ����������
                MediaPlayer.Play(song);
                // ³���������� �������
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }
        // ������� ��������� ����
        private void UpdateMenu(GameTime gameTime)
        {
            // �������� ��� ���������
            score = 0;
            player.Health = 100;
            player.Position.X = player.Width / 2;
            player.Position.Y = GraphicsDevice.Viewport.Height / 2 - player.Height / 2 + 35;
            // ������� ������
            for (int i = 0; i < enemies.Count - 1; i++)
            {
                enemies[i].Active = false;
            }
            // ��� ��������� ������ ����, �������������� ������ ���� ����
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
            // ��� ��������� ������ �����, �������������� ������ ���� �����
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
            // ���� ������ ����
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
        // ���� ��������� ���
        private void UpdateGameOver(GameTime gameTime)
        {
            // ������� ���������� ������  
            MediaPlayer.Stop();
            // ������� ������
            player.Active = false;
            // ������� ������
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Active = false;
            }
            // ������� ������
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Active = false;
            }
            // ������� �������
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                }
            }
            // ����� ���� ���
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
            // ����� � ������� ����
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
        // ���� �����
        private void UpdatePause(GameTime gameTime)
        {
            // ����� ����������
            MediaPlayer.Pause();
            // ������� ������� ���������
            GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
            // ����������� ���
            if (currentKeyboardState.IsKeyDown(Keys.Enter) || currentGamePadState.Buttons.Start == ButtonState.Pressed)
            {
                MediaPlayer.Resume();
                currentGameState = GameState.Playing;
            }
            //����� � ������� ����
            if (currentKeyboardState.IsKeyDown(Keys.Back) || currentGamePadState.Buttons.B == ButtonState.Pressed)
            {
                PlayMusic(menu.MenuMusic);
                currentGameState = GameState.MainMenu;
            }
        }
        // ���� �����������
        private void UpdateOptions(GameTime gameTime)
        {
            // �������� ������ �����������
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
            // ���� �����������(���������)
            if ((currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed || currentGamePadState.ThumbSticks.Left.X > 0)
                && gameTime.TotalGameTime - previousMenuTime > TimeSpan.FromSeconds(.2f))
            {
                previousMenuTime = gameTime.TotalGameTime;
                switch (options.pointer)
                {
                    // �������� ���� ������
                    case 1:
                        if (enemyMoveSpeed <= 29)
                            enemyMoveSpeed += .3f;
                        break;
                    // ʳ������ ������'� ������
                    case 2:
                        if (enemyHealth <= 45)
                            enemyHealth += 1;
                        break;
                }
            }
            // ���� �����������(���������)
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
            // ���������� �� ��������� ����
            if (currentKeyboardState.IsKeyDown(Keys.Escape) || currentGamePadState.Buttons.Back == ButtonState.Pressed
                || currentGamePadState.Buttons.B == ButtonState.Pressed)
            {
                previousMenuTime = gameTime.TotalGameTime;
                currentGameState = GameState.MainMenu;
            }
        }
/// <summary>
/// ������, �� ���������������� �� ��� ���
/// </summary>
        // ��������� ������� ������
        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 1f, false);
            explosions.Add(explosion);
        }
        // ��������� ������
        private void AddEnemy()
        {
            // ��������� ��'���� �������
            Animation enemyAnimation = new Animation();
            // ����������� ������� ������
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);
            // ��������� ��������� ������� ������
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));
            // ��������� ��'���� �����
            Enemy enemy = new Enemy();
            // ����������� ������
            enemy.Initialize(enemyAnimation, position, enemyHealth, enemyMoveSpeed);
            // ��������� ������ �� ������ ��������, � ����� ������, ������
            enemies.Add(enemy);
        }
        // ������� ���������� ������
        private void UpdateEnemies(GameTime gameTime)
        {
            // ��������� ��������� ��� � �����
            if (score >= 2000 && gameTime.TotalGameTime - enemyDificultTime > TimeSpan.FromSeconds(20f))
            {
                enemyDificultTime = gameTime.TotalGameTime;
                enemyHealth += 2;
                enemyMoveSpeed += .3f;
                enemySpawnTime -= TimeSpan.FromSeconds(.1f);
            }
            // ���������� ������ ������ ����� ������� ���
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;
                // ��������� ������
                AddEnemy();
            }
            // ���������� �� ����������� ������
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);
                if (enemies[i].Active == false && currentGameState != GameState.Pause)
                {
                    // ���� � ������ ����� ���� �����
                    if (enemies[i].Health <= 0)
                    {
                        // ������ �����
                        AddExplosion(enemies[i].Position);
                        // ³�������� ���� ������
                        explosionSound.Play();
                        // ��������� ����
                        score += enemies[i].Value;
                    }
                    // �������� ������
                    enemies.RemoveAt(i);
                }
            }
        }
        // �������� �������
        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                // ³������ �������� �� ��� ������
                GamePad.SetVibration(PlayerIndex.One,0.3f,0.3f);
                if (explosions[i].Active == false)
                {
                    //��������� ������ � ������� �������
                    explosions.RemoveAt(i);
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                }
            }
        }
        // ��������� ������
        private void AddProjectile(Vector2 position)
        {
            Projectile projectile = new Projectile();
            projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
            projectiles.Add(projectile);
        }
        // �������� ������
        private void UpdateProjectiles()
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();
                // ��������� ������
                if (projectiles[i].Active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }
        // �������� ����� ������
        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);
            // ��������� �� ��������� ����
            player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;
            // ��������� ��������� ��� ���������
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
            // �����
            if (currentKeyboardState.IsKeyDown(Keys.Escape) || currentGamePadState.Buttons.Back == ButtonState.Pressed)
                currentGameState = GameState.Pause;
            // �������
            if ((currentKeyboardState.IsKeyDown(Keys.Space) || currentGamePadState.Triggers.Right > 0)
                && gameTime.TotalGameTime - previousFireTime > fireTime)
            {
                // �������� ��������� ����
                previousFireTime = gameTime.TotalGameTime;
                // ��������� ������
                AddProjectile(player.Position + new Vector2(player.Width / 2, 0));
                // ³��������� ����� ������
                laserSound.Play();
            }
            // ��������� ���� ������ �� ����� �������� ����
            player.Position.X = MathHelper.Clamp(player.Position.X, 0 +player.Width / 2, GraphicsDevice.Viewport.Width - player.Width / 2);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0 +player.Height / 2+35, GraphicsDevice.Viewport.Height - player.Height / 2);
            // ³��������� ������'� ������
            if (player.Health <= 0)
                currentGameState = GameState.GameOver;
        }
        // �������� �������
        private void UpdateCollision()
        {
            // ������������ ������������ ��� ���������� �������
            Rectangle rectangle1;
            Rectangle rectangle2;
            // ��������� ������������ � �������� ������
            rectangle1 = new Rectangle((int)player.Position.X,
            (int)player.Position.Y,
            player.Width,
            player.Height);
            // ǳ������� ������ � ������
            for (int i = 0; i < enemies.Count; i++)
            {
                // ��������� ������������ � �������� ������
                rectangle2 = new Rectangle((int)enemies[i].Position.X,
                (int)enemies[i].Position.Y,
                enemies[i].Width,
                enemies[i].Height);
                // �������� ��������
                if (rectangle1.Intersects(rectangle2))
                {
                    // ��������� ����� ������
                    player.Health -= enemies[i].Damage;
                    // �������� ������
                    enemies[i].Health = 0;
                }

            }
            // ǳ������� ������ � ������
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // ������� ������
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);
                    //������� ������
                    rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
                    (int)enemies[j].Position.Y - enemies[j].Height / 2,
                    enemies[j].Width, enemies[j].Height);
                    // �������� ��������
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
