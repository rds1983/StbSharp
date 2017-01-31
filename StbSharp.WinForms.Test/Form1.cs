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

		public void LoadProc(object state)
		{
			try
			{
				DoInvoke(() =>
				{
					button1.Enabled = false;
					labelStatus.Text = string.Format("Trying to load '{0}'", _fileName);
				});

				var bytes = File.ReadAllBytes(_fileName);

				var pointer = new Pointer<byte>(bytes);
				var x = new Pointer<int>(1);
				var y = new Pointer<int>(1);
				var comp = new Pointer<int>(1);

				var result = Image.stbi_load_from_memory(pointer, (int) pointer.Size, x, y, comp, Image.STBI_rgb_alpha);

				if (result.IsNull)
				{
					throw new Exception(Image.LastError);
				}

				// Convert rgba to argb
				for (var i = 0; i < x.CurrentValue*y.CurrentValue; ++i)
				{
					var r = result.Data[i*4];
					var g = result.Data[i*4 + 1];
					var b = result.Data[i*4 + 2];
					var a = result.Data[i*4 + 3];

					result.Data[i*4] = b;
					result.Data[i*4 + 1] = g;
					result.Data[i*4 + 2] = r;
					result.Data[i*4 + 3] = a;
				}

				// Convert to Bitmap
				var bmp = new Bitmap(x.CurrentValue, y.CurrentValue, PixelFormat.Format32bppArgb);
				var bmpData = bmp.LockBits(new Rectangle(0, 0, x.CurrentValue, y.CurrentValue),
					ImageLockMode.WriteOnly,
					bmp.PixelFormat);

				Marshal.Copy(result.Data, 0, bmpData.Scan0, bmpData.Stride*bmp.Height);
				bmp.UnlockBits(bmpData);

				DoInvoke(() =>
				{
					pictureBox1.Image = bmp;
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