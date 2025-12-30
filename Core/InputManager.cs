using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MastersHall.Core
{
    public class InputManager
    {
        public Vector2 input;
        public InputManager() {
        }

        public void handleKeyboard()
        {

            input = Vector2.Zero;
            
            // Keyboard (arrow keys + WASD)
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                input.X += 1f;
            }
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                input.X -= 1f;
            }
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                input.Y -= 1f;
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                input.Y += 1f;
            }
            normalizeForDiagonalMovement();
        }

        public void handleGamePad() {
            // Gamepad (left stick + DPad)
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            if (pad.IsConnected)
            {
                input += new Vector2(pad.ThumbSticks.Left.X, -pad.ThumbSticks.Left.Y);

                if (pad.DPad.Left == ButtonState.Pressed) input.X -= 1f;
                if (pad.DPad.Right == ButtonState.Pressed) input.X += 1f;
                if (pad.DPad.Up == ButtonState.Pressed) input.Y -= 1f;
                if (pad.DPad.Down == ButtonState.Pressed) input.Y += 1f;

                normalizeForDiagonalMovement();
            }
        }

        public void handleInput() {
            handleKeyboard();

            handleGamePad();
        }

        public void normalizeForDiagonalMovement() {
            // Normalize so diagonal movement isn't faster
            if (input != Vector2.Zero)
            {
                input.Normalize();
            }
        }
    }
}
