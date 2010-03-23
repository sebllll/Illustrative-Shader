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

        // Diffuse Warping Colour and Specular Highlight Texture
        Texture2D warpTexture;

        // Test Ship
        Model frank, defFrank, ship, defShip;
        Vector3 frankPosition, frankForward;
        float frankAngle;
        Quaternion frankQ;

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

            // Setup Frank
            frankPosition = new Vector3(-4.0f, -10.0f, -25.0f);
            frankForward = Vector3.Backward;
            frankQ = Quaternion.Identity;
            frankAngle = 0.0f;

            // Start and add Game Services/Components
            inputManager = new InputManager(this);
            ServiceHelper.Add<IInputManagerService>(inputManager);
            this.Components.Add(inputManager);
            camera = new Camera(this);
            ServiceHelper.Add<ICameraService>(camera);
            this.Components.Add(camera);

            base.Initialize();
        }

        /// <summary>
        /// Alters a model so it will draw using a custom effect, while preserving
        /// whatever textures were set on it as part of the original effects.
        /// </summary>
        private void ChangeModelEffect(Model model, Effect replacementEffect, string specularMask, string specularExponent)
        {
            // Table mapping the original effects to our replacement versions.
            Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect>();

            foreach (ModelMesh mesh in model.Meshes)
            {
                // Scan over all the effects currently on the mesh.
                foreach (BasicEffect oldEffect in mesh.Effects)
                {
                    // If we haven't already seen this effect...
                    if (!effectMapping.ContainsKey(oldEffect))
                    {
                        // Make a clone of our replacement effect. We can't just use
                        // it directly, because the same effect might need to be
                        // applied several times to different parts of the model using
                        // a different texture each time, so we need a fresh copy each
                        // time we want to set a different texture into it.
                        Effect newEffect = replacementEffect.Clone(replacementEffect.GraphicsDevice);

                        // Copy across the texture from the original effect.
                        newEffect.Parameters["WarpTexture"].SetValue(warpTexture);
                        Texture2D specText = Content.Load<Texture2D>(@specularExponent);
                        Texture2D specMaskText = Content.Load<Texture2D>(@specularMask);
                        newEffect.Parameters["SpecTexture"].SetValue(specText);
                        newEffect.Parameters["SpecMaskTexture"].SetValue(specMaskText);
                        newEffect.Parameters["Texture"].SetValue(oldEffect.Texture);
                        
                        newEffect.Parameters["TextureEnabled"].SetValue(oldEffect.TextureEnabled);

                        effectMapping.Add(oldEffect, newEffect);
                    }
                }

                // Now that we've found all the effects in use on this mesh,
                // update it to use our new replacement versions.
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effectMapping[meshPart.Effect];
                }
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice); // Create a new SpriteBatch, which can be used to draw textures.

            // Load the warp colour (From mitchell p19)
            warpTexture = Content.Load<Texture2D>(@"Textures/warp");

            // A ship using the default BasicEffect
            defShip = Content.Load<Model>(@"Models/Ship2");
            defFrank = Content.Load<Model>(@"Models/Frank2");

            // Change the model to use the illustrative shader
            Effect illustrative = Content.Load<Effect>(@"Shaders/Illustrative");
            
            frank = Content.Load<Model>(@"Models/Frank");
            ChangeModelEffect(frank, illustrative, "Textures/BodySpecularMask", "Textures/BodySpecular");

            ship = Content.Load<Model>(@"Models/Ship");
            ChangeModelEffect(ship, illustrative, "Textures/ShipSpecularMask", "Textures/ShipSpecular");
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

            const float scale = 0.5f;

            Vector3 right = Vector3.Cross(frankForward, Vector3.Up);
            right.Normalize();

            // Move Frank forward and backward
            if (inputManager.CurrentKBState.IsKeyDown(Keys.I))
                frankPosition += frankForward * scale;
            else if (inputManager.CurrentKBState.IsKeyDown(Keys.K))
                frankPosition -= frankForward * scale;
            // Strafe Frank left and right
            if (inputManager.CurrentKBState.IsKeyDown(Keys.J))
                frankPosition -= right * (scale - 0.15f);
            else if (inputManager.CurrentKBState.IsKeyDown(Keys.L))
                frankPosition += right * (scale - 0.15f);
            // Move Frank up and down
            if (inputManager.CurrentKBState.IsKeyDown(Keys.Y))
                frankPosition += Vector3.Up * (scale - 0.15f);
            else if (inputManager.CurrentKBState.IsKeyDown(Keys.H))
                frankPosition -= Vector3.Up * (scale - 0.15f);
                         
            base.Update(gameTime);
        }

        void DrawModel(Model model, Matrix world)
        {
            // Look up the bone transform matrices.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model with default lighting
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    Matrix locWorld = transforms[mesh.ParentBone.Index] * world;
                    be.World = locWorld;
                    be.View = camera.View;
                    be.Projection = camera.Projection;
                }
                mesh.Draw();
            }
        }

        void DrawIllustrativeModel(Model model, Matrix world)
        {
            // Look up the bone transform matrices.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    Matrix locWorld = transforms[mesh.ParentBone.Index] * world;
                    effect.Parameters["World"].SetValue(locWorld);
                    effect.Parameters["View"].SetValue(camera.View);
                    effect.Parameters["Projection"].SetValue(camera.Projection);
                    effect.Parameters["Camera"].SetValue(camera.Position);
                    //effect.Parameters["Light"].SetValue(new Vector4(camera.Position, 1.0f));
                    effect.Parameters["Light"].SetValue(new Vector4(10.0f, 10.0f, 0.0f, 1.0f)); // Top right corner of screen
                    effect.Parameters["LightColour"].SetValue(new Vector4(0.7f, 0.5f, 0.5f, 1.0f));
                }
                mesh.Draw();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // Draw Frank
            Matrix world = Matrix.CreateRotationY(MathHelper.ToRadians(frankAngle)) * Matrix.CreateTranslation(frankPosition) * Matrix.CreateTranslation(4.5f, 5.0f, 10.0f);
            DrawIllustrativeModel(frank, world);
            
            // Draw Frank with default lighting
            world *= Matrix.CreateTranslation(new Vector3(5.0f, 0.0f, 0.0f));
            DrawModel(defFrank, world);

            // Draw the ship with default lighting
            world = Matrix.CreateScale(0.01f) * Matrix.CreateRotationY((float)Math.PI) * Matrix.CreateTranslation(frankPosition + new Vector3(0.0f, 5.0f, -20.0f));
            DrawModel(defShip, world);
            
            // Draw the ship
            world = Matrix.CreateScale(0.01f) * Matrix.CreateRotationY((float)Math.PI) * Matrix.CreateTranslation(frankPosition + new Vector3(0.0f, 5.0f, -20.0f)) * Matrix.CreateTranslation(new Vector3(25.0f, 0.0f, 0.0f));
            DrawIllustrativeModel(ship, world);

            base.Draw(gameTime);
        }
    }
}
