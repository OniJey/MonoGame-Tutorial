using System;
using Microsoft.Xna.Framework;
namespace MonoGameLibrary.Graphics;

public class AnimatedPhysicsSprite : PhysicsSprite
{  
    private int _currentFrame;
    private TimeSpan _elapsed;
    private Animation _animation;

    private bool _isPlaying = true;

    /// <summary>
    /// The animation to be played 
    /// </summary>
    /// <value></value>
    public Animation Animation
    {
        get => _animation;

        set
        {
            _animation = value;
            if(_animation?.Frames.Count > 0)
            {
                Region = _animation.Frames[0];
            }
        }
    }

    /// <summary>
    /// pauses the currently playing animation
    /// </summary>
    public void PauseAnimation()
    {
        _isPlaying = false;
    }

    /// <summary>
    /// plays the animation
    /// </summary>
    public void PlayAnimation()
    {
        _isPlaying = true;
    }

    /// <summary>
    /// Creates new empty animated physics Sprite
    /// </summary>
    public AnimatedPhysicsSprite () {}


    /// <summary>
    /// Creates a new animated physics sprite with the specified animation
    /// </summary>
    /// <param name="animation">The animation that is to be used</param>
    public AnimatedPhysicsSprite(Animation animation)
    {
        Animation = animation;
    }

    /// <summary>
    /// Creates a new animated physics sprite with the specified animation and velocity
    /// </summary>
    /// <param name="animation">The animation that is to be used</param>
    /// <param name="velocity">The how fast and in what direction the PhysicsSprite is moving in pixels/second</param>
    public AnimatedPhysicsSprite(Animation animation, Vector2 velocity)
    {
        Animation = animation;
        Velocity = velocity;
    }


    /// <summary>
    /// Creates a new animated physics sprite with the specified animation and velocity
    /// </summary>
    /// <param name="animation">The animation that is to be used</param>
    /// <param name="velocity">How fast and in what direction the PhysicsSprite is moving in pixels/second</param>
    /// <param name="acceleration">The rate of change of the velocity in pixels/second/second</param>
    public AnimatedPhysicsSprite(Animation animation, Vector2 velocity, Vector2 acceleration)
    {
        Animation = animation;
        Velocity = velocity;
        Acceleration = acceleration;
    }


    /// <summary>
    /// plays the sprite animation & moves it according to the GameTime
    /// </summary>
    /// <param name="gameTime">the GameTime</param>
    new public void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if(!_isPlaying || _animation == null) return;
    
        _elapsed += gameTime.ElapsedGameTime;

        if(_elapsed >= _animation.Frametime)
        {
            _elapsed -= _animation.Frametime;

            if(++_currentFrame >= _animation.Frames.Count)
            {
                _currentFrame = 0;
            }

            Region = _animation.Frames[_currentFrame];
        }
    }
}