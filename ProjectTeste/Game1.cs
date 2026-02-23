using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectTeste
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int largura = 800;
        private int altura = 600;

        private KeyboardState tecladoAnterior; // Variável para armazenar o estado do teclado no frame anterior

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = largura;
            _graphics.PreferredBackBufferHeight = altura;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState tecladoAtual = Keyboard.GetState();  // Variável local para o estado atual do teclado

            // Permite sair do jogo com o botão "Back" do controle ou a tecla "Escape"
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                tecladoAtual.IsKeyDown(Keys.Escape))
                Exit();
            
            // Aumentar tamanho (+)
            // OemPlus é a tecla '+' no teclado e Add é a tecla '+' no teclado numérico
            if (tecladoAtual.IsKeyDown(Keys.Add) && // Verifica se a tecla + do teclado numérico está pressionada neste frame.
                tecladoAnterior.IsKeyUp(Keys.Add))  // Verifica se a tecla + do teclado numérico estava solta no frame anterior.
                                                   // Garante que o aumento ocorra apenas uma vez por pressionamento.
            {
                largura += 100;
                altura += 75;

                _graphics.PreferredBackBufferWidth = largura;
                _graphics.PreferredBackBufferHeight = altura;
                _graphics.ApplyChanges();
            }

            // Diminuir tamanho (-)
            // OemMinus é a tecla '-' no teclado e Subtract é a tecla '-' no teclado numérico
            if (tecladoAtual.IsKeyDown(Keys.Subtract) && 
                tecladoAnterior.IsKeyUp(Keys.Subtract))
            {
                largura -= 100;
                altura -= 75;

                // Evita tamanho muito pequeno
                if (largura < 200) largura = 200;
                if (altura < 150) altura = 150;

                _graphics.PreferredBackBufferWidth = largura;
                _graphics.PreferredBackBufferHeight = altura;
                _graphics.ApplyChanges();
            }

            tecladoAnterior = tecladoAtual;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
