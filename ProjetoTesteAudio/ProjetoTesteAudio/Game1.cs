using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Media;

namespace ProjetoTesteAudio
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SoundEffect _collisionSound;
        //private SoundEffectInstance _collisionSoundInstance;
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
                /*
                 * _collisionSound.Play() 
                 * Método direto e simples
                 * Reproduz o som imediatamente
                 * Sem controle após iniciar
                 * Ideal para sons "fire-and-forget" (disparar e esquecer)
                 */
                _collisionSound.Play();
                //_collisionSoundInstance.Play();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
