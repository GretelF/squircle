using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Squircle
{
    public class InputHandler
    {
        KeyboardState _currentState;
        KeyboardState _previousState;

        public InputHandler()
        {
            _currentState = Keyboard.GetState();
            _previousState = _currentState;
        }

        public bool IsUp(Keys key)
        {
            return _currentState.IsKeyUp(key);
        }

        public bool IsDown(Keys key)
        {
            return _currentState.IsKeyDown(key);
        }

        public bool WasTriggered(Keys key)
        {
            return _currentState.IsKeyDown(key) && _previousState.IsKeyUp(key);
        }

        public void Update(GameTime gameTime)
        {
            _previousState = _currentState;
            _currentState = Keyboard.GetState();
        }
    }
}
