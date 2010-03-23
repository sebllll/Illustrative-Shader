using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MobRules.Helpers;
using MobRules.Interfaces;

namespace MobRules.GameComponents
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InputManager : Microsoft.Xna.Framework.GameComponent, IInputManagerService
    {
        private KeyboardState currentKBState, previousKBState;
        private MouseState currentMouseState, previousMouseState;
        private Point lastMouseLocation;
        private Vector2 mouseMoved;

        public KeyboardState CurrentKBState
        {
            get { return currentKBState; }
        }

        public Vector2 MouseMoved
        {
            get { return mouseMoved; }
        }

        public InputManager(Game game): base(game)
        {
            Enabled = true;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public override void Initialize()
        {
            currentKBState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
        }

        /// <summary>
        /// Allows the Input Manager to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            previousKBState = currentKBState;
            previousMouseState = currentMouseState;

            currentKBState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            int centreX = Game.GraphicsDevice.Viewport.X + Game.Window.ClientBounds.Width / 2, centreY = Game.GraphicsDevice.Viewport.X + Game.Window.ClientBounds.Height / 2;

            mouseMoved = new Vector2(currentMouseState.X - centreX, currentMouseState.Y - centreY);
            lastMouseLocation = new Point(currentMouseState.X, currentMouseState.Y);

            // Reset the mouse to the centre of the screen
            Mouse.SetPosition(centreX, centreY);

            base.Update(gameTime);
        }
    }
}