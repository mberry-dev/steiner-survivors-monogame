using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

public class BackButton
{
    public Rectangle Bounds;
    public string Text;
    public SpriteFont Font;
    public Action OnClick;
    public Texture2D Texture;
    public Color TextColor = Color.White;

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Bounds, Color.White);
        Vector2 textSize = Font.MeasureString(Text);
        Vector2 textPos = new Vector2(
            Bounds.X + (Bounds.Width - textSize.X) / 2,
            Bounds.Y + (Bounds.Height - textSize.Y) / 2
        );
        spriteBatch.DrawString(Font, Text, textPos, TextColor);
    }

    public void HandleClick(MouseState mouse)
    {
        if (Bounds.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed)
        {
            OnClick?.Invoke();
        }
    }
}