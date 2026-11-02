using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


public static class ButtonPositioner
{
    public static Rectangle CenterButton(Texture2D texture, int screenWidth, int y)
    {
        int x = (screenWidth - texture.Width) / 2;
        return new Rectangle(x, y, texture.Width, texture.Height);
    }

    public static void CenterAllButtons(GraphicsDevice graphicsDevice,
                                        Texture2D playTexture,
                                        Texture2D settingsTexture,
                                        Texture2D exitTexture,
                                        out Rectangle playRect,
                                        out Rectangle settingsRect,
                                        out Rectangle exitRect)
    {
        int screenWidth = graphicsDevice.Viewport.Width;

        // You can tweak the vertical spacing here
        int spacing = 20;

        int startY = (graphicsDevice.Viewport.Height -
                      (playTexture.Height + settingsTexture.Height + exitTexture.Height + spacing * 2)) / 2;

        playRect = CenterButton(playTexture, screenWidth, startY);
        settingsRect = CenterButton(settingsTexture, screenWidth, startY + playTexture.Height + spacing);
        exitRect = CenterButton(exitTexture, screenWidth, startY + playTexture.Height + settingsTexture.Height + spacing * 2);
    }
}