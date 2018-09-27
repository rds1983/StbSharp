using System;
using static StbSharp.StbVorbis;

namespace StbSharp
{
	public unsafe class Vorbis : IDisposable
	{
		private readonly stb_vorbis _vorbis;
		private readonly stb_vorbis_info _vorbisInfo;
		private readonly float _lengthInSeconds;
		private readonly short[] _songBuffer;
		private int _decoded;

		public stb_vorbis StbVorbis
		{
			get
			{
				return _vorbis;
			}
		}

		public stb_vorbis_info StbVorbisInfo
		{
			get
			{
				return _vorbisInfo;
			}
		}

		public float LengthInSeconds
		{
			get
			{
				return _lengthInSeconds;
			}
		}

		public short[] SongBuffer
		{
			get
			{
				return _songBuffer;
			}
		}

		public int Decoded
		{
			get
			{
				return _decoded;
			}
		}

		private Vorbis(stb_vorbis vorbis)
		{
			if (vorbis == null)
			{
				throw new ArgumentNullException("vorbis");
			}

			_vorbis = vorbis;
			_vorbisInfo = stb_vorbis_get_info(vorbis);
			_lengthInSeconds = stb_vorbis_stream_length_in_seconds(_vorbis);

			_songBuffer = new short[_vorbisInfo.sample_rate * _vorbisInfo.channels * 2];

			Restart();
		}

		public void Dispose()
		{
		}

		public void Restart()
		{
			stb_vorbis_seek_start(_vorbis);
		}

		public void SubmitBuffer()
		{
			fixed (short* ptr = _songBuffer)
			{
				_decoded = stb_vorbis_get_samples_short_interleaved(_vorbis, _vorbisInfo.channels, ptr, (int)_vorbisInfo.sample_rate);
			}
		}

		public static Vorbis FromMemory(byte[] data)
		{
			stb_vorbis vorbis;
			fixed (byte* b = data)
			{
				vorbis = stb_vorbis_open_memory(b, data.Length, null, null);
			}

			return new Vorbis(vorbis);
		}
	}
}