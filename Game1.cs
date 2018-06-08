using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CrepuscularRays_PostProcess.PostProcessing;
using System.Collections.Generic;
using System;

namespace CrepuscularRays_PostProcess
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        RenderTarget2D finalBackBuffer;
        RenderTarget2D finalDepthBuffer;

        Base3DCamera camera;
        PostProcessingManager ppManager;
        CrepuscularRays GodRays;

        KeyboardState thisKBState;
        KeyboardState lastKBState;

        MouseState thisMouseState;
        MouseState lastMouseState;

        Vector3 sunPosition = new Vector3(0, 0, -1000);

        List<Base3DModel> objects = new List<Base3DModel>();

        Random rnd = new Random(DateTime.Now.Millisecond);
        Color clearColor = Color.Black;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            camera = new Base3DCamera(this, .1f, 2000f);

            ppManager = new PostProcessingManager(this);
            GodRays = new CrepuscularRays(this, sunPosition, "Textures/flare", 100, .99f, .99f, .5f, .12f, .25f);
            ppManager.AddEffect(GodRays);


            int sqr = 50;
            //for (int a = 0; a < 100; a++)
            //{
            //    float x = MathHelper.Lerp(-sqr, sqr, (float)rnd.NextDouble());
            //    float y = MathHelper.Lerp(-sqr, sqr, (float)rnd.NextDouble());
            //    float z = MathHelper.Lerp(-sqr, -sqr * 2, (float)rnd.NextDouble());
            //    float s = MathHelper.Lerp(.01f, 2, (float)rnd.NextDouble());

            //    //asteroids.Add(new Base3DModel(this, "Models/asteroid1", "Shaders/ColorDepthRender", "Textures/stone"));
            //    if (rnd.NextDouble() >= .5f)
            //        objects.Add(new Base3DModel(this, "Models/tank", "Shaders/ColorDepthRender"));
            //    else
            //        objects.Add(new Base3DModel(this, "Models/LandShark", "Shaders/ColorDepthRender"));
            //    objects[objects.Count - 1].Position = new Vector3(x, y, z);
            //    objects[objects.Count - 1].Scale = Vector3.One * s;
            //    Components.Add(objects[objects.Count - 1]);

            //    //asteroidsRotDir.Add(asteroidsRotDir.Count, Vector3.Normalize(asteroids[asteroids.Count - 1].Position));
            //}
            objects.Add(new Base3DModel(this, "Models/LandShark", "Shaders/ColorDepthRender"));
            objects[0].Position = new Vector3(0, 0, -100);
            objects[0].Scale = Vector3.One;
            Components.Add(objects[0]);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            finalBackBuffer = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            finalDepthBuffer = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Put the sprice batch up as a service..
            Services.AddService(typeof(SpriteBatch), spriteBatch);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            GodRays.lightSource = sunPosition;

            thisKBState = Keyboard.GetState();
            thisMouseState = Mouse.GetState();
            float speedTran = .1f;
            float speedRot = .01f;
            if (thisKBState.IsKeyDown(Keys.W))
                camera.Translate(Vector3.Forward * speedTran);
            if (thisKBState.IsKeyDown(Keys.S))
                camera.Translate(Vector3.Backward * speedTran);
            if (thisKBState.IsKeyDown(Keys.A))
                camera.Translate(Vector3.Left * speedTran);
            if (thisKBState.IsKeyDown(Keys.D))
                camera.Translate(Vector3.Right * speedTran);

            if (thisKBState.IsKeyDown(Keys.Left))
                camera.Rotate(Vector3.Up, speedRot);
            if (thisKBState.IsKeyDown(Keys.Right))
                camera.Rotate(Vector3.Up, -speedRot);
            if (thisKBState.IsKeyDown(Keys.Up))
                camera.Rotate(Vector3.Right, speedRot);
            if (thisKBState.IsKeyDown(Keys.Down))
                camera.Rotate(Vector3.Right, -speedRot);
            base.Update(gameTime);
            lastMouseState = thisMouseState;
            lastKBState = thisKBState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Lets set up our RT's
            GraphicsDevice.SetRenderTargets(finalBackBuffer, finalDepthBuffer);

            GraphicsDevice.Clear(clearColor);

            // Make sure our device is how we want it
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            base.Draw(gameTime);

            // Now get the data back from the GPU
            GraphicsDevice.SetRenderTarget(null);

            // Once we have rendered the scene we ca do our post processing
            ppManager.Draw(gameTime, finalBackBuffer, finalDepthBuffer);
        }
    }
}
