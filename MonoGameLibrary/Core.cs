using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary;

/// <summary>
/// Allows us to simplify the main class by keeping the more complex systems in this reusable library class
/// </summary>
public class Core : Game
{

    internal static Core s_instance;

    /// <summary>
    /// gets a reference to core instance
    /// </summary>
    public static Core Instance => s_instance;

    ///<summary>
    ///Gets the graphics device manager to control graphics presentation
    ///</summary>
    public GraphicsDeviceManager Graphics {get;private set;}

    /// <summary>
    /// Gets graphics device used to perform primative rendering tasks
    /// </summary>
    public GraphicsDevice GraphicsDevice {get; private set;}
    

    /// <summary>
    /// gets spritebatch used for all 2d rendering
    /// </summary>
    public SpriteBatch SpriteBatch {get; private set;}

    /// <summary>
    /// gets the Content manager used to load assets that are global in scope
    /// </summary>
    public static ContentManager Content {get; private set;}

    /// <summary>
    /// instantiates new core
    /// </summary>
    /// <param name = "title"> The title of the game window
    /// <param name = "width"> The width of the game window
    /// <param name = "height"> The height of the game window
    /// <param name = "fullscreen" Whether or not the window is fullscreen
    public Core(string title, int width, int height, bool fullscreen)
    {
        //check if there is already a running instance and close if so
        if(s_instance != null)
        {
            throw new InvalidOperationException("A Core instance is already present");
        }
        
        //set the global instance to this new instance
        s_instance = this;
        Graphics = new GraphicsDeviceManager(this);

        //set Default Graphics State
        Graphics.PreferredBackBufferHeight = height;
        Graphics.PreferredBackBufferWidth = width;
        Graphics.IsFullScreen = fullscreen;

        //apply graphics changes
        Graphics.ApplyChanges();

        //Set window title
        Window.Title = title;

        //set Content manager to reference of basegame's content manager
        Content = base.Content;

        Content.RootDirectory = "Content";

        //set mouse invisible by default
        IsMouseVisible = true;

    }

    protected override void Initialize()
    {
        base.Initialize();

        GraphicsDevice = base.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }
}
