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
using MonoGameTutorial.UI;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using Gum.Wireframe;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Managers;

namespace MonoGameTutorial.Scenes;

public class GameScene : Scene
{
    private Random _rng = new Random();
    private AnimatedPhysicsSprite _bat;
    private Sprite _bounds;

    private SoundEffect _uiSoundEffect;

    private TileMap _tileMap;

    private Rectangle _roomBounds;

    private SoundEffect _batBounce;

    private SoundEffect _slimeEat;

    private Song _theme;

    SlimeSegment _slimes;

    private SpriteFont _font;

    private Vector2 _scoreTextPosition;

    private Vector2 _scoreTextOrigin;

    private int _score => SlimeSegment.Score;

    private InputAction _left;
    private InputAction _right;
    private InputAction _down;
    private InputAction _up;
    private Effect _greyscaleEffect;
    private float _saturation = 1.0f;
    private const float FADE_SPEED = 0.02f; 

    public List<Vector2> SpawnablePositions;

    //UI componants
    Panel _pausePanel;
    AnimatedButton _resumeButton;

    Panel _gameOverPanel;

    private TextureAtlas _atlas;

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

        SpawnablePositions = new List<Vector2>();

        // Initial slime position will be the center tile of the tile map.
        int centerRow = _tileMap.Rows / 2;
        int centerColumn = _tileMap.Columns / 2;
        _slimes = new SlimeSegment(4, new Vector2(centerColumn * _tileMap.TileWidth, centerRow * _tileMap.TileHeight));

        // Initial bat position will be in the top left corner of the room
        _bat.Position = new Vector2(2* _tileMap.TileWidth, 2* _tileMap.TileHeight);

        _left = new InputAction(_leftInputs, input, () => {
            SlimeSegment.Turn(SlimeSegment.Directions.Left);
        });
        _right = new InputAction(_rightInputs, input, ()=> {
            SlimeSegment.Turn(SlimeSegment.Directions.Right);
        });
        _up = new InputAction(_upInputs, input, () => {
            SlimeSegment.Turn(SlimeSegment.Directions.Up);
        });
        _down = new InputAction(_downInputs, input, () => {
            SlimeSegment.Turn(SlimeSegment.Directions.Down);
        });

        Core.Audio.PlaySong(_theme);

        // Set the position of the score text to align to the left edge of the
        // room bounds, and to vertically be at the center of the first tile.
        _scoreTextPosition = new Vector2(_roomBounds.Left, _tileMap.TileHeight * 0.5f);

        // Set the origin of the text so it is left-centered.
        float scoreTextYOrigin = _font.MeasureString("Score").Y * 0.5f;
        _scoreTextOrigin = new Vector2(0, scoreTextYOrigin);

        InitializeUI();
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

        
        _atlas = TextureAtlas.FromFile(Content, "images/atlas.xml");

        _tileMap = TileMap.FromFile(Content, "images/map-definition.xml");
        _tileMap.Scale *= 4;
        
        //Initialize Sprites
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

        _bat = _atlas.CreateAnimatedPhysicsSprite("bat-animation", randomDir, "bat");
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

        _uiSoundEffect = Content.Load<SoundEffect>("audio/ui");
        _greyscaleEffect = Content.Load<Effect>("effects/greyScaleEffect");
    }

    public override void Update(GameTime gameTime)
    {     
        // Ensure the UI is always updated
        GumService.Default.Update(gameTime);

        // If the game is paused, do not continue
        if (_pausePanel.IsVisible || _gameOverPanel.IsVisible)
        {
            _saturation = Math.Max(0.0f, _saturation - FADE_SPEED);
            return;
        }

        AnimatedSprite head = SlimeSegment.Head.Sprite;

        _bat.Update(gameTime);
        _bat.doCollisionReaction(_bounds);
        foreach(SlimeSegment segment in SlimeSegment.Segments)
        {
            if(segment != SlimeSegment.Head)
            {
                Sprite sprite = segment.Sprite;
                _bat.doCollisionReaction(sprite);
                head.doCollisionReaction(sprite);
            }
        }

       head.doCollisionReaction(_bat);
       head.doCollisionReaction(_bounds);

        SlimeSegment.Update(gameTime);

        _bounds.doCollisionReaction(head);

        DoActionsOnInputHeld(_left, _right, _up, _down);

        if (Core.Input.Keyboard.JustPressed(Keys.Escape) || Core.Input.GamePads[1].justPressed(Buttons.Start))
        {
            PauseGame();
        }

        GumService.Default.Update(gameTime);
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch spriteBatch = Core.SpriteBatch;

        if (_pausePanel.IsVisible || _gameOverPanel.IsVisible)
        {
            // We are in a game over state, so apply the saturation parameter.
            _greyscaleEffect.Parameters["Saturation"].SetValue(_saturation);

            // And begin the sprite batch using the grayscale effect.
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _greyscaleEffect);
        }
        else
        {
            // Otherwise, just begin the sprite batch as normal.
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        }


                _tileMap.Draw(spriteBatch);
                SlimeSegment.Draw(spriteBatch);
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

        Core.SpriteBatch.End();
        GumService.Default.Draw();

        base.Draw(gameTime);
    }
    
    private void PauseGame()
    {
        // Make the pause panel UI element visible.
        _pausePanel.IsVisible = true;

        // Set the resume button to have focus
        _resumeButton.IsFocused = true;

        //set the saturation of the game to 1.0f;
        _saturation = 1.0f;
    }
    private static void drawSpriteBatch(SpriteBatch batch, Action func)
    {
        batch.Begin();
        func();
        batch.End();
    }

    private void DoActionsOnInputHeld(params InputAction[] actions)
    {
        foreach(InputAction action in actions)
        {
            action.DoOnInputHeld();
        }
    }

    public override void InitializeUI()
    {
        GumService.Default.Root.Children.Clear();

        CreatePausePanel();
        CreateGameOverPanel();
    }

    private void CreatePausePanel()
    {
        _pausePanel = new Panel();
        _pausePanel.Anchor(Anchor.Center);
        _pausePanel.WidthUnits = DimensionUnitType.Absolute;
        _pausePanel.HeightUnits = DimensionUnitType.Absolute;
        _pausePanel.Height = 70;
        _pausePanel.Width = 264;
        _pausePanel.IsVisible = false;
        _pausePanel.AddToRoot();

        TextureRegion bgRegion = _atlas.GetRegion("panel-background");

        NineSliceRuntime bg = new NineSliceRuntime();
        bg.Dock(Dock.Fill);
        bg.Texture = bgRegion.Texture;
        bg.TextureAddress = TextureAddress.Custom;
        bg.TextureHeight = bgRegion.Height;
        bg.TextureLeft = bgRegion.SourceRectangle.Left;
        bg.TextureTop = bgRegion.SourceRectangle.Top;
        bg.TextureWidth = bgRegion.Width;
        _pausePanel.AddChild(bg);

        var background = new ColoredRectangleRuntime();
        background.Dock(Dock.Fill);
        background.Color = Color.DarkBlue;
        _pausePanel.AddChild(background);

        var textInstance = new TextRuntime();
        textInstance.Text = "PAUSED";
        textInstance.CustomFontFile = @"fongs/04b_30.fnt";
        textInstance.UseCustomFont = true;
        textInstance.FontScale = 0.5f;
        textInstance.X = 10f;
        textInstance.Y = 10f;
        _pausePanel.AddChild(textInstance);

        _resumeButton = new AnimatedButton(_atlas);
        _resumeButton.Text = "RESUME";
        _resumeButton.Anchor(Anchor.BottomLeft);
        _resumeButton.X = 9f;
        _resumeButton.Y = -9f;
        _resumeButton.Width = 80;
        _resumeButton.Click += HandleResumeButtonClicked;
        _pausePanel.AddChild(_resumeButton);

        var quitButton = new AnimatedButton(_atlas);
        quitButton.Text = "QUIT";
        quitButton.Anchor(Anchor.BottomRight);
        quitButton.X = -9f;
        quitButton.Y = -9f;
        quitButton.Width = 80;
        quitButton.Click += HandleQuitButtonClicked;

        _pausePanel.AddChild(quitButton);
    }

    private void HandleResumeButtonClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Make the pause panel invisible to resume the game.
        _pausePanel.IsVisible = false;
    }

    private void HandleQuitButtonClicked(object sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        Core.ChangeScene(new TitleScene());
    }
    public void GameOver()
    {
        _gameOverPanel.IsVisible = true;
        _saturation = 1.0f;
        
    }


    public void CreateGameOverPanel()
    {
        _gameOverPanel = new Panel();
        _gameOverPanel.Anchor(Anchor.Center);
        _gameOverPanel.WidthUnits = DimensionUnitType.Absolute;
        _gameOverPanel.HeightUnits = DimensionUnitType.Absolute;
        _gameOverPanel.Height = 70;
        _gameOverPanel.Width = 264;
        _gameOverPanel.IsVisible = false;
        _gameOverPanel.AddToRoot();

        TextureRegion bgRegion = _atlas.GetRegion("panel-background");

        NineSliceRuntime bg = new NineSliceRuntime();
        bg.Dock(Dock.Fill);
        bg.Texture = bgRegion.Texture;
        bg.TextureAddress = TextureAddress.Custom;
        bg.TextureHeight = bgRegion.Height;
        bg.TextureLeft = bgRegion.SourceRectangle.Left;
        bg.TextureTop = bgRegion.SourceRectangle.Top;
        bg.TextureWidth = bgRegion.Width;
        _gameOverPanel.AddChild(bg);

        var textInstance = new TextRuntime();
        textInstance.Text = "GAME OVER!";
        textInstance.CustomFontFile = @"fongs/04b_30.fnt";
        textInstance.UseCustomFont = true;
        textInstance.FontScale = 0.5f;
        textInstance.X = 10f;
        textInstance.Y = 10f;
        _gameOverPanel.AddChild(textInstance);

        var _restartButton = new AnimatedButton(_atlas);
        _restartButton.Text = "RESTART";
        _restartButton.Anchor(Anchor.BottomLeft);
        _restartButton.X = 9f;
        _restartButton.Y = -9f;
        _restartButton.Width = 80;
        _restartButton.Click += HandleRestartButtonClicked;
        _gameOverPanel.AddChild(_restartButton);

        var quitButton = new AnimatedButton(_atlas);
        quitButton.Text = "QUIT";
        quitButton.Anchor(Anchor.BottomRight);
        quitButton.X = -9f;
        quitButton.Y = -9f;
        quitButton.Width = 80;
        quitButton.Click += HandleQuitButtonClicked;
        _gameOverPanel.AddChild(quitButton);
    }

    private void HandleRestartButtonClicked(object sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        SlimeSegment.Reset();
        Core.ChangeScene(new GameScene());

    }
}