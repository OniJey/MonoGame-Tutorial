using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Scenes;
using MonoGameLibrary;

namespace MonoGameTutorial.Scenes;

public class TitleScene : Scene
{
    private const string DUNGEON_TEXT = "Dungeon";
    private const string SLIME_TEXT = "Slime";
    private const string PRESS_ENTER_TEXT = "Press enter to start";

    private SpriteFont _font;
    private SpriteFont _font5x;

    private Vector2 _dungeonTextOrigin;

    private Vector2 _dungeonTextPos;

    private Vector2 _slimeTextOrigin;

    private Vector2 _slimeTextPos;

    private Vector2 _enterTextOrigin;

    private Vector2 _enterTextPos;

    private Texture2D _bgTexture;

    private Rectangle _bgDestination;

    private Vector2 _bgOffset;


    private float _scorllSpeed = 50.0f;

    public override void Initialize()
    {
        base.Initialize();

        Core.ExitOnEscape = true;

            // Set the position and origin for the Dungeon text.
        Vector2 size = _font5x.MeasureString(DUNGEON_TEXT);
        _dungeonTextPos = new Vector2(640, 100);
        _dungeonTextOrigin = size * 0.5f;

        // Set the position and origin for the Slime text.
        size = _font5x.MeasureString(SLIME_TEXT);
        _slimeTextPos = new Vector2(757, 207);
        _slimeTextOrigin = size * 0.5f;

        // Set the position and origin for the press enter text.
        size = _font.MeasureString(PRESS_ENTER_TEXT);
        _enterTextPos = new Vector2(640, 620);
        _enterTextOrigin = size * 0.5f;

        //set the starting offset to zero 
        _bgOffset = Vector2.Zero;

        //set the bg to fill the screen
        _bgDestination = Core.GraphicsDevice.PresentationParameters.Bounds;
    }

    public override void LoadContent()
    {
        //load the standard size font from Content
        _font = Content.Load<SpriteFont>("fonts/04B_30");

        //load the 5x sized font from Content
        _font5x = Content.Load<SpriteFont>("fonts/04B_30_5x");

        //load the background texture from Content
        _bgTexture = Content.Load<Texture2D>("images/background-pattern");
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        // If the user presses enter, switch to the game scene.
        if (Core.Input.Keyboard.JustPressed(Keys.Enter))
        {
            Core.ChangeScene(new GameScene());
        }

        //update the background offset
        float offset = _scorllSpeed*(float) gameTime.ElapsedGameTime.TotalSeconds;
        _bgOffset.X -= offset;
        _bgOffset.Y -= offset;

        //ensure patterns do go beyond the bounds of the image
        _bgOffset.X %= _bgTexture.Width;
        _bgOffset.Y %= _bgTexture.Height;
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(32, 40, 78, 255));

        // Draw the background pattern first using the PointWrap sampler state.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        Core.SpriteBatch.Draw(_bgTexture, _bgDestination, new Rectangle(_bgOffset.ToPoint(), _bgDestination.Size), Color.White * 0.5f);
        Core.SpriteBatch.End();

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // The color to use for the drop shadow text.
        Color dropShadowColor = Color.Black * 0.5f;

        // Draw the Dungeon text slightly offset from it is original position and
        // with a transparent color to give it a drop shadow.
        Core.SpriteBatch.DrawString(_font5x, DUNGEON_TEXT, _dungeonTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _dungeonTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw the Dungeon text on top of that at its original position.
        Core.SpriteBatch.DrawString(_font5x, DUNGEON_TEXT, _dungeonTextPos, Color.White, 0.0f, _dungeonTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw the Slime text slightly offset from it is original position and
        // with a transparent color to give it a drop shadow.
        Core.SpriteBatch.DrawString(_font5x, SLIME_TEXT, _slimeTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _slimeTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw the Slime text on top of that at its original position.
        Core.SpriteBatch.DrawString(_font5x, SLIME_TEXT, _slimeTextPos, Color.White, 0.0f, _slimeTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw the press enter text.
        Core.SpriteBatch.DrawString(_font, PRESS_ENTER_TEXT, _enterTextPos, Color.White, 0.0f, _enterTextOrigin, 1.0f, SpriteEffects.None, 0.0f);

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();
    }


}