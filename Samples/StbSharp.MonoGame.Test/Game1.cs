using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		private const int FontBitmapWidth = 1024;
		private const int FontBitmapHeight = 1024;

		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;

		private Texture2D _image;
		private Texture2D _fontTexture;
		private DynamicSoundEffectInstance _effect;
		private bool _startedPlaying;
		private Dictionary<char, StbTrueType.stbtt_packedchar> _charData = new Dictionary<char, StbTrueType.stbtt_packedchar>();
		private Texture2D _white;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1400,
				PreferredBackBufferHeight = 960
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
			// Create white texture
			_white = new Texture2D(GraphicsDevice, 1, 1);
			_white.SetData(new[] {Color.White});
			
			// Load image data into memory
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			path = Path.Combine(path, "image.jpg");
			var buffer = File.ReadAllBytes(path);

			var image = StbImage.LoadFromMemory(buffer, StbImage.STBI_rgb_alpha);
			_image = new Texture2D(GraphicsDevice, image.Width, image.Height, false, SurfaceFormat.Color);
			_image.SetData(image.Data);

			// Load ttf
			buffer = File.ReadAllBytes("Fonts/DroidSans.ttf");
			var buffer2 = File.ReadAllBytes("Fonts/DroidSansJapanese.ttf");
			
			var tempBitmap = new byte[FontBitmapWidth * FontBitmapHeight];

			var fontBaker = new FontBaker();
			
			fontBaker.Begin(tempBitmap, FontBitmapWidth, FontBitmapHeight);
			fontBaker.Add(buffer, 32, new []
			{
				FontBakerCharacterRange.BasicLatin,
				FontBakerCharacterRange.Latin1Supplement,
				FontBakerCharacterRange.LatinExtendedA,
				FontBakerCharacterRange.Cyrillic,
			});
			
			fontBaker.Add(buffer2, 32, new []
			{
				FontBakerCharacterRange.Hiragana,
				FontBakerCharacterRange.Katakana
			});

			_charData = fontBaker.End();

			// Offset by minimal offset
			float minimumOffsetY = 10000;
			foreach (var pair in _charData)
			{
				if (pair.Value.yoff < minimumOffsetY)
				{
					minimumOffsetY = pair.Value.yoff;
				}
			}

			var keys = _charData.Keys.ToArray();
			foreach (var key in keys)
			{
				var pc = _charData[key];
				pc.yoff -= minimumOffsetY;
				_charData[key] = pc;
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
				StbTrueType.stbtt_packedchar cd;
				var pos = location;
				
				if (!_charData.TryGetValue(c, out cd))
				{
					// Draw red rectangle
					batch.Draw(_white,
						new Rectangle((int)pos.X + 2, (int)pos.Y - 20, 20, 20),
						new Rectangle(0, 0, 1, 1),
						Color.Red);

					location.X += 24;
					continue;
				}

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

			DrawTTFString(_spriteBatch, "E: The quick brown fox jumps over the lazy dog",
				new Vector2(0, _image.Height + 30), Color.White);
			DrawTTFString(_spriteBatch, "G: Üben quält finſteren Jagdſchloß höfliche Bäcker größeren, N: Blåbærsyltetøy",
				new Vector2(0, _image.Height + 60), Color.White);
			DrawTTFString(_spriteBatch, "D: Høj bly gom vandt fræk sexquiz på wc, S: bäckasiner söka",
				new Vector2(0, _image.Height + 90), Color.White);
			DrawTTFString(_spriteBatch, "I: Sævör grét áðan því úlpan var ónýt, P: Pchnąć w tę łódź jeża lub osiem skrzyń fig",
				new Vector2(0, _image.Height + 120), Color.White);
			DrawTTFString(_spriteBatch, "C: Příliš žluťoučký kůň úpěl ďábelské kódy, R: В чащах юга жил-был цитрус? Да, но фальшивый экземпляр! ёъ.",
				new Vector2(0, _image.Height + 150), Color.White);
			DrawTTFString(_spriteBatch, "S: kilómetros y frío, añoraba, P: vôo à noite, F: Les naïfs ægithales hâtifs pondant à Noël où",
				new Vector2(0, _image.Height + 180), Color.White);
			DrawTTFString(_spriteBatch, "J: いろはにほへど",
				new Vector2(0, _image.Height + 210), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}