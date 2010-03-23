using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MobRules.Interfaces
{
    interface IInputManagerService
    {
        /// <summary>
        /// Get the current state of the keyboard
        /// </summary>
        KeyboardState CurrentKBState { get; }

        /// <summary>
        /// Get the change in mouse location between this update and the last update
        /// </summary>
        Vector2 MouseMoved { get; }

        void Update(GameTime gameTime);
    }
}
