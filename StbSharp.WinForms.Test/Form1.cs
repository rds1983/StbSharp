using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using StbNative;

namespace StbSharp.WinForms.Test
{
	public partial class Form1 : Form
	{
		private string _fileName;

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
						"PNG Files (*.png)|*.png|JPEG Files (*.jpg)|*.jpg|BMP Files (*.bmp)|*.bmp|PSD Files (*.psd)|*.psd|PIC Files (*.pic)|*.pic|TGA Files (*.tga)|*.tga|GIF Files (*.gif)|*.gif|All Files (*.*)|*.*";
					if (dlg.ShowDialog() != DialogResult.OK)
					{
						return;
					}

					_fileName = dlg.FileName;
					ThreadPool.QueueUserWorkItem(LoadProc);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error", ex.Message);
			}
		}

		private void DoInvoke(Action action)
		{
			Invoke(new MethodInvoker(action));
		}

		public void LoadProc(object state)
		{
			try
			{
				DoInvoke(() =>
				{
					button1.Enabled = false;
					labelStatus.Text = string.Format("Trying to load through StbSharp '{0}'", _fileName);
				});

				var bytes = File.ReadAllBytes(_fileName);

				int x, y, comp;

				var data2 = Native.load_from_memory(bytes, out x, out y, out comp, Stb.STBI_rgb_alpha);

				var stamp = DateTime.Now;
				var data = Stb.LoadFromMemory(bytes, out x, out y, out comp, Stb.STBI_rgb_alpha);

				var wrongCount = 0;
				for (var i = 0; i < data.Length; ++i)
				{
					if (data[i] != data2[i])
					{
						++wrongCount;

						var xc = i/4%x;
						var yc = i/4/x;
					}
				}

				// Convert rgba to bgra
				DoInvoke(() =>
				{
					labelStatus.Text = string.Format("Converting to bgra", _fileName);
				});

				for (var i = 0; i < x*y; ++i)
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
				var bmp = new Bitmap(x, y, PixelFormat.Format32bppArgb);
				var bmpData = bmp.LockBits(new Rectangle(0, 0, x, y), ImageLockMode.WriteOnly, bmp.PixelFormat);

				Marshal.Copy(data, 0, bmpData.Scan0, bmpData.Stride*bmp.Height);
				bmp.UnlockBits(bmpData);

				var passed = DateTime.Now - stamp;

				DoInvoke(() =>
				{
					pictureBox1.Image = bmp;
					labelStbSharp.Text = string.Format("{0:0.00} ms", passed.TotalMilliseconds);
					labelWrongCount.Text = wrongCount.ToString();
					labelStatus.Text = "Success";
				});

			}
			catch (Exception ex)
			{
				DoInvoke(() =>
				{
					labelStatus.Text = ex.Message;
				});
			}
			finally
			{
				DoInvoke(() =>
				{
					button1.Enabled = true;
				});
			}
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			try
			{
				string fileName;
				using (var dlg = new SaveFileDialog())
				{
					dlg.Filter = "BMP Files (*.bmp)|*.bmp|TGA Files (*.tga)|*.tga|PNG Files (*.png)|*.png|HDR Files (*.hdr)|*.psd";
					if (dlg.ShowDialog() != DialogResult.OK)
					{
						return;
					}

					fileName = dlg.FileName;
				}

				var type = ImageWriterType.Bmp;
				if (fileName.EndsWith(".tga"))
				{
					type = ImageWriterType.Tga;
				}
				else if (fileName.EndsWith("png"))
				{
					type = ImageWriterType.Png;
				}
				else if (fileName.EndsWith("hdr"))
				{
					type = ImageWriterType.Hdr;
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
				for (var i = 0; i < x * y; ++i)
				{
					var b = data[i * 4];
					var g = data[i * 4 + 1];
					var r = data[i * 4 + 2];
					var a = data[i * 4 + 3];

					data[i * 4] = r;
					data[i * 4 + 1] = g;
					data[i * 4 + 2] = b;
					data[i * 4 + 3] = a;
				}

				// Call StbSharp
				using (var stream = File.Create(fileName))
				{
					var writer = new ImageWriterToStream();
					writer.stbi_write_to(data, x, y, 4, type, stream);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error", ex.Message);
			}
		}
	}
}