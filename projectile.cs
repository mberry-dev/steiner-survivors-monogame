using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SteinerSurvivors
{
    public class Projectile
    {
        public Vector2 Position { get; private set; }
        public Vector2 Direction { get; private set; }
        public bool IsActive { get; set; } = true;
        private float speed = 400f; // Projectile speed (adjust as needed)
        private float lifetime = 2f; // How long the projectile lives in seconds
        private float timer = 0f;

        public Projectile(Vector2 position, Vector2 direction)
        {
            Position = position;
            Direction = direction;

            // Ensure direction is normalized
            if (Direction != Vector2.Zero)
                Direction.Normalize();
        }

        public void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move the projectile
            Position += Direction * speed * dt;

            // Update lifetime
            timer += dt;
            if (timer >= lifetime)
            {
                IsActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            if (IsActive)
            {
                spriteBatch.Draw(texture,
                    new Rectangle((int)Position.X, (int)Position.Y, 16, 16),
                    Color.White);
            }
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, 16, 16);
        }
    }
}