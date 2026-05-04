using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Colisor
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SoundEffect _collisionSound;
        private SoundEffectInstance _collisionSoundInstance;

        private Song _song;
        private float _volume = 0.5f;

        private Texture2D _circleTexture;
        private Texture2D _rectangleTexture;
        private Vector2 _circlePosition;
        private Rectangle _rectangle1;
        private Rectangle _rectangle2;
        private Random _random;

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

            // TODO: use this.Content to load your game content here

            // Carrega Sons - Efeitos
            _collisionSound = Content.Load<SoundEffect>("collision"); // Carrega o som de colisão
            _collisionSoundInstance = _collisionSound.CreateInstance(); 

            // Carrega Sons - Musica
            _song = Content.Load<Song>("sound");
            MediaPlayer.Volume = 0.25f; //[0,1] - Define o volume da música
            MediaPlayer.Play(_song);

            // Carrega objetos
            _circleTexture = CreateCircleTexture(50, Color.Red); // Cria uma textura para o círculo
            _rectangleTexture = CreateRectangleTexture(100, 200, Color.Blue); // Cria uma textura para o retângulo
            _random = new Random();

            // Posição inicial do círculo
            _circlePosition = new Vector2(_random.Next(0, _graphics.PreferredBackBufferWidth - _circleTexture.Width),
                                                      _random.Next(0, _graphics.PreferredBackBufferHeight - _circleTexture.Height));

            // Define a posição inicial aleatória dos retângulos
            _rectangle1 = GenerateRandomRectangle();
            _rectangle2 = GenerateRandomRectangle();
            while (_rectangle2.Intersects(_rectangle1))
            {
                _rectangle2 = GenerateRandomRectangle();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouseState = Mouse.GetState();

            // Atualiza a posição do círculo com base na posição do rato
            _circlePosition = new Vector2(mouseState.X - _circleTexture.Width / 2, mouseState.Y - _circleTexture.Height / 2);

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
                //_collisionSound.Play();
                _collisionSoundInstance.Play();
            }

            // Controla o volume da musica
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                _volume += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                _volume -= 0.1f;
            }

            // Garante que o volume fique entre 0 e 1
            _volume = (float)Math.Clamp(_volume, 0.0, 1.0);

            // Modifica o volume da música
            MediaPlayer.Volume = _volume;

            // Verifica a colisão entre o círculo e os retângulos
            if (_circlePosition.X + _circleTexture.Width > _rectangle1.Left &&
                _circlePosition.X < _rectangle1.Right &&
                _circlePosition.Y + _circleTexture.Height > _rectangle1.Top &&
                _circlePosition.Y < _rectangle1.Bottom)
            {
                // Reproduz o som de colisão
                _collisionSoundInstance.Play();
            }

            if (_circlePosition.X + _circleTexture.Width > _rectangle2.Left &&
                _circlePosition.X < _rectangle2.Right &&
                _circlePosition.Y + _circleTexture.Height > _rectangle2.Top &&
                _circlePosition.Y < _rectangle2.Bottom)
            {
                // Reproduz o som de colisão
                _collisionSoundInstance.Play();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            // Desenha o círculo na posição atual
            _spriteBatch.Draw(_circleTexture, _circlePosition, Color.White);

            // Desenha os retângulos
            _spriteBatch.Draw(_rectangleTexture, _rectangle1, Color.White);
            _spriteBatch.Draw(_rectangleTexture, _rectangle2, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /* Cria uma textura circular de forma procedural
         * radius   ->  Raio do círculo em pixels
         * color    ->  Cor do círculo
         * returns  ->  Textura 2D contendo o círculo
         */
        private Texture2D CreateCircleTexture(int radius, Color color)
        {
            // Cria uma textura quadrada com lado = diâmetro do círculo (2 * raio) - Textura básica: quadrada e vazia
            Texture2D texture = new Texture2D(GraphicsDevice, radius * 2, radius * 2);

            // Array que armazenará a cor de cada pixel da textura
            Color[] colorData = new Color[(radius * 2) * (radius * 2)];

            // Calcula o raio ao quadrado (para otimizar, evitando raiz quadrada no loop)
            float radiusSquared = radius * radius;

            // Percorre todos os pixels da textura
            for (int x = 0; x < radius * 2; x++)
            {
                for (int y = 0; y < radius * 2; y++)
                {
                    // Calcula a distância do pixel atual ao centro do círculo
                    int dx = x - radius;
                    int dy = y - radius;

                    // Verifica se o pixel está dentro do círculo usando a equação: x² + y² ≤ r²
                    if (dx * dx + dy * dy <= radiusSquared)
                    {
                        // Pixel dentro do círculo: aplica a cor
                        colorData[x + y * (radius * 2)] = color;
                    }
                    else
                    {
                        // Pixel fora do círculo: torna transparente
                        colorData[x + y * (radius * 2)] = Color.Transparent;
                    }
                }
            }

            // Aplica os dados de cor à textura
            texture.SetData(colorData);

            return texture;
        }

        private Texture2D CreateRectangleTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, width, height);

            Color[] colorData = new Color[width * height];

            for (int i = 0; i < colorData.Length; i++)
            {
                colorData[i] = color;
            }

            texture.SetData(colorData);

            return texture;
        }

        private Rectangle GenerateRandomRectangle()
        {
            int width = _random.Next(50, 200); // Largura do retângulo
            int height = _random.Next(50, 200); // Altura do retângulo
            int x = _random.Next(0, _graphics.PreferredBackBufferWidth - width); // Posição X do retângulo
            int y = _random.Next(0, _graphics.PreferredBackBufferHeight - height); // Posição Y do retângulo

            return new Rectangle(x, y, width, height);
        }

    }
}
