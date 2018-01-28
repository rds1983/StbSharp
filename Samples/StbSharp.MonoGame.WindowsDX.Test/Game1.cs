using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StbSharp.MonoGame.WindowsDX.Test
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private const int FontBitmapWidth = 512;
		private const int FontBitmapHeight = 512;

		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;

		private Texture2D _image;
		private Texture2D _fontTexture;
		private DynamicSoundEffectInstance _effect;
		private bool _startedPlaying;
		private readonly Dictionary<char, StbTrueType.stbtt_bakedchar> _charData = new Dictionary<char, StbTrueType.stbtt_bakedchar>();

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this)
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
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			// Load image data into memory
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			path = Path.Combine(path, "image.jpg");
			var buffer = File.ReadAllBytes(path);

			var image = StbImage.LoadFromMemory(buffer, StbImage.STBI_rgb_alpha);
			_image = new Texture2D(GraphicsDevice, image.Width, image.Height, false, SurfaceFormat.Color);
			_image.SetData(image.Data);

			// Load ttf
			buffer = File.ReadAllBytes("OpenSans/OpenSans-Regular.ttf");
			var tempBitmap = new byte[FontBitmapWidth * FontBitmapHeight];
			var charData = new StbTrueType.stbtt_bakedchar[256];

			StbTrueType.stbtt_BakeFontBitmap(buffer, 0, 48, tempBitmap, FontBitmapWidth, FontBitmapHeight, 32, 96, charData);

			var c = 32;
			foreach (var cd in charData)
			{
				_charData[(char)c] = cd;
				++c;
			}

			var rgb = new Color[FontBitmapWidth * FontBitmapHeight];
			for (var i = 0; i < tempBitmap.Length; ++i)
			{
				var b = tempBitmap[i];
				rgb[i].R = b;
				rgb[i].G = b;
				rgb[i].B = b;
				
				rgb[i].A = b;
			}

			_fontTexture = new Texture2D(GraphicsDevice, FontBitmapWidth, FontBitmapHeight);
			_fontTexture.SetData(rgb);

			// Load ogg
			path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			path = Path.Combine(path, "Adeste_Fideles.ogg");
			buffer = File.ReadAllBytes(path);

			int chan, sampleRate;
			var audioShort = StbVorbis.decode_vorbis_from_memory(buffer, out sampleRate, out chan);

			byte[] audioData = new byte[audioShort.Length / 2 * 4];
			for (var i = 0; i < audioShort.Length; ++i)
			{
				if (i*2 >= audioData.Length)
				{
					break;
				}

				var b1 = (byte) (audioShort[i] >> 8);
				var b2 = (byte) (audioShort[i] & 256);

				audioData[i*2 + 0] = b2;
				audioData[i*2 + 1] = b1;
			}

			_effect = new DynamicSoundEffectInstance(sampleRate, AudioChannels.Stereo)
			{
				Volume = 0.5f
			};


			_effect.SubmitBuffer(audioData);

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
			if (!_startedPlaying)
			{
				_effect.Play();
				_startedPlaying = true;
			}

			base.Update(gameTime);
		}

		private void DrawTTFString(SpriteBatch batch, string str, Vector2 location, Color color)
		{
			if (string.IsNullOrEmpty(str))
			{
				return;
			}

			for (var i = 0; i < str.Length; ++i)
			{
				var c = str[i];
				StbTrueType.stbtt_bakedchar cd;
				if (!_charData.TryGetValue(c, out cd))
				{
					continue;
				}

				var pos = location;
				pos.X += cd.xoff;
				pos.Y += cd.yoff;

				batch.Draw(_fontTexture, pos, 
					new Rectangle(cd.x0, cd.y0, cd.x1 - cd.x0, cd.y1 - cd.y0),
					color);

				location.X += cd.xadvance;
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			_spriteBatch.Draw(_image, new Vector2(0, 0));
			_spriteBatch.Draw(_fontTexture, new Vector2(_image.Width + 10, 0));

			DrawTTFString(_spriteBatch, string.Format("Sichem Allocated: {0}", Pointer.AllocatedTotal),
				new Vector2(0, _image.Height + 30), Color.White);
			DrawTTFString(_spriteBatch, "Hello, World!",
				new Vector2(0, _image.Height + 60), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}