using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SteinerSurvivors;

namespace SteinerSurvivors
{
    public enum EnemyType
    {
        Slime,
        Skull
    }


    internal class Enemy
    {
        public Vector2 Position;
        public float Speed = 50f;
        public int HP = 3;
        public bool IsDead { get; private set; }

        private EnemyType _type;





        public Enemy(Vector2 startPos, EnemyType type = EnemyType.Slime)
        {
            Position = startPos;
            _type = type;
            IsDead = false;
        }

        // Method to drop XP items when the enemy dies
        public List<XPItem> DropXP()
        {
            List<XPItem> xpCrystals = new List<XPItem>();
            xpCrystals.Add(new XPItem(Position, 10));  // Drop one XP crystal with value
            return xpCrystals;
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
        }

        public bool IsAlive => HP > 0;

        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            if (HP <= 0 && !IsDead)
            {
                IsDead = true; // Mark the enemy as dead when HP is 0
                return;
            }

            // Handle movement and other behaviors while alive
            if (!IsDead)
            {
                Vector2 direction = playerPosition - Position;
                if (direction != Vector2.Zero)
                    direction.Normalize();

                Position += direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D slimeTexture, Texture2D skullTexture)
        {
            if (!IsDead)
            {
                Texture2D texture = _type == EnemyType.Slime ? slimeTexture : skullTexture;
                Rectangle rect = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
                spriteBatch.Draw(texture, rect, Color.White);
            }
        }
    }
}