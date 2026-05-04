using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IPCA.Monogame;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;


namespace polygon
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private IPCA.Monogame.Debug _debug;
        private SoundEffect _collisionSound;
        private SoundEffectInstance _collisionSoundInstance;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _debug = new IPCA.Monogame.Debug(GraphicsDevice, _spriteBatch);
            // Carrega Sons - Efeitos
            _collisionSound = Content.Load<SoundEffect>("collision"); // Carrega o som de colisão
            //_collisionSoundInstance = _collisionSound.CreateInstance();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            // Executa o efeito sonoro
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _collisionSound.Play();
                //_collisionSoundInstance.Play();
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            
            _debug.DrawPixel(Vector2.One * 100, Color.DarkRed);
            _debug.DrawLine(new Vector2(0, 100), new Vector2(400, 100), Color.Red); // Horizontal Line
            _debug.DrawLine(new Vector2(50, 100), new Vector2(50, 400), Color.Yellow); // Vertical Line
            _debug.DrawLine(new Vector2(50, 100), new Vector2(100, 400), Color.GreenYellow); // Diagonal Vertical
            _debug.DrawLine(new Vector2(50, 100), new Vector2(400, 200), Color.White); // Diagonal Horizontal
            _debug.DrawRectangle(new Vector2(300, 300), new Vector2(400, 50), Color.DarkRed); 
            _debug.DrawCircle(new Vector2(400, 200), 100, Color.Magenta);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
