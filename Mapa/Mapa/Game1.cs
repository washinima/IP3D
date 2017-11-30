using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Mapa
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Map mapa;
        Camera camera;
        private Collisions collisions;
        List<Tanque> tanques;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                //PreferredBackBufferWidth = 1920,
                //PreferredBackBufferHeight = 1080,
                //IsFullScreen = true
            };
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            camera = new Camera(Window);
            mapa = new Map(Content, GraphicsDevice, camera);
            tanques = new List<Tanque>
            {
                new Tanque(Content, camera, 2, new Vector3(100f, 4f, 100f)),
                new Tanque(Content, camera, 1, new Vector3(100f, 4f, 98f))
            };
            foreach (Tanque tanque in tanques)
                tanque.LoadMapNormalsPos(mapa.normalPosition);

            collisions = new Collisions(tanques);


            base.Initialize();
        }


        protected override void LoadContent()
        {

        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            camera.Update();
            foreach (Tanque tanque in tanques)
                tanque.Update();

            collisions.TankCollisionUpdate();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Constants.ScreenColor);

            mapa.Draw(GraphicsDevice);
            foreach (Tanque tanque in tanques)
                tanque.Draw(GraphicsDevice);


            base.Draw(gameTime);
        }
    }
}
