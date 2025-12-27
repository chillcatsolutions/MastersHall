using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace MastersHall
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private RenderTarget2D _renderTarget;

        public float scale = 1f;
        public float characterScale = 0.25f;

        private float moveSpeed = 400f;

        Texture2D character1;
        Texture2D background;

        Vector2 playerPosition;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            character1 = Content.Load<Texture2D>("chatgpt_character_1");
            background = Content.Load<Texture2D>("images/backgrounds/reference_background");

            _renderTarget = new RenderTarget2D(
                GraphicsDevice,
                1920,
                1080);

            
            
            playerPosition = new Vector2((_renderTarget.Width / 2) - (character1.Width * characterScale / 2), (_renderTarget.Height / 2) - (character1.Height * characterScale / 2));


        }

        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 input = Vector2.Zero;

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

            // Gamepad (left stick + DPad)
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            if (pad.IsConnected)
            {
                input += new Vector2(pad.ThumbSticks.Left.X, -pad.ThumbSticks.Left.Y);

                if (pad.DPad.Left == ButtonState.Pressed) input.X -= 1f;
                if (pad.DPad.Right == ButtonState.Pressed) input.X += 1f;
                if (pad.DPad.Up == ButtonState.Pressed) input.Y -= 1f;
                if (pad.DPad.Down == ButtonState.Pressed) input.Y += 1f;
            }

            // Normalize so diagonal movement isn't faster
            if (input != Vector2.Zero)
            {
                input.Normalize();
            }

            playerPosition += input * moveSpeed * dt;

            // Clamp to render target bounds (taking scaled sprite size into account)
            float spriteW = character1.Width * characterScale;
            float spriteH = character1.Height * characterScale;
            playerPosition.X = MathHelper.Clamp(playerPosition.X, 0f, _renderTarget.Width - spriteW);
            playerPosition.Y = MathHelper.Clamp(playerPosition.Y, 0f, _renderTarget.Height - spriteH);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            scale = 1F / (1080F / _graphics.GraphicsDevice.Viewport.Height);
            GraphicsDevice.SetRenderTarget(_renderTarget);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(background, Vector2.Zero, Color.White);
            _spriteBatch.Draw(texture: character1, position: playerPosition, sourceRectangle: null, color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: characterScale, effects: SpriteEffects.None, layerDepth: 0f);


            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_renderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            _spriteBatch.End();



            base.Draw(gameTime);
        }
    }
}