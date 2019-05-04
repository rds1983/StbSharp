using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
		private Vorbis _vorbis;
		private DynamicSoundEffectInstance _effect;
		private bool _startedPlaying;
		private Texture2D _white, _fontTexture;
		private SpriteFont _font;

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
		
		private void LoadFont()
		{
			var buffer = File.ReadAllBytes("Fonts/DroidSans.ttf");
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

			var _charData = fontBaker.End();

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

			var glyphBounds = new List<Rectangle>();
			var cropping = new List<Rectangle>();
			var chars = new List<char>();
			var kerning = new List<Vector3>();

			var orderedKeys = _charData.Keys.OrderBy(a => a);
			foreach (var key in orderedKeys)
			{
				var character = _charData[key];

				var bounds = new Rectangle(character.x0, character.y0, 
										character.x1 - character.x0,
										character.y1 - character.y0);

				glyphBounds.Add(bounds);
				cropping.Add(new Rectangle((int)character.xoff, (int)character.yoff, bounds.Width, bounds.Height));

				chars.Add(key);

				kerning.Add(new Vector3(0, bounds.Width, character.xadvance - bounds.Width));
			}

			var constructorInfo = typeof(SpriteFont).GetTypeInfo().DeclaredConstructors.First();
			_font = (SpriteFont) constructorInfo.Invoke(new object[]
			{
				_fontTexture, glyphBounds, cropping,
				chars, 20, 0, kerning, ' '
			});
		}

		private void SubmitBuffer()
		{
			_vorbis.SubmitBuffer();

			if (_vorbis.Decoded == 0)
			{
				// Restart
				_vorbis.Restart();
				_vorbis.SubmitBuffer();
			}

			var audioShort = _vorbis.SongBuffer;
			byte[] audioData = new byte[_vorbis.Decoded * _vorbis.Channels * 2];
			for (var i = 0; i < _vorbis.Decoded * _vorbis.Channels; ++i)
			{
				if (i * 2 >= audioData.Length)
				{
					break;
				}

				var b1 = (byte)(audioShort[i] >> 8);
				var b2 = (byte)(audioShort[i] & 256);

				audioData[i * 2 + 0] = b2;
				audioData[i * 2 + 1] = b1;
			}

			_effect.SubmitBuffer(audioData);
		}

		private void LoadSong()
		{
			var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			path = Path.Combine(path, "Audio/music.ogg");
			var buffer = File.ReadAllBytes(path);

			_vorbis = Vorbis.FromMemory(buffer);

			_effect = new DynamicSoundEffectInstance(_vorbis.SampleRate, (AudioChannels)_vorbis.Channels)
			{
				Volume = 0.5f
			};

			_effect.BufferNeeded += (s, a) => SubmitBuffer();

			SubmitBuffer();
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
			LoadFont();

			// Load ogg
			LoadSong();

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


			_spriteBatch.DrawString(_font, "E: The quick brown fox jumps over the lazy dog",
				new Vector2(0, _image.Height + 30), Color.White);
			_spriteBatch.DrawString(_font, "G: Üben quält finſteren Jagdſchloß höfliche Bäcker größeren, N: Blåbærsyltetøy",
				new Vector2(0, _image.Height + 60), Color.White);
			_spriteBatch.DrawString(_font, "D: Høj bly gom vandt fræk sexquiz på wc, S: bäckasiner söka",
				new Vector2(0, _image.Height + 90), Color.White);
			_spriteBatch.DrawString(_font, "I: Sævör grét áðan því úlpan var ónýt, P: Pchnąć w tę łódź jeża lub osiem skrzyń fig",
				new Vector2(0, _image.Height + 120), Color.White);
			_spriteBatch.DrawString(_font, "C: Příliš žluťoučký kůň úpěl ďábelské kódy, R: В чащах юга жил-был цитрус? Да, но фальшивый экземпляр! ёъ.",
				new Vector2(0, _image.Height + 150), Color.White);
			_spriteBatch.DrawString(_font, "S: kilómetros y frío, añoraba, P: vôo à noite, F: Les naïfs ægithales hâtifs pondant à Noël où",
				new Vector2(0, _image.Height + 180), Color.White);
			_spriteBatch.DrawString(_font, "J: いろはにほへど",
				new Vector2(0, _image.Height + 210), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}