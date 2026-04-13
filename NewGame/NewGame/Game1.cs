using IPCA.Monogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Numerics;

namespace NewGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player _player;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this); // Gerencia a configuração gráfica do jogo (resolução, etc)
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            new KeyboardManager(this);  // Singleton
            KeyboardManager.Register(Keys.Escape, KeysState.GoingDown, Exit);
            // Register Recebe:
            //  - A tecla a "analisar"
            //  - O estado a considerar para essa tecla
            //  - Funcao a ser executada quando essa tecla atinge o estado indicado
            _player = new Player();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferHeight = 100;
            _graphics.PreferredBackBufferWidth = 100;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
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
