using MastersHall.Core;
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

        public InputManager inputManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            inputManager = new InputManager();
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

            inputManager.handleInput();

            playerPosition += inputManager.input * moveSpeed * dt;

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