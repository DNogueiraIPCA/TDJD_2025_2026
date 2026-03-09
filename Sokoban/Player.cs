using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Sokoban
{
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

        private Game1 game; //reference from Game1 to Player - para o Player ter acesso a variáveis e funções do Game1 (ex: tileSize, level, etc)

        // Function to initialize the player position
        //public Player( int x, int y)
        public Player(Game1 game1, int x, int y) //constructor que dada a as posições guarda a sua posição
        {
            position = new Point(x, y);
            game = game1;
        }

        // Function to update the player position based on keyboard input
        public void Update(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();
            if (keysReleased)
            {
                Point lastPosition = position; // Guarda a posição anterior do player para o caso de ter que voltar atrás (ex: se tentar mover-se para uma parede ou caixa que não pode ser movida)

                keysReleased = false;
                if ((kState.IsKeyDown(Keys.A)) || (kState.IsKeyDown(Keys.Left))) position.X--;
                else if ((kState.IsKeyDown(Keys.W)) || (kState.IsKeyDown(Keys.Up))) position.Y--;
                else if ((kState.IsKeyDown(Keys.S)) || (kState.IsKeyDown(Keys.Down))) position.Y++;
                else if ((kState.IsKeyDown(Keys.D)) || (kState.IsKeyDown(Keys.Right))) position.X++;
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
                        position = lastPosition;
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



    }
}
