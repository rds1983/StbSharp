using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace StbSharp.WinForms.Test
{
	public partial class Form1 : Form
	{
		private string _fileName;
		private Image _loadedImage;

		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				using (var dlg = new OpenFileDialog())
				{
					dlg.Filter =
						"PNG Files (*.png)|*.png|JPEG Files (*.jpg)|*.jpg|BMP Files (*.bmp)|*.bmp|PSD Files (*.psd)|*.psd|TGA Files (*.tga)|*.tga|GIF Files (*.gif)|*.gif|All Files (*.*)|*.*";
					if (dlg.ShowDialog() != DialogResult.OK)
					{
						return;
					}

					_fileName = dlg.FileName;

					var bytes = File.ReadAllBytes(_fileName);

					_loadedImage = StbImage.LoadFromMemory(bytes, StbImage.STBI_rgb_alpha);
					SetImage();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error", ex.Message);
			}
		}

		private void SetImage()
		{
			// Convert to bgra
			var data = new byte[_loadedImage.Data.Length];
			Array.Copy(_loadedImage.Data, data, data.Length);

			for (var i = 0; i < _loadedImage.Width*_loadedImage.Height; ++i)
			{
				var r = data[i*4];
				var g = data[i*4 + 1];
				var b = data[i*4 + 2];
				var a = data[i*4 + 3];


				data[i*4] = b;
				data[i*4 + 1] = g;
				data[i*4 + 2] = r;
				data[i*4 + 3] = a;
			}

			// Convert to Bitmap
			var bmp = new Bitmap(_loadedImage.Width, _loadedImage.Height, PixelFormat.Format32bppArgb);
			var bmpData = bmp.LockBits(new Rectangle(0, 0, _loadedImage.Width, _loadedImage.Height), ImageLockMode.WriteOnly,
				bmp.PixelFormat);

			Marshal.Copy(data, 0, bmpData.Scan0, bmpData.Stride*bmp.Height);
			bmp.UnlockBits(bmpData);

			pictureBox1.Image = bmp;
			_numericWidth.Value = _loadedImage.Width;
			_numericHeight.Value = _loadedImage.Height;
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			try
			{
				string fileName;
				using (var dlg = new SaveFileDialog())
				{
					dlg.Filter =
						"BMP Files (*.bmp)|*.bmp|TGA Files (*.tga)|*.tga|PNG Files (*.png)|*.png|HDR Files (*.hdr)|*.hdr|JPG Files (*.jpg)|*.jpg";
					if (dlg.ShowDialog() != DialogResult.OK)
					{
						return;
					}

					fileName = dlg.FileName;
				}

				// Get bitmap bytes
				var bmp = (Bitmap) pictureBox1.Image;
				var x = bmp.Width;
				var y = bmp.Height;
				var bmpData = bmp.LockBits(new Rectangle(0, 0, x, y), ImageLockMode.ReadOnly, bmp.PixelFormat);

				var data = new byte[y*bmpData.Stride];
				Marshal.Copy(bmpData.Scan0, data, 0, bmpData.Stride*bmp.Height);
				bmp.UnlockBits(bmpData);

				// Convert bgra to rgba
				for (var i = 0; i < x*y; ++i)
				{
					var b = data[i*4];
					var g = data[i*4 + 1];
					var r = data[i*4 + 2];
					var a = data[i*4 + 3];

					data[i*4] = r;
					data[i*4 + 1] = g;
					data[i*4 + 2] = b;
					data[i*4 + 3] = a;
				}

				// Call StbSharp
				using (var stream = File.Create(fileName))
				{
					var writer = new ImageWriter();
					var image = new Image
					{
						Data = data,
						Width = x,
						Height = y,
						Comp = 4
					};

					if (fileName.EndsWith(".bmp"))
					{
						writer.WriteBmp(image, stream);
					}
					else if (fileName.EndsWith(".tga"))
					{
						writer.WriteTga(image, stream);
					}
					else if (fileName.EndsWith("png"))
					{
						writer.WritePng(image, stream);
					}
					else if (fileName.EndsWith("hdr"))
					{
						writer.WriteHdr(image, stream);
					}
					else if (fileName.EndsWith("jpg") || fileName.EndsWith("jpeg"))
					{
						writer.WriteJpg(image, stream, 75);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error", ex.Message);
			}
		}

		private void buttonResize_Click(object sender, EventArgs e)
		{
			if (_loadedImage == null)
			{
				return;
			}

			_loadedImage = _loadedImage.CreateResized((int) _numericWidth.Value, (int) _numericHeight.Value);
			SetImage();
		}
	}
}