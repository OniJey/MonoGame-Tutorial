using System;
using Microsoft.Xna.Framework;
namespace MonoGameLibrary.Graphics;

public class AnimatedSprite : Sprite
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
    /// pauses the sprite's animation
    /// </summary>
    public void PauseAnimation()
    {
        _isPlaying = false;
    }
    
    /// <summary>
    /// plays the sprites animation
    /// </summary>
    public void PlayAnimation()
    {
        _isPlaying = true;
    }

    /// <summary>
    /// Creates new empty animated Sprite
    /// </summary>
    public AnimatedSprite () {}

    /// <summary>
    /// create a new animated sprite using the specified animation
    /// </summary>
    /// <param name="animation">the animation that will be played where this sprite is</param>
    public AnimatedSprite(Animation animation, string name)
    {
        Animation = animation;
        Name = name;
    }


    /// <summary>
    /// plays the sprite animation
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
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