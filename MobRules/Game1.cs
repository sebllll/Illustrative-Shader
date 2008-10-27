using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using MobRules.GameComponents;
using MobRules.Helpers;
using MobRules.Interfaces;

namespace MobRules
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Game Components and Mangers
        InputManager inputManager = null;
        Camera camera = null;

        // Testing code
        Model asteroid;
        float planetsize = 0.01f;
        Vector3[] block;
        int numblocks;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            ServiceHelper.Game = this;
        }

        // Testing Code
        void InitializeField()
        {
            numblocks = 400;
            block = new Vector3[numblocks];

            Random r = new Random();

            for (int i = 0; i < numblocks; i++)
            {
                block[i].X = 500 - (float)r.NextDouble() * 1000;
                block[i].Y = 500 - (float)r.NextDouble() * 1000;
                block[i].Z = 500 - (float)r.NextDouble() * 1000;

            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Start and add Game Services/Components
            inputManager = new InputManager(this);
            ServiceHelper.Add<IInputManagerService>(inputManager);
            this.Components.Add(inputManager);
            camera = new Camera(this);
            ServiceHelper.Add<ICameraService>(camera);
            this.Components.Add(camera);

            // Testing
            InitializeField();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            System.Console.WriteLine(Content.RootDirectory);
            spriteBatch = new SpriteBatch(GraphicsDevice); // Create a new SpriteBatch, which can be used to draw textures.
            asteroid = Content.Load<Model>(@"Models/asteroid1");
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (inputManager.CurrentKBState.IsKeyDown(Keys.Escape))
                this.Exit();

            base.Update(gameTime);
        }


        //----------------------------------------------------------------------------------------------------
        void DrawOne(Model m, Matrix world)
        {
            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = camera.Projection;
                    be.View = camera.View;
                    be.World = world * mesh.ParentBone.Transform;
                }
                mesh.Draw();
            }
        }

        void DrawField()
        {
            Matrix w;
            for (int i = 0; i < numblocks; i++)
            {
                w = Matrix.CreateScale(planetsize) * Matrix.CreateTranslation(block[i]);
                DrawOne(asteroid, w);
            }
        }
        //----------------------------------------------------------------------------------------------------


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Testing Code
            DrawField();

            base.Draw(gameTime);
        }
    }
}
