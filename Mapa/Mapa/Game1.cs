using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mapa
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Map mapa;
        Camera camera;
        Tanque tanque, tanque2;

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
            tanque = new Tanque(Content, GraphicsDevice, camera, 1);
            //tanque2 = new Tanque(Content, GraphicsDevice, camera, 2);
            tanque.LoadMapNormalsPos(mapa.normalPosition);
            //tanque2.LoadMapNormalsPos(mapa.normalPosition);

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
            tanque.Update();
            //tanque2.Update();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Constants.ScreenColor);

            mapa.Draw(GraphicsDevice);
            tanque.Draw();
            //tanque2.Draw();

            base.Draw(gameTime);
        }
    }
}
