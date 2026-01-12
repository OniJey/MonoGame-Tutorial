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
    private Random _rng = new Random();
    private AnimatedPhysicsSprite _bat;

    private Sprite _bounds;

    private AnimatedSprite _slime;

    private TileMap _tileMap;

    private Rectangle _roomBounds;

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

    public Game1() : base("Dungeon Slime", 1280, 720, false)
    {
        
    }

    protected override void Initialize()
    {

        base.Initialize();

        Rectangle screenBounds = GraphicsDevice.PresentationParameters.Bounds;

        _roomBounds = new Rectangle(
            (int) _tileMap.TileWidth,
            (int) _tileMap.TileHeight,
            screenBounds.Width - (int)_tileMap.TileWidth*2,
            screenBounds.Height - (int)_tileMap.TileHeight*2
        );

        // Initial slime position will be the center tile of the tile map.
        int centerRow = _tileMap.Rows / 2;
        int centerColumn = _tileMap.Columns / 2;
        _slime.Position = new Vector2(centerColumn * _tileMap.TileWidth, centerRow * _tileMap.TileHeight);

        // Initial bat position will be in the top left corner of the room
        _bat.Position = new Vector2(_roomBounds.Left, _roomBounds.Top);

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
        _slime.CollisionType = CollisionTypes.AABB;
        _slime.CollisionReaction = CollisionReactions.Trigger;
        _slime.TriggerAction = slimeCollisionAction;
        _slime.Scale = Vector2.One * 4;

        Vector2 randomDir = new Vector2(_rng.Next(-100, 100)/100.0f, _rng.Next(-100, 100)/100.0f);
        if(randomDir == Vector2.Zero) randomDir = new Vector2(0, 1);
        randomDir.Normalize();
        randomDir *= 400.0f;

        _bounds = new Sprite();
        _bounds.Region = new TextureRegion(null, new Rectangle(0,0,1,1)); // No visual
        _bounds.Position = Vector2.Zero;
        _bounds.Scale = new Vector2(Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
        _bounds.CollisionType = CollisionTypes.Container;
        _bounds.CollisionReaction = CollisionReactions.BlockAnchored;

        _bat = atlas.CreateAnimatedPhysicsSprite("bat-animation", randomDir);
        _bat.Scale = Vector2.One * 4;
        _bat.CollisionRadius =  (int) _bat.Width/2; 
        _bat.CollisionType = CollisionTypes.Circle;
        _bat.CollisionReaction = CollisionReactions.Bounce;
        _bat.CenterOrigin();

        // Create the tilemap from the XML configuration file.
        _tileMap = TileMap.FromFile(Content, "images/map-definition.xml");
        _tileMap.Scale *= 4;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        
        _bat.Update(gameTime);

        _bat.doCollisionReaction(_bounds);
        _bat.doCollisionReaction(_slime);

        _slime.Update(gameTime);

        _bounds.doCollisionReaction(_slime);


        DoActionsOnInputHeld(_left, _right, _up, _down);


        // Debug output
        Console.WriteLine($"Slime Position: {_slime.Position}");
        Console.WriteLine($"Bat Position: {_bat.Position}");
        Console.WriteLine($"Bat Velocity: {_bat.Velocity}");
        Console.WriteLine($"Slime CollisionCircle: Center={_slime.CollisionCircle.Position}, Radius={_bat.CollisionCircle.Radius}");
        Console.WriteLine($"Slime Origin: {_slime.Origin}");
        Console.WriteLine($"Bounds AABB: {_bounds.AABB}");
        Console.WriteLine($"Slime AABB: {_slime.AABB}");
        Console.WriteLine($"Collides: {_slime.CollidesWith(_bounds)}");
        Console.WriteLine("---");

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        drawSpriteBatch(SpriteBatch, () =>
            {
                _tileMap.Draw(SpriteBatch);
                _slime.Draw(SpriteBatch, _slime.Position);
                _bat.Draw(SpriteBatch, _bat.Position);

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
                _slime.Move(new Vector2(1, 0), magnitude);
                break;
            case "left":
                _slime.Move(new Vector2(-1, 0), magnitude);
                break;
            case "up":
                _slime.Move(new Vector2(0, -1), magnitude);
                break;
            case "down":
                _slime.Move(new Vector2(0, 1), magnitude);
                break;
        }
    }

    private static void Debug()
    {
        
    }

    private static void slimeCollisionAction(Sprite sprite1, Sprite sprite2)
    {
        
    }


    private void DoActionsOnInputHeld(params InputAction[] actions)
    {
        foreach(InputAction action in actions)
        {
            action.DoOnInputHeld();
        }
    }
}