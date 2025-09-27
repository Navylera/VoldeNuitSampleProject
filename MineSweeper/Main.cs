// Copyright (c) 2025 Navylera

using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoldeNuit.Framework;

namespace MineSweeper;

using static Heart;

public class Main : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Main()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        InitResolution(640, 480);

        InitMonoGameEnvironment(Assembly.GetExecutingAssembly(), this, _graphics);

        InitEntryPoint(typeof(RMain));

        Configuration.ANGLE_FORMAT = Configuration.AngleFormat.LEGACY;
        Configuration.COLOR_FORMAT = Configuration.ColorFormat.ARGB;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        InitSpriteBatch(_spriteBatch);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        // if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        //     Exit();

        // TODO: Add your update logic here

        Heart.Beat();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Heart.Draw();

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
