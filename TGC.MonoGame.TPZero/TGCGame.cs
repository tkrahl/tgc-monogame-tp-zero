using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Content.Models;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Clase principal del juego.
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
        private float CarRotation { get; set; }
        private Vector3 CarPosition { get; set; }
        private Vector3 CarVelocity { get; set; }
        private FollowCamera FollowCamera { get; set; }
        private bool OnGround { get; set; }
        private static bool Compare(float a, float b)
        {
            return MathF.Abs(a - b) < float.Epsilon;
        }

        private const float RotationSpeed = 3f;
        private const float ForwardSpeed = 500f;
        private const float HorizontalAcceleration = 25000f;
        private const float Friction = 0.5f;
        private const float Gravity = 350f;
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        public TGCGame()
        {
            // Se encarga de la configuracion y administracion del Graphics Device.
            Graphics = new GraphicsDeviceManager(this);

            // Carpeta donde estan los recursos que vamos a usar.
            Content.RootDirectory = "Content";

            // Hace que el mouse sea visible.
            IsMouseVisible = true;
        }

        /// <summary>
        ///     Llamada una vez en la inicializacion de la aplicacion.
        ///     Escribir aca todo el codigo de inicializacion: Todo lo que debe estar precalculado para la aplicacion.
        /// </summary>
        protected override void Initialize()
        {
            // Enciendo Back-Face culling.
            // Configuro Blend State a Opaco.
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Opaque;

            // Configuro las dimensiones de la pantalla.
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            Graphics.ApplyChanges();

            // Creo una camaar para seguir a nuestro auto.
            FollowCamera = new FollowCamera(GraphicsDevice.Viewport.AspectRatio);

            // Configuro la matriz de mundo del auto.
            CarWorld = Matrix.Identity;
            OnGround = false;

            base.Initialize();
        }

        /// <summary>
        ///     Llamada una sola vez durante la inicializacion de la aplicacion, luego de Initialize, y una vez que fue configurado GraphicsDevice.
        ///     Debe ser usada para cargar los recursos y otros elementos del contenido.
        /// </summary>
        protected override void LoadContent()
        {
            // Creo la escena de la ciudad.
            City = new CityScene(Content);

            // La carga de contenido debe ser realizada aca.
            CarModel = Content.Load<Model>(ContentFolder3D + "scene/car");

            base.LoadContent();
        }

        /// <summary>
        ///     Es llamada N veces por segundo. Generalmente 60 veces pero puede ser configurado.
        ///     La logica general debe ser escrita aca, junto al procesamiento de mouse/teclas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 accel = Vector3.Zero;

            // Caputo el estado del teclado.
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                // Salgo del juego.
                Exit();

            if (keyboardState.IsKeyDown(Keys.A))
                CarRotation += elapsedTime * RotationSpeed;
            else if (keyboardState.IsKeyDown(Keys.D))
                CarRotation -= elapsedTime * RotationSpeed;

            if (keyboardState.IsKeyDown(Keys.W))
                //CarPosition += CarWorld.Forward * elapsedTime * ForwardSpeed; // Sin accel
                accel += CarWorld.Forward * HorizontalAcceleration; // con accel
            else if (keyboardState.IsKeyDown(Keys.S))
                //CarPosition -= CarWorld.Forward * elapsedTime * ForwardSpeed; // Sin accel
                accel -= CarWorld.Forward * HorizontalAcceleration; // Con accel

            if (keyboardState.IsKeyDown(Keys.Space) && (OnGround == true)) {
                CarVelocity += Vector3.Up * 300f;
                OnGround = false;
            }

            CarVelocity = new Vector3(CarVelocity.X * Friction, CarVelocity.Y - Gravity * elapsedTime, CarVelocity.Z * Friction);

            CarVelocity += accel * elapsedTime;
            CarPosition += CarVelocity * elapsedTime;
            var minimumFloor = MathHelper.Max(0f, CarPosition.Y);
            CarPosition = new Vector3(CarPosition.X, minimumFloor, CarPosition.Z);

            if (Compare(CarPosition.Y, 0.0f) && (OnGround == false)) {
                CarVelocity = new Vector3(CarVelocity.X, 0f, CarVelocity.Z);
                OnGround = true;
            }
                
            // La logica debe ir aca.
            CarWorld = Matrix.CreateRotationY(CarRotation) * Matrix.CreateTranslation(CarPosition);

            // Actualizo la camara, enviandole la matriz de mundo del auto.
            FollowCamera.Update(gameTime, CarWorld);

            base.Update(gameTime);
        }


        /// <summary>
        ///     Llamada para cada frame.
        ///     La logica de dibujo debe ir aca.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Limpio la pantalla.
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Dibujo la ciudad.
            City.Draw(gameTime, FollowCamera.View, FollowCamera.Projection);

            // El dibujo del auto debe ir aca.
            CarModel.Draw(CarWorld, FollowCamera.View, FollowCamera.Projection);

            base.Draw(gameTime);
        }

        /// <summary>
        ///     Libero los recursos cargados.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos cargados dessde Content Manager.
            Content.Unload();

            base.UnloadContent();
        }
    }
}