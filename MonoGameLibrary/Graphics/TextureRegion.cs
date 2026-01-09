using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MonoGameLibrary.Graphics;


/// <summary>
/// represents rectangular region in texture
/// </summary>
public class TextureRegion
{
    /// <summary>
    /// The texture that The region is binded to
    /// </summary>
    public Texture2D Texture {get; set;}
    

    /// <summary>
    /// The region of the texture that is represented. (pixels as units)
    /// </summary>
    public Rectangle SourceRectangle {get; set;}

    /// <summary>
    /// Width (pixels) of the region
    /// </summary>
    public int Width => SourceRectangle.Width;

    /// <summary>
    /// Height (pixels) of the region
    /// </summary>
    public int Height => SourceRectangle.Height;

    /// <summary>
    /// Creates an empty TextureRegion instance
    /// </summary>
    public TextureRegion() {}

    /// <summary>
    /// Creates a Texture region in the specified texture in the specified position with the specified bounds
    /// </summary>
    /// <param name="texture">The base texture</param>
    /// <param name="x">The x posision of the base texture the top left corner of the region will be</param>
    /// <param name="y"> The y position of the base texture where the top left corner of the region will be</param>
    /// <param name="width">The width of the region (in pixels)</param>
    /// <param name="height">the height of the region (in pixels)</param>
    public TextureRegion(Texture2D texture, int x, int y, int width, int height)
    {
        Texture = texture;
        SourceRectangle = new Rectangle(x, y, width, height);
    }

    /// <summary>
    /// Creates a Texture region in the specified texture in the specified position with the specified bounds
    /// </summary>
    /// <param name="texture">The base texture</param>
    /// <param name="origin">The x and y coordinates of the base texture where the top left corner of the region will be</param>
    /// <param name="bounds">The x and y coordinates defining the width and height of the region</param>
    public TextureRegion(Texture2D texture, Vector2 origin, Vector2 bounds)
    {
        Texture = texture;
        SourceRectangle = new Rectangle((int) origin.X, (int) origin.Y,(int) bounds.X,(int) bounds.Y);
        
    }

    /// <summary>
    /// Creates a Texture region in the specified texture in the specified position with the specified bounds
    /// </summary>
    /// <param name="texture">The base texture</param>
    /// <param name="rect">The bounds of the region</param>
    public TextureRegion(Texture2D texture, Rectangle bounds)
    {
        Texture = texture;
        SourceRectangle = bounds;
    }

    

    /// <summary>
    /// submits this texture region for drawing in the current batch
    /// </summary>
    /// <param name="batch">the SpriteBatch that is active</param>
    /// <param name="pos">The global position where the sprite is to be drawn</param>
    /// <param name="color">The color mask to be applied to the sprite</param>
    public void Draw(SpriteBatch batch, Vector2 pos, Color color)
    {
        batch.Draw(
            Texture,
            pos,
            SourceRectangle,
            color,
            0f,
            Vector2.Zero,
            Vector2.One,
            SpriteEffects.None,
            1.0f
        );
    }


    /// <summary>
    /// submits this texture region for drawing in the current batch
    /// </summary>
    /// <param name="batch">the SpriteBatch that is active</param>
    /// <param name="pos">The global position where the sprite is to be drawn</param>
    /// <param name="color">The color mask to be applied to the sprite</param>
    /// <param name="rotation">the rotation (in radians) to be applied to the sprite</param>
    /// <param name="origin">the rotational origin of the image</param>
    /// <param name="scale">the scale factor</param>
    /// <param name="effect">SpriteEffects to be applied when the sprite is rendered</param>
    /// <param name="layerDepth">The layerdepth of the sprite</param>
    public void Draw(SpriteBatch batch, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float layerDepth)
    {
        batch.Draw(
            Texture,
            pos,
            SourceRectangle,
            color,
            rotation,
            origin,
            new Vector2(scale, scale),
            effect,
            layerDepth
        );
    }


    /// <summary>
    /// submits this texture region for drawing in the current batch
    /// </summary>
    /// <param name="batch">the SpriteBatch that is active</param>
    /// <param name="pos">The global position where the sprite is to be drawn</param>
    /// <param name="color">The color mask to be applied to the sprite</param>
    /// <param name="rotation">the rotation (in radians) to be applied to the sprite</param>
    /// <param name="origin">the rotational origin of the image</param>
    /// <param name="scale">the scale vector</param>
    /// <param name="effect">SpriteEffects to be applied when the sprite is rendered</param>
    /// <param name="layerDepth">The layerdepth of the sprite</param>
    public void Draw(SpriteBatch batch, Vector2 pos, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float layerDepth)
    {
        batch.Draw(
            Texture,
            pos,
            SourceRectangle,
            color,
            rotation,
            origin,
            scale,
            effect,
            layerDepth
        );  
    }


}