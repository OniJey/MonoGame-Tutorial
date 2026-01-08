using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MonoGameTutorial.Graphics;


/// <summary>
/// represents rectangular region in texture
/// </summary>
class TextureRegion
{
    /// <summary>
    /// The texture that The region is binded to
    /// </summary>
    public Texture2D Texture {get; set;}
    

    /// <summary>
    /// The region of the texture that is represented. (pixels as units)
    /// </summary>
    public Rectangle SourceRectange {get; set;}

    /// <summary>
    /// Width (pixels) of the region
    /// </summary>
    public int Width => SourceRectange.Width;

    /// <summary>
    /// Height (pixels) of the region
    /// </summary>
    public int Height => SourceRectange.Height;


    public TextureRegion() {}

    public TextureRegion(Texture2D texture, int x, int y, int width, int height)
    {
        this.Texture = texture;
        this.SourceRectange = new Rectangle(x, y, width, height);
    }

    public TextureRegion(Texture2D texture, Vector2 origin, Vector2 bounds)
    {
        this.Texture = texture;
        this.SourceRectange = new Rectangle((int) origin.X, (int) origin.Y,(int) bounds.X,(int) bounds.Y);
        
    }

    public TextureRegion(Texture2D texture, Rectangle rect)
    {
        this.Texture = texture;
        this.SourceRectange = rect;
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
            color
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
            SourceRectange,
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
            SourceRectange,
            color,
            rotation,
            origin,
            scale,
            effect,
            layerDepth
        );  
    }


}