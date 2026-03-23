using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace Sokoban
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont font;
        private SpriteFont arial14;

        private int nrLinhas = 0;
        private int nrColunas = 0;
        public char[,] level;
        //private char[,] level;
        private string[] levelNames = {"level2.txt" , "level1.txt"}; // Level list
        private int currentLevel = 0; // Current level
        
        private bool rDown = false; // if R is still pressed down (restart)
        private int liveCount = 3;
        
        private bool isWin = false; // if the player has won the game (passed all levels)

        private double levelTime = 0f; // timer to count how long the player takes to complete the level (in seconds)

        private Texture2D dot, box, wall; //Load images Texture 
        public int tileSize = 64; //potencias de 2 (operações binárias)
        private Player sokoban; //instancia da classe Player.cs

        public List<Point> boxes;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
           
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            LoadLevel(levelNames[currentLevel]);

            _graphics.PreferredBackBufferHeight = tileSize * (1+level.GetLength(1)); //definição da altura
            _graphics.PreferredBackBufferWidth = tileSize * level.GetLength(0); //definição da largura
            _graphics.ApplyChanges(); //aplica a atualização da janela

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("FontSokoban"); //Use the name of sprite font file ('File')
            arial14 = Content.Load<SpriteFont>("Arial14"); 
            
            dot = Content.Load<Texture2D>("EndPoint_Blue");
            box = Content.Load<Texture2D>("Crate_Brown");
            wall = Content.Load<Texture2D>("Wall_Brown");

            sokoban.LoadContents(); //chama a função para carregar o conteúdo do Player.cs
         
        }

        protected override void Update(GameTime gameTime)
        {
            // increment the timer according to the elapsed time between invocations to Update.
         
            if (!isWin) levelTime += gameTime.ElapsedGameTime.TotalSeconds; // only count time if the player hasn't won yet
          
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

          
            if (!rDown && Keyboard.GetState().IsKeyDown(Keys.R))
            {
                rDown = true;
                liveCount--;
                //if (liveCount < 0)
                if (isWin || liveCount < 0)
                {
                    // Reset level
                    currentLevel = 0;
                    levelTime = 0f;
                    liveCount = 3;
                    isWin = false;
                }
                Initialize(); // Game restart

            }
            else if (Keyboard.GetState().IsKeyUp(Keys.R))
            {
                rDown = false;
            }
            //if (Victory()) Exit(); // FIXME: Change current level
            if (Victory())
            {
                if (currentLevel < levelNames.Length - 1)
                {
                    currentLevel++;
                    Initialize();
                }
                else
                {
                
                    isWin = true;
                }
            }
         
            if (!isWin) sokoban.Update(gameTime); 

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) // equivalente ao Delta Time do Unity, ou seja, o tempo que demora a desenhar um frame
        {
          
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin();

            // Draw UI
            _spriteBatch.DrawString(arial14, // Tipo de letra
                                    $"Time: {levelTime:F0}", //string.Format("Time: {0:F2}", levelTime) // Texto
                                    new Vector2(5, level.GetLength(1) * tileSize + 10), // Posição do texto
                                    Color.White, // Cor da letra
                                    0f, //Rotação
                                    Vector2.Zero, // Origem
                                    2f, // Escala
                                    SpriteEffects.None, //FlipHorizontally, //Sprite effect
                                    0); // Ordenar sprites


            string lives = $"Lives: {liveCount}";
            Point measure = arial14.MeasureString(lives).ToPoint();
            int posX = level.GetLength(0) * tileSize - measure.X * 2 - 5;
            _spriteBatch.DrawString(arial14, // Tipo de Letra
                                    lives, // Texto
                                    new Vector2(posX, level.GetLength(1) * tileSize + 10), // Posição do texto
                                    Color.White, //Cor da Letra
                                    0f, //Rotação
                                    Vector2.Zero, // Origem
                                    2f, // Escala
                                    SpriteEffects.None, //FlipHorizontally, //Sprite effect
                                    0); // Ordenar sprites

            Rectangle position = new Rectangle(0, 0, tileSize, tileSize); //calculo do retangulo a depender do tileSize
            for (int x = 0; x < level.GetLength(0); x++)  //pega a primeira dimensão
            {
                for (int y = 0; y < level.GetLength(1); y++) //pega a segunda dimensão
                {
                    position.X = x * tileSize; // define o position
                    position.Y = y * tileSize; // define o position

                    switch (level[x, y])
                    {
                        case '.':
                            _spriteBatch.Draw(dot, position, Color.White);
                            break;
                        case 'X':
                            _spriteBatch.Draw(wall, position, Color.White);
                            break;
                    }
                }
                 
                sokoban.Draw(_spriteBatch); //desenha o Player usando a função Draw do Player.cs

                foreach (Point b in boxes)
                {
                    position.X = b.X * tileSize;
                    position.Y = b.Y * tileSize;
                    _spriteBatch.Draw(box, position, Color.White);
                }

            }
            // Draw win screen
            if (isWin)
            {
                // Get the window size
                Vector2 windowSize = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

                // Transparent Layer
                Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1); // create a 1x1 pixel texture
                pixel.SetData(new[] { Color.White }); // set the pixel color to white
                
                // Draw a semi-transparent green rectangle over the entire window
                _spriteBatch.Draw(pixel, // texture
                                 new Rectangle(Point.Zero, windowSize.ToPoint()), // destination rectangle
                                 new Color(Color.Green, 0.5f)); // color with 50% opacity

                // Draw Win Message
                string win = $"You took {levelTime:F1} seconds to Win!";
                Vector2 winMeasures = arial14.MeasureString(win) / 2f;
                Vector2 windowCenter = windowSize / 2f;
                Vector2 pos = windowCenter - winMeasures;
                _spriteBatch.DrawString(arial14, win, pos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        //Função para ler o ficheiro do nível e armazenar os dados numa matriz de caracteres
        void LoadLevel(string levelFile)
        {
         
            boxes = new List<Point>();

            string[] linhas = File.ReadAllLines($"Content/{levelFile}");  // "Content/" + level
            nrLinhas = linhas.Length;
            nrColunas = linhas[0].Length;

            level = new char[nrColunas, nrLinhas];

            for (int x = 0; x < nrColunas; x++)
            {
                for (int y = 0; y < nrLinhas; y++)
                {
                    if (linhas[y][x] == '#')
                    {
                        boxes.Add(new Point(x, y));
                        level[x, y] = ' '; // put a blank instead of the box '#'
                    }
                    else if (linhas[y][x] == 'Y')
                    {
                        sokoban = new Player(this, x, y);
                        level[x, y] = ' '; // put a blank instead of the sokoban 'Y'
                    }
                    else
                    {
                        level[x, y] = linhas[y][x];
                    }

                }
            }

        }
        //Função para verificar se existe uma caixa na posição dada
        public bool HasBox(int x, int y) // x e y é a posição do Player
        {
            foreach (Point b in boxes)
            {
                if (b.X == x && b.Y == y) return true; // se a caixa tiver a mesma posição do Player
            }
            return false;
        }

        //Função para verificar se a posição dada é um tile livre (sem parede e sem caixa)
        public bool FreeTile(int x, int y)
        {
            if (level[x, y] == 'X') return false;  // se for uma parede está ocupada
            if (HasBox(x, y)) return false; // verifica se é uma caixa
            return true;

            /* The same as:    return level[x,y] != 'X' && !HasBox(x,y);   */
        }

        // Função para verificar se o jogador venceu o nível (todas as caixas estão nos pontos)
        public bool Victory()
        {
            foreach (Point b in boxes) // pecorrer a lista das caixas
            {
                if (level[b.X, b.Y] != '.') return false; // verifica se há caixas sem pontos
            }
            return true;
        }

    }
}
