using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SteinerSurvivors
{
    public class XPItem
    {
        public Vector2 Position { get; set; }
        public bool IsActive { get; set; } = true;
        private Texture2D xpTexture;
        public int XPValue { get; private set; }
        
        // Animation variables
        private float timer = 0f;
        private float bobHeight = 4f;
        private Vector2 originalPosition;
        
        public XPItem(Vector2 position, int value = 10)
        {
            Position = position;
            originalPosition = position;
            XPValue = value;
        }
        
        public void SetTexture(Texture2D texture)
        {
            if (texture == null)
        throw new ArgumentNullException(nameof(texture), "XPItem texture cannot be null.");

            xpTexture = texture;
        }
        
        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Make crystal bob up and down
            Position = new Vector2(
                originalPosition.X,
                originalPosition.Y + (float)Math.Sin(timer * 2) * bobHeight
            );
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive && xpTexture != null)
            {
                spriteBatch.Draw(
                    xpTexture,
                    new Rectangle((int)Position.X, (int)Position.Y, 32, 32),
                    Color.White
                );
            }
        }
    }
}