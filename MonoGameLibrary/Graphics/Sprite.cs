using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MonoGameLibrary.Graphics;

public class Sprite
{

    /// <summary>
    /// The region that the sprite occupies in it's TextureAtlas
    /// </summary>
    public TextureRegion Region {get; set;}

    /// <summary>
    /// The color mask of the sprite
    /// </summary>
    /// <remarks>
    /// Default: Color.White
    /// </remarks>
    public Color Color {get; set;} = Color.White;

    /// <summary>
    /// The SpriteEffects applied to the sprite on render
    /// </summary>
    /// <remarks>
    /// Default: SpriteEffects.None
    /// </remarks>
    public SpriteEffects SpriteEffects {get; set;} = SpriteEffects.None;


    /// <summary>
    /// The scale that the sprite is rendered with
    /// </summary>
    /// <remarks>
    /// Default (1,1)
    /// </remarks>
    public Vector2 Scale {get; set;} = Vector2.One;

    /// <summary>
    /// The origin for the sprite's rotation and transition
    /// </summary>
    /// <remarks>
     /// Default (0,0)
     /// </remarks>
    public Vector2 Origin {get; set;} = Vector2.Zero;

    /// <summary>
    /// The rotation of the sprite, in radians
    /// </summary>
    /// <remarks>
    /// Default: 0
    /// </remarks>
    public float Rotation {get; set;} = 0;

    /// <summary>
    /// The depth that the Sprite is drawn at, determines draw order if SpriteBatch.Start has been called 
    /// with the parameters "sortMode: SpriteSortMode.FrontToBack" or "sortMode: SpriteSortMode.BackToFront"
    /// </summary>
    /// <value></value>
    public float Depth {get; set;} = 0;

    /// <summary>
    /// The width of the rendered sprite
    /// </summary>
    /// <remarks>
    /// calculated by taking the width of the sprite's region and multiplying it by the scale's X componant
    /// </remarks>
    public float Width => Region.Width * Scale.X;

    /// <summary>
    /// The height of the rendered sprite
    /// </summary>
    /// <remarks>
    /// calculated by taking the height of the sprite's region and multiplying by the scale's Y componant
    /// </remarks>
    public float Height => Region.Height * Scale.Y;
    

    /// <summary>
    /// creates an empty sprite
    /// </summary>
    public Sprite(){}

    /// <summary>
    /// creates a sprite that uses the specified region as its texture
    /// </summary>
    /// <param name="region"></param>
    public Sprite(TextureRegion region)
    {
        Region = region;
    }

    /// <summary>
    /// moves the scale and rotate origin to the center of the sprite
    /// </summary>
    public void CenterOrigin()
    {
        Origin = new Vector2(Region.Width, Region.Height) * 0.5f;
    }

    /// <summary>
    /// Draws the sprite
    /// </summary>
    /// <param name="batch">The SpriteBatch instance that will draw the sprite</param>
    /// <param name="position">The position the sprite will be drawn</param>
    public void Draw(SpriteBatch batch, Vector2 position)
    {
        Region.Draw(
            batch,
            position,
            Color,
            Rotation,
            Origin,
            Scale,
            SpriteEffects,
            Depth
        );
    }
}