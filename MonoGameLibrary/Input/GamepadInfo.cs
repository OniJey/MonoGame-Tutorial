using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

public class GamePadInfo
{
    /// <summary>
    /// The time the gamepad has left vibrating
    /// </summary>
    private TimeSpan _vibrationTimeRemaining = TimeSpan.Zero;
    /// <summary>
    /// The state the GamePad was in on the previous frame
    /// </summary>
    public GamePadState PreviousState {get; set;}
    /// <summary>
    /// The state the GamePad is in
    /// </summary>
    public GamePadState CurrentState {get; set;}

    /// <summary>
    /// The index that specifies which connected controller the GamePad is connected to
    /// </summary>
    public PlayerIndex PlayerIndex {get;}

    /// <summary>
    /// Whether or not the PlayerIndex this instance of GamePadInfo is connected to a controller
    /// </summary>
    public bool IsConnected => CurrentState.IsConnected;

    /// <summary>
    /// 2D vector representing the right thumbstick's position
    /// </summary>
    public Vector2 RightThumbstickPosition => CurrentState.ThumbSticks.Right;


    /// <summary>
    /// 2D vector representing the left thumbstick's position
    /// </summary>
    public Vector2 LeftThumbStickPosition => CurrentState.ThumbSticks.Left;

    /// <summary>
    /// Floating-point value representing how far the right trigger is pressed down. 0 is unpressed, 1 is fully pressed.
    /// </summary>
    public float RightTriggerValue => CurrentState.Triggers.Right;

    /// <summary>
    /// Floating-point value representing how far the left trigger is pressed down. 0 is unpressed, 1 is fully pressed.
    /// </summary>
    public float LeftTriggerValue => CurrentState.Triggers.Left;


    /// <summary>
    /// Instantiates a new GamePadInfo instance with the specified PlayerIndex
    /// </summary>
    /// <param name="playerIndex"></param>
    public GamePadInfo(PlayerIndex playerIndex)
    {
        PlayerIndex = playerIndex;
        PreviousState = new GamePadState();
        CurrentState = GamePad.GetState(playerIndex);
    }

    /// <summary>
    /// Updates the CurrentState and PreviousState values, to be called every frame
    /// </summary>
    /// <param name="gameTime">Deltatime since last frame or something idk<param>
    public void Update(GameTime gameTime)
    {
        PreviousState = CurrentState;
        CurrentState = GamePad.GetState(PlayerIndex);

        if(_vibrationTimeRemaining > TimeSpan.Zero)
        {
            _vibrationTimeRemaining -= gameTime.ElapsedGameTime;
            if(_vibrationTimeRemaining <= TimeSpan.Zero)
            {
                StopVibration();
                _vibrationTimeRemaining = TimeSpan.Zero;
            }
        }

    }

    /// <summary>
    /// Returns true when the given GamePad button was just pressed
    /// </summary>
    /// <param name="button">The button to check</param>
    /// <returns>True when the button was not pressed last frame but is pressed this frame, false otherwise</returns>
    public bool justPressed(Buttons button)
    {
        return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
    }

    /// <summary>
    /// Returns true when the given mouse button was just released
    /// </summary>
    /// <param name="button">The button to check</param>
    /// <returns>True when the button was not pressed last frame but is released this frame, false otherwise</returns>
    public bool justReleased(Buttons button)
    {
        return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
    }


    /// <summary>
    /// Returns wheter or not the specified button is down
    /// </summary>
    /// <param name="button">The button to check</param>
    public bool IsButtonDown(Buttons button)
    {
        return CurrentState.IsButtonDown(button);
    }

    /// <summary>
    /// Returns whether or not the button is up
    /// </summary>
    /// <param name="button"></param>
    public bool IsButtonUp(Buttons button)
    {
        return CurrentState.IsButtonUp(button);
    }

    /// <summary>
    /// Sets the vibration of both sides of the controller to one value
    /// </summary>
    /// <param name="strength">The value to set the vibration to</param>
    public void SetVibration(float strength)
    {
        SetVibration(strength, strength);
    }

    /// <summary>
    /// Sets the vibration of the controller
    /// </summary>
    /// <param name="strengthLeft">The vibration strength of the left side's motor</param>
    /// <param name="strengthRight">The vibration strength of the right side's motor</param>
    public void SetVibration(float strengthLeft, float strengthRight)
    {
        GamePad.SetVibration(PlayerIndex, strengthLeft, strengthRight);
    }

    /// <summary>
    /// Stops the GamePad's vibration
    /// </summary>
    /// <remarks>
    /// Sets the vibration value to 0
    /// </remarks>
    public void StopVibration()
    {
        SetVibration(0);
    }

    /// <summary>
    /// Makes the gamepad vibrate at the specified strength, for the specified time
    /// </summary>
    /// <param name="strength">The strength of the motors vibration</param>
    /// <param name="time">The time the motors should be active for</param>
    public void startVibration(float strength, TimeSpan time)
    {
        SetVibration(strength);
        _vibrationTimeRemaining = time;
    }

    public void startVibration(float leftStrength, float rightStrength, TimeSpan time)
    {
        SetVibration(leftStrength, rightStrength);
        _vibrationTimeRemaining = time;
    }
}