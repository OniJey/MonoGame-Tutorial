using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace MonoGameTutorial.Graphics;

class TextureAtlas
{
    /// <summary>
    /// The texture to be parsed into an atlas
    /// </summary>
    public Texture2D Texture {get;set;} 


    private Dictionary<string, TextureRegion> _regions;

    public TextureAtlas()
    {
        _regions = new Dictionary<string, TextureRegion>();
    }

    public TextureAtlas(Texture2D texture)
    {
        Texture = texture;
        _regions = new Dictionary<string, TextureRegion>();
    }

    public void AddRegion(string name, Rectangle rect)
    {
        TextureRegion region = new TextureRegion(this.Texture, rect);
        _regions.Add(name, region);
    }

    public void AddRegion (string name, int x, int y, int width, int height)
    {
        TextureRegion region = new TextureRegion(this.Texture, x, y, width, height);
        _regions.Add(name, region);
    }

    public TextureRegion GetRegion(string name)
    {
        return _regions[name];
    }

    public static TextureAtlas FromFile(ContentManager content, string filename)
    {
        TextureAtlas atlas = new TextureAtlas();

        string filePath = Path.Combine(content.RootDirectory, filename);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
        using(XmlReader reader = XmlReader.Create(stream))
        {
        XDocument doc = XDocument.Load(reader);
        XElement root = doc.Root;

        //The <Texture> Element must contain the Content path for the Texture2D to load
        string texturePath = root.Element("Texture").Value;

        // The <Regions> element contains individual <Region> elements, each one describing
        // a different texture region within the atlas.  
        //
        // Example:
        // <Regions>
        //      <Region name="spriteOne" x="0" y="0" width="32" height="32" />
        //      <Region name="spriteTwo" x="32" y="0" width="32" height="32" />
        // </Regions>
        //
        // So we retrieve all of the <Region> elements then loop through each one
        // and generate a new TextureRegion instance from it and add it to this atlas.

        var regions = root.Element("Regions")?.Elements("region");

        if(regions != null)
        {
            foreach(var region in regions)
            {
                string name = region.Attribute("name")?.Value;
                int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                int height = int.Parse(region.Attribute("height")?.Value ?? "0");


                if(!string.IsNullOrEmpty(name))
                {
                    atlas.AddRegion(name, x, y, width, height);
                }
            }
        }
        }
        }

        return atlas;
        
    }
}