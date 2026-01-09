using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace MonoGameLibrary.Graphics;

public class TextureAtlas
{
    //The TextureRegions in the TextureAtlas
    private Dictionary<string, TextureRegion> _regions;
    //The animations in the TextureAtlas
    private Dictionary<string, Animation> _animations;


    /// <summary>
    /// The texture to be parsed into an atlas
    /// </summary>
    public Texture2D Texture {get;set;} 

    /// <summary>
    /// Creates a new empty texture atlas
    /// </summary>
    public TextureAtlas()
    {
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    /// <summary>
    /// Creates a texture atlas witht he assigned texture
    /// </summary>
    /// <param name="texture">The texture to be assigned to the atlas</param>
    public TextureAtlas(Texture2D texture)
    {
        Texture = texture;
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    /// <summary>
    /// Add an animation to the atlas under the specified name
    /// </summary>
    /// <param name="name">the name of the animation</param>
    /// <param name="animation">the animation to be added to the atlas</param>
    public void AddAnimation(string name, Animation animation)
    {
        _animations.Add(name, animation);
    }

    /// <summary>
    /// adds a region to the texture atlas
    /// </summary>
    /// <param name="name">the name of the new region</param>
    /// <param name="rect">the bounds of the region</param>
    public void AddRegion(string name, Rectangle rect)
    {
        TextureRegion region = new TextureRegion(this.Texture, rect);
        _regions.Add(name, region);
    }

    /// <summary>
    /// adds a region to the texture atlas
    /// </summary>
    /// <param name="name">The name of the new region</param>
    /// <param name="x">The x coordinate of the top right corner of the region</param>
    /// <param name="y">The y coordinate of the top right corner of the region</param>
    /// <param name="width">The width of the region</param>
    /// <param name="height">The height of the region</param>
    public void AddRegion (string name, int x, int y, int width, int height)
    {
        TextureRegion region = new TextureRegion(this.Texture, x, y, width, height);
        _regions.Add(name, region);
    }

    /// <summary>
    /// returns a region given its name
    /// </summary>
    /// <param name="name">the name of the region</param>
    public TextureRegion GetRegion(string name)
    {
        return _regions[name];
    }

    /// <summary>
    /// returns an animation given its name
    /// </summary>
    /// <param name="name">the name of the animation</param>
    public Animation GetAnimation(string name)
    {
        return _animations[name];
    }

    /// <summary>
    /// Creates a Sprite using the name of a region 
    /// </summary>
    /// <param name="regionName">the name of the region as specified in the .xml file or AddRegion</param>
    /// <returns>Sprite that uses the region of the atlas specified to be drawn</returns>
    public Sprite CreateSprite(string regionName)
    {
        return new Sprite(GetRegion(regionName));
    }

    public AnimatedSprite CreateAnimatedSprite(string animationName)
    {
        return new AnimatedSprite(GetAnimation(animationName));
    }

    /// <summary>
    /// Parses an XML file to create a new TextureAtlas instance
    /// </summary>
    /// <param name="content">The content manager that loads the atlas texture</param>
    /// <param name="filename">The file that contains the data for the regions contained in the atlas</param>
    /// <returns>A TextureAtlas complete with regions from the xml file definition</returns>
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
            atlas.Texture = content.Load<Texture2D>(texturePath);

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

            var regions = root.Element("Regions")?.Elements("Region");

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


                //The <Animations> element contains <Animation> elements, each with their own <Frame> elements
                //which derive from <Region> elements

                var animations = root.Element("Animations")?.Elements("Animation");
                if(animations != null)
                {
                    foreach(var animation in animations)
                    {
                        List<TextureRegion> regs = new List<TextureRegion>();
                        string name = animation.Attribute("name")?.Value;
                        int frametime = int.Parse(animation.Attribute("frametime")?.Value ?? "100");

                        var frames = animation.Elements("Frame");
                        foreach(var frame in frames)
                        {
                            string region = frame.Attribute("region")?.Value;
                            
                            if(!string.IsNullOrEmpty(region))
                            {
                                regs.Add(atlas.GetRegion(region));
                            }
                        }
                        
                        if(!string.IsNullOrEmpty(name) && regs.Count > 0)
                        {
                            atlas.AddAnimation(name, new Animation(regs, frametime));
                        }
                    }
                }
            }
        }

        return atlas;
        
    }
}