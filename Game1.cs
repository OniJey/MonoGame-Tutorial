using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Gum.Forms;
using Gum.Forms.Controls;
using MonoGameGum;
using MonoGameLibrary;
using MonoGameTutorial.Scenes;

namespace MonoGameTutorial;

public class Game1 : Core
{

    public Game1() : base("Dungeon Slime", 1280, 720, false)
    {
        
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeGum();

        s_activeScene = new TitleScene();
        s_activeScene.Initialize();
        ExitOnEscape = false;
    }

    protected override void LoadContent()
    {
        base.LoadContent();

    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }

    private void InitializeGum()
    {
        //initialize the gum service, DefaultVisualsVersion specifies the version of defaultVisuals to use
        GumService.Default.Initialize(this, DefaultVisualsVersion.V3);

        GumService.Default.ContentLoader.XnaContentManager =  Content;

        FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);

        FrameworkElement.GamePadsForUiControl.AddRange(GumService.Default.Gamepads);

        FrameworkElement.TabReverseKeyCombos.Add(
            new KeyCombo() {PushedKey = Keys.Up}
        );

        FrameworkElement.TabKeyCombos.Add(
            new KeyCombo() {PushedKey = Keys.Down}
        );

        GumService.Default.CanvasWidth = GraphicsDevice.PresentationParameters.BackBufferWidth / 4.0f;
        GumService.Default.CanvasHeight = GraphicsDevice.PresentationParameters.BackBufferHeight / 4.0f;
        GumService.Default.Renderer.Camera.Zoom = 4.0f;

    }
    private static void drawSpriteBatch(SpriteBatch batch, Action func)
    {
        batch.Begin();
        func();
        batch.End();
    }
}