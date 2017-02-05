using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StbSharp.MonoGame.WindowsDX.Test
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		private Texture2D texLoadedByMG;
		private Texture2D texLoadedBySTB;
		private SpriteFont spriteFont;
		private double mgLoadTime, stbLoadTime;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1280,
				PreferredBackBufferHeight = 800
			};

			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.AllowUserResizing = true;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
		}
		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			// Load image data into memory
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			path = Path.Combine(path, "image.jpg");
			var buffer = File.ReadAllBytes(path);

			// Loading through Texture2D.FromStream
			var now = DateTime.Now;
			using (var ms = new MemoryStream(buffer))
			{
				texLoadedByMG = Texture2D.FromStream(GraphicsDevice, ms);
			}
			mgLoadTime = (DateTime.Now - now).TotalMilliseconds;

			// Loading through StbSharp
			now = DateTime.Now;

			int x, y, comp;
			var data = Image.stbi_load_from_memory(buffer, out x, out y, out comp, Image.STBI_rgb_alpha);

			var colors = new Color[x*y];
			for (var i = 0; i < colors.Length; ++i)
			{
				colors[i].R = data[i*4];
				colors[i].G = data[i*4 + 1];
				colors[i].B = data[i*4 + 2];
				colors[i].A = data[i*4 + 3];
			}

			texLoadedBySTB = new Texture2D(GraphicsDevice, x, y, false, SurfaceFormat.Color);
			texLoadedBySTB.SetData(colors);

			stbLoadTime = (DateTime.Now - now).TotalMilliseconds;

			spriteFont = Content.Load<SpriteFont>("DefaultFont");
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
			    Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			spriteBatch.Begin();

			spriteBatch.Draw(texLoadedByMG, Vector2.Zero);
			spriteBatch.DrawString(spriteFont, string.Format("{0}", (int) mgLoadTime),
				new Vector2(0, texLoadedByMG.Height + 10), Color.White);

			var x = texLoadedByMG.Width + 10;
			spriteBatch.Draw(texLoadedBySTB, new Vector2(x, 0));
			spriteBatch.DrawString(spriteFont, string.Format("{0}", (int) stbLoadTime),
				new Vector2(x, texLoadedBySTB.Height + 10), Color.White);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}