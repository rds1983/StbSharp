using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using NAudio.Wave;

namespace StbSharp.OggPlayer
{
	public partial class Form1 : Form
	{
		private int _sampleRate;
		private int _channels;
		private byte[] _audioData;
		private bool _isPlaying;
		private WaveOut _waveOut;
		private Thread _playThread;

		public Form1()
		{
			InitializeComponent();

			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			_buttonPlay.Enabled = _audioData != null && !_isPlaying;
			_buttonStop.Enabled = _audioData != null && _isPlaying;
		}

		private void _buttonOpen_Click(object sender, EventArgs e)
		{
			try
			{
				using (var dlg = new OpenFileDialog())
				{
					dlg.Filter =
						"OGG Files (*.ogg)|*.ogg|OGA Files (*.oga)|*.oga|All Files (*.*)|*.*";
					if (dlg.ShowDialog() != DialogResult.OK)
					{
						return;
					}

					var bytes = File.ReadAllBytes(dlg.FileName);

					var audioShort = Stb.decode_vorbis_from_memory(bytes, out _sampleRate, out _channels);

					_audioData = new byte[audioShort.Length/2*4];
					for (var i = 0; i < audioShort.Length; ++i)
					{
						if (i*2 >= _audioData.Length)
						{
							break;
						}

						var b1 = (byte) (audioShort[i] >> 8);
						var b2 = (byte) (audioShort[i] & 256);

						_audioData[i*2 + 0] = b2;
						_audioData[i*2 + 1] = b1;
					}

					Text = dlg.FileName;

					UpdateEnabled();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error", ex.Message);
			}
		}

		private void _buttonPlay_Click(object sender, EventArgs e)
		{
			try
			{
				var waveFormat = new WaveFormat(_sampleRate, _channels);
				var waveStream = new RawSourceWaveStream(_audioData, 0, _audioData.Length, waveFormat);

				_waveOut = new WaveOut();
				_waveOut.Init(waveStream);

				_playThread = new Thread(PlayThreadProc);
				_playThread.Start();

				_isPlaying = true;
				UpdateEnabled();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error", ex.Message);
			}
		}

		private void PlayThreadProc()
		{
			try
			{
				_waveOut.Play();
			}
			catch (Exception)
			{
			}
		}

		private void _buttonStop_Click(object sender, EventArgs e)
		{
			try
			{
				_waveOut.Stop();
				_playThread.Abort();
				_isPlaying = false;
				UpdateEnabled();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error", ex.Message);
			}
		}
	}
}
