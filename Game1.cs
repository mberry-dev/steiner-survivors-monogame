using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SteinerSurvivors;

namespace SteinerSurvivors
{
    public class Game1 : Game
    {
        enum GameState
        {
            Menu,
            Playing,
            Settings,
            Exit,
            Paused
        }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D slimeFront, slimeBack, slimeLeft, slimeRight;
        private Texture2D currentTexture;
        Texture2D grassSpriteSheet;
        private Camera2D camera;

        int grassFrame = 0;
        float grasstimer = 0f;
        float grassFrameSpeed = 0.2f; // Seconds per frame
        int totalGrassFrames = 4;
        int tileSize = 32;

        private Vector2 playerPosition;
        private float playerSpeed = 200f;
        private Matrix _cameraTransform;

        List<Enemy> enemies = new List<Enemy>();
        Texture2D enemyTexture;

        float spawnTimer = 0f;
        float spawnInterval = 2f;

        Random rng = new Random();
        float RandomBetween(float min, float max) => (float)(rng.NextDouble() * (max - min) + min);

        List<Projectile> projectiles = new List<Projectile>();
        Texture2D projectileTexture;

        float projectileCooldown = 0.5f;  // Half a second between shots
        float projectileTimer = 0f;

        bool isGameOver = false;
        int playerHP = 100; // Starts with 100 HP
        int playerScore = 0; // Player starts with a score of 0
        KeyboardState oldState;
        KeyboardState previousKeyboardState;

        private SpriteFont font;// Windows based
        private SpriteBatch spriteBatch;

        private Texture2D xpBarTexture;

        private int playerLevel = 1;
        private int playerXP = 0;
        private int xpToLevelUp = 100;  // Amount of XP needed to level up
        private bool isLevelingUp = false;
        private List<XPItem> xpCrystals = new List<XPItem>();
        private Texture2D xpCrystalTexture;



        private bool isChoosingUpgrade = false;
        private bool hasDefenseUpgrade = false;
        private int playerAttack = 1;  // Default damage
        private float playerFireRate = 0.5f; // Lower = faster shooting
        private float damageCooldown = 1.0f; // seconds between damage ticks
        private float damageTimer = 0f;

        Texture2D skullTexture;

        private Rectangle playRect;
        private Rectangle settingRect;
        private Rectangle exitRect;

        // Settings screen rectangles
        private Rectangle awsdRect;
        private Rectangle arrowKeysRect;
        private Rectangle backRect;


        private GameState currentGameState = GameState.Menu;
        private Rectangle playerBounds;
        Texture2D playButtonTexture, settingButtonTexture, exitButtonTexture;
        Texture2D awsdKeysTexture, arrowKeysTexture;
        Rectangle playButtonRect, settingButtonRect, exitButtonRect;

        Texture2D pauseButtonTexture;
        Rectangle pauseButtonRect;

        private string movementControl = "AWSD";  // Default control scheme

        private bool isPaused = false;
        private KeyboardState previousPauseKeyState;
        private KeyboardState keyboardState;
        private Texture2D backButtonTexture;
        BackButton btnGoBack;
        bool showConfirmationDialog = false;
        Rectangle yesButton, noButton;

        GameSoundtrack soundtrack;
        private MouseState previousMouseState;
        private GameState previousGameState;
        private bool isMusicPlaying = false;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            currentGameState = GameState.Menu;
        }

        protected override void Initialize()
        {
            playerPosition = new Vector2(400, 300); // Starting position
            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Initialize soundtrack manager
            soundtrack = new GameSoundtrack(Content);


            camera = new Camera2D();
            previousKeyboardState = Keyboard.GetState();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("GameFont");

            backButtonTexture = Content.Load<Texture2D>("BackButton"); // A simple box texture

            btnGoBack = new BackButton
            {
                Bounds = new Rectangle(50, 50, 200, 40),
                Text = "Go Back",
                Font = font,
                Texture = backButtonTexture,
                OnClick = BtnGoBack_Click
            };



            playButtonTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/play.png"));
            settingButtonTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/setting.png"));
            exitButtonTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/exit.png"));

            ButtonPositioner.CenterAllButtons(
                GraphicsDevice,
                playButtonTexture,
                settingButtonTexture,
                exitButtonTexture,
                out playButtonRect,
                out settingButtonRect,
                out exitButtonRect
            );

            playRect = playButtonRect;
            settingRect = settingButtonRect;
            exitRect = exitButtonRect;

            // Load your pause button texture here
            pauseButtonTexture = Content.Load<Texture2D>("Pause");

            // Set the rectangle for the pause button (position and size)
            pauseButtonRect = new Rectangle(GraphicsDevice.Viewport.Width - 100, 20, 80, 40);  // Position it at the top right corner

            xpCrystalTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/XP_Crystal.png"));
            slimeFront = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/slime_front.png"));
            slimeBack = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/slime_back.png"));
            slimeLeft = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/slime_left.png"));
            slimeRight = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/slime_right.png"));
            enemyTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/slime_front.png"));
            skullTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/Skull_mob.png"));
            projectileTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/projectile.png"));
            grassSpriteSheet = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/grass_sprite_sheet.png"));
            awsdKeysTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/awsd_keys.png"));
            arrowKeysTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/arrow_keys.png"));

            int buttonWidth = 200;
            int buttonHeight = 60;
            int centerX = GraphicsDevice.Viewport.Width / 2 - buttonWidth / 2;
            awsdRect = new Rectangle(centerX, 200, buttonWidth, buttonHeight);
            arrowKeysRect = new Rectangle(centerX, 280, buttonWidth, buttonHeight);
            backRect = new Rectangle(centerX, 360, buttonWidth, buttonHeight);

            currentTexture = slimeFront; // default facing down

            // Create a simple texture for the XP bar
            xpBarTexture = new Texture2D(GraphicsDevice, 1, 1);
            xpBarTexture.SetData(new Color[] { Color.White });
        }



        protected override void Update(GameTime gameTime)
        {
            // Update previous game state at the beginning of the method
            previousGameState = currentGameState;

            camera.Position = playerPosition;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            MouseState mouse = Mouse.GetState();
            Point mousePos = mouse.Position;
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (playerHP <= 0) isGameOver = true;

            if (isGameOver)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Y))
                {
                    RestartGame();
                    isGameOver = false;
                    currentGameState = GameState.Playing;
                   
                }
                else if (currentKeyboardState.IsKeyDown(Keys.N))
                {
                    soundtrack.Stop();
                    GoToMainMenu();
                }
                return;
            }

            // SPACEBAR to toggle pause and show confirmation dialog
            if (currentKeyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space))
            {
                isPaused = !isPaused;
                showConfirmationDialog = isPaused;
            }

            if (isPaused)
            {
                // Pause the music when the game is paused
                soundtrack.Pause();

                if (showConfirmationDialog)
                {
                    if (currentKeyboardState.IsKeyDown(Keys.Y) && previousKeyboardState.IsKeyUp(Keys.Y))
                    {
                        GoToMainMenu();
                        isPaused = false;
                        showConfirmationDialog = false;
                        soundtrack.Stop();
                        isMusicPlaying = false;
                    }
                    else if (currentKeyboardState.IsKeyDown(Keys.N) && previousKeyboardState.IsKeyUp(Keys.N))
                    {
                        showConfirmationDialog = false;
                        // Still paused but no dialog showing
                    }
                }
                previousMouseState = mouse;
                previousKeyboardState = currentKeyboardState;
                return;
            }
            else
            {
                // Resume the music when the game is unpaused
                if (soundtrack.IsMusicPaused())
                {
                    soundtrack.Resume();
                }
            }
            // Add the ENTER key detection here for changing the music to a random song
            if (currentKeyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
            {
                soundtrack.PlayRandom();  // Play a random song
            }
        
            // MUSIC
            switch (currentGameState)
            {
                case GameState.Menu:

                    // Stop music if coming back to the menu from another state
                    if (previousGameState != GameState.Menu)
                    {
                        soundtrack.Stop();  // Stop music when returning to the menu
                        soundtrack.PlaySendIt();  // Start music when entering the menu
                    }
                    
                    if (mouse.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                    {
                        if (playRect.Contains(mousePos))
                        {
                            currentGameState = GameState.Playing;
                            soundtrack.PlaySendIt();
                        }
                        else if (settingRect.Contains(mousePos))
                            currentGameState = GameState.Settings;
                        else if (exitRect.Contains(mousePos))
                            Exit();
                    }
                    break;

                case GameState.Settings:
                    if (mouse.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                    {
                        if (awsdRect.Contains(mousePos)) movementControl = "AWSD";
                        else if (arrowKeysRect.Contains(mousePos)) movementControl = "ArrowKeys";
                        currentGameState = GameState.Menu;
                    }
                    break;

                case GameState.Playing:
                    spawnTimer += dt;
                    if (spawnTimer >= spawnInterval)
                    {
                        Vector2 spawnPos = playerPosition + new Vector2(RandomBetween(-400, 400), RandomBetween(-400, 400));
                        enemies.Add(new Enemy(spawnPos, rng.Next(2) == 0 ? EnemyType.Slime : EnemyType.Skull));
                        spawnTimer = 0f;
                    }

                    damageTimer += dt;

                    Vector2 movement = Vector2.Zero;
                    if (movementControl == "AWSD")
                    {
                        if (currentKeyboardState.IsKeyDown(Keys.W)) { movement.Y -= 1; currentTexture = slimeBack; }
                        if (currentKeyboardState.IsKeyDown(Keys.S)) { movement.Y += 1; currentTexture = slimeFront; }
                        if (currentKeyboardState.IsKeyDown(Keys.A)) { movement.X -= 1; currentTexture = slimeLeft; }
                        if (currentKeyboardState.IsKeyDown(Keys.D)) { movement.X += 1; currentTexture = slimeRight; }
                    }
                    else
                    {
                        if (currentKeyboardState.IsKeyDown(Keys.Up)) { movement.Y -= 1; currentTexture = slimeBack; }
                        if (currentKeyboardState.IsKeyDown(Keys.Down)) { movement.Y += 1; currentTexture = slimeFront; }
                        if (currentKeyboardState.IsKeyDown(Keys.Left)) { movement.X -= 1; currentTexture = slimeLeft; }
                        if (currentKeyboardState.IsKeyDown(Keys.Right)) { movement.X += 1; currentTexture = slimeRight; }
                    }
                    playerPosition += movement * playerSpeed * dt;

                    _cameraTransform = Matrix.CreateTranslation(new Vector3(-playerPosition.X + GraphicsDevice.Viewport.Width / 2, -playerPosition.Y + GraphicsDevice.Viewport.Height / 2, 0));

                    grasstimer += dt;
                    if (grasstimer >= grassFrameSpeed)
                    {
                        grassFrame = (grassFrame + 1) % totalGrassFrames;
                        grasstimer = 0f;
                    }

                    projectileTimer += dt;
                    if (projectileTimer >= projectileCooldown)
                    {
                        Vector2 mouseWorldPos = Vector2.Transform(new Vector2(mouse.X, mouse.Y), Matrix.Invert(_cameraTransform));
                        Vector2 direction = mouseWorldPos - playerPosition;
                        if (direction != Vector2.Zero)
                        {
                            direction.Normalize();
                            projectiles.Add(new Projectile(playerPosition + direction * 16, direction));
                            projectileTimer = 0f;
                        }
                    }

                    Rectangle playerBounds = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, currentTexture.Width, currentTexture.Height);
                    foreach (var enemy in enemies) enemy.Update(gameTime, playerPosition);
                    foreach (var proj in projectiles) proj.Update(gameTime);

                    projectiles.RemoveAll(p =>
                    {
                        if (!p.IsActive) return true;
                        foreach (var enemy in enemies)
                        {
                            if (enemy.GetBounds().Intersects(p.GetBounds()))
                            {
                                enemy.HP -= playerAttack;
                                p.IsActive = false;
                                return true;
                            }
                        }
                        return false;
                    });

                    for (int i = enemies.Count - 1; i >= 0; i--)
                    {
                        var enemy = enemies[i];
                        enemy.Update(gameTime, playerPosition);
                        
                        if (enemy.IsDead)
                        {
                            foreach (var xp in enemy.DropXP())
                            {
                                xp.SetTexture(xpCrystalTexture);
                                xpCrystals.Add(xp);
                            }
                            playerScore += 10;
                            enemies.RemoveAt(i);
                        }
                    }

                    foreach (var enemy in enemies)
                    {
                        if (damageTimer >= damageCooldown && !enemy.IsDead && Vector2.Distance(playerPosition, enemy.Position) < 40f && enemy.GetBounds().Intersects(playerBounds))
                        {
                            playerHP -= hasDefenseUpgrade ? 3 : 5;
                            damageTimer = 0f;
                            if (playerHP <= 0) { playerHP = 0; isGameOver = true; }
                            break;
                        }
                    }

                    Rectangle playerPickupBounds = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, currentTexture.Width, currentTexture.Height);
                    xpCrystals.RemoveAll(crystal =>
                    {
                        crystal.Update(gameTime);
                        if (playerPickupBounds.Intersects(new Rectangle((int)crystal.Position.X, (int)crystal.Position.Y, 32, 32)))
                        {
                            playerXP += crystal.XPValue;
                            Console.WriteLine($"XP Collected! Current XP: {playerXP}");
                            return true;
                        }
                        return false;
                    });

                    if (playerXP >= xpToLevelUp && !isChoosingUpgrade)
                    {
                        playerLevel++;
                        playerXP -= xpToLevelUp;
                        xpToLevelUp += 50;
                        isChoosingUpgrade = true;
                        foreach (var enemy in enemies) enemy.HP++;
                    }

                    if (isChoosingUpgrade)
                    {
                        if (currentKeyboardState.IsKeyDown(Keys.D1)) { hasDefenseUpgrade = true; isChoosingUpgrade = false; }
                        else if (currentKeyboardState.IsKeyDown(Keys.D2)) { playerAttack++; isChoosingUpgrade = false; }
                        else if (currentKeyboardState.IsKeyDown(Keys.D3)) { projectileCooldown = Math.Max(0.1f, projectileCooldown - 0.05f); isChoosingUpgrade = false; }
                    }
                    break;
            }

            previousMouseState = mouse;
            previousKeyboardState = currentKeyboardState;
            return;
        }
        // Toggle the game's paused state
        private void TogglePause()
        {
            isPaused = !isPaused;  // Toggle the pause state
        }


        private void BtnGoBack_Click()
        {
            GoToMainMenu();
        }

        private void PauseGame()
        {
            isPaused = true;
            // Show pause menu logic, stop timers, etc.
        }

        private void ResumeGame()
        {
            isPaused = false;
            showConfirmationDialog = false;
        }

        private void GoToMainMenu()
        {
            // Switch game state to Menu
            currentGameState = GameState.Menu;
            isGameOver = false;
            isPaused = false;
            showConfirmationDialog = false;
            previousMouseState = Mouse.GetState();
            RestartGame();
        }
        private void DrawText(string text, Vector2 position)
        {
            // Draw the string directly using SpriteFont
            _spriteBatch.DrawString(font, text, position, Color.White);
        }
        private void DrawHoverButton(Texture2D texture, Rectangle rect, Point mousePos, Color? hoverColor = null)
        {
            Color color = rect.Contains(mousePos) ? hoverColor ?? Color.Gray : Color.White;
            _spriteBatch.Draw(texture, rect, color);
        }
        // Restart the game (reset game state)
        private void RestartGame()
        {
            playerHP = 100;
            playerScore = 0;
            playerPosition = new Vector2(400, 300); // Reset player's position
            enemies.Clear(); // Clear any existing enemies
            projectiles.Clear(); // Clear any projectiles
            playerLevel = 1;
            playerXP = 0;
            xpToLevelUp = 100;
            isChoosingUpgrade = false;
            hasDefenseUpgrade = false;
            isGameOver = false;
            xpCrystals.Clear();
            playerAttack = 1;
            playerSpeed = 200f;
            projectileCooldown = 0.5f;
            damageTimer = 0f;
            projectileTimer = 0f;
            spawnTimer = 0f;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // --- Begin: Drawing World Elements ---
            _spriteBatch.Begin(transformMatrix: camera.GetTransformation(GraphicsDevice));




            // Draw grass background (always visible)
            Rectangle grassSourceRect = new Rectangle(grassFrame * tileSize, 0, tileSize, tileSize);
            for (int x = -10; x < 40; x++)
            {
                for (int y = -10; y < 30; y++)
                {
                    Vector2 tilePos = new Vector2(x * tileSize, y * tileSize);
                    _spriteBatch.Draw(grassSpriteSheet, tilePos, grassSourceRect, Color.White);
                }
            }

            // Draw projectiles and enemies (always visible)
            foreach (var proj in projectiles)
                proj.Draw(_spriteBatch, projectileTexture);

            // Draw enemies
            foreach (var enemy in enemies)
                enemy.Draw(_spriteBatch, slimeFront, skullTexture);

            // Draw XP crystals
            foreach (var crystal in xpCrystals)
            {
                if (crystal.IsActive)
                {
                    _spriteBatch.Draw(xpCrystalTexture, new Rectangle((int)crystal.Position.X, (int)crystal.Position.Y, 32, 32), Color.White);
                }
            }
            // Draw the player character
            Rectangle targetRect = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 32, 32);
            _spriteBatch.Draw(currentTexture, targetRect, Color.White);

            // End the sprite batch for world rendering
            _spriteBatch.End();


            // Begin sprite batch for UI elements (without camera transformation)
            _spriteBatch.Begin();

            // Draw HP and score at the top left corner
            DrawText("HP- " + playerHP.ToString(), new Vector2(10, 10));
            DrawText("Score- " + playerScore.ToString(), new Vector2(10, 30));
            DrawText("Level " + playerLevel.ToString(), new Vector2(10, 50));

            // Draw XP bar
            int barWidth = 200;
            int barHeight = 20;
            int screenWidth = GraphicsDevice.Viewport.Width;
            int barX = (screenWidth / 2) - (barWidth / 2);
            int barY = 10;

            // Background
            _spriteBatch.Draw(xpBarTexture, new Rectangle(barX, barY, barWidth, barHeight), Color.DarkGray);

            // Fill bar based on XP progress
            float xpProgress = (float)playerXP / xpToLevelUp;
            _spriteBatch.Draw(xpBarTexture, new Rectangle(barX + 2, barY + 2, (int)((barWidth - 4) * xpProgress), barHeight - 4), Color.LightGreen);

            // Display XP text
            string xpText = $"{playerXP}/{xpToLevelUp} XP";
            int textX = screenWidth / 2 - (xpText.Length * 6); // Rough centering estimate
            int textY = barY + 2;
            DrawText(xpText, new Vector2(textX, barY + barHeight + 4));

            // Draw upgrade UI if the player is choosing upgrades
            if (isChoosingUpgrade)
            {
                int screenH = GraphicsDevice.Viewport.Height;
                int boxWidth = 180;
                int boxHeight = 100;
                int spacing = 40;

                // Total width of all boxes + spacing
                int totalWidth = (3 * boxWidth) + (2 * spacing);
                int startX = (screenWidth - totalWidth) / 2;
                int startY = screenH / 2 - boxHeight / 2;

                // Draw 3 upgrade boxes with labels
                for (int i = 0; i < 3; i++)
                {
                    int x = startX + i * (boxWidth + spacing);
                    Rectangle box = new Rectangle(x, startY, boxWidth, boxHeight);
                    _spriteBatch.Draw(xpBarTexture, box, Color.Gray);

                    string label = "";
                    switch (i)
                    {
                        case 0: label = "1 - Defense"; break;
                        case 1: label = "2 - Attack"; break;
                        case 2: label = "3 - Fire Rate"; break;
                    }

                    string[] lines = label.Split('\n');
                    for (int l = 0; l < lines.Length; l++)
                    {
                        DrawText(lines[l], new Vector2(x + 10, startY + 10 + l * 14));
                    }
                }

                // Draw Level Up prompt
                DrawText("LEVEL UP! Choose an Upgrade", new Vector2(screenWidth / 2 - 100, startY - 40));
            }

            Point mousePos = Mouse.GetState().Position;

            if (currentGameState == GameState.Settings)
            {
                DrawHoverButton(awsdKeysTexture, awsdRect, mousePos);
                DrawHoverButton(arrowKeysTexture, arrowKeysRect, mousePos);
                DrawHoverButton(backButtonTexture, backRect, mousePos);
            }
            else if (currentGameState == GameState.Menu)
            {
                DrawHoverButton(playButtonTexture, playRect, mousePos, Color.LightGreen);
                DrawHoverButton(settingButtonTexture, settingRect, mousePos, Color.LightGreen);
                DrawHoverButton(exitButtonTexture, exitRect, mousePos, Color.LightGreen);
            }


            // Pause Logic
            if (currentGameState == GameState.Playing)
            {
                if (isPaused)
                {
                    _spriteBatch.DrawString(font, "Game Paused", new Vector2(screenWidth / 2 - 50, 100), Color.Red);

                    if (showConfirmationDialog)
                    {
                        Rectangle modal = new Rectangle(250, 200, 300, 150);
                        

                        yesButton = new Rectangle(270, 270, 100, 40);
                        noButton = new Rectangle(430, 270, 100, 40);
                        _spriteBatch.DrawString(font, "Yes", new Vector2(yesButton.X + 25, yesButton.Y + 10), Color.White);
                        _spriteBatch.DrawString(font, "No", new Vector2(noButton.X + 30, noButton.Y + 10), Color.White);
                        // Exit confirmation message
                        _spriteBatch.DrawString(font, "Exit Game? Press Y or N", new Vector2(300, 200), Color.White);

                    }
                }
                
            }

            // Game Over Screen
            if (isGameOver && currentGameState == GameState.Playing)
            {
                Vector2 center = new Vector2(screenWidth / 2, GraphicsDevice.Viewport.Height / 2);
                DrawText("GAME OVER", center - new Vector2(100, 30));
                DrawText("Score: " + playerScore, center + new Vector2(-100, -5));
                DrawText("Press Y to restart", center + new Vector2(-100, 20));
                DrawText("Press N to exit", center + new Vector2(-100, 50));
            }



            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}