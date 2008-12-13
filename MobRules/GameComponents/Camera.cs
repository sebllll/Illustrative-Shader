using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MobRules.Helpers;
using MobRules.Interfaces;

namespace MobRules.GameComponents
{
    /// <summary>
    /// This is the game camera class
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent, ICameraService
    {
        private Vector3 position, up, focus, focusConst, upConst;
        private Matrix view, projection;
        private Quaternion q;
        private float timeLapse; // Time since last update

        /// <summary>
        /// Get the Camera Position.
        /// </summary>
        public Vector3 Position { get { return position; } }
        
        /// <summary>
        /// Get the view matrix.
        /// </summary>
        public Matrix View { get { return view; } }

        /// <summary>
        /// Get the projection matrix.
        /// </summary>
        public Matrix Projection { get { return projection; } }

        public Camera(Game game) : base(game)
        {
            Enabled = true;
        }

        /// <summary>
        /// Initialize the camera and setup the projection matrix.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            position = Vector3.Zero;
            focus = Vector3.Forward;
            focusConst = focus;
            up = Vector3.Up;
            upConst = up;
            q = Quaternion.Identity;

            // Setup the projection matrix
            IGraphicsDeviceService graphicsService = ServiceHelper.Get<IGraphicsDeviceService>();
            float aspectRatio = (float) graphicsService.GraphicsDevice.Viewport.Width / (float)graphicsService.GraphicsDevice.Viewport.Height;
            float fov = MathHelper.PiOver4; // Default FOV of 90 degrees
            float nearPlane = 0.1f;
            float farPlane = 10000.0f;

            projection = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, nearPlane, farPlane);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            timeLapse = (float)gameTime.ElapsedGameTime.Milliseconds; // Get the time since the last update
            IInputManagerService inputManager = ServiceHelper.Get<IInputManagerService>();

            // Move the camera
            float move = 0.0f, strafe = 0.0f;
            if (inputManager.CurrentKBState.IsKeyDown(Keys.W))
                move += 1.0f;
            if (inputManager.CurrentKBState.IsKeyDown(Keys.S))
                move -= 1.0f;
            if (inputManager.CurrentKBState.IsKeyDown(Keys.A))
                strafe -= 1.0f;
            if (inputManager.CurrentKBState.IsKeyDown(Keys.D))
                strafe += 1.0f;
            
            // Rotate the camera
            Vector2 mouseMoved = inputManager.MouseMoved;
            float xScale = timeLapse / 100.0f, yScale = timeLapse / 400.0f;
            mouseMoved.X /= 100.0f;
            mouseMoved.Y /= 400.0f;
            Move(move);
            Strafe(strafe);
            RotateCamera(mouseMoved);
            view = Matrix.CreateLookAt(position, position + focus, up); // Update the view matrix
            base.Update(gameTime);
        }

        /// <summary>
        /// Moves the Camera forward and backward.
        /// </summary>
        /// <param name="camSpeed">Speed of the camera</param>
        private void Move(float camSpeed)
        {
            const float scale = 0.05f;
            camSpeed *= scale;
            Vector3 xz = focus;
            xz.Y = 0.0f;
            xz.Normalize();
            position += xz * (camSpeed * timeLapse);
        }

        /// <summary>
        /// Strafes the Camera left and right.
        /// </summary>
        /// <param name="camSpeed">Speed of the camera</param>
        private void Strafe(float camSpeed)
        {
            const float scale = 0.045f;
            camSpeed *= scale;
            Vector3 right = Vector3.Cross(focus, up);
            right.Normalize();
            position += right * (camSpeed * timeLapse);
        }

        private void Rotate(Quaternion qrot)
        {
            q = Quaternion.Multiply(qrot, q);
            q.Normalize();

            focus = Vector3.Transform(focusConst, q);
            up = Vector3.Transform(upConst, q);
        }

        private void RotateCamera(Vector2 control)
        {
            if (control == Vector2.Zero)
                return;
            
            float yRot = control.X * timeLapse / 150.0f;
            float xRot = control.Y * timeLapse / 50.0f;

            Vector3 right = Vector3.Cross(focus, up);
            right.Normalize();

            if (xRot != 0.0f)
                Rotate(Quaternion.CreateFromAxisAngle(right, -xRot));
            if (yRot != 0.0f)
                Rotate(Quaternion.CreateFromAxisAngle(upConst, -yRot));
        }
    }
}