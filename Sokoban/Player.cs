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
        private Game1 game; //reference from Game1 to Player
        //public Player( int x, int y)
        public Player(Game1 game, int x, int y) //constructor que dada a as posições guarda a sua posição
        {
            position = new Point(x, y);
        }
        public void Update(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();
            if (keysReleased)
            {
                keysReleased = false;
                if (kState.IsKeyDown(Keys.A)) position.X--;
                else if (kState.IsKeyDown(Keys.W)) position.Y--;
                else if (kState.IsKeyDown(Keys.S)) position.Y++;
                else if (kState.IsKeyDown(Keys.D)) position.X++;
                else keysReleased = true;
            }
            else
            {
                if (kState.IsKeyUp(Keys.A) && kState.IsKeyUp(Keys.W) &&
                    kState.IsKeyUp(Keys.S) && kState.IsKeyUp(Keys.D))
                {
                    keysReleased = true;
                }
            }
        }



    }
}
