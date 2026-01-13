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
using MonoGameTutorial.Scenes;

namespace MonoGameTutorial;

public class Game1 : Core
{

    public Game1() : base("Dungeon Slime", 1280, 720, false)
    {
        
    }

    protected override void Initialize()
    {
        s_activeScene = new TitleScene();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
    private static void drawSpriteBatch(SpriteBatch batch, Action func)
    {
        batch.Begin();
        func();
        batch.End();
    }
}