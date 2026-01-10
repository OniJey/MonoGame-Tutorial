using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Input;

public class InputAction
{
    /// <summary>
    /// A list of the valid inputs that would perform this action
    /// </summary>
    private List<Enum> _inputs;

    /// <summary>
    /// The function that the action should call
    /// </summary> <summary>
    private Action _action;

    public InputManager Input {get; private set;}


    /// <summary>
    /// Instantiates an actionless InputAction with the list of valid inputs
    /// </summary>
    /// <param name="inputs">A list of the Input enums of type Buttons or Keys that will induce the Action</param>
    public InputAction(List<Enum> inputs, InputManager inputManager)
    {
        _inputs = inputs;
        Input = inputManager;
    }

    /// <summary>
    /// Instantiates an actionless InputAction with the list of valid inputs
    /// </summary>
    /// <param name="inputs">A list of the Input enums of type Buttons or Keys that will induce the Action</param>
    /// <param name="action">The function that the InputAction should perform</param>
    public InputAction(List<Enum> inputs, InputManager inputManager, Action action)
    {
        _inputs = inputs;
        _action = action;
        Input = inputManager;
    }

    /// <summary>
    /// Run the InputAction's assigned action
    /// </summary>
    public void DoAction()
    {
        if(_action != null)
            _action();
    }
    
    /// <summary>
    /// calls action each frame any of the InputAction's inputs are held
    /// </summary>
    public void DoOnInputHeld()
    {
        if(IsInputHeld()) DoAction();
    }

    public void DoOnInputPress()
    {
        if(InputJustPressed()) DoAction();
    }

    /// <summary>
    /// returns true if any of the InputAction's triggers are down
    /// </summary>
    public bool IsInputHeld(PlayerIndex playerIndex = PlayerIndex.One)
    {
        if(_inputs.Count == 0) return false;

        foreach(Enum input in _inputs)
        {
            switch(input)
            {
                case Keys key:
                    if(Input.Keyboard.IsKeyDown(key)) return true;
                    break;
                case Buttons button:
                    if(Input.GamePads[(int) playerIndex].IsButtonDown(button)) return true;
                    break;
                case MouseButtons button:
                    if(Input.Mouse.IsButtonDown(button)) return true;
                    break;
            }
        }
        return false;
    }

    public bool InputJustPressed(PlayerIndex playerIndex = PlayerIndex.One)
    {
        if(_inputs.Count == 0) return false;

        foreach(Enum input in _inputs)
        {
            switch(input)
            {
                case Keys key:
                    if(Input.Keyboard.justPressed(key)) return true;
                    break;
                case Buttons button:
                    if(Input.GamePads[(int)playerIndex].justPressed(button)) return true;
                    break;
                case MouseButtons button:
                    if(Input.Mouse.justPressed(button)) return true;
                    break;
            }
        }
        return false;
    }
}