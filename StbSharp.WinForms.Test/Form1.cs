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
					dlg.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpg)|*.jpg";
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
					labelStatus.Text = string.Format("Trying to load ordinary way '{0}'", _fileName);
				});

				var stamp = DateTime.Now;

				var bytes = File.ReadAllBytes(_fileName);

				Bitmap bmp;
				using (var stream = new MemoryStream(bytes))
				{
					bmp = (Bitmap)Bitmap.FromStream(stream);
				}

				var passed = DateTime.Now - stamp;
				stamp = DateTime.Now;

				var passed1 = passed;
				DoInvoke(() =>
				{
					labelOrdinary.Text = string.Format("{0:0.00} ms", passed1.TotalMilliseconds);
					labelStatus.Text = string.Format("Trying to load through StbSharp '{0}'", _fileName);
					pictureBox1.Image = bmp;
				});

				int x, y, comp;
				
				var data2 = Loader.load_from_memory(bytes, out x, out y, out comp, Image.STBI_rgb_alpha);
				var data = Image.stbi_load_from_memory(bytes, out x, out y, out comp, Image.STBI_rgb_alpha);

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

				for (var i = 0; i < x * y; ++i)
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
				var pixelFormat = PixelFormat.Format32bppArgb;
				bmp = new Bitmap(x, y, pixelFormat);
				var bmpData = bmp.LockBits(new Rectangle(0, 0, x, y), ImageLockMode.WriteOnly,bmp.PixelFormat);

				Marshal.Copy(data, 0, bmpData.Scan0, bmpData.Stride*bmp.Height);
				bmp.UnlockBits(bmpData);

				passed = DateTime.Now - stamp;

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
	}
}