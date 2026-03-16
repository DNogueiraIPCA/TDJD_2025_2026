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

        private bool keysReleased = true;

        //public Vector2 Position
        //{
        //	get{return position;}
        //}

        private Texture2D[] sprites;
        //private Texture2D[][] sprites;

        //private int delta = 0;
        //private int speed = 2; // NOTE: must be tileSize divider

        private Direction direction = Direction.Down; // direção inicial do player (para escolher o sprite correto para desenhar o player)
        private Game1 game; //reference from Game1 to Player - para o Player ter acesso a variáveis e funções do Game1 (ex: tileSize, level, etc)

        // Function to initialize the player position
        //public Player( int x, int y)
        public Player(Game1 game1, int x, int y) //constructor que dada a as posições guarda a sua posição
        {
            position = new Point(x, y);
            game = game1;
        }

        public void LoadContents()
        {
            sprites = new Texture2D[4];//[];
            sprites[(int)Direction.Down] = game.Content.Load<Texture2D>("Character4");
            sprites[(int)Direction.Up] = game.Content.Load<Texture2D>("Character7");
            sprites[(int)Direction.Left] = game.Content.Load<Texture2D>("Character1");
            sprites[(int)Direction.Right] = game.Content.Load<Texture2D>("Character2");
            
            //sprites = new Texture2D[4][];
            //sprites[(int)Direction.Up] = new[] {
            //    game.Content.Load<Texture2D>("Character7"),
            //    game.Content.Load<Texture2D>("Character8"),
            //    game.Content.Load<Texture2D>("Character9")  };

            //sprites[(int)Direction.Down] = new[] {
            //game.Content.Load<Texture2D>("Character4"),
            //game.Content.Load<Texture2D>("Character5"),
            //game.Content.Load<Texture2D>("Character6") };

            //sprites[(int)Direction.Left] = new[] {
            //game.Content.Load<Texture2D>("Character1"),
            //game.Content.Load<Texture2D>("Character10") };

            //sprites[(int)Direction.Right] = new[] {
            //game.Content.Load<Texture2D>("Character2"),
            //game.Content.Load<Texture2D>("Character3") };

        }
        // Function to update the player position based on keyboard input
        public void Update(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();
            if (keysReleased)
            {
                Point lastPosition = position; // Guarda a posição anterior do player para o caso de ter que voltar atrás (ex: se tentar mover-se para uma parede ou caixa que não pode ser movida)

                keysReleased = false;
                if ((kState.IsKeyDown(Keys.A)) || (kState.IsKeyDown(Keys.Left)))
                {
                    position.X--;
                    direction = Direction.Left;
                }
                else if ((kState.IsKeyDown(Keys.W)) || (kState.IsKeyDown(Keys.Up)))
                {
                    position.Y--;
                    direction = Direction.Up;
                }
                else if ((kState.IsKeyDown(Keys.S)) || (kState.IsKeyDown(Keys.Down)))
                {
                    position.Y++;
                    direction = Direction.Down;
                }
                else if ((kState.IsKeyDown(Keys.D)) || (kState.IsKeyDown(Keys.Right)))
                {
                    position.X++;
                    direction = Direction.Right;
                }
                else keysReleased = true;

                // destino é caixa?
                if (game.HasBox(position.X, position.Y)) // se sim, calcular a posição para onde a caixa seria empurrada
                {
                    //     _ # Y 
                    // Y = x0 = 10
                    // # = x1 = 9
                    // delta = x1 - x0 = -1
                    // _ = x1 + delta
                    // diferença entre a posição atual e a posição anterior do player (ex: se moveu para a esquerda, deltaX = -1)
                    int deltaX = position.X - lastPosition.X;
                    int deltaY = position.Y - lastPosition.Y;

                    Point boxTarget = new Point(deltaX + position.X, deltaY + position.Y); // posição para onde a caixa seria empurrada
                    //  se sim, caixa pode mover-se?
                    if (game.FreeTile(boxTarget.X, boxTarget.Y)) // se a posição para onde a caixa seria empurrada é livre, move a caixa e o player
                    {
                        for (int i = 0; i < game.boxes.Count; i++) // atualizar a posição da caixa que foi movida (atualizar a posição da caixa na lista de caixas do Game1)
                        {
                            if (game.boxes[i].X == position.X && game.boxes[i].Y == position.Y) // encontrar a caixa que está na posição para onde o player se moveu (posição da caixa é a mesma que a posição do player)
                            {
                                game.boxes[i] = boxTarget; // atualizar a posição da caixa para a nova posição (posição para onde a caixa foi empurrada)
                            }
                        }
                    }
                    else
                    {
                        position = lastPosition; // se a caixa não pode ser movida (porque a posição para onde a caixa seria empurrada não é livre), o player volta para a posição anterior (fica parado)
                    }
                }
                else
                {
                    //  se não é caixa, se não está livre, parado!
                    if (!game.FreeTile(position.X, position.Y)) // se a posição para onde o player se moveu não é livre (ex: é uma parede), o player volta para a posição anterior (fica parado)
                                                                // position = lastPosition;
                    {
                        position = lastPosition;
                        //delta = 0;
                    }

                }
            }
            else
            {
                if (kState.IsKeyUp(Keys.A) && kState.IsKeyUp(Keys.W) &&
                    kState.IsKeyUp(Keys.S) && kState.IsKeyUp(Keys.D) &&
                    kState.IsKeyUp(Keys.Left) && kState.IsKeyUp(Keys.Up) &&
                    kState.IsKeyUp(Keys.Down) && kState.IsKeyUp(Keys.Right))
                {
                    keysReleased = true;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle rect = new Rectangle(game.tileSize * position.X,
                                           game.tileSize * position.Y,
                                           game.tileSize, game.tileSize);

            sb.Draw(sprites[(int)direction], rect, Color.White); //desenha o Player
        }


    }
}
