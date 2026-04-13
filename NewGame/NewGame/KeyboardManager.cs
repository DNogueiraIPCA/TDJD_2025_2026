using System;
using System.Collections.Generic;  // Dictionary and List
using System.Linq;  // To use .Except() in the Update method (Collections' functions)
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace IPCA.Monogame
{
    /*
       * MonoGame
       * Keyboard.IsDown(Key)       => bool
       * Keyboard.IsUp(Key)         => bool
       * 
       * IPCA.KeyboardManager
       * Keyboard.IsKeyDown(Key)    => bool
       * Keyboard.IsKeyUp(Key)      => bool
       * Keyboard.IsGoingDown(Key)  => bool
       * Keyboard.IsGoingUp(Key)    => bool
    */

    public enum KeysState
    {
        Up,
        Down,
        GoingUp,
        GoingDown
    }

    public class KeyboardManager : GameComponent // Herda de GameComponent para ser um componente do jogo, e ser atualizado automaticamente a cada frame
    {
        // Variáveis de classe (suporte ao Singleton)
        private static KeyboardManager _instance; // Referencia à única instância de KeyboardManager
        // Variáveis de instância
        private Dictionary<Keys, KeysState> _keyboardState; // Guarda o estado atual de cada tecla que já foi pressionada durante o jogo. Se uma tecla nunca foi pressionada, não existe no dicionário.
        /*
                Keys.A => {
                             KeysState.GoingUp => [ Action1, Action2 ]
                             KeysState.Down => [ Action1, Action2 ]
                          }
                Keys.B => { 
                            KeysState.Up => [ Action1 ]
                            KeysState.GoingUp => [ Action2 ]
                          }
         */
        private Dictionary<Keys, Dictionary<KeysState, List<Action>>> _actions; // Guarda as ações a executar para cada par de tecla/estado. Se uma tecla nunca foi pressionada durante o jogo, não existe no dicionário. Se um estado nunca foi atingido para uma tecla, não existe no dicionário.

        public KeyboardManager(Game game) : base(game) // O construtor recebe o jogo para o qual este componente é criado, e passa esse jogo para a classe base (GameComponent)
        {
            // Validar que Singleton ainda não foi criado.
            if (_instance != null) throw new Exception("KeyboardManager constructor called twice");
            _instance = this; // guardar a única instância no singleton.

            _keyboardState = new Dictionary<Keys, KeysState>();
            _actions = new Dictionary<Keys, Dictionary<KeysState, List<Action>>>();
            
            game.Components.Add(this);   // "auto instalável"
        }

        // Action é uma referência a uma função void f(void)
        public static void Register(Keys key, KeysState state, Action code)
        {
            // Do we have this key already in the dictionary?
            if (!_instance._actions.ContainsKey(key))
                _instance._actions[key] = new Dictionary<KeysState, List<Action>>();

            // For this key, do we have that state created?
            if (!_instance._actions[key].ContainsKey(state))
                _instance._actions[key][state] = new List<Action>();

            // Add the code to the key/state pair
            _instance._actions[key][state].Add(code);
            // Add the key to the keyboard state dictionary
            _instance._keyboardState[key] = KeysState.Up;
        }

        // Static methods to query the state of a key
        public static bool IsKeyDown(Keys k) =>
            _instance._keyboardState.ContainsKey(k) && _instance._keyboardState[k] == KeysState.Down; // If we know about this key, and it is down, return true. Otherwise, return false.
        public static bool IsKeyUp(Keys k) =>
            _instance._keyboardState.ContainsKey(k) && _instance._keyboardState[k] == KeysState.Up;
        public static bool IsGoingDown(Keys k) =>
            _instance._keyboardState.ContainsKey(k) && _instance._keyboardState[k] == KeysState.GoingDown;
        public static bool IsGoingUp(Keys k) =>
            _instance._keyboardState.ContainsKey(k) && _instance._keyboardState[k] == KeysState.GoingUp;


        public override void Update(GameTime gameTime) // Este método é chamado automaticamente a cada frame, porque esta classe é um GameComponent
        {
            KeyboardState state = Keyboard.GetState(); // Get the current state of the keyboard
            List<Keys> pressedKeys = state.GetPressedKeys().ToList(); // Get the list of currently pressed keys

            // Processed pressed keys
            foreach (Keys key in pressedKeys)
            {
                // If we didn't know anything about this key, then probably it was up.
                if (!_keyboardState.ContainsKey(key)) _keyboardState[key] = KeysState.Up; // default state

                // What was the previous state, and decide what is our next state
                switch (_keyboardState[key]) // Previous state
                {
                    /*   Estado Anterior  Agora   Guardo
                     *      DOWN           DOWN    Down
                     *    GOING DOWN       DOWN    Down
                     *       UP            DOWN    Going Down
                     *    GOING UP         DOWN    Going Down
                     */
                    case KeysState.Down: // If it was already down, it is still down
                    case KeysState.GoingDown: // If it was going down, it is now down
                        _keyboardState[key] = KeysState.Down;
                        break;
                    case KeysState.Up: // If it was up, it is now going down
                    case KeysState.GoingUp: // If it was going up, it is now going down
                        _keyboardState[key] = KeysState.GoingDown;
                        break;
                }
            }
            // Faz uma copia da lista
            //   Keys[] x = _keyboardState.Keys.Except(pressedKeys).ToArray(); 
            //   foreach (Keys key in x)
            // same as...
            // Processed released keys
            foreach (Keys key in _keyboardState.Keys.Except(pressedKeys).ToArray()) // Todas as teclas já pressionadas durante o jogo e que continuam pressionadas
            {
                /*   Estado Anterior  Agora   Guardo
                 *      DOWN           UP      GoingUp
                 *    GOING DOWN       UP      GoingUp
                 *       UP            UP      UP
                 *    GOING UP         UP      UP
                 */
                switch (_keyboardState[key])
                {
                    case KeysState.Down:
                    case KeysState.GoingDown:
                        _keyboardState[key] = KeysState.GoingUp;
                        break;
                    case KeysState.Up:
                    case KeysState.GoingUp:
                        _keyboardState[key] = KeysState.Up;
                        break;
                }
            }

            // Invocar as funções registadas!
            foreach (Keys key in _actions.Keys)
            {
                KeysState kstate = _keyboardState[key];
                if (_actions[key].ContainsKey(kstate))
                {
                    foreach (Action action in _actions[key][kstate])
                    {
                        action();
                    }
                }
            }
        }
    }
}
