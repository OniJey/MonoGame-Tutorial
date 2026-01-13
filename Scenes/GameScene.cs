using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.Audio;

namespace MonoGameTutorial.Scenes;

public class GameScene : Scene
{
        private Random _rng = new Random();
    private AnimatedPhysicsSprite _bat;

    private Sprite _bounds;

    private AnimatedSprite _slime;

    private TileMap _tileMap;

    private Rectangle _roomBounds;

    private SoundEffect _batBounce;

    private SoundEffect _slimeEat;

    private Song _theme;

    private readonly float _slimeSpeed = 5.0f;

    private SpriteFont _font;

    private Vector2 _scoreTextPosition;

    private Vector2 _scoreTextOrigin;

    private int _score;

    private InputAction _left;
    private InputAction _right;
    private InputAction _down;
    private InputAction _up;

    private readonly List<Enum> _leftInputs = [
        Keys.A, 
        Keys.Left,
        Buttons.DPadLeft
    ];
    
    private readonly List<Enum> _rightInputs = [
        Keys.D,
        Keys.Right,
        Buttons.DPadRight
    ];

    private readonly List<Enum> _downInputs = [
        Keys.S,
        Keys.Down,
        Buttons.DPadDown
    ];

    private readonly List<Enum> _upInputs = [
        Keys.W,
        Keys.Up,
        Buttons.DPadUp
    ];

    public override void Initialize()
    {

        base.Initialize();

        Rectangle screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;
        InputManager input = Core.Input;
        
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
        _bat.Position = new Vector2(2* _tileMap.TileWidth, 2* _tileMap.TileHeight);

        _left = new InputAction(_leftInputs, input, () => {
            MoveSlime("left", _slimeSpeed);
        });
        _right = new InputAction(_rightInputs, input, ()=> {
            MoveSlime("right", _slimeSpeed);
        });
        _up = new InputAction(_upInputs, input, () => {
            MoveSlime("up", _slimeSpeed);
        });
        _down = new InputAction(_downInputs, input, () => {
            MoveSlime("down", _slimeSpeed);
        });

        Core.Audio.PlaySong(_theme);

        // Set the position of the score text to align to the left edge of the
        // room bounds, and to vertically be at the center of the first tile.
        _scoreTextPosition = new Vector2(_roomBounds.Left, _tileMap.TileHeight * 0.5f);

        // Set the origin of the text so it is left-centered.
        float scoreTextYOrigin = _font.MeasureString("Score").Y * 0.5f;
        _scoreTextOrigin = new Vector2(0, scoreTextYOrigin);
    }

    public override void LoadContent()
    {
        base.LoadContent();

        //Load Font
        _font = Content.Load<SpriteFont>("fonts/04B_30");

        //Load Sounds
        _batBounce = Content.Load<SoundEffect>("audio/bounce");
        _slimeEat = Content.Load<SoundEffect>("audio/collect");

        //Load Theme Song

        _theme = Content.Load<Song>("audio/theme");

        if(MediaPlayer.State == MediaState.Playing)
        {
            MediaPlayer.Stop();
        }

        MediaPlayer.Play(_theme);  

        MediaPlayer.IsRepeating = true;

        //Load textures

        
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas.xml");

        _tileMap = TileMap.FromFile(Content, "images/map-definition.xml");
        _tileMap.Scale *= 4;
        
        //Initialize Sprites

        _slime  = atlas.CreateAnimatedSprite("slime-animation", "slime");
        _slime.CollisionType = CollisionTypes.AABB;
        _slime.CollisionReaction = CollisionReactions.Trigger;
        _slime.TriggerAction = ( Sprite x, Sprite y) => slimeCollisionAction(x, (PhysicsSprite) y);
        _slime.Scale = Vector2.One * 4;

        Vector2 randomDir = new Vector2(_rng.Next(-100, 100)/100.0f, _rng.Next(-100, 100)/100.0f);
        if(randomDir == Vector2.Zero) randomDir = new Vector2(0, 1);
        randomDir.Normalize();
        randomDir *= 400.0f;

        _bounds = new Sprite();
        _bounds.Region = new TextureRegion(null, new Rectangle(0,0,1,1)); // No visual
        _bounds.Position = Vector2.One * _tileMap.TileHeight;
        _bounds.Scale = new Vector2(Core.Graphics.PreferredBackBufferWidth-2*_tileMap.TileWidth, Core.Graphics.PreferredBackBufferHeight-2*_tileMap.TileHeight);
        _bounds.CollisionType = CollisionTypes.Container;
        _bounds.CollisionReaction = CollisionReactions.BlockAnchored;

        _bat = atlas.CreateAnimatedPhysicsSprite("bat-animation", randomDir, "bat");
        _bat.Scale = Vector2.One * 4;
        _bat.CollisionRadius =  (int) _bat.Width/2; 
        _bat.CollisionType = CollisionTypes.Circle;
        _bat.CollisionReaction = CollisionReactions.BounceTrigger;
        _bat.TriggerAction = (Sprite bat, Sprite other) =>
        {
            if(other.Name != "slime")
            {
                Core.Audio.PlaySoundEffect(_batBounce);
            }
        };
        _bat.CenterOrigin();


    }

    public override void Update(GameTime gameTime)
    {       
        _bat.Update(gameTime);
        _bat.doCollisionReaction(_bounds);

        _slime.Update(gameTime);
        _slime.doCollisionReaction(_bat);

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

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch spriteBatch = Core.SpriteBatch;

        drawSpriteBatch(spriteBatch, () =>
            {
                _tileMap.Draw(spriteBatch);
                _slime.Draw(spriteBatch, _slime.Position);
                _bat.Draw(spriteBatch, _bat.Position);
                spriteBatch.DrawString(
                    _font,              
                    $"Score: {_score}", 
                    _scoreTextPosition, 
                    Color.White,        
                    0.0f,               
                    _scoreTextOrigin,   
                    1.0f,               
                    SpriteEffects.None, 
                    0.0f                
                );
            }
        );

        base.Draw(gameTime);
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

    private void slimeCollisionAction(Sprite slime, PhysicsSprite bat)
    {
        //randomize bat's position
        int column = _rng.Next(2, _tileMap.Columns - 2);
        int row = _rng.Next(2, _tileMap.Rows -2);

        _bat.Position = new Vector2(column * _tileMap.TileWidth, row * _tileMap.TileHeight);
        Vector2 NewVelocity = new Vector2(_rng.Next(1,100)/100f, _rng.Next(1, 100)/100f);
        NewVelocity.Normalize();
        NewVelocity *= _bat.Velocity.Length();
        _bat.Velocity = NewVelocity;

        Core.Audio.PlaySoundEffect(_slimeEat);

        _score += 100;
    }

    private void DoActionsOnInputHeld(params InputAction[] actions)
    {
        foreach(InputAction action in actions)
        {
            action.DoOnInputHeld();
        }
    }
}