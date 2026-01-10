using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

public class KeyboardInfo
{


    /// <summary>
    /// The current state of the Keyboard
    /// </summary>
    public KeyboardState CurrentState {get; private set;}

    /// <summary>
    /// The previous state of the Keyboard
    /// </summary>
    public KeyboardState PreviousState {get; private set;}

    /// <summary>
    /// Instantiates a new KeyboardInfo 
    /// </summary>
    public KeyboardInfo()
    {
        PreviousState = new KeyboardState();
        CurrentState = Keyboard.GetState();
    }

    /// <summary>
    /// Updates the PreviousState and CurrentState values
    /// </summary>
    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Keyboard.GetState();
    }
    

    /// <summary>
    /// Returns whether or not the specified key was pressed on this frame.
    /// </summary>
    /// <param name="key">The key that was pressed</param>
    /// <returns>true if key was up last frame and down this frame; false otherwise</returns>
    public bool justPressed(Keys key)
    {
        return PreviousState.IsKeyUp(key) && IsKeyDown(key);
    }

    /// <summary>
    /// Returns whether or not the specified key was released on this frame
    /// </summary>
    /// <param name="key"></param>
    /// <returns>true if key was down last frame and up this frame; false otherwise</returns>
    public bool justReleased(Keys key)
    {
        return PreviousState.IsKeyDown(key) && IsKeyUp(key);
    }

    /// <summary>
    /// returns whether or not the key is currently being held
    /// </summary>
    /// <param name="key">the key to be checked</param>
    /// <returns>true if the key is down</returns>
    public bool IsKeyDown(Keys key)
    {
        return CurrentState.IsKeyDown(key);
    }

    /// <summary>
    /// returns whether or not the key is currently not being pressed
    /// </summary>
    /// <param name="key">the key to be checked</param>
    /// <returns>true if the key is not being held down</returns>
    public bool IsKeyUp(Keys key)
    {
        return CurrentState.IsKeyUp(key);
    }
}