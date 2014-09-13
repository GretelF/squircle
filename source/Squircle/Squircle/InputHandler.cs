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
        KeyboardState _currentKeyboardState;
        KeyboardState _previousKeyboardState;

        GamePadState _currentGamepadState;
        GamePadState _previousGamepadState;

        public GamePadState GamePadState { get { return _currentGamepadState; } }

        public InputHandler()
        {
            _currentKeyboardState = Keyboard.GetState();
            _previousKeyboardState = _currentKeyboardState;

            _currentGamepadState = GamePad.GetState(PlayerIndex.One);
            _previousGamepadState = _currentGamepadState;
        }

        public bool IsUp(Keys key)
        {
            return _currentKeyboardState.IsKeyUp(key);
        }

        public bool IsUp(Buttons button)
        {
            return _currentGamepadState.IsConnected && _currentGamepadState.IsButtonUp(button);
        }

        public bool IsDown(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        public bool IsDown(Buttons button)
        {
            return _currentGamepadState.IsConnected && _currentGamepadState.IsButtonDown(button);
        }

        public bool WasTriggered(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }

        public bool WasTriggered(Buttons button)
        {
            return _currentGamepadState.IsConnected && _currentGamepadState.IsButtonDown(button) && _previousGamepadState.IsButtonUp(button);
        }

        public bool WasReleased(Keys key)
        {
            return _previousKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyUp(key);
        }

        public bool WasReleased(Buttons button)
        {
            return _currentGamepadState.IsConnected && _currentGamepadState.IsButtonUp(button) && _previousGamepadState.IsButtonDown(button);
        }

        public void Update(GameTime gameTime)
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _previousGamepadState = _currentGamepadState;
            _currentGamepadState = GamePad.GetState(PlayerIndex.One);
        }
    }
}
