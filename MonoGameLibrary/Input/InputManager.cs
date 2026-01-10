using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

public class InputManager
{
    public KeyboardInfo Keyboard;

    public MouseInfo Mouse;

    public GamePadInfo[] GamePads;

    public InputManager()
    {
        Keyboard = new KeyboardInfo();
        Mouse = new MouseInfo();
        
        GamePads = new GamePadInfo[4];
        for(int i = 0; i < 4; i++)
        {
            GamePads[i] = new GamePadInfo((PlayerIndex) i);
        }
    }

    public void Update(GameTime gameTime)
    {
        Keyboard.Update();
        Mouse.Update();
        foreach(GamePadInfo gamePad in GamePads)
        {
            gamePad.Update(gameTime);
        }
    }
}