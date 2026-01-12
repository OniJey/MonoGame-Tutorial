using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

/// <summary>
/// Represents a map of tileset indices to map tiles to an environement
/// </summary>
class TileMap
{  
    /// <summary>
    /// The tileset that this TileMap will read from
    /// </summary>
    private readonly TileSet _tileSet;

    /// <summary>
    /// List of indicies that this tileset can use 
    /// </summary>
    private readonly int[] _tiles;

    /// <summary>
    /// The scale of the tiles in the tileset
    /// </summary>
    public Vector2 Scale {get; set;} = Vector2.One;
    
    /// <summary>
    /// The number of rows in this TileMap
    /// </summary>
    public int Rows {get;}

    /// <summary>
    /// The number of columns in this TileMap
    /// </summary>
    public int Columns {get;}

    /// <summary>
    /// Returns the amount of columns multiplied by the amount rows
    /// </summary>
    public int Count => Rows*Columns;

    /// <summary>
    /// The tileset's TileWidth by the scale's X component
    /// </summary>
    public float TileWidth => _tileSet.TileWidth*Scale.X;

    /// <summary>
    /// The TileSet's TileHeight by the scale's Y component
    /// </summary>
    public float TileHeight => _tileSet.TileHeight*Scale.Y;

    /// <summary>
    /// Creates a new tilemap with the speicified amount of rows and columns in the specified tileset
    /// </summary>
    /// <param name="tileSet">the TileSet that this TileMap will use</param>
    /// <param name="rows">the amount of rows of the TileSet that this TileMap can use</param>
    /// <param name="columns">the amount of columns of the TileSet that this TileMap can use</param>
    public TileMap(TileSet tileSet, int rows, int columns)
    {
        _tileSet = tileSet;
        Rows = rows;
        Columns = columns;
        _tiles = new int[Count];
    }

    public void SetTile(int index, int TileSetID)
    {
        _tiles[index] = TileSetID;
    }

    public void SetTile(int column, int row, int TileSetID)
    {
        _tiles[row*Columns + column] = TileSetID;
    }

    public TextureRegion GetTile(int index)
    {
        return _tileSet.GetTile(_tiles[index]);
    }

    public TextureRegion GetTile(int row, int column)
    {
        return _tileSet.GetTile(_tiles[row*Columns + column]);
    }

/// <summary>
/// Draws this tilemap using the given sprite batch.
/// </summary>
/// <param name="spriteBatch">The sprite batch used to draw this tilemap.</param>
public void Draw(SpriteBatch spriteBatch)
{
    for (int i = 0; i < Count; i++)
    {
        int tilesetIndex = _tiles[i];
        TextureRegion tile = _tileSet.GetTile(tilesetIndex);

        int x = i % Columns;
        int y = i / Columns;

        Vector2 position = new Vector2(x * TileWidth, y * TileHeight);
        tile.Draw(spriteBatch, position, Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 1.0f);
    }
}


/// <summary>
/// Creates a new tilemap based on a tilemap xml configuration file.
/// </summary>
/// <param name="content">The content manager used to load the texture for the tileset.</param>
/// <param name="filename">The path to the xml file, relative to the content root directory.</param>
/// <returns>The tilemap created by this method.</returns>
public static TileMap FromFile(ContentManager content, string filename)
{
    string filePath = Path.Combine(content.RootDirectory, filename);

    using (Stream stream = TitleContainer.OpenStream(filePath))
    {
        using (XmlReader reader = XmlReader.Create(stream))
        {
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;

            // The <Tileset> element contains the information about the tileset
            // used by the tilemap.
            //
            // Example
            // <Tileset region="0 0 100 100" tileWidth="10" tileHeight="10">contentPath</Tileset>
            //
            // The region attribute represents the x, y, width, and height
            // components of the boundary for the texture region within the
            // texture at the contentPath specified.
            //
            // the tileWidth and tileHeight attributes specify the width and
            // height of each tile in the tileset.
            //
            // the contentPath value is the contentPath to the texture to
            // load that contains the tileset
            XElement tilesetElement = root.Element("Tileset");

            string regionAttribute = tilesetElement.Attribute("region").Value;
            string[] split = regionAttribute.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            int x = int.Parse(split[0]);
            int y = int.Parse(split[1]);
            int width = int.Parse(split[2]);
            int height = int.Parse(split[3]);

            int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
            int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
            string contentPath = tilesetElement.Value;

            // Load the texture 2d at the content path
            Texture2D texture = content.Load<Texture2D>(contentPath);

            // Create the texture region from the texture
            TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);

            // Create the tileset using the texture region
            TileSet tileset = new TileSet(textureRegion, tileWidth, tileHeight);

            // The <Tiles> element contains lines of strings where each line
            // represents a row in the tilemap.  Each line is a space
            // separated string where each element represents a column in that
            // row.  The value of the column is the id of the tile in the
            // tileset to draw for that location.
            //
            // Example:
            // <Tiles>
            //      00 01 01 02
            //      03 04 04 05
            //      03 04 04 05
            //      06 07 07 08
            // </Tiles>
            XElement tilesElement = root.Element("Tiles");

            // Split the value of the tiles data into rows by splitting on
            // the new line character
            string[] rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // Split the value of the first row to determine the total number of columns
            int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

            // Create the tilemap
            TileMap tilemap = new TileMap(tileset, rows.Length, columnCount);

            // Process each row
            for (int row = 0; row < rows.Length; row++)
            {
                // Split the row into individual columns
                string[] columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                // Process each column of the current row
                for (int column = 0; column < columnCount; column++)
                {
                    // Get the tileset index for this location
                    int tilesetIndex = int.Parse(columns[column]);

                    // Add that region to the tilemap at the row and column location
                    tilemap.SetTile(column, row, tilesetIndex);
                }
            }

            return tilemap;
        }
    }
}
}