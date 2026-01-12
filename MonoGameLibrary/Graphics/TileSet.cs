using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

/// <summary>
/// Represents a set of tiles that use a single texture file and share the same width
/// </summary>
class TileSet
{  
    /// <summary>
    /// Array of tiles in the TileSet
    /// </summary>
    private TextureRegion[] _tiles;

    /// <summary>
    /// The width (in pixels) of the tiles in the tileset
    /// </summary>
    public int TileWidth {get;}
    /// <summary>
    /// The height (in pixels) of the tiles in the tileset
    /// </summary>
    public int TileHeight {get;}

    /// <summary>
    /// The number of columns in the TileSet
    /// </summary>
    public int Columns {get;}

    /// <summary>
    /// The numebr of Rows in the TileSet
    /// </summary>
    public int Rows {get;}

    /// <summary>
    /// The amount of tiles in the tileset
    /// </summary>
    /// <remarks>
    /// Calculated by takin the amount of columns by the amount of rows, so if there are "dead" or unused tiles at the end of a TileSet texture they will still be counted
    /// </remarks>
    public int Count => Columns*Rows;
    
    /// <summary>
    /// Creates a new tileset within the specified TextureRegion and with tiles with the specified TileWidth and TileHeight
    /// </summary>
    /// <param name="region">The TexureRegion that this TileSet will inhabit </param>
    /// <param name="tileWidth">the width of the individual tiles within this TileSet</param>
    /// <param name="tileHeight">the height of the individual tiles within this TileSet</param>
    public TileSet(TextureRegion region, int tileWidth, int tileHeight)
    {
        TileHeight = tileHeight;
        TileWidth = tileWidth;
        Columns = region.Height/tileWidth;
        Rows = region.Width/tileHeight;

        _tiles = new TextureRegion[Count];


    for (int i = 0; i < Count; i++)
    {
        int x = i % Columns * tileWidth;
        int y = i / Columns * tileHeight;
        _tiles[i] = new TextureRegion(region.Texture, region.SourceRectangle.X + x, region.SourceRectangle.Y + y, tileWidth, tileHeight);
    }
    }

    /// <summary>
    /// Returns the TextureRegion of the Tile at the specific index of the TileSEt
    /// </summary>
    /// <param name="index">the index of the TileSet that the tile is at</param>
    public TextureRegion GetTile(int index) => _tiles[index];

    /// <summary>
    /// Returns the TextureRegion of the Tile at the specific index of the TileSet
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public TextureRegion GetTile(int row, int column)
    {
        int index = row * Columns + column;
        return _tiles[index];
    }
}