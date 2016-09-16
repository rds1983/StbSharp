namespace StbSharp
{
	public static partial class Image
	{
		private unsafe static void stbi__start_mem(stbi__context s, Pointer<byte> buffer, int len)
		{
			s.io.read = null;
			s.read_from_callbacks = 0;
			s.img_buffer = s.img_buffer_original = buffer;
			s.img_buffer_end = s.img_buffer_original_end = buffer + len;
		}

		private unsafe static void stbi__start_callbacks(stbi__context s, stbi_io_callbacks c, void* user)
		{
			s.io = c;
			s.io_user_data = user;
			s.buflen = s.buffer_start.Size;
			s.read_from_callbacks = 1;
			s.img_buffer_original = s.buffer_start;
			stbi__refill_buffer(s);
			s.img_buffer_original_end = s.img_buffer_end;
		}

		private unsafe static void stbi__rewind(stbi__context s)
		{
			s.img_buffer = s.img_buffer_original;
			s.img_buffer_end = s.img_buffer_original_end;
		}

		private unsafe static string stbi_failure_reason()
		{
			return stbi__g_failure_reason;
		}

		private unsafe static int stbi__err(string str)
		{
			stbi__g_failure_reason = str;
			return 0;
		}

		private unsafe static void stbi_set_flip_vertically_on_load(int flag_true_if_should_flip)
		{
			stbi__vertically_flip_on_load = flag_true_if_should_flip;
		}

		private unsafe static byte* stbi__load_main(stbi__context s, int* x, int* y, int* comp, int req_comp)
		{
			if (stbi__jpeg_test(s)) return stbi__jpeg_load(s, x, y, comp, req_comp);
			if (stbi__png_test(s)) return stbi__png_load(s, x, y, comp, req_comp);
			if (stbi__bmp_test(s)) return stbi__bmp_load(s, x, y, comp, req_comp);
			if (stbi__gif_test(s)) return stbi__gif_load(s, x, y, comp, req_comp);
			if (stbi__psd_test(s)) return stbi__psd_load(s, x, y, comp, req_comp);
			if (stbi__pic_test(s)) return stbi__pic_load(s, x, y, comp, req_comp);
			if (stbi__pnm_test(s)) return stbi__pnm_load(s, x, y, comp, req_comp);
			if (stbi__tga_test(s)) return stbi__tga_load(s, x, y, comp, req_comp);
			return ((byte*) (int) (stbi__err("unknown image type") ? null : null));
		}

		private unsafe static byte* stbi__load_flip(stbi__context s, int* x, int* y, int* comp, int req_comp)
		{
			byte* result = stbi__load_main(s, x, y, comp, req_comp);
			if (stbi__vertically_flip_on_load && result != null)
			{
				int w = *x, h = *y;
				int depth = req_comp ? req_comp : *comp;
				int row, col, z;
				byte temp;
				for (row = 0;
					row < (h >> 1);
					row++)
				{
					for (col = 0;
						col < w;
						col++)
					{
						for (z = 0;
							z < depth;
							z++)
						{
							temp = result[(row*w + col)*depth + z];
							result[(row*w + col)*depth + z] = result[((h - row - 1)*w + col)*depth + z];
							result[((h - row - 1)*w + col)*depth + z] = temp;
						}
					}
				}
			}
			return result;
		}

		private unsafe static Pointer<byte> stbi_load_from_memory(Pointer<byte> buffer, int len, int* x, int* y, int* comp,
			int req_comp)
		{
			stbi__context s;
			stbi__start_mem(&s, buffer, len);
			return stbi__load_flip(&s, x, y, comp, req_comp);
		}

		private unsafe static Pointer<byte> stbi_load_from_callbacks(stbi_io_callbacks clbk, void* user, int* x, int* y,
			int* comp, int req_comp)
		{
			stbi__context s;
			stbi__start_callbacks(&s, (stbi_io_callbacks) clbk, user);
			return stbi__load_flip(&s, x, y, comp, req_comp);
		}

		private unsafe static int stbi_is_hdr_from_memory(Pointer<byte> buffer, int len)
		{
			()
			(buffer);
			()
			(len);
			return 0;
		}

		private unsafe static int stbi_is_hdr_from_callbacks(stbi_io_callbacks clbk, void* user)
		{
			()
			(clbk);
			()
			(user);
			return 0;
		}

		private unsafe static void stbi_hdr_to_ldr_gamma(float gamma)
		{
			stbi__h2l_gamma_i = 1/gamma;
		}

		private unsafe static void stbi_hdr_to_ldr_scale(float scale)
		{
			stbi__h2l_scale_i = 1/scale;
		}

		private unsafe static void stbi__refill_buffer(stbi__context s)
		{
			int n = (s.io.read) (s.io_user_data,
				
			(string) s.buffer_start,
			s.buflen)
			;
			if (n == 0)
			{
				s.read_from_callbacks = 0;
				s.img_buffer = s.buffer_start;
				s.img_buffer_end = s.buffer_start + 1;
				*s.img_buffer = 0;
			}
			else
			{
				s.img_buffer = s.buffer_start;
				s.img_buffer_end = s.buffer_start + n;
			}
		}

		private unsafe static byte stbi__get8(stbi__context s)
		{
			if (s.img_buffer < s.img_buffer_end) return *s.img_buffer++;
			if (s.read_from_callbacks)
			{
				stbi__refill_buffer(s);
				return *s.img_buffer++;
			}
			return 0;
		}

		private unsafe static int stbi__at_eof(stbi__context s)
		{
			if (s.io.read)
			{
				if (!(s.io.eof) (s.io_user_data)) return 0;
				if (s.read_from_callbacks == 0) return 1;
			}
			return s.img_buffer >= s.img_buffer_end;
		}

		private unsafe static void stbi__skip(stbi__context s, int n)
		{
			if (n < 0)
			{
				s.img_buffer = s.img_buffer_end;
				return;
			}
			if (s.io.read)
			{
				int blen = (int) (s.img_buffer_end - s.img_buffer);
				if (blen < n)
				{
					s.img_buffer = s.img_buffer_end;
					(s.io.skip) (s.io_user_data,
					n - blen)
					;
					return;
				}
			}
			s.img_buffer += n;
		}

		private unsafe static int stbi__getn(stbi__context s, Pointer<byte> buffer, int n)
		{
			if (s.io.read)
			{
				int blen = (int) (s.img_buffer_end - s.img_buffer);
				if (blen < n)
				{
					int res, count;
					memcpy(buffer, s.img_buffer, blen);
					count = (s.io.read) (s.io_user_data,
					(string) buffer + blen,
					n - blen)
					;
					res = (count == (n - blen));
					s.img_buffer = s.img_buffer_end;
					return res;
				}
			}
			if (s.img_buffer + n <= s.img_buffer_end)
			{
				memcpy(buffer, s.img_buffer, n);
				s.img_buffer += n;
				return 1;
			}
			else
				return 0;
		}

		private unsafe static int stbi__get16be(stbi__context s)
		{
			int z = stbi__get8(s);
			return (z << 8) + stbi__get8(s);
		}

		private unsafe static uint stbi__get32be(stbi__context s)
		{
			uint z = stbi__get16be(s);
			return (z << 16) + stbi__get16be(s);
		}

		private unsafe static int stbi__get16le(stbi__context s)
		{
			int z = stbi__get8(s);
			return z + (stbi__get8(s) << 8);
		}

		private unsafe static uint stbi__get32le(stbi__context s)
		{
			uint z = stbi__get16le(s);
			return z + (stbi__get16le(s) << 16);
		}

		private unsafe static byte stbi__compute_y(int r, int g, int b)
		{
			return (byte) (((r*77) + (g*150) + (29*b)) >> 8);
		}

		private unsafe static int stbi__build_huffman(stbi__huffman* h, int* count)
		{
			int i, j, k = 0, code;
			for (i = 0;
				i < 16;
				++i)
				for (j = 0;
					j < count[i];
					++j) h.size[k++] = (byte) (i + 1);
			h.size[k] = 0;
			code = 0;
			k = 0;
			for (j = 1;
				j <= 16;
				++j)
			{
				h.delta[j] = k - code;
				if (h.size[k] == j)
				{
					while (h.size[k] == j) h.code[k++] = (stbi__uint16) (code++);
					if (code - 1 >= (1 << j)) return stbi__err("bad code lengths");
				}
				h.maxcode[j] = code << (16 - j);
				code <<= 1;
			}
			h.maxcode[j] = 0xffffffff;
			memset(h.fast, 255, 1 << 9);
			for (i = 0;
				i < k;
				++i)
			{
				int s = h.size[i];
				if (s <= 9)
				{
					int c = h.code[i] << (9 - s);
					int m = 1 << (9 - s);
					for (j = 0;
						j < m;
						++j)
					{
						h.fast[c + j] = (byte) i;
					}
				}
			}
			return 1;
		}

		private unsafe static void stbi__build_fast_ac(stbi__int16* fast_ac, stbi__huffman* h)
		{
			int i;
			for (i = 0;
				i < (1 << 9);
				++i)
			{
				byte fast = h.fast[i];
				fast_ac[i] = 0;
				if (fast < 255)
				{
					int rs = h.values[fast];
					int run = (rs >> 4) & 15;
					int magbits = rs & 15;
					int len = h.size[fast];
					if (magbits && len + magbits <= 9)
					{
						int k = ((i << len) & ((1 << 9) - 1)) >> (9 - magbits);
						int m = 1 << (magbits - 1);
						if (k < m) k += (-1 << magbits) + 1;
						if (k >= -128 && k <= 127) fast_ac[i] = (stbi__int16) ((k << 8) + (run << 4) + (len + magbits));
					}
				}
			}
		}

		private unsafe static void stbi__grow_buffer_unsafe(stbi__jpeg* j)
		{
			do
			{
				int b = j.nomore ? 0 : stbi__get8(j.s);
				if (b == 0xff)
				{
					int c = stbi__get8(j.s);
					if (c != 0)
					{
						j.marker = (byte) c;
						j.nomore = 1;
						return;
					}
				}
				j.code_buffer |= b << (24 - j.code_bits);
				j.code_bits += 8;
			} while (j.code_bits <= 24);
		}

		private unsafe static int stbi__jpeg_huff_decode(stbi__jpeg* j, stbi__huffman* h)
		{
			unsigned
			int temp;
			int c, k;
			if (j.code_bits < 16) stbi__grow_buffer_unsafe(j);
			c = (j.code_buffer >> (32 - 9)) & ((1 << 9) - 1);
			k = h.fast[c];
			if (k < 255)
			{
				int s = h.size[k];
				if (s > j.code_bits) return -1;
				j.code_buffer <<= s;
				j.code_bits -= s;
				return h.values[k];
			}
			temp = j.code_buffer >> 16;
			for (k = 9 + 1;
				;
				++k) if (temp < h.maxcode[k]) break;
			if (k == 17)
			{
				j.code_bits -= 16;
				return -1;
			}
			if (k > j.code_bits) return -1;
			c = ((j.code_buffer >> (32 - k)) & stbi__bmask[k]) + h.delta[k];
			assert((((j.code_buffer) >> (32 - h.size[c])) & stbi__bmask[h.size[c]]) == h.code[c]);
			j.code_bits -= k;
			j.code_buffer <<= k;
			return h.values[c];
		}

		private unsafe static int stbi__extend_receive(stbi__jpeg* j, int n)
		{
			unsigned
			int k;
			int sgn;
			if (j.code_bits < n) stbi__grow_buffer_unsafe(j);
			sgn = (stbi__int32) j.code_buffer >> 31;
			k = _lrotl(j.code_buffer, n);
			assert(n >= 0 && n < (int) (sizeof (stbi__bmask)/sizeof (*stbi__bmask)))
			;
			j.code_buffer = k & ~stbi__bmask[n];
			k &= stbi__bmask[n];
			j.code_bits -= n;
			return k + (stbi__jbias[n] & ~sgn);
		}

		private unsafe static int stbi__jpeg_get_bits(stbi__jpeg* j, int n)
		{
			unsigned
			int k;
			if (j.code_bits < n) stbi__grow_buffer_unsafe(j);
			k = _lrotl(j.code_buffer, n);
			j.code_buffer = k & ~stbi__bmask[n];
			k &= stbi__bmask[n];
			j.code_bits -= n;
			return k;
		}

		private unsafe static int stbi__jpeg_get_bit(stbi__jpeg* j)
		{
			unsigned
			int k;
			if (j.code_bits < 1) stbi__grow_buffer_unsafe(j);
			k = j.code_buffer;
			j.code_buffer <<= 1;
			--j.code_bits;
			return k & 0x80000000;
		}

		private unsafe static int stbi__jpeg_decode_block(stbi__jpeg* j, short data 
		[
		
		64],

		stbi__huffman* hdc, stbi__huffman
	*
		hac 
	,
		stbi__int16* fac,
		int b, Pointer
	<
		byte 
	>
		dequant 
	)
		{
			int diff, dc, k;
			int t;
			if (j.code_bits < 16) stbi__grow_buffer_unsafe(j);
			t = stbi__jpeg_huff_decode(j, hdc);
			if (t < 0) return stbi__err("bad huffman code");
			memset(data, 0, 64*sizeof (data[
			0]))
			;
			diff = t ? stbi__extend_receive(j, t) : 0;
			dc = j.img_comp[b].dc_pred + diff;
			j.img_comp[b].dc_pred = dc;
			data[0] = (short) (dc*dequant[0]);
			k = 1;
			do
			{
				unsigned
				int zig;
				int c, r, s;
				if (j.code_bits < 16) stbi__grow_buffer_unsafe(j);
				c = (j.code_buffer >> (32 - 9)) & ((1 << 9) - 1);
				r = fac[c];
				if (r)
				{
					k += (r >> 4) & 15;
					s = r & 15;
					j.code_buffer <<= s;
					j.code_bits -= s;
					zig = stbi__jpeg_dezigzag[k++];
					data[zig] = (short) ((r >> 8)*dequant[zig]);
				}
				else
				{
					int rs = stbi__jpeg_huff_decode(j, hac);
					if (rs < 0) return stbi__err("bad huffman code");
					s = rs & 15;
					r = rs >> 4;
					if (s == 0)
					{
						if (rs != 0xf0) break;
						k += 16;
					}
					else
					{
						k += r;
						zig = stbi__jpeg_dezigzag[k++];
						data[zig] = (short) (stbi__extend_receive(j, s)*dequant[zig]);
					}
				}
			} while (k < 64);
			return 1;
		}

		private unsafe static int stbi__jpeg_decode_block_prog_dc(stbi__jpeg* j, short data 
		[
		
		64],

		stbi__huffman* hdc,
		int b 
	)
		{
			int diff, dc;
			int t;
			if (j.spec_end != 0) return stbi__err("can't merge dc and ac");
			if (j.code_bits < 16) stbi__grow_buffer_unsafe(j);
			if (j.succ_high == 0)
			{
				memset(data, 0, 64*sizeof (data[
				0]))
				;
				t = stbi__jpeg_huff_decode(j, hdc);
				diff = t ? stbi__extend_receive(j, t) : 0;
				dc = j.img_comp[b].dc_pred + diff;
				j.img_comp[b].dc_pred = dc;
				data[0] = (short) (dc << j.succ_low);
			}
			else
			{
				if (stbi__jpeg_get_bit(j)) data[0] += (short) (1 << j.succ_low);
			}
			return 1;
		}

		private unsafe static int stbi__jpeg_decode_block_prog_ac(stbi__jpeg* j, short data 
		[
		
		64],

		stbi__huffman* hac, stbi__int16
	*
		fac 
	)
		{
			int k;
			if (j.spec_start == 0) return stbi__err("can't merge dc and ac");
			if (j.succ_high == 0)
			{
				int shift = j.succ_low;
				if (j.eob_run)
				{
					--j.eob_run;
					return 1;
				}
				k = j.spec_start;
				do
				{
					unsigned
					int zig;
					int c, r, s;
					if (j.code_bits < 16) stbi__grow_buffer_unsafe(j);
					c = (j.code_buffer >> (32 - 9)) & ((1 << 9) - 1);
					r = fac[c];
					if (r)
					{
						k += (r >> 4) & 15;
						s = r & 15;
						j.code_buffer <<= s;
						j.code_bits -= s;
						zig = stbi__jpeg_dezigzag[k++];
						data[zig] = (short) ((r >> 8) << shift);
					}
					else
					{
						int rs = stbi__jpeg_huff_decode(j, hac);
						if (rs < 0) return stbi__err("bad huffman code");
						s = rs & 15;
						r = rs >> 4;
						if (s == 0)
						{
							if (r < 15)
							{
								j.eob_run = (1 << r);
								if (r) j.eob_run += stbi__jpeg_get_bits(j, r);
								--j.eob_run;
								break;
							}
							k += 16;
						}
						else
						{
							k += r;
							zig = stbi__jpeg_dezigzag[k++];
							data[zig] = (short) (stbi__extend_receive(j, s) << shift);
						}
					}
				} while (k <= j.spec_end);
			}
			else
			{
				short bit = (short) (1 << j.succ_low);
				if (j.eob_run)
				{
					--j.eob_run;
					for (k = j.spec_start;
						k <= j.spec_end;
						++k)
					{
						short* p = &data[stbi__jpeg_dezigzag[k]];
						if (*p != 0)
							if (stbi__jpeg_get_bit(j))
								if ((*p & bit) == 0)
								{
									if (*p > 0) *p += bit;
									else
										*p -= bit;
								}
					}
				}
				else
				{
					k = j.spec_start;
					do
					{
						int r, s;
						int rs = stbi__jpeg_huff_decode(j, hac);
						if (rs < 0) return stbi__err("bad huffman code");
						s = rs & 15;
						r = rs >> 4;
						if (s == 0)
						{
							if (r < 15)
							{
								j.eob_run = (1 << r) - 1;
								if (r) j.eob_run += stbi__jpeg_get_bits(j, r);
								r = 64;
							}
							else
							{
							}
						}
						else
						{
							if (s != 1) return stbi__err("bad huffman code");
							if (stbi__jpeg_get_bit(j)) s = bit;
							else
								s = -bit;
						}
						while (k <= j.spec_end)
						{
							short* p = &data[stbi__jpeg_dezigzag[k++]];
							if (*p != 0)
							{
								if (stbi__jpeg_get_bit(j))
									if ((*p & bit) == 0)
									{
										if (*p > 0) *p += bit;
										else
											*p -= bit;
									}
							}
							else
							{
								if (r == 0)
								{
									*p = (short) s;
									break;
								}
								--r;
							}
						}
					} while (k <= j.spec_end);
				}
			}
			return 1;
		}

		private unsafe static byte stbi__clamp(int x)
		{
			if ((unsigned
			int  )
			x > 255)
			{
				if (x < 0) return 0;
				if (x > 255) return 255;
			}
			return (byte) x;
		}

		private unsafe static void stbi__idct_block(Pointer<byte> out, int out_stride, short data 
		[
		
		64])

		{
			int i, val [
			64],
			*v = val;
			Pointer<byte> o;
			short* d = data;
			for (i = 0;
				i < 8;
				++i, ++d, ++v)
			{
				if (d[8] == 0 && d[16] == 0 && d[24] == 0 && d[32] == 0 && d[40] == 0 && d[48] == 0 && d[56] == 0)
				{
					int dcterm = d[0] << 2;
					v[0] = v[8] = v[16] = v[24] = v[32] = v[40] = v[48] = v[56] = dcterm;
				}
				else
				{
					int t0, t1, t2, t3, p1, p2, p3, p4, p5, x0, x1, x2, x3;
					p2 = d[16];
					p3 = d[48];
					p1 = (p2 + p3)*((int) (((0.5411961f)*4096 + 0.5)));
					t2 = p1 + p3*((int) (((-1.847759065f)*4096 + 0.5)));
					t3 = p1 + p2*((int) (((0.765366865f)*4096 + 0.5)));
					p2 = d[0];
					p3 = d[32];
					t0 = ((p2 + p3) << 12);
					t1 = ((p2 - p3) << 12);
					x0 = t0 + t3;
					x3 = t0 - t3;
					x1 = t1 + t2;
					x2 = t1 - t2;
					t0 = d[56];
					t1 = d[40];
					t2 = d[24];
					t3 = d[8];
					p3 = t0 + t2;
					p4 = t1 + t3;
					p1 = t0 + t3;
					p2 = t1 + t2;
					p5 = (p3 + p4)*((int) (((1.175875602f)*4096 + 0.5)));
					t0 = t0*((int) (((0.298631336f)*4096 + 0.5)));
					t1 = t1*((int) (((2.053119869f)*4096 + 0.5)));
					t2 = t2*((int) (((3.072711026f)*4096 + 0.5)));
					t3 = t3*((int) (((1.501321110f)*4096 + 0.5)));
					p1 = p5 + p1*((int) (((-0.899976223f)*4096 + 0.5)));
					p2 = p5 + p2*((int) (((-2.562915447f)*4096 + 0.5)));
					p3 = p3*((int) (((-1.961570560f)*4096 + 0.5)));
					p4 = p4*((int) (((-0.390180644f)*4096 + 0.5)));
					t3 += p1 + p4;
					t2 += p2 + p3;
					t1 += p2 + p4;
					t0 += p1 + p3;
					x0 += 512;
					x1 += 512;
					x2 += 512;
					x3 += 512;
					v[0] = (x0 + t3) >> 10;
					v[56] = (x0 - t3) >> 10;
					v[8] = (x1 + t2) >> 10;
					v[48] = (x1 - t2) >> 10;
					v[16] = (x2 + t1) >> 10;
					v[40] = (x2 - t1) >> 10;
					v[24] = (x3 + t0) >> 10;
					v[32] = (x3 - t0) >> 10;
				}
			}
			for (i = 0, v = val, o =out;
				i < 8;
				++i, v += 8, o += out_stride)
			{
				int t0, t1, t2, t3, p1, p2, p3, p4, p5, x0, x1, x2, x3;
				p2 = v[2];
				p3 = v[6];
				p1 = (p2 + p3)*((int) (((0.5411961f)*4096 + 0.5)));
				t2 = p1 + p3*((int) (((-1.847759065f)*4096 + 0.5)));
				t3 = p1 + p2*((int) (((0.765366865f)*4096 + 0.5)));
				p2 = v[0];
				p3 = v[4];
				t0 = ((p2 + p3) << 12);
				t1 = ((p2 - p3) << 12);
				x0 = t0 + t3;
				x3 = t0 - t3;
				x1 = t1 + t2;
				x2 = t1 - t2;
				t0 = v[7];
				t1 = v[5];
				t2 = v[3];
				t3 = v[1];
				p3 = t0 + t2;
				p4 = t1 + t3;
				p1 = t0 + t3;
				p2 = t1 + t2;
				p5 = (p3 + p4)*((int) (((1.175875602f)*4096 + 0.5)));
				t0 = t0*((int) (((0.298631336f)*4096 + 0.5)));
				t1 = t1*((int) (((2.053119869f)*4096 + 0.5)));
				t2 = t2*((int) (((3.072711026f)*4096 + 0.5)));
				t3 = t3*((int) (((1.501321110f)*4096 + 0.5)));
				p1 = p5 + p1*((int) (((-0.899976223f)*4096 + 0.5)));
				p2 = p5 + p2*((int) (((-2.562915447f)*4096 + 0.5)));
				p3 = p3*((int) (((-1.961570560f)*4096 + 0.5)));
				p4 = p4*((int) (((-0.390180644f)*4096 + 0.5)));
				t3 += p1 + p4;
				t2 += p2 + p3;
				t1 += p2 + p4;
				t0 += p1 + p3;
				x0 += 65536 + (128 << 17);
				x1 += 65536 + (128 << 17);
				x2 += 65536 + (128 << 17);
				x3 += 65536 + (128 << 17);
				o[0] = stbi__clamp((x0 + t3) >> 17);
				o[7] = stbi__clamp((x0 - t3) >> 17);
				o[1] = stbi__clamp((x1 + t2) >> 17);
				o[6] = stbi__clamp((x1 - t2) >> 17);
				o[2] = stbi__clamp((x2 + t1) >> 17);
				o[5] = stbi__clamp((x2 - t1) >> 17);
				o[3] = stbi__clamp((x3 + t0) >> 17);
				o[4] = stbi__clamp((x3 - t0) >> 17);
			}
		}

		private unsafe static byte stbi__get_marker(stbi__jpeg* j)
		{
			byte x;
			if (j.marker != 0xff)
			{
				x = j.marker;
				j.marker = 0xff;
				return x;
			}
			x = stbi__get8(j.s);
			if (x != 0xff) return 0xff;
			while (x == 0xff) x = stbi__get8(j.s);
			return x;
		}

		private unsafe static void stbi__jpeg_reset(stbi__jpeg* j)
		{
			j.code_bits = 0;
			j.code_buffer = 0;
			j.nomore = 0;
			j.img_comp[0].dc_pred = j.img_comp[1].dc_pred = j.img_comp[2].dc_pred = 0;
			j.marker = 0xff;
			j.todo = j.restart_interval ? j.restart_interval : 0x7fffffff;
			j.eob_run = 0;
		}

		private unsafe static int stbi__parse_entropy_coded_data(stbi__jpeg* z)
		{
			stbi__jpeg_reset(z);
			if (!z.progressive)
			{
				if (z.scan_n == 1)
				{
					int i, j;
					short data [
					64]
					;
					int n = z.order[0];
					int w = (z.img_comp[n].x + 7) >> 3;
					int h = (z.img_comp[n].y + 7) >> 3;
					for (j = 0;
						j < h;
						++j)
					{
						for (i = 0;
							i < w;
							++i)
						{
							int ha = z.img_comp[n].ha;
							if (
								!stbi__jpeg_decode_block(z, data, z.huff_dc + z.img_comp[n].hd, z.huff_ac + ha, z.fast_ac[ha], n,
									z.dequant[z.img_comp[n].tq])) return 0;
							z.idct_block_kernel(z.img_comp[n].data + z.img_comp[n].w2*j*8 + i*8, z.img_comp[n].w2, data);
							if (--z.todo <= 0)
							{
								if (z.code_bits < 24) stbi__grow_buffer_unsafe(z);
								if (!((z.marker) >= 0xd0 && (z.marker) <= 0xd7)) return 1;
								stbi__jpeg_reset(z);
							}
						}
					}
					return 1;
				}
				else
				{
					int i, j, k, x, y;
					short data [
					64]
					;
					for (j = 0;
						j < z.img_mcu_y;
						++j)
					{
						for (i = 0;
							i < z.img_mcu_x;
							++i)
						{
							for (k = 0;
								k < z.scan_n;
								++k)
							{
								int n = z.order[k];
								for (y = 0;
									y < z.img_comp[n].v;
									++y)
								{
									for (x = 0;
										x < z.img_comp[n].h;
										++x)
									{
										int x2 = (i*z.img_comp[n].h + x)*8;
										int y2 = (j*z.img_comp[n].v + y)*8;
										int ha = z.img_comp[n].ha;
										if (
											!stbi__jpeg_decode_block(z, data, z.huff_dc + z.img_comp[n].hd, z.huff_ac + ha, z.fast_ac[ha], n,
												z.dequant[z.img_comp[n].tq])) return 0;
										z.idct_block_kernel(z.img_comp[n].data + z.img_comp[n].w2*y2 + x2, z.img_comp[n].w2, data);
									}
								}
							}
							if (--z.todo <= 0)
							{
								if (z.code_bits < 24) stbi__grow_buffer_unsafe(z);
								if (!((z.marker) >= 0xd0 && (z.marker) <= 0xd7)) return 1;
								stbi__jpeg_reset(z);
							}
						}
					}
					return 1;
				}
			}
			else
			{
				if (z.scan_n == 1)
				{
					int i, j;
					int n = z.order[0];
					int w = (z.img_comp[n].x + 7) >> 3;
					int h = (z.img_comp[n].y + 7) >> 3;
					for (j = 0;
						j < h;
						++j)
					{
						for (i = 0;
							i < w;
							++i)
						{
							short* data = z.img_comp[n].coeff + 64*(i + j*z.img_comp[n].coeff_w);
							if (z.spec_start == 0)
							{
								if (!stbi__jpeg_decode_block_prog_dc(z, data, &z.huff_dc[z.img_comp[n].hd], n)) return 0;
							}
							else
							{
								int ha = z.img_comp[n].ha;
								if (!stbi__jpeg_decode_block_prog_ac(z, data, &z.huff_ac[ha], z.fast_ac[ha])) return 0;
							}
							if (--z.todo <= 0)
							{
								if (z.code_bits < 24) stbi__grow_buffer_unsafe(z);
								if (!((z.marker) >= 0xd0 && (z.marker) <= 0xd7)) return 1;
								stbi__jpeg_reset(z);
							}
						}
					}
					return 1;
				}
				else
				{
					int i, j, k, x, y;
					for (j = 0;
						j < z.img_mcu_y;
						++j)
					{
						for (i = 0;
							i < z.img_mcu_x;
							++i)
						{
							for (k = 0;
								k < z.scan_n;
								++k)
							{
								int n = z.order[k];
								for (y = 0;
									y < z.img_comp[n].v;
									++y)
								{
									for (x = 0;
										x < z.img_comp[n].h;
										++x)
									{
										int x2 = (i*z.img_comp[n].h + x);
										int y2 = (j*z.img_comp[n].v + y);
										short* data = z.img_comp[n].coeff + 64*(x2 + y2*z.img_comp[n].coeff_w);
										if (!stbi__jpeg_decode_block_prog_dc(z, data, &z.huff_dc[z.img_comp[n].hd], n)) return 0;
									}
								}
							}
							if (--z.todo <= 0)
							{
								if (z.code_bits < 24) stbi__grow_buffer_unsafe(z);
								if (!((z.marker) >= 0xd0 && (z.marker) <= 0xd7)) return 1;
								stbi__jpeg_reset(z);
							}
						}
					}
					return 1;
				}
			}
		}

		private unsafe static void stbi__jpeg_dequantize(short* data, Pointer<byte> dequant)
		{
			int i;
			for (i = 0;
				i < 64;
				++i) data[i] *= dequant[i];
		}

		private unsafe static void stbi__jpeg_finish(stbi__jpeg* z)
		{
			if (z.progressive)
			{
				int i, j, n;
				for (n = 0;
					n < z.s.img_n;
					++n)
				{
					int w = (z.img_comp[n].x + 7) >> 3;
					int h = (z.img_comp[n].y + 7) >> 3;
					for (j = 0;
						j < h;
						++j)
					{
						for (i = 0;
							i < w;
							++i)
						{
							short* data = z.img_comp[n].coeff + 64*(i + j*z.img_comp[n].coeff_w);
							stbi__jpeg_dequantize(data, z.dequant[z.img_comp[n].tq]);
							z.idct_block_kernel(z.img_comp[n].data + z.img_comp[n].w2*j*8 + i*8, z.img_comp[n].w2, data);
						}
					}
				}
			}
		}

		private unsafe static int stbi__process_marker(stbi__jpeg* z, int m)
		{
			int L;
			switch (m)
			{
					case0xff:
					return stbi__err("expected marker");
					case0xDD:
					if (stbi__get16be(z.s) != 4) return stbi__err("bad DRI len");
					z.restart_interval = stbi__get16be(z.s);
					return 1;
					case0xDB:
					L = stbi__get16be(z.s) - 2;
					while (L > 0)
					{
						int q = stbi__get8(z.s);
						int p = q >> 4;
						int t = q & 15, i;
						if (p != 0) return stbi__err("bad DQT type");
						if (t > 3) return stbi__err("bad DQT table");
						for (i = 0;
							i < 64;
							++i) z.dequant[t][stbi__jpeg_dezigzag[i]] = stbi__get8(z.s);
						L -= 65;
					}
					return L == 0;
					case0xC4:
					L = stbi__get16be(z.s) - 2;
					while (L > 0)
					{
						Pointer<byte> v;
						int sizes [
						16],
						i,
						n = 0;
						int q = stbi__get8(z.s);
						int tc = q >> 4;
						int th = q & 15;
						if (tc > 1 || th > 3) return stbi__err("bad DHT header");
						for (i = 0;
							i < 16;
							++i)
						{
							sizes[i] = stbi__get8(z.s);
							n += sizes[i];
						}
						L -= 17;
						if (tc == 0)
						{
							if (!stbi__build_huffman(z.huff_dc + th, sizes)) return 0;
							v = z.huff_dc[th].values;
						}
						else
						{
							if (!stbi__build_huffman(z.huff_ac + th, sizes)) return 0;
							v = z.huff_ac[th].values;
						}
						for (i = 0;
							i < n;
							++i) v[i] = stbi__get8(z.s);
						if (tc != 0) stbi__build_fast_ac(z.fast_ac[th], z.huff_ac + th);
						L -= n;
					}
					return L == 0;
			}
			if ((m >= 0xE0 && m <= 0xEF) || m == 0xFE)
			{
				stbi__skip(z.s, stbi__get16be(z.s) - 2);
				return 1;
			}
			return 0;
		}

		private unsafe static int stbi__process_scan_header(stbi__jpeg* z)
		{
			int i;
			int Ls = stbi__get16be(z.s);
			z.scan_n = stbi__get8(z.s);
			if (z.scan_n < 1 || z.scan_n > 4 || z.scan_n > (int) z.s.img_n) return stbi__err("bad SOS component count");
			if (Ls != 6 + 2*z.scan_n) return stbi__err("bad SOS len");
			for (i = 0;
				i < z.scan_n;
				++i)
			{
				int id = stbi__get8(z.s), which;
				int q = stbi__get8(z.s);
				for (which = 0;
					which < z.s.img_n;
					++which) if (z.img_comp[which].id == id) break;
				if (which == z.s.img_n) return 0;
				z.img_comp[which].hd = q >> 4;
				if (z.img_comp[which].hd > 3) return stbi__err("bad DC huff");
				z.img_comp[which].ha = q & 15;
				if (z.img_comp[which].ha > 3) return stbi__err("bad AC huff");
				z.order[i] = which;
			}
			{
				int aa;
				z.spec_start = stbi__get8(z.s);
				z.spec_end = stbi__get8(z.s);
				aa = stbi__get8(z.s);
				z.succ_high = (aa >> 4);
				z.succ_low = (aa & 15);
				if (z.progressive)
				{
					if (z.spec_start > 63 || z.spec_end > 63 || z.spec_start > z.spec_end || z.succ_high > 13 || z.succ_low > 13)
						return stbi__err("bad SOS");
				}
				else
				{
					if (z.spec_start != 0) return stbi__err("bad SOS");
					if (z.succ_high != 0 || z.succ_low != 0) return stbi__err("bad SOS");
					z.spec_end = 63;
				}
			}
			return 1;
		}

		private unsafe static int stbi__decode_jpeg_header(stbi__jpeg* z, int scan)
		{
			int m;
			z.marker = 0xff;
			m = stbi__get_marker(z);
			if (!((m) == 0xd8)) return stbi__err("no SOI");
			if (scan == STBI__SCAN_type) return 1;
			m = stbi__get_marker(z);
			while (!((m) == 0xc0 || (m) == 0xc1 || (m) == 0xc2))
			{
				if (!stbi__process_marker(z, m)) return 0;
				m = stbi__get_marker(z);
				while (m == 0xff)
				{
					if (stbi__at_eof(z.s)) return stbi__err("no SOF");
					m = stbi__get_marker(z);
				}
			}
			z.progressive = ((m) == 0xc2);
			if (!stbi__process_frame_header(z, scan)) return 0;
			return 1;
		}

		private unsafe static int stbi__decode_jpeg_image(stbi__jpeg* j)
		{
			int m;
			for (m = 0;
				m < 4;
				m++)
			{
				j.img_comp[m].raw_data = null;
				j.img_comp[m].raw_coeff = null;
			}
			j.restart_interval = 0;
			if (!stbi__decode_jpeg_header(j, STBI__SCAN_load)) return 0;
			m = stbi__get_marker(j);
			while (!((m) == 0xd9))
			{
				if (((m) == 0xda))
				{
					if (!stbi__process_scan_header(j)) return 0;
					if (!stbi__parse_entropy_coded_data(j)) return 0;
					if (j.marker == 0xff)
					{
						while (!stbi__at_eof(j.s))
						{
							int x = stbi__get8(j.s);
							if (x == 255)
							{
								j.marker = stbi__get8(j.s);
								break;
							}
							else if (x != 0)
							{
								return stbi__err("junk before marker");
							}
						}
					}
				}
				else
				{
					if (!stbi__process_marker(j, m)) return 0;
				}
				m = stbi__get_marker(j);
			}
			if (j.progressive) stbi__jpeg_finish(j);
			return 1;
		}

		private unsafe static Pointer<byte> resample_row_1(Pointer<byte> out, Pointer<byte> in_near, Pointer<byte> in_far,
			int w, int hs)
		{
			()
			(out );
			()
			(in_far);
			()
			(w);
			()
			(hs);
			return in_near;
		}

		private unsafe static Pointer<byte> stbi__resample_row_v_2(Pointer<byte> out, Pointer<byte> in_near,
			Pointer<byte> in_far, int w, int hs)
		{
			int i;
			()
			(hs);
			for (i = 0;
				i < w;
				++i) out[ i]=
			((byte) ((3*in_near[i] + in_far[i] + 2) >> 2));
			return out
			;
		}

		private unsafe static Pointer<byte> stbi__resample_row_h_2(Pointer<byte> out, Pointer<byte> in_near,
			Pointer<byte> in_far, int w, int hs)
		{
			int i;
			Pointer<byte> input = in_near;
			if (w == 1)
			{
			out[
				0]=out[
				1]=
				input[0];
				return out
				;
			}
			out[
			0]=
			input[0];
			out[
			1]=
			((byte) ((input[0]*3 + input[1] + 2) >> 2));
			for (i = 1;
				i < w - 1;
				++i)
			{
				int n = 3*input[i] + 2;
				out[
				i*2 + 0]=
				((byte) ((n + input[i - 1]) >> 2));
				out[
				i*2 + 1]=
				((byte) ((n + input[i + 1]) >> 2));
			}
			out[
			i*2 + 0]=
			((byte) ((input[w - 2]*3 + input[w - 1] + 2) >> 2));
			out[
			i*2 + 1]=
			input[w - 1];
			()
			(in_far);
			()
			(hs);
			return out
			;
		}

		private unsafe static Pointer<byte> stbi__resample_row_hv_2(Pointer<byte> out, Pointer<byte> in_near,
			Pointer<byte> in_far, int w, int hs)
		{
			int i, t0, t1;
			if (w == 1)
			{
			out[
				0]=out[
				1]=
				((byte) ((3*in_near[0] + in_far[0] + 2) >> 2));
				return out
				;
			}
			t1 = 3*in_near[0] + in_far[0];
			out[
			0]=
			((byte) ((t1 + 2) >> 2));
			for (i = 1;
				i < w;
				++i)
			{
				t0 = t1;
				t1 = 3*in_near[i] + in_far[i];
				out[
				i*2 - 1]=
				((byte) ((3*t0 + t1 + 8) >> 4));
				out[
				i*2]=
				((byte) ((3*t1 + t0 + 8) >> 4));
			}
			out[
			w*2 - 1]=
			((byte) ((t1 + 2) >> 2));
			()
			(hs);
			return out
			;
		}

		private unsafe static Pointer<byte> stbi__resample_row_generic(Pointer<byte> out, Pointer<byte> in_near,
			Pointer<byte> in_far, int w, int hs)
		{
			int i, j;
			()
			(in_far);
			for (i = 0;
				i < w;
				++i)
				for (j = 0;
					j < hs;
					++j) out[ i*hs + j]=
			in_near[i];
			return out
			;
		}

		private unsafe static void stbi__YCbCr_to_RGB_row(Pointer<byte> out, Pointer<byte> y, Pointer<byte> pcb,
			Pointer<byte> pcr, int count, int step)
		{
			int i;
			for (i = 0;
				i < count;
				++i)
			{
				int y_fixed = (y[i] << 20) + (1 << 19);
				int r, g, b;
				int cr = pcr[i] - 128;
				int cb = pcb[i] - 128;
				r = y_fixed + cr*(((int) ((1.40200f)*4096.0f + 0.5f)) << 8);
				g = y_fixed + (cr*-(((int) ((0.71414f)*4096.0f + 0.5f)) << 8)) +
				    ((cb*-(((int) ((0.34414f)*4096.0f + 0.5f)) << 8)) & 0xffff0000);
				b = y_fixed + cb*(((int) ((1.77200f)*4096.0f + 0.5f)) << 8);
				r >>= 20;
				g >>= 20;
				b >>= 20;
				if ((unsigned) r > 255)
				{
					if (r < 0) r = 0;
					else
						r = 255;
				}
				if ((unsigned) g > 255)
				{
					if (g < 0) g = 0;
					else
						g = 255;
				}
				if ((unsigned) b > 255)
				{
					if (b < 0) b = 0;
					else
						b = 255;
				}
				out[
				0]=
				(byte) r;
				out[
				1]=
				(byte) g;
				out[
				2]=
				(byte) b;
				out[
				3]=
				255;
				out+=
				step;
			}
		}

		private unsafe static void stbi__setup_jpeg(stbi__jpeg* j)
		{
			j.idct_block_kernel = stbi__idct_block;
			j.YCbCr_to_RGB_kernel = stbi__YCbCr_to_RGB_row;
			j.resample_row_hv_2_kernel = stbi__resample_row_hv_2;
		}

		private unsafe static void stbi__cleanup_jpeg(stbi__jpeg* j)
		{
			int i;
			for (i = 0;
				i < j.s.img_n;
				++i)
			{
				if (j.img_comp[i].raw_data)
				{
					free(j.img_comp[i].raw_data);
					j.img_comp[i].raw_data = null;
					j.img_comp[i].data = null;
				}
				if (j.img_comp[i].raw_coeff)
				{
					free(j.img_comp[i].raw_coeff);
					j.img_comp[i].raw_coeff = 0;
					j.img_comp[i].coeff = 0;
				}
				if (j.img_comp[i].linebuf)
				{
					free(j.img_comp[i].linebuf);
					j.img_comp[i].linebuf = null;
				}
			}
		}

		private unsafe static int stbi__jpeg_test(stbi__context s)
		{
			int r;
			stbi__jpeg j;
			j.s = s;
			stbi__setup_jpeg(&j);
			r = stbi__decode_jpeg_header(&j, STBI__SCAN_type);
			stbi__rewind(s);
			return r;
		}

		private unsafe static int stbi__jpeg_info_raw(stbi__jpeg* j, int* x, int* y, int* comp)
		{
			if (!stbi__decode_jpeg_header(j, STBI__SCAN_header))
			{
				stbi__rewind(j.s);
				return 0;
			}
			if (x) *x = j.s.img_x;
			if (y) *y = j.s.img_y;
			if (comp) *comp = j.s.img_n;
			return 1;
		}

		private unsafe static int stbi__bitreverse16(int n)
		{
			n = ((n & 0xAAAA) >> 1) | ((n & 0x5555) << 1);
			n = ((n & 0xCCCC) >> 2) | ((n & 0x3333) << 2);
			n = ((n & 0xF0F0) >> 4) | ((n & 0x0F0F) << 4);
			n = ((n & 0xFF00) >> 8) | ((n & 0x00FF) << 8);
			return n;
		}

		private unsafe static int stbi__bit_reverse(int v, int bits)
		{
			assert(bits <= 16);
			return stbi__bitreverse16(v) >> (16 - bits);
		}

		private unsafe static int stbi__zbuild_huffman(stbi__zhuffman* z, Pointer<byte> sizelist, int num)
		{
			int i, k = 0;
			int code, next_code [
			16],
			sizes[17];
			memset(sizes, 0, sizeof (sizes));
			memset(z.fast, 0, sizeof (z.fast));
			for (i = 0;
				i < num;
				++i) ++sizes[sizelist[i]];
			sizes[0] = 0;
			for (i = 1;
				i < 16;
				++i) if (sizes[i] > (1 << i)) return stbi__err("bad sizes");
			code = 0;
			for (i = 1;
				i < 16;
				++i)
			{
				next_code[i] = code;
				z.firstcode[i] = (stbi__uint16) code;
				z.firstsymbol[i] = (stbi__uint16) k;
				code = (code + sizes[i]);
				if (sizes[i]) if (code - 1 >= (1 << i)) return stbi__err("bad codelengths");
				z.maxcode[i] = code << (16 - i);
				code <<= 1;
				k += sizes[i];
			}
			z.maxcode[16] = 0x10000;
			for (i = 0;
				i < num;
				++i)
			{
				int s = sizelist[i];
				if (s)
				{
					int c = next_code[s] - z.firstcode[s] + z.firstsymbol[s];
					stbi__uint16 fastv = (stbi__uint16) ((s << 9) | i);
					z.size[c] = (byte) s;
					z.value[c] = (stbi__uint16) i;
					if (s <= 9)
					{
						int j = stbi__bit_reverse(next_code[s], s);
						while (j < (1 << 9))
						{
							z.fast[j] = fastv;
							j += (1 << s);
						}
					}
					++next_code[s];
				}
			}
			return 1;
		}

		private unsafe static byte stbi__zget8(stbi__zbuf* z)
		{
			if (z.zbuffer >= z.zbuffer_end) return 0;
			return *z.zbuffer++;
		}

		private unsafe static void stbi__fill_bits(stbi__zbuf* z)
		{
			do
			{
				assert(z.code_buffer < (1U << z.num_bits));
				z.code_buffer |= (unsigned
				int  )
				stbi__zget8(z) << z.num_bits;
				z.num_bits += 8;
			} while (z.num_bits <= 24);
		}

		private unsafe static unsigned 

		int stbi__zreceive(stbi__zbuf* z, int n)
		{
			unsigned
			int k;
			if (z.num_bits < n) stbi__fill_bits(z);
			k = z.code_buffer & ((1 << n) - 1);
			z.code_buffer >>= n;
			z.num_bits -= n;
			return k;
		}

		private unsafe static int stbi__zhuffman_decode_slowpath(stbi__zbuf* a, stbi__zhuffman* z)
		{
			int b, s, k;
			k = stbi__bit_reverse(a.code_buffer, 16);
			for (s = 9 + 1;
				;
				++s) if (k < z.maxcode[s]) break;
			if (s == 16) return -1;
			b = (k >> (16 - s)) - z.firstcode[s] + z.firstsymbol[s];
			assert(z.size[b] == s);
			a.code_buffer >>= s;
			a.num_bits -= s;
			return z.value[b];
		}

		private unsafe static int stbi__zhuffman_decode(stbi__zbuf* a, stbi__zhuffman* z)
		{
			int b, s;
			if (a.num_bits < 16) stbi__fill_bits(a);
			b = z.fast[a.code_buffer & ((1 << 9) - 1)];
			if (b)
			{
				s = b >> 9;
				a.code_buffer >>= s;
				a.num_bits -= s;
				return b & 511;
			}
			return stbi__zhuffman_decode_slowpath(a, z);
		}

		private unsafe static int stbi__zexpand(stbi__zbuf* z, string zout, int n)
		{
			string q;
			int cur, limit, old_limit;
			z.zout = zout;
			if (!z.z_expandable) return stbi__err("output buffer limit");
			cur = (int) (z.zout - z.zout_start);
			limit = old_limit = (int) (z.zout_end - z.zout_start);
			while (cur + n > limit) limit *= 2;
			q = (string) realloc(z.zout_start, limit);
			()
			(old_limit);
			if (q == null) return stbi__err("outofmem");
			z.zout_start = q;
			z.zout = q + cur;
			z.zout_end = q + limit;
			return 1;
		}

		private unsafe static int stbi__parse_huffman_block(stbi__zbuf* a)
		{
			string zout = a.zout;
			for (;
				;
				)
			{
				int z = stbi__zhuffman_decode(a, &a.z_length);
				if (z < 256)
				{
					if (z < 0) return stbi__err("bad huffman code");
					if (zout >= a.zout_end)
					{
						if (!stbi__zexpand(a, zout, 1)) return 0;
						zout = a.zout;
					}
					*zout++ = (char) z;
				}
				else
				{
					Pointer<byte> p;
					int len, dist;
					if (z == 256)
					{
						a.zout = zout;
						return 1;
					}
					z -= 257;
					len = stbi__zlength_base[z];
					if (stbi__zlength_extra[z]) len += stbi__zreceive(a, stbi__zlength_extra[z]);
					z = stbi__zhuffman_decode(a, &a.z_distance);
					if (z < 0) return stbi__err("bad huffman code");
					dist = stbi__zdist_base[z];
					if (stbi__zdist_extra[z]) dist += stbi__zreceive(a, stbi__zdist_extra[z]);
					if (zout - a.zout_start < dist) return stbi__err("bad dist");
					if (zout + len > a.zout_end)
					{
						if (!stbi__zexpand(a, zout, len)) return 0;
						zout = a.zout;
					}
					p = (zout - dist);
					if (dist == 1)
					{
						byte v = *p;
						if (len)
						{
							do *zout++ = v; while (--len);
						}
					}
					else
					{
						if (len)
						{
							do *zout++ = *p++; while (--len);
						}
					}
				}
			}
		}

		private unsafe static int stbi__compute_huffman_codes(stbi__zbuf* a)
		{
			stbi__zhuffman z_codelength;
			byte lencodes [
			286 + 32 + 137]
			;
			byte codelength_sizes [
			19]
			;
			int i, n;
			int hlit = stbi__zreceive(a, 5) + 257;
			int hdist = stbi__zreceive(a, 5) + 1;
			int hclen = stbi__zreceive(a, 4) + 4;
			memset(codelength_sizes, 0, sizeof (codelength_sizes));
			for (i = 0;
				i < hclen;
				++i)
			{
				int s = stbi__zreceive(a, 3);
				codelength_sizes[length_dezigzag[i]] = (byte) s;
			}
			if (!stbi__zbuild_huffman(&z_codelength, codelength_sizes, 19)) return 0;
			n = 0;
			while (n < hlit + hdist)
			{
				int c = stbi__zhuffman_decode(a, &z_codelength);
				if (c < 0 || c >= 19) return stbi__err("bad codelengths");
				if (c < 16) lencodes[n++] = (byte) c;
				else if (c == 16)
				{
					c = stbi__zreceive(a, 2) + 3;
					memset(lencodes + n, lencodes[n - 1], c);
					n += c;
				}
				else if (c == 17)
				{
					c = stbi__zreceive(a, 3) + 3;
					memset(lencodes + n, 0, c);
					n += c;
				}
				else
				{
					assert(c == 18);
					c = stbi__zreceive(a, 7) + 11;
					memset(lencodes + n, 0, c);
					n += c;
				}
			}
			if (n != hlit + hdist) return stbi__err("bad codelengths");
			if (!stbi__zbuild_huffman(&a.z_length, lencodes, hlit)) return 0;
			if (!stbi__zbuild_huffman(&a.z_distance, lencodes + hlit, hdist)) return 0;
			return 1;
		}

		private unsafe static int stbi__parse_uncompressed_block(stbi__zbuf* a)
		{
			byte header [
			4]
			;
			int len, nlen, k;
			if (a.num_bits & 7) stbi__zreceive(a, a.num_bits & 7);
			k = 0;
			while (a.num_bits > 0)
			{
				header[k++] = (byte) (a.code_buffer & 255);
				a.code_buffer >>= 8;
				a.num_bits -= 8;
			}
			assert(a.num_bits == 0);
			while (k < 4) header[k++] = stbi__zget8(a);
			len = header[1]*256 + header[0];
			nlen = header[3]*256 + header[2];
			if (nlen != (len ^ 0xffff)) return stbi__err("zlib corrupt");
			if (a.zbuffer + len > a.zbuffer_end) return stbi__err("read past buffer");
			if (a.zout + len > a.zout_end) if (!stbi__zexpand(a, a.zout, len)) return 0;
			memcpy(a.zout, a.zbuffer, len);
			a.zbuffer += len;
			a.zout += len;
			return 1;
		}

		private unsafe static int stbi__parse_zlib_header(stbi__zbuf* a)
		{
			int cmf = stbi__zget8(a);
			int cm = cmf & 15;
			int flg = stbi__zget8(a);
			if ((cmf*256 + flg)%31 != 0) return stbi__err("bad zlib header");
			if (flg & 32) return stbi__err("no preset dict");
			if (cm != 8) return stbi__err("bad compression");
			return 1;
		}

		private unsafe static void stbi__init_zdefaults()
		{
			int i;
			for (i = 0;
				i <= 143;
				++i) stbi__zdefault_length[i] = 8;
			for (;
				i <= 255;
				++i) stbi__zdefault_length[i] = 9;
			for (;
				i <= 279;
				++i) stbi__zdefault_length[i] = 7;
			for (;
				i <= 287;
				++i) stbi__zdefault_length[i] = 8;
			for (i = 0;
				i <= 31;
				++i) stbi__zdefault_distance[i] = 5;
		}

		private unsafe static int stbi__parse_zlib(stbi__zbuf* a, int parse_header)
		{
			int final, type;
			if (parse_header) if (!stbi__parse_zlib_header(a)) return 0;
			a.num_bits = 0;
			a.code_buffer = 0;
			do
			{
				final = stbi__zreceive(a, 1);
				type = stbi__zreceive(a, 2);
				if (type == 0)
				{
					if (!stbi__parse_uncompressed_block(a)) return 0;
				}
				else if (type == 3)
				{
					return 0;
				}
				else
				{
					if (type == 1)
					{
						if (!stbi__zdefault_distance[31]) stbi__init_zdefaults();
						if (!stbi__zbuild_huffman(&a.z_length, stbi__zdefault_length, 288)) return 0;
						if (!stbi__zbuild_huffman(&a.z_distance, stbi__zdefault_distance, 32)) return 0;
					}
					else
					{
						if (!stbi__compute_huffman_codes(a)) return 0;
					}
					if (!stbi__parse_huffman_block(a)) return 0;
				}
			} while (!final);
			return 1;
		}

		private unsafe static int stbi__do_zlib(stbi__zbuf* a, string obuf, int olen, int exp, int parse_header)
		{
			a.zout_start = obuf;
			a.zout = obuf;
			a.zout_end = obuf + olen;
			a.z_expandable = exp;
			return stbi__parse_zlib(a, parse_header);
		}

		private unsafe static string stbi_zlib_decode_malloc(string buffer, int len, int* outlen)
		{
			return stbi_zlib_decode_malloc_guesssize(buffer, len, 16384, outlen);
		}

		private unsafe static int stbi_zlib_decode_buffer(string obuffer, int olen, string ibuffer, int ilen)
		{
			stbi__zbuf a;
			a.zbuffer = ibuffer;
			a.zbuffer_end = ibuffer + ilen;
			if (stbi__do_zlib(&a, obuffer, olen, 0, 1)) return (int) (a.zout - a.zout_start);
			else
				return -1;
		}

		private unsafe static int stbi_zlib_decode_noheader_buffer(string obuffer, int olen, string ibuffer, int ilen)
		{
			stbi__zbuf a;
			a.zbuffer = ibuffer;
			a.zbuffer_end = ibuffer + ilen;
			if (stbi__do_zlib(&a, obuffer, olen, 0, 0)) return (int) (a.zout - a.zout_start);
			else
				return -1;
		}

		private unsafe static stbi__pngchunk stbi__get_chunk_header(stbi__context s)
		{
			stbi__pngchunk c;
			c.length = stbi__get32be(s);
			c.type = stbi__get32be(s);
			return c;
		}

		private unsafe static int stbi__check_png_header(stbi__context s)
		{
			int i;
			for (i = 0;
				i < 8;
				++i) if (stbi__get8(s) != png_sig[i]) return stbi__err("bad png sig");
			return 1;
		}

		private unsafe static int stbi__paeth(int a, int b, int c)
		{
			int p = a + b - c;
			int pa = abs(p - a);
			int pb = abs(p - b);
			int pc = abs(p - c);
			if (pa <= pb && pa <= pc) return a;
			if (pb <= pc) return b;
			return c;
		}

		private unsafe static int stbi__compute_transparency(stbi__png* z, byte tc 
		[
		
		3],

		int out_n 
	)
		{
			stbi__context s = z.s;
			uint i, pixel_count = s.img_x*s.img_y;
			Pointer<byte> p = z.out;
			assert(out_n == 2 || out_n == 4);
			if (out_n == 2)
			{
				for (i = 0;
					i < pixel_count;
					++i)
				{
					p[1] = (p[0] == tc[0] ? 0 : 255);
					p += 2;
				}
			}
			else
			{
				for (i = 0;
					i < pixel_count;
					++i)
				{
					if (p[0] == tc[0] && p[1] == tc[1] && p[2] == tc[2]) p[3] = 0;
					p += 4;
				}
			}
			return 1;
		}

		private unsafe static int stbi__compute_transparency16(stbi__png* z, stbi__uint16 tc 
		[
		
		3],

		int out_n 
	)
		{
			stbi__context s = z.s;
			uint i, pixel_count = s.img_x*s.img_y;
			stbi__uint16* p = (stbi__uint16*) z.out;
			assert(out_n == 2 || out_n == 4);
			if (out_n == 2)
			{
				for (i = 0;
					i < pixel_count;
					++i)
				{
					p[1] = (p[0] == tc[0] ? 0 : 65535);
					p += 2;
				}
			}
			else
			{
				for (i = 0;
					i < pixel_count;
					++i)
				{
					if (p[0] == tc[0] && p[1] == tc[1] && p[2] == tc[2]) p[3] = 0;
					p += 4;
				}
			}
			return 1;
		}

		private unsafe static void stbi_set_unpremultiply_on_load(int flag_true_if_should_unpremultiply)
		{
			stbi__unpremultiply_on_load = flag_true_if_should_unpremultiply;
		}

		private unsafe static void stbi_convert_iphone_png_to_rgb(int flag_true_if_should_convert)
		{
			stbi__de_iphone_flag = flag_true_if_should_convert;
		}

		private unsafe static void stbi__de_iphone(stbi__png* z)
		{
			stbi__context s = z.s;
			uint i, pixel_count = s.img_x*s.img_y;
			Pointer<byte> p = z.out;
			if (s.img_out_n == 3)
			{
				for (i = 0;
					i < pixel_count;
					++i)
				{
					byte t = p[0];
					p[0] = p[2];
					p[2] = t;
					p += 3;
				}
			}
			else
			{
				assert(s.img_out_n == 4);
				if (stbi__unpremultiply_on_load)
				{
					for (i = 0;
						i < pixel_count;
						++i)
					{
						byte a = p[3];
						byte t = p[0];
						if (a)
						{
							p[0] = p[2]*255/a;
							p[1] = p[1]*255/a;
							p[2] = t*255/a;
						}
						else
						{
							p[0] = p[2];
							p[2] = t;
						}
						p += 4;
					}
				}
				else
				{
					for (i = 0;
						i < pixel_count;
						++i)
					{
						byte t = p[0];
						p[0] = p[2];
						p[2] = t;
						p += 4;
					}
				}
			}
		}

		private unsafe static int stbi__parse_png_file(stbi__png* z, int scan, int req_comp)
		{
			byte palette [
			1024],
			pal_img_n = 0;
			byte has_trans = 0, tc [
			3]
			;
			stbi__uint16
			tc16[3];
			uint ioff = 0, idata_limit = 0, i, pal_len = 0;
			int first = 1, k, interlace = 0, color = 0, is_iphone = 0;
			stbi__context s = z.s;
			z.expanded = null;
			z.idata = null;
			z.out=
			null;
			if (!stbi__check_png_header(s)) return 0;
			if (scan == STBI__SCAN_type) return 1;
			for (;
				;
				)
			{
				stbi__pngchunk c = stbi__get_chunk_header(s);
				switch (c.type)
				{
					case ((('C') << 24) + (('g') << 16) + (('B') << 8) + ('I')):
						is_iphone = 1;
						stbi__skip(s, c.length);
						break;
					case ((('I') << 24) + (('H') << 16) + (('D') << 8) + ('R')):
					{
						int comp, filter;
						if (!first) return stbi__err("multiple IHDR");
						first = 0;
						if (c.length != 13) return stbi__err("bad IHDR len");
						s.img_x = stbi__get32be(s);
						if (s.img_x > (1 << 24)) return stbi__err("too large");
						s.img_y = stbi__get32be(s);
						if (s.img_y > (1 << 24)) return stbi__err("too large");
						z.depth = stbi__get8(s);
						if (z.depth != 1 && z.depth != 2 && z.depth != 4 && z.depth != 8 && z.depth != 16)
							return stbi__err("1/2/4/8/16-bit only");
						color = stbi__get8(s);
						if (color > 6) return stbi__err("bad ctype");
						if (color == 3 && z.depth == 16) return stbi__err("bad ctype");
						if (color == 3) pal_img_n = 3;
						else if (color & 1) return stbi__err("bad ctype");
						comp = stbi__get8(s);
						if (comp) return stbi__err("bad comp method");
						filter = stbi__get8(s);
						if (filter) return stbi__err("bad filter method");
						interlace = stbi__get8(s);
						if (interlace > 1) return stbi__err("bad interlace method");
						if (!s.img_x || !s.img_y) return stbi__err("0-pixel image");
						if (!pal_img_n)
						{
							s.img_n = (color & 2 ? 3 : 1) + (color & 4 ? 1 : 0);
							if ((1 << 30)/s.img_x/s.img_n < s.img_y) return stbi__err("too large");
							if (scan == STBI__SCAN_header) return 1;
						}
						else
						{
							s.img_n = 1;
							if ((1 << 30)/s.img_x/4 < s.img_y) return stbi__err("too large");
						}
						break;
					}
					case ((('P') << 24) + (('L') << 16) + (('T') << 8) + ('E')):
					{
						if (first) return stbi__err("first not IHDR");
						if (c.length > 256*3) return stbi__err("invalid PLTE");
						pal_len = c.length/3;
						if (pal_len*3 != c.length) return stbi__err("invalid PLTE");
						for (i = 0;
							i < pal_len;
							++i)
						{
							palette[i*4 + 0] = stbi__get8(s);
							palette[i*4 + 1] = stbi__get8(s);
							palette[i*4 + 2] = stbi__get8(s);
							palette[i*4 + 3] = 255;
						}
						break;
					}
					case ((('t') << 24) + (('R') << 16) + (('N') << 8) + ('S')):
					{
						if (first) return stbi__err("first not IHDR");
						if (z.idata) return stbi__err("tRNS after IDAT");
						if (pal_img_n)
						{
							if (scan == STBI__SCAN_header)
							{
								s.img_n = 4;
								return 1;
							}
							if (pal_len == 0) return stbi__err("tRNS before PLTE");
							if (c.length > pal_len) return stbi__err("bad tRNS len");
							pal_img_n = 4;
							for (i = 0;
								i < c.length;
								++i) palette[i*4 + 3] = stbi__get8(s);
						}
						else
						{
							if (!(s.img_n & 1)) return stbi__err("tRNS with alpha");
							if (c.length != (uint) s.img_n*2) return stbi__err("bad tRNS len");
							has_trans = 1;
							if (z.depth == 16)
							{
								for (k = 0;
									k < s.img_n;
									++k) tc16[k] = stbi__get16be(s);
							}
							else
							{
								for (k = 0;
									k < s.img_n;
									++k) tc[k] = (byte) (stbi__get16be(s) & 255)*stbi__depth_scale_table[z.depth];
							}
						}
						break;
					}
					case ((('I') << 24) + (('D') << 16) + (('A') << 8) + ('T')):
					{
						if (first) return stbi__err("first not IHDR");
						if (pal_img_n && !pal_len) return stbi__err("no PLTE");
						if (scan == STBI__SCAN_header)
						{
							s.img_n = pal_img_n;
							return 1;
						}
						if ((int) (ioff + c.length) < (int) ioff) return 0;
						if (ioff + c.length > idata_limit)
						{
							uint idata_limit_old = idata_limit;
							Pointer<byte> p;
							if (idata_limit == 0) idata_limit = c.length > 4096 ? c.length : 4096;
							while (ioff + c.length > idata_limit) idata_limit *= 2;
							()
							(idata_limit_old);
							p = realloc(z.idata, idata_limit);
							if (p == null) return stbi__err("outofmem");
							z.idata = p;
						}
						if (!stbi__getn(s, z.idata + ioff, c.length)) return stbi__err("outofdata");
						ioff += c.length;
						break;
					}
					case ((('I') << 24) + (('E') << 16) + (('N') << 8) + ('D')):
					{
						uint raw_len, bpl;
						if (first) return stbi__err("first not IHDR");
						if (scan != STBI__SCAN_load) return 1;
						if (z.idata == null) return stbi__err("no IDAT");
						bpl = (s.img_x*z.depth + 7)/8;
						raw_len = bpl*s.img_y*s.img_n + s.img_y;
						z.expanded = stbi_zlib_decode_malloc_guesssize_headerflag((string) z.idata, ioff, raw_len, (int*) &raw_len,
							!is_iphone);
						if (z.expanded == null) return 0;
						free(z.idata);
						z.idata = null;
						if ((req_comp == s.img_n + 1 && req_comp != 3 && !pal_img_n) || has_trans) s.img_out_n = s.img_n + 1;
						else
							s.img_out_n = s.img_n;
						if (!stbi__create_png_image(z, z.expanded, raw_len, s.img_out_n, z.depth, color, interlace)) return 0;
						if (has_trans)
						{
							if (z.depth == 16)
							{
								if (!stbi__compute_transparency16(z, tc16, s.img_out_n)) return 0;
							}
							else
							{
								if (!stbi__compute_transparency(z, tc, s.img_out_n)) return 0;
							}
						}
						if (is_iphone && stbi__de_iphone_flag && s.img_out_n > 2) stbi__de_iphone(z);
						if (pal_img_n)
						{
							s.img_n = pal_img_n;
							s.img_out_n = pal_img_n;
							if (req_comp >= 3) s.img_out_n = req_comp;
							if (!stbi__expand_png_palette(z, palette, pal_len, s.img_out_n)) return 0;
						}
						free(z.expanded);
						z.expanded = null;
						return 1;
					}
					default:
						if (first) return stbi__err("first not IHDR");
						if ((c.type & (1 << 29)) == 0)
						{
							invalid_chunk[0] = ((byte) ((c.type >> 24) & 255));
							invalid_chunk[1] = ((byte) ((c.type >> 16) & 255));
							invalid_chunk[2] = ((byte) ((c.type >> 8) & 255));
							invalid_chunk[3] = ((byte) ((c.type >> 0) & 255));
							return stbi__err(invalid_chunk);
						}
						stbi__skip(s, c.length);
						break;
				}
				stbi__get32be(s);
			}
		}

		private unsafe static byte* stbi__do_png(stbi__png* p, int* x, int* y, int* n, int req_comp)
		{
			byte* result = null;
			if (req_comp < 0 || req_comp > 4) return ((byte*) (int) (stbi__err("bad req_comp") ? null : null));
			if (stbi__parse_png_file(p, STBI__SCAN_load, req_comp))
			{
				if (p.depth == 16)
				{
					if (!stbi__reduce_png(p))
					{
						return result;
					}
				}
				result = p.out;
				p.out=
				null;
				if (req_comp && req_comp != p.s.img_out_n)
				{
					result = stbi__convert_format(result, p.s.img_out_n, req_comp, p.s.img_x, p.s.img_y);
					p.s.img_out_n = req_comp;
					if (result == null) return result;
				}
				*x = p.s.img_x;
				*y = p.s.img_y;
				if (n) *n = p.s.img_n;
			}
			free(p.out);
			p.out=
			null;
			free(p.expanded);
			p.expanded = null;
			free(p.idata);
			p.idata = null;
			return result;
		}

		private unsafe static byte* stbi__png_load(stbi__context s, int* x, int* y, int* comp, int req_comp)
		{
			stbi__png p;
			p.s = s;
			return stbi__do_png(&p, x, y, comp, req_comp);
		}

		private unsafe static int stbi__png_test(stbi__context s)
		{
			int r;
			r = stbi__check_png_header(s);
			stbi__rewind(s);
			return r;
		}

		private unsafe static int stbi__png_info_raw(stbi__png* p, int* x, int* y, int* comp)
		{
			if (!stbi__parse_png_file(p, STBI__SCAN_header, 0))
			{
				stbi__rewind(p.s);
				return 0;
			}
			if (x) *x = p.s.img_x;
			if (y) *y = p.s.img_y;
			if (comp) *comp = p.s.img_n;
			return 1;
		}

		private unsafe static int stbi__png_info(stbi__context s, int* x, int* y, int* comp)
		{
			stbi__png p;
			p.s = s;
			return stbi__png_info_raw(&p, x, y, comp);
		}

		private unsafe static int stbi__bmp_test_raw(stbi__context s)
		{
			int r;
			int sz;
			if (stbi__get8(s) != 'B') return 0;
			if (stbi__get8(s) != 'M') return 0;
			stbi__get32le(s);
			stbi__get16le(s);
			stbi__get16le(s);
			stbi__get32le(s);
			sz = stbi__get32le(s);
			r = (sz == 12 || sz == 40 || sz == 56 || sz == 108 || sz == 124);
			return r;
		}

		private unsafe static int stbi__bmp_test(stbi__context s)
		{
			int r = stbi__bmp_test_raw(s);
			stbi__rewind(s);
			return r;
		}

		private unsafe static int stbi__high_bit(unsigned int z)
		{
			int n = 0;
			if (z == 0) return -1;
			if (z >= 0x10000) n += 16,
			z >>= 16;
			if (z >= 0x00100) n += 8,
			z >>= 8;
			if (z >= 0x00010) n += 4,
			z >>= 4;
			if (z >= 0x00004) n += 2,
			z >>= 2;
			if (z >= 0x00002) n += 1,
			z >>= 1;
			return n;
		}

		private unsafe static int stbi__bitcount(unsigned int a)
		{
			a = (a & 0x55555555) + ((a >> 1) & 0x55555555);
			a = (a & 0x33333333) + ((a >> 2) & 0x33333333);
			a = (a + (a >> 4)) & 0x0f0f0f0f;
			a = (a + (a >> 8));
			a = (a + (a >> 16));
			return a & 0xff;
		}

		private unsafe static int stbi__shiftsigned(int v, int shift, int bits)
		{
			int result;
			int z = 0;
			if (shift < 0) v <<= -shift;
			else
				v >>= shift;
			result = v;
			z = bits;
			while (z < 8)
			{
				result += v >> z;
				z += bits;
			}
			return result;
		}

		private unsafe static void* stbi__bmp_parse_header(stbi__context s, stbi__bmp_data* info)
		{
			int hsz;
			if (stbi__get8(s) != 'B' || stbi__get8(s) != 'M') return ((byte*) (int) (stbi__err("not BMP") ? null : null));
			stbi__get32le(s);
			stbi__get16le(s);
			stbi__get16le(s);
			info.offset = stbi__get32le(s);
			info.hsz = hsz = stbi__get32le(s);
			info.mr = info.mg = info.mb = info.ma = 0;
			if (hsz != 12 && hsz != 40 && hsz != 56 && hsz != 108 && hsz != 124)
				return ((byte*) (int) (stbi__err("unknown BMP") ? null : null));
			if (hsz == 12)
			{
				s.img_x = stbi__get16le(s);
				s.img_y = stbi__get16le(s);
			}
			else
			{
				s.img_x = stbi__get32le(s);
				s.img_y = stbi__get32le(s);
			}
			if (stbi__get16le(s) != 1) return ((byte*) (int) (stbi__err("bad BMP") ? null : null));
			info.bpp = stbi__get16le(s);
			if (info.bpp == 1) return ((byte*) (int) (stbi__err("monochrome") ? null : null));
			if (hsz != 12)
			{
				int compress = stbi__get32le(s);
				if (compress == 1 || compress == 2) return ((byte*) (int) (stbi__err("BMP RLE") ? null : null));
				stbi__get32le(s);
				stbi__get32le(s);
				stbi__get32le(s);
				stbi__get32le(s);
				stbi__get32le(s);
				if (hsz == 40 || hsz == 56)
				{
					if (hsz == 56)
					{
						stbi__get32le(s);
						stbi__get32le(s);
						stbi__get32le(s);
						stbi__get32le(s);
					}
					if (info.bpp == 16 || info.bpp == 32)
					{
						if (compress == 0)
						{
							if (info.bpp == 32)
							{
								info.mr = 0xffu << 16;
								info.mg = 0xffu << 8;
								info.mb = 0xffu << 0;
								info.ma = 0xffu << 24;
								info.all_a = 0;
							}
							else
							{
								info.mr = 31u << 10;
								info.mg = 31u << 5;
								info.mb = 31u << 0;
							}
						}
						else if (compress == 3)
						{
							info.mr = stbi__get32le(s);
							info.mg = stbi__get32le(s);
							info.mb = stbi__get32le(s);
							if (info.mr == info.mg && info.mg == info.mb)
							{
								return ((byte*) (int) (stbi__err("bad BMP") ? null : null));
							}
						}
						else
							return ((byte*) (int) (stbi__err("bad BMP") ? null : null));
					}
				}
				else
				{
					int i;
					if (hsz != 108 && hsz != 124) return ((byte*) (int) (stbi__err("bad BMP") ? null : null));
					info.mr = stbi__get32le(s);
					info.mg = stbi__get32le(s);
					info.mb = stbi__get32le(s);
					info.ma = stbi__get32le(s);
					stbi__get32le(s);
					for (i = 0;
						i < 12;
						++i) stbi__get32le(s);
					if (hsz == 124)
					{
						stbi__get32le(s);
						stbi__get32le(s);
						stbi__get32le(s);
						stbi__get32le(s);
					}
				}
			}
			return (void*) 1;
		}

		private unsafe static int stbi__tga_get_comp(int bits_per_pixel, int is_grey, int* is_rgb16)
		{
			if (is_rgb16) *is_rgb16 = 0;
			switch (bits_per_pixel)
			{
					case8:
					return STBI_grey;
					case16:
					if (is_grey) return STBI_grey_alpha;
					case15:
					if (is_rgb16) *is_rgb16 = 1;
					return STBI_rgb;
					case24:
					case32:
					return bits_per_pixel/8;
				default:
					return 0;
			}
		}

		private unsafe static int stbi__tga_info(stbi__context s, int* x, int* y, int* comp)
		{
			int tga_w, tga_h, tga_comp, tga_image_type, tga_bits_per_pixel, tga_colormap_bpp;
			int sz, tga_colormap_type;
			stbi__get8(s);
			tga_colormap_type = stbi__get8(s);
			if (tga_colormap_type > 1)
			{
				stbi__rewind(s);
				return 0;
			}
			tga_image_type = stbi__get8(s);
			if (tga_colormap_type == 1)
			{
				if (tga_image_type != 1 && tga_image_type != 9)
				{
					stbi__rewind(s);
					return 0;
				}
				stbi__skip(s, 4);
				sz = stbi__get8(s);
				if ((sz != 8) && (sz != 15) && (sz != 16) && (sz != 24) && (sz != 32))
				{
					stbi__rewind(s);
					return 0;
				}
				stbi__skip(s, 4);
				tga_colormap_bpp = sz;
			}
			else
			{
				if ((tga_image_type != 2) && (tga_image_type != 3) && (tga_image_type != 10) && (tga_image_type != 11))
				{
					stbi__rewind(s);
					return 0;
				}
				stbi__skip(s, 9);
				tga_colormap_bpp = 0;
			}
			tga_w = stbi__get16le(s);
			if (tga_w < 1)
			{
				stbi__rewind(s);
				return 0;
			}
			tga_h = stbi__get16le(s);
			if (tga_h < 1)
			{
				stbi__rewind(s);
				return 0;
			}
			tga_bits_per_pixel = stbi__get8(s);
			stbi__get8(s);
			if (tga_colormap_bpp != 0)
			{
				if ((tga_bits_per_pixel != 8) && (tga_bits_per_pixel != 16))
				{
					stbi__rewind(s);
					return 0;
				}
				tga_comp = stbi__tga_get_comp(tga_colormap_bpp, 0, null);
			}
			else
			{
				tga_comp = stbi__tga_get_comp(tga_bits_per_pixel, (tga_image_type == 3) || (tga_image_type == 11), null);
			}
			if (!tga_comp)
			{
				stbi__rewind(s);
				return 0;
			}
			if (x) *x = tga_w;
			if (y) *y = tga_h;
			if (comp) *comp = tga_comp;
			return 1;
		}

		private unsafe static int stbi__tga_test(stbi__context s)
		{
			int res = 0;
			int sz, tga_color_type;
			stbi__get8(s);
			tga_color_type = stbi__get8(s);
			if (tga_color_type > 1) gotoerrorEnd;
			sz = stbi__get8(s);
			if (tga_color_type == 1)
			{
				if (sz != 1 && sz != 9) gotoerrorEnd;
				stbi__skip(s, 4);
				sz = stbi__get8(s);
				if ((sz != 8) && (sz != 15) && (sz != 16) && (sz != 24) && (sz != 32)) gotoerrorEnd;
				stbi__skip(s, 4);
			}
			else
			{
				if ((sz != 2) && (sz != 3) && (sz != 10) && (sz != 11)) gotoerrorEnd;
				stbi__skip(s, 9);
			}
			if (stbi__get16le(s) < 1) gotoerrorEnd;
			if (stbi__get16le(s) < 1) gotoerrorEnd;
			sz = stbi__get8(s);
			if ((tga_color_type == 1) && (sz != 8) && (sz != 16)) gotoerrorEnd;
			if ((sz != 8) && (sz != 15) && (sz != 16) && (sz != 24) && (sz != 32)) gotoerrorEnd;
			res = 1;
			errorEnd:
			stbi__rewind(s);
			return res;
		}

		private unsafe void stbi__tga_read_rgb16(stbi__context s, Pointer<byte> out)
		{
			stbi__uint16 px = stbi__get16le(s);
			stbi__uint16 fiveBitMask = 31;
			int r = (px >> 10) & fiveBitMask;
			int g = (px >> 5) & fiveBitMask;
			int b = px & fiveBitMask;
			out[
			0]=
			(r*255)/31;
			out[
			1]=
			(g*255)/31;
			out[
			2]=
			(b*255)/31;
		}

		private unsafe static int stbi__psd_test(stbi__context s)
		{
			int r = (stbi__get32be(s) == 0x38425053);
			stbi__rewind(s);
			return r;
		}

		private unsafe static int stbi__pic_is4(stbi__context s, string str)
		{
			int i;
			for (i = 0;
				i < 4;
				++i) if (stbi__get8(s) != (byte) str[i]) return 0;
			return 1;
		}

		private unsafe static int stbi__pic_test_core(stbi__context s)
		{
			int i;
			if (!stbi__pic_is4(s, "\x53\x80\xF6\x34")) return 0;
			for (i = 0;
				i < 84;
				++i) stbi__get8(s);
			if (!stbi__pic_is4(s, "PICT")) return 0;
			return 1;
		}

		private unsafe static Pointer<byte> stbi__readval(stbi__context s, int channel, Pointer<byte> dest)
		{
			int mask = 0x80, i;
			for (i = 0;
				i < 4;
				++i, mask >>= 1)
			{
				if (channel & mask)
				{
					if (stbi__at_eof(s)) return ((byte*) (int) (stbi__err("bad file") ? null : null));
					dest[i] = stbi__get8(s);
				}
			}
			return dest;
		}

		private unsafe static void stbi__copyval(int channel, Pointer<byte> dest, Pointer<byte> src)
		{
			int mask = 0x80, i;
			for (i = 0;
				i < 4;
				++i, mask >>= 1) if (channel & mask) dest[i] = src[i];
		}

		private unsafe static Pointer<byte> stbi__pic_load_core(stbi__context s, int width, int height, int* comp,
			Pointer<byte> result)
		{
			int act_comp = 0, num_packets = 0, y, chained;
			stbi__pic_packet
			packets[10];
			do
			{
				stbi__pic_packet* packet;
				if (num_packets == sizeof (packets)/sizeof (packets[
				0]))
				return ((byte*) (int) (stbi__err("bad format") ? null : null));
				packet = &packets[num_packets++];
				chained = stbi__get8(s);
				packet.size = stbi__get8(s);
				packet.type = stbi__get8(s);
				packet.channel = stbi__get8(s);
				act_comp |= packet.channel;
				if (stbi__at_eof(s)) return ((byte*) (int) (stbi__err("bad file") ? null : null));
				if (packet.size != 8) return ((byte*) (int) (stbi__err("bad format") ? null : null));
			} while (chained);
			*comp = (act_comp & 0x10 ? 4 : 3);
			for (y = 0;
				y < height;
				++y)
			{
				int packet_idx;
				for (packet_idx = 0;
					packet_idx < num_packets;
					++packet_idx)
				{
					stbi__pic_packet* packet = &packets[packet_idx];
					Pointer<byte> dest = result + y*width*4;
					switch (packet.type)
					{
						default:
							return ((byte*) (int) (stbi__err("bad format") ? null : null));
							case0:
						{
							int x;
							for (x = 0;
								x < width;
								++x, dest += 4) if (!stbi__readval(s, packet.channel, dest)) return 0;
							break;
						}
							case1:
						{
							int left = width, i;
							while (left > 0)
							{
								byte count, value [
								4]
								;
								count = stbi__get8(s);
								if (stbi__at_eof(s)) return ((byte*) (int) (stbi__err("bad file") ? null : null));
								if (count > left) count = (byte) left;
								if (!stbi__readval(s, packet.channel, value)) return 0;
								for (i = 0;
									i < count;
									++i, dest += 4) stbi__copyval(packet.channel, dest, value);
								left -= count;
							}
						}
							break;
							case2:
						{
							int left = width;
							while (left > 0)
							{
								int count = stbi__get8(s), i;
								if (stbi__at_eof(s)) return ((byte*) (int) (stbi__err("bad file") ? null : null));
								if (count >= 128)
								{
									byte value [
									4]
									;
									if (count == 128) count = stbi__get16be(s);
									else
										count -= 127;
									if (count > left) return ((byte*) (int) (stbi__err("bad file") ? null : null));
									if (!stbi__readval(s, packet.channel, value)) return 0;
									for (i = 0;
										i < count;
										++i, dest += 4) stbi__copyval(packet.channel, dest, value);
								}
								else
								{
									++count;
									if (count > left) return ((byte*) (int) (stbi__err("bad file") ? null : null));
									for (i = 0;
										i < count;
										++i, dest += 4) if (!stbi__readval(s, packet.channel, dest)) return 0;
								}
								left -= count;
							}
							break;
						}
					}
				}
			}
			return result;
		}

		private unsafe static int stbi__pic_test(stbi__context s)
		{
			int r = stbi__pic_test_core(s);
			stbi__rewind(s);
			return r;
		}

		private unsafe static int stbi__gif_test_raw(stbi__context s)
		{
			int sz;
			if (stbi__get8(s) != 'G' || stbi__get8(s) != 'I' || stbi__get8(s) != 'F' || stbi__get8(s) != '8') return 0;
			sz = stbi__get8(s);
			if (sz != '9' && sz != '7') return 0;
			if (stbi__get8(s) != 'a') return 0;
			return 1;
		}

		private unsafe static int stbi__gif_test(stbi__context s)
		{
			int r = stbi__gif_test_raw(s);
			stbi__rewind(s);
			return r;
		}

		private unsafe static void stbi__gif_parse_colortable(stbi__context s, byte pal 
		[
		
		256]

		[
		
		4],

		int num_entries,
		int transp 
	)
		{
			int i;
			for (i = 0;
				i < num_entries;
				++i)
			{
				pal[i][2] = stbi__get8(s);
				pal[i][1] = stbi__get8(s);
				pal[i][0] = stbi__get8(s);
				pal[i][3] = transp == i ? 0 : 255;
			}
		}

		private unsafe static int stbi__gif_header(stbi__context s, stbi__gif* g, int* comp, int is_info)
		{
			byte version;
			if (stbi__get8(s) != 'G' || stbi__get8(s) != 'I' || stbi__get8(s) != 'F' || stbi__get8(s) != '8')
				return stbi__err("not GIF");
			version = stbi__get8(s);
			if (version != '7' && version != '9') return stbi__err("not GIF");
			if (stbi__get8(s) != 'a') return stbi__err("not GIF");
			stbi__g_failure_reason = "";
			g.w = stbi__get16le(s);
			g.h = stbi__get16le(s);
			g.flags = stbi__get8(s);
			g.bgindex = stbi__get8(s);
			g.ratio = stbi__get8(s);
			g.transparent = -1;
			if (comp != 0) *comp = 4;
			if (is_info) return 1;
			if (g.flags & 0x80) stbi__gif_parse_colortable(s, g.pal, 2 << (g.flags & 7), -1);
			return 1;
		}

		private unsafe static void stbi__out_gif_code(stbi__gif* g, stbi__uint16 code)
		{
			Pointer<byte> p, c;
			if (g.codes[code].prefix >= 0) stbi__out_gif_code(g, g.codes[code].prefix);
			if (g.cur_y >= g.max_y) return;
			p = &g.out[
			g.cur_x + g.cur_y]
			;
			c = &g.color_table[g.codes[code].suffix*4];
			if (c[3] >= 128)
			{
				p[0] = c[2];
				p[1] = c[1];
				p[2] = c[0];
				p[3] = c[3];
			}
			g.cur_x += 4;
			if (g.cur_x >= g.max_x)
			{
				g.cur_x = g.start_x;
				g.cur_y += g.step;
				while (g.cur_y >= g.max_y && g.parse > 0)
				{
					g.step = (1 << g.parse)*g.line_size;
					g.cur_y = g.start_y + (g.step >> 1);
					--g.parse;
				}
			}
		}

		private unsafe static Pointer<byte> stbi__process_gif_raster(stbi__context s, stbi__gif* g)
		{
			byte lzw_cs;
			stbi__int32 len, init_code;
			uint first;
			stbi__int32 codesize, codemask, avail, oldcode, bits, valid_bits, clear;
			stbi__gif_lzw* p;
			lzw_cs = stbi__get8(s);
			if (lzw_cs > 12) return null;
			clear = 1 << lzw_cs;
			first = 1;
			codesize = lzw_cs + 1;
			codemask = (1 << codesize) - 1;
			bits = 0;
			valid_bits = 0;
			for (init_code = 0;
				init_code < clear;
				init_code++)
			{
				g.codes[init_code].prefix = -1;
				g.codes[init_code].first = (byte) init_code;
				g.codes[init_code].suffix = (byte) init_code;
			}
			avail = clear + 2;
			oldcode = -1;
			len = 0;
			for (;
				;
				)
			{
				if (valid_bits < codesize)
				{
					if (len == 0)
					{
						len = stbi__get8(s);
						if (len == 0) return g.out;
					}
					--len;
					bits |= (stbi__int32) stbi__get8(s) << valid_bits;
					valid_bits += 8;
				}
				else
				{
					stbi__int32 code = bits & codemask;
					bits >>= codesize;
					valid_bits -= codesize;
					if (code == clear)
					{
						codesize = lzw_cs + 1;
						codemask = (1 << codesize) - 1;
						avail = clear + 2;
						oldcode = -1;
						first = 0;
					}
					else if (code == clear + 1)
					{
						stbi__skip(s, len);
						while ((len = stbi__get8(s)) > 0) stbi__skip(s, len);
						return g.out;
					}
					else if (code <= avail)
					{
						if (first) return ((byte*) (int) (stbi__err("no clear code") ? null : null));
						if (oldcode >= 0)
						{
							p = &g.codes[avail++];
							if (avail > 4096) return ((byte*) (int) (stbi__err("too many codes") ? null : null));
							p.prefix = (stbi__int16) oldcode;
							p.first = g.codes[oldcode].first;
							p.suffix = (code == avail) ? p.first : g.codes[code].first;
						}
						else if (code == avail) return ((byte*) (int) (stbi__err("illegal code in raster") ? null : null));
						stbi__out_gif_code(g, (stbi__uint16) code);
						if ((avail & codemask) == 0 && avail <= 0x0FFF)
						{
							codesize++;
							codemask = (1 << codesize) - 1;
						}
						oldcode = code;
					}
					else
					{
						return ((byte*) (int) (stbi__err("illegal code in raster") ? null : null));
					}
				}
			}
		}

		private unsafe static void stbi__fill_gif_background(stbi__gif* g, int x0, int y0, int x1, int y1)
		{
			int x, y;
			Pointer<byte> c = g.pal[g.bgindex];
			for (y = y0;
				y < y1;
				y += 4*g.w)
			{
				for (x = x0;
					x < x1;
					x += 4)
				{
					Pointer<byte> p = &g.out[
					y + x]
					;
					p[0] = c[2];
					p[1] = c[1];
					p[2] = c[0];
					p[3] = 0;
				}
			}
		}

		private unsafe static int stbi__gif_info(stbi__context s, int* x, int* y, int* comp)
		{
			return stbi__gif_info_raw(s, x, y, comp);
		}

		private unsafe static int stbi__bmp_info(stbi__context s, int* x, int* y, int* comp)
		{
			void* p;
			stbi__bmp_data info;
			info.all_a = 255;
			p = stbi__bmp_parse_header(s, &info);
			stbi__rewind(s);
			if (p == null) return 0;
			*x = s.img_x;
			*y = s.img_y;
			*comp = info.ma ? 4 : 3;
			return 1;
		}

		private unsafe static int stbi__psd_info(stbi__context s, int* x, int* y, int* comp)
		{
			int channelCount;
			if (stbi__get32be(s) != 0x38425053)
			{
				stbi__rewind(s);
				return 0;
			}
			if (stbi__get16be(s) != 1)
			{
				stbi__rewind(s);
				return 0;
			}
			stbi__skip(s, 6);
			channelCount = stbi__get16be(s);
			if (channelCount < 0 || channelCount > 16)
			{
				stbi__rewind(s);
				return 0;
			}
			*y = stbi__get32be(s);
			*x = stbi__get32be(s);
			if (stbi__get16be(s) != 8)
			{
				stbi__rewind(s);
				return 0;
			}
			if (stbi__get16be(s) != 3)
			{
				stbi__rewind(s);
				return 0;
			}
			*comp = 4;
			return 1;
		}

		private unsafe static int stbi__pic_info(stbi__context s, int* x, int* y, int* comp)
		{
			int act_comp = 0, num_packets = 0, chained;
			stbi__pic_packet
			packets[10];
			if (!stbi__pic_is4(s, "\x53\x80\xF6\x34"))
			{
				stbi__rewind(s);
				return 0;
			}
			stbi__skip(s, 88);
			*x = stbi__get16be(s);
			*y = stbi__get16be(s);
			if (stbi__at_eof(s))
			{
				stbi__rewind(s);
				return 0;
			}
			if ((*x) != 0 && (1 << 28)/(*x) < (*y))
			{
				stbi__rewind(s);
				return 0;
			}
			stbi__skip(s, 8);
			do
			{
				stbi__pic_packet* packet;
				if (num_packets == sizeof (packets)/sizeof (packets[
				0]))
				return 0;
				packet = &packets[num_packets++];
				chained = stbi__get8(s);
				packet.size = stbi__get8(s);
				packet.type = stbi__get8(s);
				packet.channel = stbi__get8(s);
				act_comp |= packet.channel;
				if (stbi__at_eof(s))
				{
					stbi__rewind(s);
					return 0;
				}
				if (packet.size != 8)
				{
					stbi__rewind(s);
					return 0;
				}
			} while (chained);
			*comp = (act_comp & 0x10 ? 4 : 3);
			return 1;
		}

		private unsafe static int stbi__pnm_test(stbi__context s)
		{
			char p, t;
			p = (char) stbi__get8(s);
			t = (char) stbi__get8(s);
			if (p != 'P' || (t != '5' && t != '6'))
			{
				stbi__rewind(s);
				return 0;
			}
			return 1;
		}

		private unsafe static int stbi__pnm_isspace(char c)
		{
			return c == ' ' || c == '\t' || c == '\n' || c == '\v' || c == '\f' || c == '\r';
		}

		private unsafe static void stbi__pnm_skip_whitespace(stbi__context s, string c)
		{
			for (;
				;
				)
			{
				while (!stbi__at_eof(s) && stbi__pnm_isspace(*c)) *c = (char) stbi__get8(s);
				if (stbi__at_eof(s) || *c != '#') break;
				while (!stbi__at_eof(s) && *c != '\n' && *c != '\r') *c = (char) stbi__get8(s);
			}
		}

		private unsafe static int stbi__pnm_isdigit(char c)
		{
			return c >= '0' && c <= '9';
		}

		private unsafe static int stbi__pnm_getinteger(stbi__context s, string c)
		{
			int value = 0;
			while (!stbi__at_eof(s) && stbi__pnm_isdigit(*c))
			{
				value = value*10 + (*c - '0');
				*c = (char) stbi__get8(s);
			}
			return value;
		}

		private unsafe static int stbi__pnm_info(stbi__context s, int* x, int* y, int* comp)
		{
			int maxv;
			char c, p, t;
			stbi__rewind(s);
			p = (char) stbi__get8(s);
			t = (char) stbi__get8(s);
			if (p != 'P' || (t != '5' && t != '6'))
			{
				stbi__rewind(s);
				return 0;
			}
			*comp = (t == '6') ? 3 : 1;
			c = (char) stbi__get8(s);
			stbi__pnm_skip_whitespace(s, &c);
			*x = stbi__pnm_getinteger(s, &c);
			stbi__pnm_skip_whitespace(s, &c);
			*y = stbi__pnm_getinteger(s, &c);
			stbi__pnm_skip_whitespace(s, &c);
			maxv = stbi__pnm_getinteger(s, &c);
			if (maxv > 255) return stbi__err("max value > 255");
			else
				return 1;
		}

		private unsafe static int stbi__info_main(stbi__context s, int* x, int* y, int* comp)
		{
			if (stbi__jpeg_info(s, x, y, comp)) return 1;
			if (stbi__png_info(s, x, y, comp)) return 1;
			if (stbi__gif_info(s, x, y, comp)) return 1;
			if (stbi__bmp_info(s, x, y, comp)) return 1;
			if (stbi__psd_info(s, x, y, comp)) return 1;
			if (stbi__pic_info(s, x, y, comp)) return 1;
			if (stbi__pnm_info(s, x, y, comp)) return 1;
			if (stbi__tga_info(s, x, y, comp)) return 1;
			return stbi__err("unknown image type");
		}

		private unsafe static int stbi_info_from_memory(Pointer<byte> buffer, int len, int* x, int* y, int* comp)
		{
			stbi__context s;
			stbi__start_mem(&s, buffer, len);
			return stbi__info_main(&s, x, y, comp);
		}

		private unsafe static int stbi_info_from_callbacks(stbi_io_callbacks c, void* user, int* x, int* y, int* comp)
		{
			stbi__context s;
			stbi__start_callbacks(&s, (stbi_io_callbacks) c, user);
			return stbi__info_main(&s, x, y, comp);
		}
	}
}