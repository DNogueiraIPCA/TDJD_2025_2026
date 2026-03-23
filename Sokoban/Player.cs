using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Sokoban
{
    enum Direction
    {
        Down, Up, Left, Right // 0, 1, 2, 3
    }

    internal class Player
    {

        // Current player position in the matrix (multiply by tileSize prior to drawing)

        private Point position; //Point = Vector2, mas são inteiros
        public Point Position => position; //auto função (equivalente a ter só get sem put) - AUTOPROPERTY

        /*
         public Vector2 Position
        {
        	get{return position;}
        } */


        private Texture2D[][] sprites;

        private int delta = 0;
        private int speed = 2; // NOTE: must be tileSize divider

        private Direction direction = Direction.Down; // direção inicial do player (para escolher o sprite correto para desenhar o player)
        private Vector2 directionVector; // vetor que representa a direção do movimento do player (ex: (-1,0) para esquerda, (0,-1) para cima, etc) - usado para calcular a posição do player durante o movimento suave (delta > 0)

        private Game1 game; //reference from Game1 to Player - para o Player ter acesso a variáveis e funções do Game1 (ex: tileSize, level, etc)

        // Function to initialize the player position
        
        public Player(Game1 game1, int x, int y) //constructor que dada a as posições guarda a sua posição
        {
            position = new Point(x, y);
            game = game1;
        }

        public void LoadContents()
        {
             
            sprites = new Texture2D[4][];
            sprites[(int)Direction.Up] = new[] {
                game.Content.Load<Texture2D>("Character7"),
                game.Content.Load<Texture2D>("Character8"),
                game.Content.Load<Texture2D>("Character9")  };

            sprites[(int)Direction.Down] = new[] {
                game.Content.Load<Texture2D>("Character4"),
                game.Content.Load<Texture2D>("Character5"),
                game.Content.Load<Texture2D>("Character6") };

            sprites[(int)Direction.Left] = new[] {
                game.Content.Load<Texture2D>("Character1"),
                game.Content.Load<Texture2D>("Character10") };

            sprites[(int)Direction.Right] = new[] {
                game.Content.Load<Texture2D>("Character2"),
                game.Content.Load<Texture2D>("Character3") };

        }
        // Function to update the player position based on keyboard input

        public void Update(GameTime gameTime)
        {
            if (delta > 0)
            {
                delta = (delta + speed) % game.tileSize; // incrementa delta e faz o reset para 0 quando atinge tileSize (ex: 0, 2, 4, 6, 8, 10, 12, 14, 0, 2, ...)
                //System.Diagnostics.Debug.WriteLine(delta); // imprime o valor de delta no console para debug (ver como delta varia de 0 a tileSize durante o movimento suave)

            }
            else
            {
                KeyboardState kState = Keyboard.GetState(); // lê o estado do teclado (quais teclas estão pressionadas)
                Point lastPosition = position; // guarda a posição atual do player antes de tentar mover (usado para desfazer o movimento caso o destino não seja válido)

                if ((kState.IsKeyDown(Keys.A)) || (kState.IsKeyDown(Keys.Left)))  // se a tecla A ou seta para esquerda está pressionada
                {
                    position.X--; // atualiza a posição do player (movimento para a esquerda = decrementa X)
                    direction = Direction.Left; // atualiza a direção do player (usado para escolher o sprite correto para desenhar)
                    delta = speed; // inicia o movimento suave (delta > 0) - o player começará a se mover e continuará a se mover até que delta volte a ser 0 (quando atingir a próxima tile)
                    directionVector = -Vector2.UnitX; // vetor unitário para a esquerda (-1, 0) - usado para calcular a posição do player durante o movimento suave (delta > 0)
                                                      // equivale a directionVector = new Vector2(-1, 0);
                }
                else if ((kState.IsKeyDown(Keys.W)) || (kState.IsKeyDown(Keys.Up))) // se a tecla W ou seta para cima está pressionada
                {
                    position.Y--; // atualiza a posição do player (movimento para cima = decrementa Y)
                    direction = Direction.Up;
                    delta = speed;
                    directionVector = -Vector2.UnitY; // vetor unitário para cima (0, -1) - usado para calcular a posição do player durante o movimento suave (delta > 0)
                }
                else if ((kState.IsKeyDown(Keys.S)) || (kState.IsKeyDown(Keys.Down))) // se a tecla S ou seta para baixo está pressionada
                {
                    position.Y++; // atualiza a posição do player (movimento para baixo = incrementa Y)
                    direction = Direction.Down;
                    delta = speed;
                    directionVector = Vector2.UnitY;
                }
                else if ((kState.IsKeyDown(Keys.D)) || (kState.IsKeyDown(Keys.Right))) // se a tecla D ou seta para direita está pressionada
                {
                    position.X++; // atualiza a posição do player (movimento para direita = incrementa X)
                    direction = Direction.Right;
                    delta = speed;
                    directionVector = Vector2.UnitX;
                }
                
                // destino é caixa?
                if (game.HasBox(position.X, position.Y))
                {
                    int deltaX = position.X - lastPosition.X;
                    int deltaY = position.Y - lastPosition.Y;
                    Point boxTarget = new Point(deltaX + position.X, deltaY + position.Y);
                    //  se sim, caixa pode mover-se?
                    if (game.FreeTile(boxTarget.X, boxTarget.Y))
                    {
                        for (int i = 0; i < game.boxes.Count; i++)
                            if (game.boxes[i].X == position.X && game.boxes[i].Y == position.Y)
                                game.boxes[i] = boxTarget;
                    }
                    else
                    {
                        position = lastPosition;
                        delta = 0;
                    }
                }
                else
                {
                    //  se não é caixa, se não está livre, parado!
                    if (!game.FreeTile(position.X, position.Y))
                    {
                        delta = 0;
                        position = lastPosition;
                    }
                }

            }
        }

        public void Draw(SpriteBatch sb)
        {
         
            /* -------------------
            Point(1,1) => Vector(1,1) => Vector(64,64) => Vector(64,64) + delta * Vector(1,0)
            Vector(64 + delta, 64)
            0,0 => 1,0 (64,0)
            64 - (64 - 1) ==> 64 - 63 ==> 1
            64 - (64 - 2) ==> 64 - 62 ==> 2
            ------------------- */
            
            Vector2 pos = position.ToVector2() * game.tileSize; // converte a posição do player (em tiles) para posição em pixels (multiplicando por tileSize) - usado para desenhar o player na posição correta na tela
            int frame = 0;
            if (delta > 0)
            {
                pos -= (game.tileSize - delta) * directionVector;
                float animSpeed = 8f;  // frames por segundo da animação de movimento 
                frame = (int)((delta / speed) % ((int)animSpeed * sprites[(int)direction].Length) / animSpeed);
            }

            /* Rectangle rect = new Rectangle( (int) pos.X, (int) pos.Y, Game1.tileSize, Game1.tileSize); */
            Rectangle rect = new Rectangle(pos.ToPoint(), new Point(game.tileSize));
            sb.Draw(sprites[(int)direction][frame], rect, Color.White);
        }


    }
}
