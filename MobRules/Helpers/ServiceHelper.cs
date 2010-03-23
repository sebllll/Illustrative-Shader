using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MobRules.Helpers
{
    /// <summary>
    /// Helper to add and get Services from the game.
    /// From: http://www.zaknafein.hjcrusaders.com/?p=23
    /// </summary>
    static class ServiceHelper
    {
        static Game game;

        /// <summary>
        /// Add a service to the Game.
        /// </summary>
        /// <typeparam name="T">Service Type</typeparam>
        /// <param name="service">Service</param>
        public static void Add<T>(T service) where T : class
        {
            game.Services.AddService(typeof(T), service);
        }

        /// <summary>
        /// Get a service
        /// </summary>
        /// <typeparam name="T">Service Type</typeparam>
        /// <returns>The service if found</returns>
        public static T Get<T>() where T : class
        {
            return game.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Set the game instance
        /// </summary>
        public static Game Game
        {
            set { game = value; }
        }

        /// <summary>
        /// Get the ContentManager
        /// </summary>
        public static ContentManager Content
        {
            get { return game.Content; }
        }

        /// <summary>
        /// Get the GraphicsDevice.
        /// </summary>
        public static GraphicsDevice Device
        {
            get { return game.GraphicsDevice; }
        }
    }
}
