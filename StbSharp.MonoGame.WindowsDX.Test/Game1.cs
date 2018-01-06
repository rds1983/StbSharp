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

			var image = Stb.LoadFromMemory(buffer, Stb.STBI_rgb_alpha);
			texLoadedBySTB = new Texture2D(GraphicsDevice, image.Width, image.Height, false, SurfaceFormat.Color);
			texLoadedBySTB.SetData(image.Data);

			stbLoadTime = (DateTime.Now - now).TotalMilliseconds;

			spriteFont = Content.Load<SpriteFont>("DefaultFont");
			GC.Collect();
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

			spriteBatch.DrawString(spriteFont, string.Format("Sichem Allocated: {0}", Operations.AllocatedTotal),
				new Vector2(0, texLoadedByMG.Height + 30), Color.White);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}