using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace MonoGameTutorial;

public class Game1 : Core
{
    private AnimatedSprite _bat;

    private AnimatedSprite _slime;

    private float _slimeSpeed = 5.0f;

    private InputAction _left;
    private InputAction _right;
    private InputAction _down;
    private InputAction _up;

    private List<Enum> _leftInputs = [
        Keys.A, 
        Keys.Left,
        Buttons.DPadLeft
    ];
    
    private List<Enum> _rightInputs = [
        Keys.D,
        Keys.Right,
        Buttons.DPadRight
    ];

    private List<Enum> _downInputs = [
        Keys.S,
        Keys.Down,
        Buttons.DPadDown
    ];

    private List<Enum> _upInputs = [
        Keys.W,
        Keys.Up,
        Buttons.DPadUp
    ];

    private InputManager _input;

    public Game1() : base("Dungeon Slime", 800, 450, false)
    {
        
    }

    protected override void Initialize()
    {

        base.Initialize();
        _left = new InputAction(_leftInputs, Input, () => {
            MoveSlime("left", _slimeSpeed);
        });
        _right = new InputAction(_rightInputs, Input, ()=> {
            MoveSlime("right", _slimeSpeed);
        });
        _up = new InputAction(_upInputs, Input, () => {
            MoveSlime("up", _slimeSpeed);
        });
        _down = new InputAction(_downInputs, Input, () => {
            MoveSlime("down", _slimeSpeed);
        });
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas.xml");
        
        _slime  = atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = Vector2.One * 4;

        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = Vector2.One * 4;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        
        _bat.Update(gameTime);

        _slime.Update(gameTime);

        DoActionsOnInputHeld(_left, _right, _up, _down);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        drawSpriteBatch(SpriteBatch, () =>
            {
                _slime.Draw(SpriteBatch, _slime.Position);
                _bat.Draw(SpriteBatch, new Vector2(_slime.Width + 10, 0));
            }
        );

        base.Draw(gameTime);
    }

    private static Vector2 getCenterVector(int width, int height)
    {
        return new Vector2(width * 0.5f, height * 0.5f);
    }

    private static Vector2 getCenterVector(Rectangle rect)
    {
        return new Vector2(rect.Width * 0.5f, rect.Height * 0.5f);
    }

    private static void drawSpriteBatch(SpriteBatch batch, Action func)
    {
        batch.Begin();
        func();
        batch.End();
    }
    private void MoveSlime(string direction, float magnitude = 5.0f)
    {
        switch(direction)
        {
            case "right":
                _slime.Move(magnitude, new Vector2(1, 0));
                break;
            case "left":
                _slime.Move(magnitude, new Vector2(-1, 0));
                break;
            case "up":
                _slime.Move(magnitude, new Vector2(0, -1));
                break;
            case "down":
                _slime.Move(magnitude, new Vector2(0, 1));
                break;
        }
    }

    private void DoActionsOnInputHeld(params InputAction[] actions)
    {
        foreach(InputAction action in actions)
        {
            action.DoOnInputHeld();
        }
    }
}