using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Input;

public class MouseInfo
{
    /// <summary>
    ///  The current MouseState
    /// </summary>
    public MouseState CurrentState {get; set;}
    /// <summary>
    /// The previous frame's MouseState
    /// </summary>
    public MouseState PreviousState {get; set;}

    /// <summary>
    /// The position of the mouse on the previous frame
    /// </summary>
    public Point PreviousPosition => PreviousState.Position;

    /// <summary>
    /// The position of the mouse on the current frame
    /// </summary>
    public Point Position
    {
        get => CurrentState.Position;
        set => SetPosition(value.X, value.Y);
    }

    
    /// <summary>
    /// The current mouse position's x value
    /// </summary>
    public int X
    {
        get => CurrentState.X;
        set => SetPosition(value, CurrentState.Y);
    }

    /// <summary>
    /// The current moiuse position's y value
    /// </summary>
    public int Y
    {
        get => CurrentState.Y;
        set => SetPosition(CurrentState.X, value);
    }

    /// <summary>
    /// Gets the change in mouse position as a 2D vector
    /// </summary>
    public Vector2 DeltaMousePos
    {
        get => (Position - PreviousPosition).ToVector2();
    }

    /// <summary>
    /// returns the change in scroll wheel position from last frame
    /// </summary>
    public int DeltaScroll
    {
        get => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;
    }

        /// <summary>
    /// returns whether or not the scroll wheel is currently being scrolled down
    /// </summary>
    public bool IsScrollingDown
    {
        get => DeltaScroll < 0;
    }

    /// <summary>
    /// returns whether or not the scroll wheel is currently being scrolled up
    /// </summary>
    public bool IsScrollingUp
    {
        get => DeltaScroll > 0;
    }

    /// <summary>
    /// returns whether or not the mouse has moved since last frame
    /// </summary>
    public bool MouseMoved
    {
        get => DeltaMousePos != Vector2.Zero;
    }

 

    /// <summary>
    /// Instantiate a new MouseInfo instance
    /// </summary>
    public MouseInfo()
    {
        PreviousState = new MouseState();
        CurrentState = Mouse.GetState();
    }

    /// <summary>
    /// updates the CurrentState and PreviousState values
    /// </summary>
    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();
    }

    /// <summary>
    /// Sets the mouse's position to the specified point & updates CurrentState to reflect that
    /// </summary>
    /// <param name="x">The x value of the new position</param>
    /// <param name="y">The y value of the new posision</param>
    public void SetPosition(int x, int y)
    {
        Mouse.SetPosition(x, y);
        CurrentState = new MouseState(
            x,
            y,
            CurrentState.ScrollWheelValue,
            CurrentState.LeftButton,
            CurrentState.MiddleButton,
            CurrentState.RightButton,
            CurrentState.XButton1,
            CurrentState.XButton2
        );
    }

    /// <summary>
    /// returns whether or not the button corresponding to the MouseButtons enum given is pressed
    /// </summary>
    /// <param name="button">MouseButtons member representing a mouse button</param>
    public bool IsButtonDown(MouseButtons button)
    {
        switch(button)
        {
            case MouseButtons.Right:
                return CurrentState.RightButton == ButtonState.Pressed;
            case MouseButtons.Left:
                return CurrentState.LeftButton == ButtonState.Pressed;
            case MouseButtons.Middle:
                return CurrentState.MiddleButton == ButtonState.Pressed;
            case MouseButtons.XButton1:
                return CurrentState.XButton1 == ButtonState.Pressed;
            case MouseButtons.XButton2:
                return CurrentState.XButton2 == ButtonState.Pressed;
            default:
                return false;
        }
    }

    /// <summary>
    /// returns whether or not the button corresponding to the MouseButtons enum given is pressed
    /// </summary>
    /// <param name="button">MouseButtons member representing a mouse button</param>
    /// <param name="state">the MouseState to check</param>
    /// <returns></returns>
        public bool IsButtonDown(MouseButtons button, MouseState state)
    {
        switch(button)
        {
            case MouseButtons.Right:
                return state.RightButton == ButtonState.Pressed;
            case MouseButtons.Left:
                return state.LeftButton == ButtonState.Pressed;
            case MouseButtons.Middle:
                return state.MiddleButton == ButtonState.Pressed;
            case MouseButtons.XButton1:
                return state.XButton1 == ButtonState.Pressed;
            case MouseButtons.XButton2:
                return state.XButton2 == ButtonState.Pressed;
            default:
                return false;
        }
    }

    /// <summary>
    /// returns true when the given button is not pressed
    /// </summary>
    /// <param name="button">MouseButtons member representing a mouse button</param>
    public bool IsButtonUp(MouseButtons button)
    {
        return !IsButtonDown(button);
    }

    /// <summary>
    /// returns true when the given button is not pressed
    /// </summary>
    /// <param name="button">MouseButtons member representing a mouse button</param>
    /// <param name="state">The MouseState to checked</param>
    public bool IsButtonUp(MouseButtons button, MouseState state)
    {
        return !IsButtonDown(button, state);
    }

    /// <summary>
    /// returns true when the given mouse button was just pressed
    /// </summary>
    /// <param name="button">the button to check</param>
    /// <returns>true when the button was not pressed last frame but is pressed this frame, false otherwise</returns>
    public bool justPressed(MouseButtons button)
    {
        return IsButtonDown(button) && IsButtonUp(button, PreviousState);
    }

    /// <summary>
    /// returns true when the given mouse button was just released
    /// </summary>
    /// <param name="button">the button to check</param>
    /// <returns>true when the button was pressed last frame but is not pressed this frame, false otherwise</returns>
    public bool justReleased(MouseButtons button)
    {
        return IsButtonUp(button) && IsButtonDown(button, PreviousState);
    }
}