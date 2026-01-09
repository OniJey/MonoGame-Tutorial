using System;
using System.Collections.Generic;
using MonoGameLibrary.Graphics;

public class Animation
{
    private List<TextureRegion> _frames;
    private TimeSpan _frametime;

    /// <summary>
    /// A list of the TextureRegions that compose the frames of the animation
    /// </summary>
    public List<TextureRegion> Frames
    {
        get => _frames;
        set => _frames = value ?? throw new ArgumentNullException(nameof(value), "Frames cannot be null");
    }

    /// <summary>
    /// The time in between frames (milliseconds)
    /// </summary>
    public TimeSpan Frametime
    {
        get => _frametime;
        set => _frametime = value >= TimeSpan.Zero ? value : throw new ArgumentException("Frametime cannot be negative", nameof(value));
    }

    /// <summary>
    /// Creates an empty Animation with the default frametime of 100 milliseconds   
    /// </summary>
    public Animation()
    {
        Frames = new List<TextureRegion>();
        Frametime = TimeSpan.FromMilliseconds(100);
    }

    /// <summary>
    /// creates a new Animation with the frames in the list with the specified frametime
    /// </summary>
    /// <param name="frames">A list of TextureRegions that compose the frames of the animation</param>
    /// <param name="frametime">The time in between frames (in milliseconds)</param>
    public Animation(List<TextureRegion> frames, double frametime)
    {
        Frames = frames;
        Frametime = TimeSpan.FromMilliseconds(frametime);
    }

    /// <summary>
    /// creates a new Animation with the frames in the list with the specified framerate
    /// </summary>
    /// <param name="frames">A list of the TextureRegions that compose the frames of the animation</param>
    /// <param name="frametime">The time in between frames</param>
    public Animation(List<TextureRegion> frames, TimeSpan frametime)
    {
        Frames = frames;
        Frametime = frametime;
    }
}