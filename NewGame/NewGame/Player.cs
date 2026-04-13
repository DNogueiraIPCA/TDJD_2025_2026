using System;
using IPCA.Monogame;
using Microsoft.Xna.Framework.Input;

namespace NewGame
{
    public class Player
    {
        //private KeyboardManager KeyboardManager { get; } = new KeyboardManager();
        public Player()
        {
            KeyboardManager.Register(Keys.Space, KeysState.GoingDown,
                () => Console.WriteLine("Space is Going Down!!!"));

            KeyboardManager.Register(Keys.Space, KeysState.Down,
                () => Console.WriteLine("Space is Down"));

            KeyboardManager.Register(Keys.Space, KeysState.GoingUp,
                () => Console.WriteLine("Space is going Up!"));
            
            //KeyboardManager.Register(Keys.Space, KeysState.Up,
            //    () => Console.WriteLine("Space is Up!"));

        }
    }
}