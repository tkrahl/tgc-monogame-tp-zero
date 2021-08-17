using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Content.Models;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Main class of the Game.
    /// </summary>
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";
        
        private GraphicsDeviceManager Graphics { get; }
        private CityScene City { get; set; }
        private Model CarModel { get; set; }
        private Matrix CarWorld { get; set; }
        private FollowCamera FollowCamera { get; set; }


        /// <summary>
        ///     Game constructor
        /// </summary>
        public TGCGame()
        {
            // Handles the config and administration of the Graphics Device
            Graphics = new GraphicsDeviceManager(this);

            // Root folder where resources are located
            Content.RootDirectory = "Content";

            // Makes the mouse visible
            IsMouseVisible = true;
        }

        /// <summary>
        ///     Called once at the initialization of the application.
        ///     Write here the code about initialization: everything that can be pre-calculated for the application.
        /// </summary>
        protected override void Initialize()
        {
            // Turn on Back-Face culling
            // Also set Blend State to opaque
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Opaque;

            // Set the window size
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            Graphics.ApplyChanges();

            // Create a camera to follow our car
            FollowCamera = new FollowCamera(GraphicsDevice.Viewport.AspectRatio);

            // Configure the Car World matrix
            CarWorld = Matrix.Identity;

            base.Initialize();
        }

        /// <summary>
        ///     Called once at the initialization of the application, after Initialize, and once the GraphicsDevice is set.
        ///     Should be used for resource and content loading.
        /// </summary>
        protected override void LoadContent()
        {
            // Create the City Scene
            City = new CityScene(Content);

            // Content loading should be done here

            base.LoadContent();
        }

        /// <summary>
        ///     Called N times each second. Generally 60 times, but can be configured.
        ///     The main logic should be written here, including processing inputs.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Capture the keyboard state
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                // Exit the game
                Exit();

            // Update logic should go here

            // Update the camera, passing the World matrix of the car
            FollowCamera.Update(gameTime, CarWorld);

            base.Update(gameTime);
        }


        /// <summary>
        ///     Called every frame.
        ///     Draw logic should be written here.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw the city
            City.Draw(gameTime, FollowCamera.View, FollowCamera.Projection);

            base.Draw(gameTime);
        }

        /// <summary>
        ///     Free the loaded resources.
        /// </summary>
        protected override void UnloadContent()
        {
            // Free the resources loaded by the Content Manager
            Content.Unload();

            base.UnloadContent();
        }
    }
}