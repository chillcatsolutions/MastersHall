using MastersHall.Core;
using MastersHall.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


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

        // Defines the tilemap to draw.
        private Tilemap _tilemap;

        private FollowCamera camera;

        private SpriteFont _debugFont;
        public bool DebugMode { get; }

        public Game1(bool debugMode = false)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            inputManager = new InputManager();
            camera = new(Vector2.Zero);
            DebugMode = debugMode;
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

            // Compute initial player position so the character starts centered in the window.
            // We must account for the render scaling that maps the 1920x1080 render target to the back buffer.
            float initialScale = 1F / (1080F / _graphics.GraphicsDevice.Viewport.Height);
            Vector2 adjustedScreen = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight) / initialScale;
            float spriteW = character1.Width * characterScale;
            float spriteH = character1.Height * characterScale;
            playerPosition = new Vector2((adjustedScreen.X / 2f) - (spriteW / 2f), (adjustedScreen.Y / 2f) - (spriteH / 2f));

            // Initialize camera position so the player is centered immediately
            camera.Follow(new Rectangle((int)playerPosition.X, (int)playerPosition.Y, (int)spriteW, (int)spriteH), adjustedScreen);

            // Create the tilemap from the XML configuration file.
            _tilemap = Tilemap.FromFile(Content, "images/tilemap-definitions.xml");
            _tilemap.Scale = new Vector2(4.0f, 4.0f);

            // Load debug font (optional). If missing, we silently skip drawing debug text.
            try
            {
                _debugFont = Content.Load<SpriteFont>("Fonts/debugfont2");
            }
            catch
            {
                _debugFont = null;
            }
        }
            
        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Compute render scale based on viewport height so camera centering accounts for scaling
            scale = 1F / (1080F / _graphics.GraphicsDevice.Viewport.Height);

            inputManager.handleInput();

            playerPosition += inputManager.input * moveSpeed * dt;

            // Clamp to render target bounds (taking scaled sprite size into account)
            float spriteW = character1.Width * characterScale;
            float spriteH = character1.Height * characterScale;
            playerPosition.X = MathHelper.Clamp(playerPosition.X, 0f, _renderTarget.Width - spriteW);
            playerPosition.Y = MathHelper.Clamp(playerPosition.Y, 0f, _renderTarget.Height - spriteH);

            // Pass the screen size adjusted by inverse scale so camera centers correctly when render target is scaled
            Vector2 adjustedScreen = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight) / scale;
            camera.Follow(new Rectangle((int)playerPosition.X, (int)playerPosition.Y, (int)spriteW, (int)spriteH), adjustedScreen);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            scale = 1F / (1080F / _graphics.GraphicsDevice.Viewport.Height);
            GraphicsDevice.SetRenderTarget(_renderTarget);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Draw the tilemap.
            _tilemap.Draw(_spriteBatch);

            _spriteBatch.Draw(texture: character1, position: playerPosition, sourceRectangle: null, color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: characterScale, effects: SpriteEffects.None, layerDepth: 0f);


            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            // Draw the world render target shifted by the camera (camera.position is used as offset)
            _spriteBatch.Draw(_renderTarget, camera.position * scale, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            _spriteBatch.End();

            // Debug overlay in screen space (bottom-left)
            if (DebugMode && _debugFont != null)
            {
                // Compute tile coordinates from world position
                float tileWidth = _tilemap.TileWidth;
                float tileHeight = _tilemap.TileHeight;
                int tileCol = (int)(playerPosition.X / tileWidth);
                int tileRow = (int)(playerPosition.Y / tileHeight);

                string debugText = $"World X: {playerPosition.X:0.0}  Y: {playerPosition.Y:0.0}    Tile: {tileCol},{tileRow}";
                Vector2 textPos = new Vector2(8, _graphics.PreferredBackBufferHeight - 24);

                _spriteBatch.Begin();
                _spriteBatch.DrawString(_debugFont, debugText, textPos, Color.White);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
