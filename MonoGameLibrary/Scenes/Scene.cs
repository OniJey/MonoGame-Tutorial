using System;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGameLibrary.Scenes;

public abstract class Scene : IDisposable
{
    /// <summary>
    /// gets the ContentManager for loading scene-specific assets
    /// </summary>
    protected ContentManager Content {get;}
    
    /// <summary>
    /// Whether or not the scene instance is disposed
    /// </summary>
    public bool IsDisposed {get; private set;}

    /// <summary>
    /// creates a new scene instance
    /// </summary>
    public Scene()
    {
        //Create a new ContentManager for the scene
        Content = new ContentManager(Core.Content.ServiceProvider);

        //Sets the root directory for the content manager to the same root directory as Core's
        Content.RootDirectory = Core.Content.RootDirectory;
    }

    ~Scene() => Dispose(false);

    /// <summary>
    /// Override to provide initialization logic for the scene
    /// </summary>
    /// <remarks>
    /// When overriding this in a derived class, ensure that base.Initialize()
    /// still called as this is when LoadContent is called.
    /// </remarks>
    public virtual void Initialize()
    {
        LoadContent();
    }

    /// <summary>
    /// Override to provide logic to load content for scene
    /// </summary>
    public virtual void LoadContent() {}

    /// <summary>
    /// Override to provide logic for unloading the scene. Unloads scene-specific content
    /// </summary>
    public virtual void UnloadContent()
    {
        Content.Unload();
    }

    /// <summary>
    /// Called every frame, updates scene
    /// </summary>
    /// <param name="gameTime">Snapshot of time information for the frame</param>
    public virtual void Update(GameTime gameTime) {}

    /// <summary>
    /// Called every frame, Draws Scene
    /// </summary>
    /// <param name="gameTime">Snapshot of time information for the frame</param>
    public virtual void Draw(GameTime gameTime) {}

    public void Dispose()
    {
        Dispose(true);
    }

    public void Dispose(bool disposing)
    {
        if(IsDisposed)
        {
            return;
        }
        if(disposing)
        {
            UnloadContent();
            Content.Dispose();
        }
        IsDisposed = true;
    }


}