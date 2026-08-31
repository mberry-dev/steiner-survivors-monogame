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
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D slimeFront, slimeBack, slimeLeft, slimeRight;
        private Texture2D currentTexture;

        Texture2D grassSpriteSheet;

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

        
        int playerHP = 100; // Starts with 100 HP
        int playerScore = 0; // Player starts with a score of 0
        
        private Texture2D fontTexture;
        private List<CharData> fontData;

        private Texture2D xpBarTexture;

        private int playerLevel = 1;
        private int playerXP = 0;
        private int xpToLevelUp = 100;  // Amount of XP needed to level up
        private bool isLevelingUp = false;
        private List<XPItem> xpCrystals = new List<XPItem>();
        private Texture2D xpCrystalTexture;

        private bool isGameOver = false;

        private bool isChoosingUpgrade = false;
        private bool hasDefenseUpgrade = false;
        private int playerAttack = 1;  // Default damage
        private float playerFireRate = 0.5f; // Lower = faster shooting
        private float damageCooldown = 1.0f; // seconds between damage ticks
        private float damageTimer = 0f;

        Texture2D skullTexture;


        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            playerPosition = new Vector2(400, 300); // Starting position
            base.Initialize();
        }

        private void RestartGame()
        {
            playerHP = 100;
            playerScore = 0;
            playerPosition = new Vector2(400, 300);
            enemies.Clear();
            projectiles.Clear();
            playerLevel = 1;
            playerXP = 0;
            xpToLevelUp = 100;
            isChoosingUpgrade = false;
            hasDefenseUpgrade = false;
            isGameOver = false;
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

           xpCrystalTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/XP_Crystal.png"));
            slimeFront = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/slime_front.png"));
            slimeBack = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/slime_back.png"));
            slimeLeft = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/slime_left.png"));
            slimeRight = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/slime_right.png"));
            enemyTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/slime_front.png"));
            skullTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/Skull_mob.png"));
            projectileTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/projectile.png"));

            grassSpriteSheet = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/grass_sprite_sheet.png"));

            fontTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/Font_0.png"));
            fontData = LoadFontData("Content/Font.fnt");
            currentTexture = slimeFront; // default facing down

            // Create a simple texture for the XP bar
            xpBarTexture = new Texture2D(GraphicsDevice, 1, 1);
            xpBarTexture.SetData(new Color[] { Color.White });

        }

private List<CharData> LoadFontData(string filePath)
{
    List<CharData> charData = new List<CharData>();

    var lines = File.ReadAllLines(filePath);
    foreach (var line in lines)
    {
        if (line.StartsWith("char"))
        {
            var tokens = line.Split(' ');

            var idToken = tokens.FirstOrDefault(t => t.StartsWith("id="));
            var xToken = tokens.FirstOrDefault(t => t.StartsWith("x="));
            var yToken = tokens.FirstOrDefault(t => t.StartsWith("y="));
            var widthToken = tokens.FirstOrDefault(t => t.StartsWith("width="));
            var heightToken = tokens.FirstOrDefault(t => t.StartsWith("height="));
            var xOffsetToken = tokens.FirstOrDefault(t => t.StartsWith("xoffset="));
            var yOffsetToken = tokens.FirstOrDefault(t => t.StartsWith("yoffset="));

            if (idToken != null && xToken != null && yToken != null && widthToken != null && heightToken != null)
            {
                var charDataItem = new CharData
                {
                    Id = int.Parse(idToken.Split('=')[1]),
                    X = int.Parse(xToken.Split('=')[1]),
                    Y = int.Parse(yToken.Split('=')[1]),
                    Width = int.Parse(widthToken.Split('=')[1]),
                    Height = int.Parse(heightToken.Split('=')[1]),
                    XOffset = int.Parse(xOffsetToken.Split('=')[1]),
                    YOffset = int.Parse(yOffsetToken.Split('=')[1])
                };

                charData.Add(charDataItem);
            }
        }
    }

    return charData;
}
        
        private void DrawText(string text, Vector2 position)
{
    int xOffset = 0;

    foreach (var character in text)
    {
        var charDataItem = fontData.FirstOrDefault(c => c.Id == character);

        if (charDataItem != null)
        {
            Rectangle sourceRect = new Rectangle(charDataItem.X, charDataItem.Y, charDataItem.Width, charDataItem.Height);
            Vector2 charPosition = new Vector2(position.X + xOffset + charDataItem.XOffset, position.Y + charDataItem.YOffset);
            _spriteBatch.Draw(fontTexture, charPosition, sourceRect, Color.White);
            xOffset += charDataItem.Width;
        }
    }
}
        
       // ✅ Refactored Update method with upgrade menu and game-over handling
protected override void Update(GameTime gameTime)
{
    if (isGameOver)
{
    var keyState = Keyboard.GetState();

    if (keyState.IsKeyDown(Keys.Y))
    {
        RestartGame(); // This resets the game state
    }
    else if (keyState.IsKeyDown(Keys.N))
    {
        Exit(); // Exits the application
    }

    return; // Prevent further updates while waiting
}

    if (isChoosingUpgrade)
    {
        var keys = Keyboard.GetState();

        if (keys.IsKeyDown(Keys.D1))
        {
            hasDefenseUpgrade = true;
            isChoosingUpgrade = false;
        }
        else if (keys.IsKeyDown(Keys.D2))
        {
            playerAttack += 1;
            isChoosingUpgrade = false;
        }
        else if (keys.IsKeyDown(Keys.D3))
        {
            playerFireRate = Math.Max(0.1f, playerFireRate - 0.05f);
            isChoosingUpgrade = false;
        }

        return;
    }

    KeyboardState kState = Keyboard.GetState();
    float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
    damageTimer += dt;

    Rectangle playerBounds = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 32, 32);

    if (kState.IsKeyDown(Keys.W))
    {
        playerPosition.Y -= playerSpeed * dt;
        currentTexture = slimeBack;
    }
    else if (kState.IsKeyDown(Keys.S))
    {
        playerPosition.Y += playerSpeed * dt;
        currentTexture = slimeFront;
    }
    else if (kState.IsKeyDown(Keys.A))
    {
        playerPosition.X -= playerSpeed * dt;
        currentTexture = slimeLeft;
    }
    else if (kState.IsKeyDown(Keys.D))
    {
        playerPosition.X += playerSpeed * dt;
        currentTexture = slimeRight;
    }

    projectileTimer += dt;

    if (projectileTimer >= playerFireRate)
    {
        MouseState mouse = Mouse.GetState();
        Vector2 mouseWorldPos = Vector2.Transform(new Vector2(mouse.X, mouse.Y), Matrix.Invert(_cameraTransform));
        Vector2 direction = mouseWorldPos - playerPosition;

        if (direction != Vector2.Zero)
        {
            direction.Normalize();
            Vector2 spawnPos = playerPosition + direction * 16;
            projectiles.Add(new Projectile(spawnPos, direction));
            projectileTimer = 0f;
        }
    }

    foreach (var proj in projectiles)
        proj.Update(gameTime);

    for (int i = projectiles.Count - 1; i >= 0; i--)
    {
        var proj = projectiles[i];
        if (!proj.IsActive) continue;

        for (int j = enemies.Count - 1; j >= 0; j--)
        {
            var enemy = enemies[j];
            if (enemy.GetBounds().Intersects(proj.GetBounds()))
            {
                enemy.HP -= playerAttack;
                proj.IsActive = false;
                break;
            }
        }
    }

    for (int i = enemies.Count - 1; i >= 0; i--)
    {
        var enemy = enemies[i];
        enemy.Update(gameTime, playerPosition);

        if (enemy.IsDead)
        {
            var xpDropped = enemy.DropXP();
            foreach (var xp in xpDropped)
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
        if (!enemy.IsDead && enemy.GetBounds().Intersects(playerBounds))
        {
            int damage = hasDefenseUpgrade ? 3 : 5;
            playerHP -= damage;
            damageTimer = 0f;

            if (playerHP <= 0)
        {
            playerHP = 0;
            isGameOver = true; 
        }
            break;
        }
    }

    for (int i = xpCrystals.Count - 1; i >= 0; i--)
    {
        var crystal = xpCrystals[i];
        crystal.Update(gameTime);

        Rectangle crystalBounds = new Rectangle((int)crystal.Position.X, (int)crystal.Position.Y, 32, 32);

        if (playerBounds.Intersects(crystalBounds))
        {
            playerXP += crystal.XPValue;
            xpCrystals.RemoveAt(i);
        }
    }

    if (playerXP >= xpToLevelUp && !isChoosingUpgrade)
    {
        playerLevel++;
        playerXP -= xpToLevelUp;
        xpToLevelUp += 50;
        isChoosingUpgrade = true;

        foreach (var enemy in enemies)
        {
            enemy.HP += 1;
        }
    }

    if (playerHP <= 0)
    {
        isGameOver = true;
        return;
    }

    spawnTimer += dt;
    if (spawnTimer >= spawnInterval)
    {
        Vector2 spawnPos = playerPosition + new Vector2(RandomBetween(-400, 400), RandomBetween(-400, 400));
        EnemyType type = rng.Next(2) == 0 ? EnemyType.Slime : EnemyType.Skull;
        enemies.Add(new Enemy(spawnPos, type));
        spawnTimer = 0f;
    }

    _cameraTransform = Matrix.CreateTranslation(
        new Vector3(-playerPosition.X + GraphicsDevice.Viewport.Width / 2,
                    -playerPosition.Y + GraphicsDevice.Viewport.Height / 2,
                    0));

    grasstimer += dt;
    if (grasstimer >= grassFrameSpeed)
    {
        grassFrame = (grassFrame + 1) % totalGrassFrames;
        grasstimer = 0f;
    }
}


        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: _cameraTransform);

            

           _spriteBatch.Draw(enemyTexture, new Rectangle(100, 100, 32, 32), Color.Red);  // Red rectangle to check if sprites are working

            Rectangle grassSourceRect = new Rectangle(grassFrame * tileSize, 0, tileSize, tileSize);

            for (int x = -10; x < 40; x++)
                {
                    for (int y = -10; y < 30; y++)
                    {
                        Vector2 tilePos = new Vector2(x * tileSize, y * tileSize);
                        _spriteBatch.Draw(grassSpriteSheet, tilePos, grassSourceRect, Color.White);
                    }
                }

            // Draw projectiles
            foreach (var proj in projectiles)
                proj.Draw(_spriteBatch, projectileTexture);

                    // Draw XP crystals
            foreach (var crystal in xpCrystals)
            {
                _spriteBatch.Draw(
                    xpCrystalTexture,
                    new Rectangle((int)crystal.Position.X, (int)crystal.Position.Y, 32, 32),
                    Color.White
                );
            }

            //Draw enemies
            foreach (var enemy in enemies)
            { enemy.Draw(_spriteBatch, slimeFront, skullTexture);}

             // Draw XP crystals
            foreach (var xp in xpCrystals)
            {
                if (xp.IsActive)
                {
                    // Draw a bright, obvious rectangle if the texture isn't showing
                    _spriteBatch.Draw(
                        xpCrystalTexture, 
                        new Rectangle((int)xp.Position.X, (int)xp.Position.Y, 32, 32), 
                        Color.HotPink); // Use a very visible color for debugging
                }
            }

            Rectangle targetRect = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 32, 32);
            _spriteBatch.Draw(currentTexture, targetRect, Color.White);
            _spriteBatch.End();

            if (isGameOver)
            {
                Matrix noTransform = Matrix.Identity;
                _spriteBatch.Begin(transformMatrix: noTransform);

                Vector2 centerScreen = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

                DrawText("GAME  OVER", centerScreen - new Vector2(100, 30));
                DrawText("Score:  " + playerScore.ToString(), centerScreen + new Vector2(-100, -5));
                DrawText("Press  Y  to  restart", centerScreen + new Vector2(-100, 20));
                DrawText("Press  N  to  exit", centerScreen + new Vector2(-100, 50));

                _spriteBatch.End();
                return; // 🚪 Exit early
            }
            _spriteBatch.Begin();

            // Draw HP
            DrawText("HP- " + playerHP.ToString(), new Vector2(10, 10));
            // Draw scored
            DrawText("Score- " + playerScore.ToString(), new Vector2(10, 30));
            // Draw player level and XP bar (in the _spriteBatch.Begin() that doesn't use camera transform)
            DrawText("Level " + playerLevel.ToString(), new Vector2(10, 50));

            int barWidth = 200;
            int barHeight = 20;
            int screenWidth = GraphicsDevice.Viewport.Width;

            int barX = (screenWidth / 2) - (barWidth / 2);
            int barY = 10; // Adjust vertically as needed

            // Background
            _spriteBatch.Draw(xpBarTexture, new Rectangle(barX, barY, barWidth, barHeight), Color.DarkGray);

            // Fill
            float xpProgress = (float)playerXP / xpToLevelUp;
            _spriteBatch.Draw(xpBarTexture, new Rectangle(barX + 2, barY + 2, (int)((barWidth - 4) * xpProgress), barHeight - 4), Color.LightGreen);

            // Text (centered)
            string xpText = $"{playerXP}/{xpToLevelUp} XP";
            int textX = screenWidth / 2 - (xpText.Length * 6); // Rough centering estimate
            int textY = barY + 2;
            DrawText(xpText, new Vector2(textX, barY + barHeight + 4));

            if (isChoosingUpgrade)
        {
            int screenW = GraphicsDevice.Viewport.Width;
            int screenH = GraphicsDevice.Viewport.Height;

            int boxWidth = 180;
            int boxHeight = 100;
            int spacing = 40;

            // Total width of all boxes + spacing
            int totalWidth = (3 * boxWidth) + (2 * spacing);
            int startX = (screenW - totalWidth) / 2;
            int startY = screenH / 2 - boxHeight / 2;

            // Draw 3 boxes with their upgrade labels
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

    DrawText("LEVEL UP! Choose an Upgrade", new Vector2(screenW / 2 - 100, startY - 40));
}
   
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}


