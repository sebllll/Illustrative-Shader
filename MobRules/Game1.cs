#region Using Statements
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
using MobRules.Effects;
using MobRules.GameComponents;
using MobRules.Helpers;
using MobRules.Interfaces;
#endregion

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

        // Shaders
        PhongEffect phongEffect;

        // Test Cube
        Vector3 cubePos;
        Vector2 cubeRot;
        private VertexPositionNormalTexture[] vertices;
        private VertexDeclaration vertexDecl;
        private VertexBuffer vertexBuffer;
        private Texture2D colourMap;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            ServiceHelper.Game = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Appliation Settings
            Window.Title = "Mob Rules";
            IsMouseVisible = false;

            // Setup frame buffer.
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width / 2;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height / 2;
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();

            // Setup Test Cube.
            cubePos = new Vector3(0.0f, 0.0f, -5.0f);
            cubeRot = Vector2.Zero;
            GenerateCube(2.0f);

            // Start and add Game Services/Components
            inputManager = new InputManager(this);
            ServiceHelper.Add<IInputManagerService>(inputManager);
            this.Components.Add(inputManager);
            camera = new Camera(this);
            ServiceHelper.Add<ICameraService>(camera);
            this.Components.Add(camera);

            phongEffect = new PhongEffect();
            phongEffect.Initialize();

            base.Initialize();
        }

        private void GenerateCube(float size)
        {
            float halfSize = size * 0.5f;

            Vector3[] corners =
            {
                new Vector3(-halfSize,  halfSize,  halfSize),   // 0
                new Vector3( halfSize,  halfSize,  halfSize),   // 1
                new Vector3( halfSize, -halfSize,  halfSize),   // 2
                new Vector3(-halfSize, -halfSize,  halfSize),   // 3
                new Vector3(-halfSize,  halfSize, -halfSize),   // 4
                new Vector3( halfSize,  halfSize, -halfSize),   // 5
                new Vector3( halfSize, -halfSize, -halfSize),   // 6
                new Vector3(-halfSize, -halfSize, -halfSize)    // 7
            };

            Vector2[] texCoords =
            {
                new Vector2(0.0f, 0.0f),    // top left corner
                new Vector2(1.0f, 0.0f),    // top right corner
                new Vector2(1.0f, 1.0f),    // bottom right corner
                new Vector2(0.0f, 1.0f)     // bottom left corner
            };

            vertices = new VertexPositionNormalTexture[36];

            // +z face = 0123 tri1 = 012 tri2 = 230
            vertices[0] = new VertexPositionNormalTexture(corners[0], Vector3.Backward, texCoords[0]);
            vertices[1] = new VertexPositionNormalTexture(corners[1], Vector3.Backward, texCoords[1]);
            vertices[2] = new VertexPositionNormalTexture(corners[2], Vector3.Backward, texCoords[2]);
            vertices[3] = new VertexPositionNormalTexture(corners[2], Vector3.Backward, texCoords[2]);
            vertices[4] = new VertexPositionNormalTexture(corners[3], Vector3.Backward, texCoords[3]);
            vertices[5] = new VertexPositionNormalTexture(corners[0], Vector3.Backward, texCoords[0]);

            // -z face = 5476 tri1 = 547 tri2 = 765
            vertices[6] = new VertexPositionNormalTexture(corners[5], Vector3.Forward, texCoords[0]);
            vertices[7] = new VertexPositionNormalTexture(corners[4], Vector3.Forward, texCoords[1]);
            vertices[8] = new VertexPositionNormalTexture(corners[7], Vector3.Forward, texCoords[2]);
            vertices[9] = new VertexPositionNormalTexture(corners[7], Vector3.Forward, texCoords[2]);
            vertices[10] = new VertexPositionNormalTexture(corners[6], Vector3.Forward, texCoords[3]);
            vertices[11] = new VertexPositionNormalTexture(corners[5], Vector3.Forward, texCoords[0]);

            // +y face = 4510 tri1 = 451 tri2 = 104
            vertices[12] = new VertexPositionNormalTexture(corners[4], Vector3.Up, texCoords[0]);
            vertices[13] = new VertexPositionNormalTexture(corners[5], Vector3.Up, texCoords[1]);
            vertices[14] = new VertexPositionNormalTexture(corners[1], Vector3.Up, texCoords[2]);
            vertices[15] = new VertexPositionNormalTexture(corners[1], Vector3.Up, texCoords[2]);
            vertices[16] = new VertexPositionNormalTexture(corners[0], Vector3.Up, texCoords[3]);
            vertices[17] = new VertexPositionNormalTexture(corners[4], Vector3.Up, texCoords[0]);

            // -y face = 3267 tri1 = 326 tri2 = 673
            vertices[18] = new VertexPositionNormalTexture(corners[3], Vector3.Down, texCoords[0]);
            vertices[19] = new VertexPositionNormalTexture(corners[2], Vector3.Down, texCoords[1]);
            vertices[20] = new VertexPositionNormalTexture(corners[6], Vector3.Down, texCoords[2]);
            vertices[21] = new VertexPositionNormalTexture(corners[6], Vector3.Down, texCoords[2]);
            vertices[22] = new VertexPositionNormalTexture(corners[7], Vector3.Down, texCoords[3]);
            vertices[23] = new VertexPositionNormalTexture(corners[3], Vector3.Down, texCoords[0]);

            // +x face = 1562 tri1 = 156 tri2 = 621
            vertices[24] = new VertexPositionNormalTexture(corners[1], Vector3.Right, texCoords[0]);
            vertices[25] = new VertexPositionNormalTexture(corners[5], Vector3.Right, texCoords[1]);
            vertices[26] = new VertexPositionNormalTexture(corners[6], Vector3.Right, texCoords[2]);
            vertices[27] = new VertexPositionNormalTexture(corners[6], Vector3.Right, texCoords[2]);
            vertices[28] = new VertexPositionNormalTexture(corners[2], Vector3.Right, texCoords[3]);
            vertices[29] = new VertexPositionNormalTexture(corners[1], Vector3.Right, texCoords[0]);

            // -x face = 4037 tri1 = 403 tri2 = 374
            vertices[30] = new VertexPositionNormalTexture(corners[4], Vector3.Left, texCoords[0]);
            vertices[31] = new VertexPositionNormalTexture(corners[0], Vector3.Left, texCoords[1]);
            vertices[32] = new VertexPositionNormalTexture(corners[3], Vector3.Left, texCoords[2]);
            vertices[33] = new VertexPositionNormalTexture(corners[3], Vector3.Left, texCoords[2]);
            vertices[34] = new VertexPositionNormalTexture(corners[7], Vector3.Left, texCoords[3]);
            vertices[35] = new VertexPositionNormalTexture(corners[4], Vector3.Left, texCoords[0]);
            
            vertexDecl = new VertexDeclaration(graphics.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            vertexBuffer = new VertexBuffer(graphics.GraphicsDevice, VertexPositionNormalTexture.SizeInBytes * vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice); // Create a new SpriteBatch, which can be used to draw textures.
            colourMap = Content.Load<Texture2D>(@"Textures\color_map");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
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

            // Update the Effect
            Matrix world = Matrix.CreateRotationY(MathHelper.ToRadians(cubeRot.Y)) * Matrix.CreateRotationX(MathHelper.ToRadians(cubeRot.X)) * Matrix.CreateTranslation(cubePos);

            Matrix wvp = world * camera.View * camera.Projection;

            // Update the Phong Effect
            phongEffect.World = world;
            phongEffect.WorldViewProjection = wvp;
            phongEffect.Texture = colourMap;
            phongEffect.LightColour = Vector4.One;
            phongEffect.LightPosition = new Vector4(camera.Position, 1.0f);
            phongEffect.LightIntensity = 2.0f;
            phongEffect.Shader.CommitChanges();

            base.Update(gameTime);
        }

        private void DrawCube()
        {
            graphics.GraphicsDevice.VertexDeclaration = vertexDecl;
            graphics.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);

            phongEffect.Shader.Begin();
            foreach (EffectPass pass in phongEffect.Shader.CurrentTechnique.Passes)
            {
                pass.Begin();
                graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 12);
                pass.End();
            }
            phongEffect.Shader.End();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawCube();

            base.Draw(gameTime);
        }
    }
}
