using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Camera2D
{
    private Matrix transform;
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Zoom { get; set; } = 1f;
    public float Rotation { get; set; } = 0f;

    public Matrix GetTransformation(GraphicsDevice graphicsDevice)
    {
        transform =
            Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
            Matrix.CreateRotationZ(Rotation) *
            Matrix.CreateScale(Zoom, Zoom, 1) *
            Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));

        return transform;
    }
}