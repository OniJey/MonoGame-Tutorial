using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace MonoGameTutorial;

public class Game1 : Core
{
    private Sprite _bat;

    private Sprite _slime;
    public Game1() : base("Dungeon Slime", 1280, 720, false)
    {
        
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas.xml");
        
        _slime  = new Sprite(atlas.GetRegion("slime")); 
        _slime.Scale = Vector2.One * 4;

        _bat = new Sprite(atlas.GetRegion("bat"));
        _bat.Scale = Vector2.One * 4;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        drawSpriteBatch(SpriteBatch, () =>
            {
                _slime.Draw(SpriteBatch, new Vector2(0,0));
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
}