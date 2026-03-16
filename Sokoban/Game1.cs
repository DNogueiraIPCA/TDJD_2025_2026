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
        
        private int nrLinhas = 0;
        private int nrColunas = 0;
        public char[,] level;
        //private char[,] level;

        //private Texture2D player, dot, box, wall; //Load images Texture 
        private Texture2D dot, box, wall; //Load images Texture 
        public int tileSize = 64; //potencias de 2 (operações binárias)
        private Player sokoban;

        public List<Point> boxes;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //_graphics.PreferredBackBufferWidth = nrColunas*tileSize; //define a largura da janela
            //_graphics.PreferredBackBufferHeight = nrLinhas*tileSize;
            //_graphics.ApplyChanges(); //define a altura da janela        
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            LoadLevel("level1.txt");
            _graphics.PreferredBackBufferHeight = tileSize * level.GetLength(1); //definição da altura
            _graphics.PreferredBackBufferWidth = tileSize * level.GetLength(0); //definição da largura
            _graphics.ApplyChanges(); //aplica a atualização da janela

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("FontSokoban"); //Use the name of sprite font file ('File')
            //player = Content.Load<Texture2D>("Character4"); //Retirado para a classe Player.cs
            dot = Content.Load<Texture2D>("EndPoint_Blue");
            box = Content.Load<Texture2D>("Crate_Brown");
            wall = Content.Load<Texture2D>("Wall_Brown");

            sokoban.LoadContents();
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            sokoban.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            
            //_spriteBatch.DrawString(font, "O texto que quiser", new Vector2(0, 40), Color.White);
            //_spriteBatch.DrawString(font, $"Numero de Linhas = {nrLinhas} -- Numero de Colunas = {nrColunas}", new Vector2(0, 0), Color.White);

            Rectangle position = new Rectangle(0, 0, tileSize, tileSize); //calculo do retangulo a depender do tileSize
            for (int x = 0; x < level.GetLength(0); x++)  //pega a primeira dimensão
            {
                for (int y = 0; y < level.GetLength(1); y++) //pega a segunda dimensão
                {
                    position.X = x * tileSize; // define o position
                    position.Y = y * tileSize; // define o position

                    switch (level[x, y])
                    {
                        //case 'Y':
                        //    _spriteBatch.Draw(player, position, Color.White);
                        //    break;
                        //case '#':
                        //    _spriteBatch.Draw(box, position, Color.White);
                        //    break;
                        case '.':
                            _spriteBatch.Draw(dot, position, Color.White);
                            break;
                        case 'X':
                            _spriteBatch.Draw(wall, position, Color.White);
                            break;
                    }
                }
                //position.X = sokoban.Position.X * tileSize; //posição do Player
                //position.Y = sokoban.Position.Y * tileSize; //posição do Player
                //_spriteBatch.Draw(player, position, Color.White); //desenha o Player
                
                sokoban.Draw(_spriteBatch); //desenha o Player usando a função Draw do Player.cs

                foreach (Point b in boxes)
                {
                    position.X = b.X * tileSize;
                    position.Y = b.Y * tileSize;
                    _spriteBatch.Draw(box, position, Color.White);
                }

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

    }
}
