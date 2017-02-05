using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Sichem;

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

		public unsafe void LoadProc(object state)
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

				int x = 0, y = 0, comp = 0;
				
				byte* result;
				fixed (byte *b = &bytes[0])
				{
					result = Image.stbi_load_from_memory(b, bytes.Length, &x, &y, &comp, Image.STBI_rgb_alpha);
				}

				if (result == null)
				{
					throw new Exception(Image.LastError);
				}

				var data = new byte[x*y*4];
				byte* bptr;
				

				// Convert rgba to bgra
				DoInvoke(() =>
				{
					labelStatus.Text = string.Format("Converting to bgra", _fileName);
				});

				bptr = result;
				for (var i = 0; i < x * y; ++i)
				{
					var r = bptr[0];
					var g = bptr[1];
					var b = bptr[2];
					var a = bptr[3];


					bptr[0] = b;
					bptr[1] = g;
					bptr[2] = r;
					bptr[3] = a;

					bptr += 4;
				}

				// Convert to array
				bptr = result;
				for (var i = 0; i < data.Length; ++i)
				{
					data[i] = bptr[i];
				}

				// Convert to Bitmap
				bmp = new Bitmap(x, y, PixelFormat.Format32bppArgb);
				var bmpData = bmp.LockBits(new Rectangle(0, 0, x, y), ImageLockMode.WriteOnly,bmp.PixelFormat);

				Marshal.Copy(data, 0, bmpData.Scan0, bmpData.Stride*bmp.Height);
				bmp.UnlockBits(bmpData);

				passed = DateTime.Now - stamp;

				DoInvoke(() =>
				{
					// pictureBox1.Image = bmp;
					labelStbSharp.Text = string.Format("{0:0.00} ms", passed.TotalMilliseconds);
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