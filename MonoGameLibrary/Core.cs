using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Audio;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;

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

    public static Scene s_activeScene;

    public static Scene s_nextScene;



    ///<summary>
    ///Gets the graphics device manager to control graphics presentation
    ///</summary>
    public static GraphicsDeviceManager Graphics {get;private set;}

    /// <summary>
    /// Gets graphics device used to perform primative rendering tasks
    /// </summary>
    new public static GraphicsDevice GraphicsDevice {get; private set;}
    

    /// <summary>
    /// gets spritebatch used for all 2d rendering
    /// </summary>
    public static SpriteBatch SpriteBatch {get; private set;}

    /// <summary>
    /// gets the Content manager used to load assets that are global in scope
    /// </summary>
    new public static ContentManager Content {get; private set;}

    public static InputManager Input {get; private set;}

    public static bool ExitOnEscape {get; set;}

    public static AudioController Audio {get; set;}


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

        //set ExitOnEscape True by default
        ExitOnEscape = true;

    }

    protected override void Initialize()
    {
        base.Initialize();

        s_activeScene.Initialize();

        GraphicsDevice = base.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        Input = new InputManager();

        Audio = new AudioController();
    }

    protected override void UnloadContent()
    {
        base.UnloadContent();

        Audio.Dispose();
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update(gameTime);
        Audio.update();

        if(s_nextScene != null)
        {
            TransitionScene();
        }

        if(s_activeScene != null)
        {
            s_activeScene.Update(gameTime);
        }

        if (ExitOnEscape && Input.Keyboard.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        if(s_activeScene != null)
        {
            s_activeScene.Draw(gameTime);
        }
        base.Draw(gameTime);
    }

    public static void ChangeScene(Scene next)
    {
        // Only set the next scene value if it is not the same
        // instance as the currently active scene.
        if (s_activeScene != next)
        {
            s_nextScene = next;
        }
    }

    private static void TransitionScene()
    {
        // If there is an active scene, dispose of it.
        if (s_activeScene != null)
        {
            s_activeScene.Dispose();
        }

        // Force the garbage collector to collect to ensure memory is cleared.
        GC.Collect();

        // Change the currently active scene to the new scene.
        s_activeScene = s_nextScene;

        // Null out the next scene value so it does not trigger a change over and over.
        s_nextScene = null;

        // If the active scene now is not null, initialize it.
        // Remember, just like with Game, the Initialize call also calls the
        // Scene.LoadContent
        if (s_activeScene != null)
        {
            s_activeScene.Initialize();
        }
    }

    
}
