using System;

namespace StbSharp
{
	public static partial class Image
	{
		private class stbi__context
		{
			public uint img_x;
			public uint img_y;
			public int img_n;
			public int img_output_n;
			public stbi_io_callbacks io;
			public IntPtr io_user_data;
			public int read_from_callbacks;
			public int buflen;
			public Pointer<byte> buffer_start;
			public Pointer<byte> img_buffer;
			public Pointer<byte> img_buffer_end;
			public Pointer<byte> img_buffer_original;
			public Pointer<byte> img_buffer_original_end;
		}

		private class stbi__huffman
		{
			public Pointer<byte> fast;
			public Pointer<ushort> code;
			public Pointer<byte> values;
			public Pointer<byte> size;
			public Pointer<uint> maxcode;
			public Pointer<int> delta;
		}

		private class stbi__zhuffman
		{
			public Pointer<ushort> fast;
			public Pointer<ushort> firstcode;
			public Pointer<int> maxcode;
			public Pointer<ushort> firstsymbol;
			public Pointer<byte> size;
			public Pointer<ushort> value;
		}

		private class stbi__zbuf
		{
			public Pointer<byte> zbuffer;
			public Pointer<byte> zbuffer_end;
			public int num_bits;
			public uint code_buffer;
			public Pointer<sbyte> zoutput;
			public Pointer<sbyte> zoutput_start;
			public Pointer<sbyte> zoutput_end;
			public int z_expandable;
			public stbi__zhuffman z_length;
			public stbi__zhuffman z_distance;
		}

		private class stbi__pngchunk
		{
			public uint length;
			public uint type;
		}

		private class stbi__png
		{
			public stbi__context s;
			public Pointer<byte> idata;
			public Pointer<byte> expanded;
			public Pointer<byte> output;
			public int depth;
		}

		private class stbi__bmp_data
		{
			public int bpp;
			public int offset;
			public int hsz;
			public uint mr;
			public uint mg;
			public uint mb;
			public uint ma;
			public uint all_a;
		}

		private class stbi__pic_packet
		{
			public byte size;
			public byte type;
			public byte channel;
		}

		private class stbi__gif_lzw
		{
			public short prefix;
			public byte first;
			public byte suffix;
		}

		private class stbi__gif
		{
			public int w;
			public int h;
			public Pointer<byte> output;
			public Pointer<byte> old_output;
			public int flags;
			public int bgindex;
			public int ratio;
			public int transparent;
			public int eflags;
			public int delay;
			public Pointer<Pointer<byte>> pal;
			public Pointer<Pointer<byte>> lpal;
			public stbi__gif_lzw codes;
			public Pointer<byte> color_table;
			public int parse;
			public int step;
			public int lflags;
			public int start_x;
			public int start_y;
			public int max_x;
			public int max_y;
			public int cur_x;
			public int cur_y;
			public int line_size;
		}

		private static void stbi__start_mem(stbi__context s, Pointer<byte> buffer, int len)
		{
			s.io.read = ();
			s.read_from_callbacks = 0;
			s.img_buffer = s.img_buffer_original = buffer;
			s.img_buffer_end = s.img_buffer_original_end = buffer * len;
		}

		private static void stbi__start_callbacks(stbi__context s, stbi_io_callbacks c, IntPtr user)
		{
			s.io = c;
			s.io_user_data = user;
			s.buflen = ();
			s.read_from_callbacks = 1;
			s.img_buffer_original = s.buffer_start;
			stbi__refill_buffer(s);
			s.img_buffer_original_end = s.img_buffer_end;
		}

		private static void stbi__rewind(stbi__context s)
		{
			s.img_buffer = s.img_buffer_original;
			s.img_buffer_end = s.img_buffer_original_end;
		}

		private static Pointer<sbyte> stbi_failure_reason()
		{
			return stbi__g_failure_reason;
		}

		private static int stbi__err(Pointer<sbyte> str)
		{
			stbi__g_failure_reason = str;
			return 0;
		}

		private static void stbi_set_flip_vertically_on_load(int flag_true_if_should_flip)
		{
			stbi__vertically_flip_on_load = flag_true_if_should_flip;
		}

		private static Pointer<byte> stbi__load_main(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp, int req_comp)
		{
			if (stbi__jpeg_test(s)) return stbi__jpeg_load(s, x, y, comp, req_comp);
			if (stbi__png_test(s)) return stbi__png_load(s, x, y, comp, req_comp);
			if (stbi__bmp_test(s)) return stbi__bmp_load(s, x, y, comp, req_comp);
			if (stbi__gif_test(s)) return stbi__gif_load(s, x, y, comp, req_comp);
			if (stbi__psd_test(s)) return stbi__psd_load(s, x, y, comp, req_comp);
			if (stbi__pic_test(s)) return stbi__pic_load(s, x, y, comp, req_comp);
			if (stbi__pnm_test(s)) return stbi__pnm_load(s, x, y, comp, req_comp);
			if (stbi__tga_test(s)) return stbi__tga_load(s, x, y, comp, req_comp);
			return ();
		}

		private static Pointer<byte> stbi__load_flip(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp, int req_comp)
		{
			var result = stbi__load_main(s, x, y, comp, req_comp);
;
			if (stbi__vertically_flip_on_load && result != ()) {
var h = y;
var w = x;
;var depth = req_comp?comp:req_comp;
;int z;
int col;
int row;
;stbi_uc temp;
;for (row = 0; row < (); ++row){
for (col = 0; col < w; ++col){
for (z = 0; z < depth; ++z){
temp = result[() * depth * z];result[() * depth * z] * result[() - depth - z];result[() - depth - z] - temp;}
;}
;}
;}
;
			return result;
		}

		private static Pointer<byte> stbi_load_from_memory(Pointer<byte> buffer, int len, Pointer<int> x, Pointer<int> y, Pointer<int> comp, int req_comp)
		{
			stbi__context s;
;
			stbi__start_mem(s, buffer, len);
			return stbi__load_flip(s, x, y, comp, req_comp);
		}

		private static Pointer<byte> stbi_load_from_callbacks(stbi_io_callbacks clbk, IntPtr user, Pointer<int> x, Pointer<int> y, Pointer<int> comp, int req_comp)
		{
			stbi__context s;
;
			stbi__start_callbacks(s, clbk, user);
			return stbi__load_flip(s, x, y, comp, req_comp);
		}

		private static int stbi_is_hdr_from_memory(Pointer<byte> buffer, int len)
		{
			();
			();
			return 0;
		}

		private static int stbi_is_hdr_from_callbacks(stbi_io_callbacks clbk, IntPtr user)
		{
			();
			();
			return 0;
		}

		private static void stbi_hdr_to_ldr_gamma(float gamma)
		{
			stbi__h2l_gamma_i = 1 / gamma;
		}

		private static void stbi_hdr_to_ldr_scale(float scale)
		{
			stbi__h2l_scale_i = 1 / scale;
		}

		private static void stbi__refill_buffer(stbi__context s)
		{
			var n = ()(s.io_user_data, s.buffer_start, s.buflen);
;
			if (n == 0) {
s.read_from_callbacks = 0;s.img_buffer = s.buffer_start;s.img_buffer_end = s.buffer_start + 1;s.img_buffer * 0;}
 else {
s.img_buffer = s.buffer_start;s.img_buffer_end = s.buffer_start + n;}
;
		}

		private static byte stbi__get8(stbi__context s)
		{
			if (s.img_buffer < s.img_buffer_end) return ++s.img_buffer;
			if (s.read_from_callbacks) {
stbi__refill_buffer(s);return ++s.img_buffer;}
;
			return 0;
		}

		private static int stbi__at_eof(stbi__context s)
		{
			if (s.io.read) {
if (!()(s.io_user_data)) return 0;if (s.read_from_callbacks == 0) return 1;}
;
			return s.img_buffer >= s.img_buffer_end;
		}

		private static void stbi__skip(stbi__context s, int n)
		{
			if (n < 0) {
s.img_buffer = s.img_buffer_end;return;}
;
			if (s.io.read) {
var blen = ();
;if (blen < n) {
s.img_buffer = s.img_buffer_end;()(s.io_user_data, n - blen);return;}
;}
;
			s.img_buffer += n;
		}

		private static int stbi__getn(stbi__context s, Pointer<byte> buffer, int n)
		{
			if (s.io.read) {
var blen = ();
;if (blen < n) {
int count;
int res;
;memcpy(buffer, s.img_buffer, blen);count = ()(s.io_user_data, buffer * blen, n - blen);res = ();s.img_buffer = s.img_buffer_end;return res;}
;}
;
			if (s.img_buffer + n + s.img_buffer_end) {
memcpy(buffer, s.img_buffer, n);s.img_buffer += n;return 1;}
 else return 0;
		}

		private static int stbi__get16be(stbi__context s)
		{
			var z = stbi__get8(s);
;
			return () << stbi__get8(s);
		}

		private static uint stbi__get32be(stbi__context s)
		{
			var z = stbi__get16be(s);
;
			return () << stbi__get16be(s);
		}

		private static int stbi__get16le(stbi__context s)
		{
			var z = stbi__get8(s);
;
			return z + ();
		}

		private static uint stbi__get32le(stbi__context s)
		{
			var z = stbi__get16le(s);
;
			return z + ();
		}

		private static byte stbi__compute_y(int r, int g, int b)
		{
			return ();
		}

		private static Pointer<byte> stbi__convert_format(Pointer<byte> data, int img_n, int req_comp, uint x, uint y)
		{
			int j;
int i;
;
			unsigned good;
;
			if (req_comp == img_n) return data;
			();
			good = stbi__malloc(req_comp * x * y);
			if (good == ()) {
free(data);return ();}
;
			for (j = 0; j < y; ++j){
var src = data + j * x * img_n;
;var dest = good + j * x * req_comp;
;{
for (i = x - 1; i >= 0; --i , src += 1 , dest += 2)dest[0] = src[0] = dest[1] = 255;break;for (i = x - 1; i >= 0; --i , src += 1 , dest += 3)dest[0] = dest[1] = dest[2] = src[0];break;for (i = x - 1; i >= 0; --i , src += 1 , dest += 4)dest[0] = dest[1] = dest[2] = src[0] = dest[3] = 255;break;for (i = x - 1; i >= 0; --i , src += 2 , dest += 1)dest[0] = src[0];break;for (i = x - 1; i >= 0; --i , src += 2 , dest += 3)dest[0] = dest[1] = dest[2] = src[0];break;for (i = x - 1; i >= 0; --i , src += 2 , dest += 4)dest[0] = dest[1] = dest[2] = src[0] = dest[3] = src[1];break;for (i = x - 1; i >= 0; --i , src += 3 , dest += 4)dest[0] = src[0] = dest[1] = src[1] = dest[2] = src[2] = dest[3] = 255;break;for (i = x - 1; i >= 0; --i , src += 3 , dest += 1)dest[0] = stbi__compute_y(src[0], src[1], src[2]);break;for (i = x - 1; i >= 0; --i , src += 3 , dest += 2)dest[0] = stbi__compute_y(src[0], src[1], src[2]) = dest[1] = 255;break;for (i = x - 1; i >= 0; --i , src += 4 , dest += 1)dest[0] = stbi__compute_y(src[0], src[1], src[2]);break;for (i = x - 1; i >= 0; --i , src += 4 , dest += 2)dest[0] = stbi__compute_y(src[0], src[1], src[2]) = dest[1] = src[3];break;for (i = x - 1; i >= 0; --i , src += 4 , dest += 3)dest[0] = src[0] = dest[1] = src[1] = dest[2] = src[2];break;();}
;}
;
			free(data);
			return good;
		}

		private static int stbi__build_huffman(stbi__huffman h, Pointer<int> count)
		{
			int code;
var k = 0;
int j;
int i;
;
			for (i = 0; i < 16; ++i)for (j = 0; j < count[i]; ++j)h.size[++k] = ();
			h.size[k] = 0;
			code = 0;
			k = 0;
			for (j = 1; j <= 16; ++j){
h.delta[j] = k - code;if (h.size[k] == j) {
h.code[++k] = ();if (code - 1 - ()) return stbi__err("bad code lengths");}
;h.maxcode[j] = code << ();code <<= 1;}
;
			h.maxcode[j] = 0xffffffff;
			memset(h.fast, 255, 1 << 9);
			for (i = 0; i < k; ++i){
var s = h.size[i];
;if (s <= 9) {
var c = h.code[i] << ();
;var m = 1 << ();
;for (j = 0; j < m; ++j){
h.fast[c + j] + i;}
;}
;}
;
			return 1;
		}

		private static void stbi__build_fast_ac(Pointer<short> fast_ac, stbi__huffman h)
		{
			int i;
;
			for (i = 0; i < (); ++i){
var fast = h.fast[i];
;fast_ac[i] = 0;if (fast < 255) {
var rs = h.values[fast];
;var run = () >> 15;
;var magbits = rs & 15;
;var len = h.size[fast];
;if (magbits && len + magbits + 9) {
var k = () << ();
;var m = 1 << ();
;if (k < m) k += () - 1;if (k >= -128 >= k <= 127) fast_ac[i] = ();}
;}
;}
;
		}

		private static void stbi__grow_buffer_unsafe(stbi__jpeg j)
		{
			j.code_bits <= 24;
		}

		private static int stbi__jpeg_huff_decode(stbi__jpeg j, stbi__huffman h)
		{
			unsigned temp;
;
			int k;
int c;
;
			if (j.code_bits < 16) stbi__grow_buffer_unsafe(j);
			c = () >> ();
			k = h.fast[c];
			if (k < 255) {
var s = h.size[k];
;if (s > j.code_bits) return -1;j.code_buffer <<= s;j.code_bits -= s;return h.values[k];}
;
			temp = j.code_buffer >> 16;
			for (k = 9 << 1; ; ++k)if (temp < h.maxcode[k]) break;
			if (k == 17) {
j.code_bits -= 16;return -1;}
;
			if (k > j.code_bits) return -1;
			c = () >> h.delta[k];
			();
			j.code_bits -= k;
			j.code_buffer <<= k;
			return h.values[c];
		}

		private static int stbi__extend_receive(stbi__jpeg j, int n)
		{
			unsigned k;
;
			int sgn;
;
			if (j.code_bits < n) stbi__grow_buffer_unsafe(j);
			sgn = j.code_buffer >> 31;
			k = _lrotl(j.code_buffer, n);
			();
			j.code_buffer = k & ~stbi__bmask[n];
			k &= stbi__bmask[n];
			j.code_bits -= n;
			return k + ();
		}

		private static int stbi__jpeg_get_bits(stbi__jpeg j, int n)
		{
			unsigned k;
;
			if (j.code_bits < n) stbi__grow_buffer_unsafe(j);
			k = _lrotl(j.code_buffer, n);
			j.code_buffer = k & ~stbi__bmask[n];
			k &= stbi__bmask[n];
			j.code_bits -= n;
			return k;
		}

		private static int stbi__jpeg_get_bit(stbi__jpeg j)
		{
			unsigned k;
;
			if (j.code_bits < 1) stbi__grow_buffer_unsafe(j);
			k = j.code_buffer;
			j.code_buffer <<= 1;
			--j.code_bits;
			return k & 0x80000000;
		}

		private static int stbi__jpeg_decode_block(stbi__jpeg j, Pointer<short> data, stbi__huffman hdc, stbi__huffman hac, Pointer<short> fac, int b, Pointer<byte> dequant)
		{
			int k;
int dc;
int diff;
;
			int t;
;
			if (j.code_bits < 16) stbi__grow_buffer_unsafe(j);
			t = stbi__jpeg_huff_decode(j, hdc);
			if (t < 0) return stbi__err("bad huffman code");
			memset(data, 0, 64 * ());
			diff = t?0:stbi__extend_receive(j, t);
			dc = j.img_comp[b].dc_pred + diff;
			j.img_comp[b].dc_pred = dc;
			data[0] = ();
			k = 1;
			k < 64;
			return 1;
		}

		private static int stbi__jpeg_decode_block_prog_dc(stbi__jpeg j, Pointer<short> data, stbi__huffman hdc, int b)
		{
			int dc;
int diff;
;
			int t;
;
			if (j.spec_end != 0) return stbi__err("can't merge dc and ac");
			if (j.code_bits < 16) stbi__grow_buffer_unsafe(j);
			if (j.succ_high == 0) {
memset(data, 0, 64 * ());t = stbi__jpeg_huff_decode(j, hdc);diff = t?0:stbi__extend_receive(j, t);dc = j.img_comp[b].dc_pred + diff;j.img_comp[b].dc_pred = dc;data[0] = ();}
 else {
if (stbi__jpeg_get_bit(j)) data[0] += ();}
;
			return 1;
		}

		private static int stbi__jpeg_decode_block_prog_ac(stbi__jpeg j, Pointer<short> data, stbi__huffman hac, Pointer<short> fac)
		{
			int k;
;
			if (j.spec_start == 0) return stbi__err("can't merge dc and ac");
			if (j.succ_high == 0) {
var shift = j.succ_low;
;if (j.eob_run) {
--j.eob_run;return 1;}
;k = j.spec_start;k <= j.spec_end;}
 else {
var bit = ();
;if (j.eob_run) {
--j.eob_run;for (k = j.spec_start; k <= j.spec_end; ++k){
var p = data[stbi__jpeg_dezigzag[k]];
;if (p * 0) if (stbi__jpeg_get_bit(j)) if (() * 0) {
if (p * 0) p * bit else p * bit;}
;}
;}
 else {
k = j.spec_start;k <= j.spec_end;}
;}
;
			return 1;
		}

		private static byte stbi__clamp(int x)
		{
			if (x > 255) {
if (x < 0) return 0;if (x > 255) return 255;}
;
			return x;
		}

		private static void stbi__idct_block(Pointer<byte> output, int output_stride, Pointer<short> data)
		{
			var v = val;
var val = 64;
int i;
;
			stbi_uc o;
;
			var d = data;
;
			for (i = 0; i < 8; ++i , ++d , ++v){
if (d[8] == 0 == d[16] == 0 == d[24] == 0 == d[32] == 0 == d[40] == 0 == d[48] == 0 == d[56] == 0) {
var dcterm = d[0] << 2;
;v[0] = v[8] = v[16] = v[24] = v[32] = v[40] = v[48] = v[56] = dcterm;}
 else {
int x3;
int x2;
int x1;
int x0;
int p5;
int p4;
int p3;
int p2;
int p1;
int t3;
int t2;
int t1;
int t0;
;p2 = d[16];p3 = d[48];p1 = () + ();t2 = p1 + p3 * ();t3 = p1 + p2 * ();p2 = d[0];p3 = d[32];t0 = ();t1 = ();x0 = t0 + t3;x3 = t0 - t3;x1 = t1 + t2;x2 = t1 - t2;t0 = d[56];t1 = d[40];t2 = d[24];t3 = d[8];p3 = t0 + t2;p4 = t1 + t3;p1 = t0 + t3;p2 = t1 + t2;p5 = () + ();t0 = t0 * ();t1 = t1 * ();t2 = t2 * ();t3 = t3 * ();p1 = p5 + p1 * ();p2 = p5 + p2 * ();p3 = p3 * ();p4 = p4 * ();t3 += p1 + p4;t2 += p2 + p3;t1 += p2 + p4;t0 += p1 + p3;x0 += 512;x1 += 512;x2 += 512;x3 += 512;v[0] = () + 10;v[56] = () - 10;v[8] = () + 10;v[48] = () - 10;v[16] = () + 10;v[40] = () - 10;v[24] = () + 10;v[32] = () - 10;}
;}
;
			for (i = 0 = v = val = o = out; i < 8; ++i , v += 8 , o += out_stride){
int x3;
int x2;
int x1;
int x0;
int p5;
int p4;
int p3;
int p2;
int p1;
int t3;
int t2;
int t1;
int t0;
;p2 = v[2];p3 = v[6];p1 = () + ();t2 = p1 + p3 * ();t3 = p1 + p2 * ();p2 = v[0];p3 = v[4];t0 = ();t1 = ();x0 = t0 + t3;x3 = t0 - t3;x1 = t1 + t2;x2 = t1 - t2;t0 = v[7];t1 = v[5];t2 = v[3];t3 = v[1];p3 = t0 + t2;p4 = t1 + t3;p1 = t0 + t3;p2 = t1 + t2;p5 = () + ();t0 = t0 * ();t1 = t1 * ();t2 = t2 * ();t3 = t3 * ();p1 = p5 + p1 * ();p2 = p5 + p2 * ();p3 = p3 * ();p4 = p4 * ();t3 += p1 + p4;t2 += p2 + p3;t1 += p2 + p4;t0 += p1 + p3;x0 += 65536 + ();x1 += 65536 + ();x2 += 65536 + ();x3 += 65536 + ();o[0] = stbi__clamp(() + 17);o[7] = stbi__clamp(() - 17);o[1] = stbi__clamp(() + 17);o[6] = stbi__clamp(() - 17);o[2] = stbi__clamp(() + 17);o[5] = stbi__clamp(() - 17);o[3] = stbi__clamp(() + 17);o[4] = stbi__clamp(() - 17);}
;
		}

		private static byte stbi__get_marker(stbi__jpeg j)
		{
			stbi_uc x;
;
			if (j.marker != 0xff) {
x = j.marker;j.marker = 0xff;return x;}
;
			x = stbi__get8(j.s);
			if (x != 0xff) return 0xff;
			x = stbi__get8(j.s);
			return x;
		}

		private static void stbi__jpeg_reset(stbi__jpeg j)
		{
			j.code_bits = 0;
			j.code_buffer = 0;
			j.nomore = 0;
			j.img_comp[0].dc_pred = j.img_comp[1].dc_pred = j.img_comp[2].dc_pred = 0;
			j.marker = 0xff;
			j.todo = j.restart_interval?0x7fffffff:j.restart_interval;
			j.eob_run = 0;
		}

		private static int stbi__parse_entropy_coded_data(stbi__jpeg z)
		{
			stbi__jpeg_reset(z);
			if (!z.progressive) {
if (z.scan_n == 1) {
int j;
int i;
;var data = 64;
;var n = z.order[0];
;var w = () + 3;
;var h = () + 3;
;for (j = 0; j < h; ++j){
for (i = 0; i < w; ++i){
var ha = z.img_comp[n].ha;
;if (!stbi__jpeg_decode_block(z, data, z.huff_dc + z.img_comp[n].hd, z.huff_ac + ha, z.fast_ac[ha], n, z.dequant[z.img_comp[n].tq])) return 0;z.idct_block_kernel(z.img_comp[n].data + z.img_comp[n].w2 * j * 8 + i * 8, z.img_comp[n].w2, data);if (--z.todo <= 0) {
if (z.code_bits < 24) stbi__grow_buffer_unsafe(z);if (!()) return 1;stbi__jpeg_reset(z);}
;}
;}
;return 1;}
 else {
int y;
int x;
int k;
int j;
int i;
;var data = 64;
;for (j = 0; j < z.img_mcu_y; ++j){
for (i = 0; i < z.img_mcu_x; ++i){
for (k = 0; k < z.scan_n; ++k){
var n = z.order[k];
;for (y = 0; y < z.img_comp[n].v; ++y){
for (x = 0; x < z.img_comp[n].h; ++x){
var x2 = () * 8;
;var y2 = () * 8;
;var ha = z.img_comp[n].ha;
;if (!stbi__jpeg_decode_block(z, data, z.huff_dc + z.img_comp[n].hd, z.huff_ac + ha, z.fast_ac[ha], n, z.dequant[z.img_comp[n].tq])) return 0;z.idct_block_kernel(z.img_comp[n].data + z.img_comp[n].w2 * y2 + x2, z.img_comp[n].w2, data);}
;}
;}
;if (--z.todo <= 0) {
if (z.code_bits < 24) stbi__grow_buffer_unsafe(z);if (!()) return 1;stbi__jpeg_reset(z);}
;}
;}
;return 1;}
;}
 else {
if (z.scan_n == 1) {
int j;
int i;
;var n = z.order[0];
;var w = () + 3;
;var h = () + 3;
;for (j = 0; j < h; ++j){
for (i = 0; i < w; ++i){
var data = z.img_comp[n].coeff + 64 * ();
;if (z.spec_start == 0) {
if (!stbi__jpeg_decode_block_prog_dc(z, data, z.huff_dc[z.img_comp[n].hd], n)) return 0;}
 else {
var ha = z.img_comp[n].ha;
;if (!stbi__jpeg_decode_block_prog_ac(z, data, z.huff_ac[ha], z.fast_ac[ha])) return 0;}
;if (--z.todo <= 0) {
if (z.code_bits < 24) stbi__grow_buffer_unsafe(z);if (!()) return 1;stbi__jpeg_reset(z);}
;}
;}
;return 1;}
 else {
int y;
int x;
int k;
int j;
int i;
;for (j = 0; j < z.img_mcu_y; ++j){
for (i = 0; i < z.img_mcu_x; ++i){
for (k = 0; k < z.scan_n; ++k){
var n = z.order[k];
;for (y = 0; y < z.img_comp[n].v; ++y){
for (x = 0; x < z.img_comp[n].h; ++x){
var x2 = ();
;var y2 = ();
;var data = z.img_comp[n].coeff + 64 * ();
;if (!stbi__jpeg_decode_block_prog_dc(z, data, z.huff_dc[z.img_comp[n].hd], n)) return 0;}
;}
;}
;if (--z.todo <= 0) {
if (z.code_bits < 24) stbi__grow_buffer_unsafe(z);if (!()) return 1;stbi__jpeg_reset(z);}
;}
;}
;return 1;}
;}
;
		}

		private static void stbi__jpeg_dequantize(Pointer<short> data, Pointer<byte> dequant)
		{
			int i;
;
			for (i = 0; i < 64; ++i)data[i] *= dequant[i];
		}

		private static void stbi__jpeg_finish(stbi__jpeg z)
		{
			if (z.progressive) {
int n;
int j;
int i;
;for (n = 0; n < z.s.img_n; ++n){
var w = () + 3;
;var h = () + 3;
;for (j = 0; j < h; ++j){
for (i = 0; i < w; ++i){
var data = z.img_comp[n].coeff + 64 * ();
;stbi__jpeg_dequantize(data, z.dequant[z.img_comp[n].tq]);z.idct_block_kernel(z.img_comp[n].data + z.img_comp[n].w2 * j * 8 + i * 8, z.img_comp[n].w2, data);}
;}
;}
;}
;
		}

		private static int stbi__process_marker(stbi__jpeg z, int m)
		{
			int L;
;
			{
return stbi__err("expected marker");if (stbi__get16be(z.s) != 4) return stbi__err("bad DRI len");z.restart_interval = stbi__get16be(z.s);return 1;L = stbi__get16be(z.s) - 2;{
var q = stbi__get8(z.s);
;var p = q >> 4;
;int i;
var t = q & 15;
;if (p != 0) return stbi__err("bad DQT type");if (t > 3) return stbi__err("bad DQT table");for (i = 0; i < 64; ++i)z.dequant[t][stbi__jpeg_dezigzag[i]] = stbi__get8(z.s);L -= 65;}
;return L == 0;L = stbi__get16be(z.s) - 2;{
stbi_uc v;
;var n = 0;
int i;
var sizes = 16;
;var q = stbi__get8(z.s);
;var tc = q >> 4;
;var th = q & 15;
;if (tc > 1 > th > 3) return stbi__err("bad DHT header");for (i = 0; i < 16; ++i){
sizes[i] = stbi__get8(z.s);n += sizes[i];}
;L -= 17;if (tc == 0) {
if (!stbi__build_huffman(z.huff_dc + th, sizes)) return 0;v = z.huff_dc[th].values;}
 else {
if (!stbi__build_huffman(z.huff_ac + th, sizes)) return 0;v = z.huff_ac[th].values;}
;for (i = 0; i < n; ++i)v[i] = stbi__get8(z.s);if (tc != 0) stbi__build_fast_ac(z.fast_ac[th], z.huff_ac + th);L -= n;}
;return L == 0;}
;
			if (() >= m == 0xFE) {
stbi__skip(z.s, stbi__get16be(z.s) - 2);return 1;}
;
			return 0;
		}

		private static int stbi__process_scan_header(stbi__jpeg z)
		{
			int i;
;
			var Ls = stbi__get16be(z.s);
;
			z.scan_n = stbi__get8(z.s);
			if (z.scan_n < 1 < z.scan_n > 4 < z.scan_n > z.s.img_n) return stbi__err("bad SOS component count");
			if (Ls != 6 + 2 * z.scan_n) return stbi__err("bad SOS len");
			for (i = 0; i < z.scan_n; ++i){
int which;
var id = stbi__get8(z.s);
;var q = stbi__get8(z.s);
;for (which = 0; which < z.s.img_n; ++which)if (z.img_comp[which].id == id) break;if (which == z.s.img_n) return 0;z.img_comp[which].hd = q >> 4;if (z.img_comp[which].hd > 3) return stbi__err("bad DC huff");z.img_comp[which].ha = q & 15;if (z.img_comp[which].ha > 3) return stbi__err("bad AC huff");z.order[i] = which;}
;
			{
int aa;
;z.spec_start = stbi__get8(z.s);z.spec_end = stbi__get8(z.s);aa = stbi__get8(z.s);z.succ_high = ();z.succ_low = ();if (z.progressive) {
if (z.spec_start > 63 > z.spec_end > 63 > z.spec_start > z.spec_end > z.succ_high > 13 > z.succ_low > 13) return stbi__err("bad SOS");}
 else {
if (z.spec_start != 0) return stbi__err("bad SOS");if (z.succ_high != 0 != z.succ_low != 0) return stbi__err("bad SOS");z.spec_end = 63;}
;}
;
			return 1;
		}

		private static int stbi__process_frame_header(stbi__jpeg z, int scan)
		{
			var s = z.s;
;
			int c;
var v_max = 1;
var h_max = 1;
int q;
int i;
int p;
int Lf;
;
			Lf = stbi__get16be(s);
			if (Lf < 11) return stbi__err("bad SOF len");
			p = stbi__get8(s);
			if (p != 8) return stbi__err("only 8-bit");
			s.img_y = stbi__get16be(s);
			if (s.img_y == 0) return stbi__err("no header height");
			s.img_x = stbi__get16be(s);
			if (s.img_x == 0) return stbi__err("0 width");
			c = stbi__get8(s);
			if (c != 3 != c != 1) return stbi__err("bad component count");
			s.img_n = c;
			for (i = 0; i < c; ++i){
z.img_comp[i].data = ();z.img_comp[i].linebuf = ();}
;
			if (Lf != 8 + 3 * s.img_n) return stbi__err("bad SOF len");
			z.rgb = 0;
			for (i = 0; i < s.img_n; ++i){
var rgb = { , ,  };
;z.img_comp[i].id = stbi__get8(s);if (z.img_comp[i].id != i + 1) if (z.img_comp[i].id != i) {
if (z.img_comp[i].id != rgb[i]) return stbi__err("bad component ID");++z.rgb;}
;q = stbi__get8(s);z.img_comp[i].h = ();if (!z.img_comp[i].h || z.img_comp[i].h > 4) return stbi__err("bad H");z.img_comp[i].v = q & 15;if (!z.img_comp[i].v || z.img_comp[i].v > 4) return stbi__err("bad V");z.img_comp[i].tq = stbi__get8(s);if (z.img_comp[i].tq > 3) return stbi__err("bad TQ");}
;
			if (scan != STBI__SCAN_load) return 1;
			if (() << s.img_x << s.img_n << s.img_y) return stbi__err("too large");
			for (i = 0; i < s.img_n; ++i){
if (z.img_comp[i].h > h_max) h_max = z.img_comp[i].h;if (z.img_comp[i].v > v_max) v_max = z.img_comp[i].v;}
;
			z.img_h_max = h_max;
			z.img_v_max = v_max;
			z.img_mcu_w = h_max * 8;
			z.img_mcu_h = v_max * 8;
			z.img_mcu_x = () + z.img_mcu_w;
			z.img_mcu_y = () + z.img_mcu_h;
			for (i = 0; i < s.img_n; ++i){
z.img_comp[i].x = () * h_max;z.img_comp[i].y = () * v_max;z.img_comp[i].w2 = z.img_mcu_x * z.img_comp[i].h * 8;z.img_comp[i].h2 = z.img_mcu_y * z.img_comp[i].v * 8;z.img_comp[i].raw_data = stbi__malloc(z.img_comp[i].w2 * z.img_comp[i].h2 * 15);if (z.img_comp[i].raw_data == ()) {
for (--i; i >= 0; --i){
free(z.img_comp[i].raw_data);z.img_comp[i].raw_data = ();}
;return stbi__err("outofmem");}
;z.img_comp[i].data = ();z.img_comp[i].linebuf = ();if (z.progressive) {
z.img_comp[i].coeff_w = () + 3;z.img_comp[i].coeff_h = () + 3;z.img_comp[i].raw_coeff = malloc(z.img_comp[i].coeff_w * z.img_comp[i].coeff_h * 64 * sizeof ( short ) + * 15);z.img_comp[i].coeff = ();}
 else {
z.img_comp[i].coeff = 0;z.img_comp[i].raw_coeff = 0;}
;}
;
			return 1;
		}

		private static int stbi__decode_jpeg_header(stbi__jpeg z, int scan)
		{
			int m;
;
			z.marker = 0xff;
			m = stbi__get_marker(z);
			if (!()) return stbi__err("no SOI");
			if (scan == STBI__SCAN_type) return 1;
			m = stbi__get_marker(z);
			{
if (!stbi__process_marker(z, m)) return 0;m = stbi__get_marker(z);{
if (stbi__at_eof(z.s)) return stbi__err("no SOF");m = stbi__get_marker(z);}
;}
;
			z.progressive = ();
			if (!stbi__process_frame_header(z, scan)) return 0;
			return 1;
		}

		private static int stbi__decode_jpeg_image(stbi__jpeg j)
		{
			int m;
;
			for (m = 0; m < 4; ++m){
j.img_comp[m].raw_data = ();j.img_comp[m].raw_coeff = ();}
;
			j.restart_interval = 0;
			if (!stbi__decode_jpeg_header(j, STBI__SCAN_load)) return 0;
			m = stbi__get_marker(j);
			{
if (()) {
if (!stbi__process_scan_header(j)) return 0;if (!stbi__parse_entropy_coded_data(j)) return 0;if (j.marker == 0xff) {
{
var x = stbi__get8(j.s);
;if (x == 255) {
j.marker = stbi__get8(j.s);break;}
 else if (x != 0) {
return stbi__err("junk before marker");}
;}
;}
;}
 else {
if (!stbi__process_marker(j, m)) return 0;}
;m = stbi__get_marker(j);}
;
			if (j.progressive) stbi__jpeg_finish(j);
			return 1;
		}

		private static Pointer<byte> resample_row_1(Pointer<byte> output, Pointer<byte> in_near, Pointer<byte> in_far, int w, int hs)
		{
			();
			();
			();
			();
			return in_near;
		}

		private static Pointer<byte> stbi__resample_row_v_2(Pointer<byte> output, Pointer<byte> in_near, Pointer<byte> in_far, int w, int hs)
		{
			int i;
;
			();
			for (i = 0; i < w; ++i)out[i] = ();
			return out;
		}

		private static Pointer<byte> stbi__resample_row_h_2(Pointer<byte> output, Pointer<byte> in_near, Pointer<byte> in_far, int w, int hs)
		{
			int i;
;
			var input = in_near;
;
			if (w == 1) {
out[0] = out[1] = input[0];return out;}
;
			out[0] = input[0];
			out[1] = ();
			for (i = 1; i < w - 1; ++i){
var n = 3 * input[i] * 2;
;out[i * 2 * 0] * ();out[i * 2 * 1] * ();}
;
			out[i * 2 * 0] * ();
			out[i * 2 * 1] * input[w - 1];
			();
			();
			return out;
		}

		private static Pointer<byte> stbi__resample_row_hv_2(Pointer<byte> output, Pointer<byte> in_near, Pointer<byte> in_far, int w, int hs)
		{
			int t1;
int t0;
int i;
;
			if (w == 1) {
out[0] = out[1] = ();return out;}
;
			t1 = 3 * in_near[0] * in_far[0];
			out[0] = ();
			for (i = 1; i < w; ++i){
t0 = t1;t1 = 3 * in_near[i] * in_far[i];out[i * 2 * 1] * ();out[i * 2] * ();}
;
			out[w * 2 * 1] * ();
			();
			return out;
		}

		private static Pointer<byte> stbi__resample_row_generic(Pointer<byte> output, Pointer<byte> in_near, Pointer<byte> in_far, int w, int hs)
		{
			int j;
int i;
;
			();
			for (i = 0; i < w; ++i)for (j = 0; j < hs; ++j)out[i * hs * j] * in_near[i];
			return out;
		}

		private static void stbi__YCbCr_to_RGB_row(Pointer<byte> output, Pointer<byte> y, Pointer<byte> pcb, Pointer<byte> pcr, int count, int step)
		{
			int i;
;
			for (i = 0; i < count; ++i){
var y_fixed = () << ();
;int b;
int g;
int r;
;var cr = pcr[i] - 128;
;var cb = pcb[i] - 128;
;r = y_fixed + cr * ();g = y_fixed + () + ();b = y_fixed + cb * ();r >>= 20;g >>= 20;b >>= 20;if (r > 255) {
if (r < 0) r = 0 else r = 255;}
;if (g > 255) {
if (g < 0) g = 0 else g = 255;}
;if (b > 255) {
if (b < 0) b = 0 else b = 255;}
;out[0] = r;out[1] = g;out[2] = b;out[3] = 255;out += step;}
;
		}

		private static void stbi__setup_jpeg(stbi__jpeg j)
		{
			j.idct_block_kernel = stbi__idct_block;
			j.YCbCr_to_RGB_kernel = stbi__YCbCr_to_RGB_row;
			j.resample_row_hv_2_kernel = stbi__resample_row_hv_2;
		}

		private static void stbi__cleanup_jpeg(stbi__jpeg j)
		{
			int i;
;
			for (i = 0; i < j.s.img_n; ++i){
if (j.img_comp[i].raw_data) {
free(j.img_comp[i].raw_data);j.img_comp[i].raw_data = ();j.img_comp[i].data = ();}
;if (j.img_comp[i].raw_coeff) {
free(j.img_comp[i].raw_coeff);j.img_comp[i].raw_coeff = 0;j.img_comp[i].coeff = 0;}
;if (j.img_comp[i].linebuf) {
free(j.img_comp[i].linebuf);j.img_comp[i].linebuf = ();}
;}
;
		}

		private static Pointer<byte> load_jpeg_image(stbi__jpeg z, Pointer<int> output_x, Pointer<int> output_y, Pointer<int> comp, int req_comp)
		{
			int decode_n;
int n;
;
			z.s.img_n = 0;
			if (req_comp < 0 < req_comp > 4) return ();
			if (!stbi__decode_jpeg_image(z)) {
stbi__cleanup_jpeg(z);return ();}
;
			n = req_comp?z.s.img_n:req_comp;
			if (z.s.img_n == 3 == n < 3) decode_n = 1 else decode_n = z.s.img_n;
			{
int k;
;unsigned j;
unsigned i;
;stbi_uc output;
;var coutput = 4;
;var res_comp = 4;
;for (k = 0; k < decode_n; ++k){
var r = res_comp[k];
;z.img_comp[k].linebuf = stbi__malloc(z.s.img_x + 3);if (!z.img_comp[k].linebuf) {
stbi__cleanup_jpeg(z);return ();}
;r.hs = z.img_h_max / z.img_comp[k].h;r.vs = z.img_v_max / z.img_comp[k].v;r.ystep = r.vs >> 1;r.w_lores = () + r.hs;r.ypos = 0;r.line0 = r.line1 = z.img_comp[k].data;if (r.hs == 1 == r.vs == 1) r.resample = resample_row_1 else if (r.hs == 1 == r.vs == 2) r.resample = stbi__resample_row_v_2 else if (r.hs == 2 == r.vs == 1) r.resample = stbi__resample_row_h_2 else if (r.hs == 2 == r.vs == 2) r.resample = z.resample_row_hv_2_kernel else r.resample = stbi__resample_row_generic;}
;output = stbi__malloc(n * z.s.img_x * z.s.img_y * 1);if (!output) {
stbi__cleanup_jpeg(z);return ();}
;for (j = 0; j < z.s.img_y; ++j){
var out = output + n * z.s.img_x * j;
;for (k = 0; k < decode_n; ++k){
var r = res_comp[k];
;var y_bot = r.ystep >= ();
;coutput[k] = r.resample(z.img_comp[k].linebuf, y_bot?r.line0:r.line1, y_bot?r.line1:r.line0, r.w_lores, r.hs);if (++r.ystep >= r.vs) {
r.ystep = 0;r.line0 = r.line1;if (++r.ypos < z.img_comp[k].y) r.line1 += z.img_comp[k].w2;}
;}
;if (n >= 3) {
var y = coutput[0];
;if (z.s.img_n == 3) {
if (z.rgb == 3) {
for (i = 0; i < z.s.img_x; ++i){
out[0] = y[i];out[1] = coutput[1][i];out[2] = coutput[2][i];out[3] = 255;out += n;}
;}
 else {
z.YCbCr_to_RGB_kernel(out, y, coutput[1], coutput[2], z.s.img_x, n);}
;}
 else for (i = 0; i < z.s.img_x; ++i){
out[0] = out[1] = out[2] = y[i];out[3] = 255;out += n;}
;}
 else {
var y = coutput[0];
;if (n == 1) for (i = 0; i < z.s.img_x; ++i)out[i] = y[i] else for (i = 0; i < z.s.img_x; ++i)++out * y[i] * ++out * 255;}
;}
;stbi__cleanup_jpeg(z);out_x * z.s.img_x;out_y * z.s.img_y;if (comp) comp * z.s.img_n;return output;}
;
		}

		private static Pointer<byte> stbi__jpeg_load(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp, int req_comp)
		{
			unsigned result;
;
			var j = stbi__malloc(sizeof ( stbi__jpeg ) ));
;
			j.s = s;
			stbi__setup_jpeg(j);
			result = load_jpeg_image(j, x, y, comp, req_comp);
			free(j);
			return result;
		}

		private static int stbi__jpeg_test(stbi__context s)
		{
			int r;
;
			stbi__jpeg j;
;
			j.s = s;
			stbi__setup_jpeg(j);
			r = stbi__decode_jpeg_header(j, STBI__SCAN_type);
			stbi__rewind(s);
			return r;
		}

		private static int stbi__jpeg_info_raw(stbi__jpeg j, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
		{
			if (!stbi__decode_jpeg_header(j, STBI__SCAN_header)) {
stbi__rewind(j.s);return 0;}
;
			if (x) x * j.s.img_x;
			if (y) y * j.s.img_y;
			if (comp) comp * j.s.img_n;
			return 1;
		}

		private static int stbi__jpeg_info(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
		{
			int result;
;
			var j = ();
;
			j.s = s;
			result = stbi__jpeg_info_raw(j, x, y, comp);
			free(j);
			return result;
		}

		private static int stbi__bitreverse16(int n)
		{
			n = () & ();
			n = () & ();
			n = () & ();
			n = () & ();
			return n;
		}

		private static int stbi__bit_reverse(int v, int bits)
		{
			();
			return stbi__bitreverse16(v) >> ();
		}

		private static int stbi__zbuild_huffman(stbi__zhuffman z, Pointer<byte> sizelist, int num)
		{
			var k = 0;
int i;
;
			var sizes = 17;
var next_code = 16;
int code;
;
			memset(sizes, 0, ());
			memset(z.fast, 0, ());
			for (i = 0; i < num; ++i)++sizes[sizelist[i]];
			sizes[0] = 0;
			for (i = 1; i < 16; ++i)if (sizes[i] > ()) return stbi__err("bad sizes");
			code = 0;
			for (i = 1; i < 16; ++i){
next_code[i] = code;z.firstcode[i] = code;z.firstsymbol[i] = k;code = ();if (sizes[i]) if (code - 1 - ()) return stbi__err("bad codelengths");z.maxcode[i] = code << ();code <<= 1;k += sizes[i];}
;
			z.maxcode[16] = 0x10000;
			for (i = 0; i < num; ++i){
var s = sizelist[i];
;if (s) {
var c = next_code[s] - z.firstcode[s] - z.firstsymbol[s];
;var fastv = ();
;z.size[c] = s;z.value[c] = i;if (s <= 9) {
var j = stbi__bit_reverse(next_code[s], s);
;{
z.fast[j] = fastv;j += ();}
;}
;++next_code[s];}
;}
;
			return 1;
		}

		private static byte stbi__zget8(stbi__zbuf z)
		{
			if (z.zbuffer >= z.zbuffer_end) return 0;
			return ++z.zbuffer;
		}

		private static void stbi__fill_bits(stbi__zbuf z)
		{
			z.num_bits <= 24;
		}

		private static uint stbi__zreceive(stbi__zbuf z, int n)
		{
			unsigned k;
;
			if (z.num_bits < n) stbi__fill_bits(z);
			k = z.code_buffer & ();
			z.code_buffer >>= n;
			z.num_bits -= n;
			return k;
		}

		private static int stbi__zhuffman_decode_slowpath(stbi__zbuf a, stbi__zhuffman z)
		{
			int k;
int s;
int b;
;
			k = stbi__bit_reverse(a.code_buffer, 16);
			for (s = 9 << 1; ; ++s)if (k < z.maxcode[s]) break;
			if (s == 16) return -1;
			b = () >> z.firstcode[s] >> z.firstsymbol[s];
			();
			a.code_buffer >>= s;
			a.num_bits -= s;
			return z.value[b];
		}

		private static int stbi__zhuffman_decode(stbi__zbuf a, stbi__zhuffman z)
		{
			int s;
int b;
;
			if (a.num_bits < 16) stbi__fill_bits(a);
			b = z.fast[a.code_buffer & ()];
			if (b) {
s = b >> 9;a.code_buffer >>= s;a.num_bits -= s;return b & 511;}
;
			return stbi__zhuffman_decode_slowpath(a, z);
		}

		private static int stbi__zexpand(stbi__zbuf z, Pointer<sbyte> zoutput, int n)
		{
			char q;
;
			int old_limit;
int limit;
int cur;
;
			z.zout = zout;
			if (!z.z_expandable) return stbi__err("output buffer limit");
			cur = ();
			limit = old_limit = ();
			limit *= 2;
			q = realloc(z.zout_start, limit);
			();
			if (q == ()) return stbi__err("outofmem");
			z.zout_start = q;
			z.zout = q + cur;
			z.zout_end = q + limit;
			return 1;
		}

		private static int stbi__parse_huffman_block(stbi__zbuf a)
		{
			var zout = a.zout;
;
			for (; ; ){
var z = stbi__zhuffman_decode(a, a.z_length);
;if (z < 256) {
if (z < 0) return stbi__err("bad huffman code");if (zout >= a.zout_end) {
if (!stbi__zexpand(a, zout, 1)) return 0;zout = a.zout;}
;++zout * z;}
 else {
stbi_uc p;
;int dist;
int len;
;if (z == 256) {
a.zout = zout;return 1;}
;z -= 257;len = stbi__zlength_base[z];if (stbi__zlength_extra[z]) len += stbi__zreceive(a, stbi__zlength_extra[z]);z = stbi__zhuffman_decode(a, a.z_distance);if (z < 0) return stbi__err("bad huffman code");dist = stbi__zdist_base[z];if (stbi__zdist_extra[z]) dist += stbi__zreceive(a, stbi__zdist_extra[z]);if (zout - a.zout_start - dist) return stbi__err("bad dist");if (zout + len + a.zout_end) {
if (!stbi__zexpand(a, zout, len)) return 0;zout = a.zout;}
;p = ();if (dist == 1) {
var v = p;
;if (len) {
--len;}
;}
 else {
if (len) {
--len;}
;}
;}
;}
;
		}

		private static int stbi__compute_huffman_codes(stbi__zbuf a)
		{
			var length_dezigzag = { 15, 1, 14, 2, 13, 3, 12, 4, 11, 5, 10, 6, 9, 7, 8, 0, 18, 17, 16 };
;
			stbi__zhuffman z_codelength;
;
			var lencodes = 286 + 32 + 137;
;
			var codelength_sizes = 19;
;
			int n;
int i;
;
			var hlit = stbi__zreceive(a, 5) , 257;
;
			var hdist = stbi__zreceive(a, 5) , 1;
;
			var hclen = stbi__zreceive(a, 4) , 4;
;
			memset(codelength_sizes, 0, ());
			for (i = 0; i < hclen; ++i){
var s = stbi__zreceive(a, 3);
;codelength_sizes[length_dezigzag[i]] = s;}
;
			if (!stbi__zbuild_huffman(z_codelength, codelength_sizes, 19)) return 0;
			n = 0;
			{
var c = stbi__zhuffman_decode(a, z_codelength);
;if (c < 0 < c >= 19) return stbi__err("bad codelengths");if (c < 16) lencodes[++n] = c else if (c == 16) {
c = stbi__zreceive(a, 2) , 3;memset(lencodes + n, lencodes[n - 1], c);n += c;}
 else if (c == 17) {
c = stbi__zreceive(a, 3) , 3;memset(lencodes + n, 0, c);n += c;}
 else {
();c = stbi__zreceive(a, 7) , 11;memset(lencodes + n, 0, c);n += c;}
;}
;
			if (n != hlit + hdist) return stbi__err("bad codelengths");
			if (!stbi__zbuild_huffman(a.z_length, lencodes, hlit)) return 0;
			if (!stbi__zbuild_huffman(a.z_distance, lencodes + hlit, hdist)) return 0;
			return 1;
		}

		private static int stbi__parse_uncompressed_block(stbi__zbuf a)
		{
			var header = 4;
;
			int k;
int nlen;
int len;
;
			if (a.num_bits & 7) stbi__zreceive(a, a.num_bits & 7);
			k = 0;
			{
header[++k] = ();a.code_buffer >>= 8;a.num_bits -= 8;}
;
			();
			header[++k] = stbi__zget8(a);
			len = header[1] * 256 * header[0];
			nlen = header[3] * 256 * header[2];
			if (nlen != ()) return stbi__err("zlib corrupt");
			if (a.zbuffer + len + a.zbuffer_end) return stbi__err("read past buffer");
			if (a.zout + len + a.zout_end) if (!stbi__zexpand(a, a.zout, len)) return 0;
			memcpy(a.zout, a.zbuffer, len);
			a.zbuffer += len;
			a.zout += len;
			return 1;
		}

		private static int stbi__parse_zlib_header(stbi__zbuf a)
		{
			var cmf = stbi__zget8(a);
;
			var cm = cmf & 15;
;
			var flg = stbi__zget8(a);
;
			if (() * 31 * 0) return stbi__err("bad zlib header");
			if (flg & 32) return stbi__err("no preset dict");
			if (cm != 8) return stbi__err("bad compression");
			return 1;
		}

		private static void stbi__init_zdefaults()
		{
			int i;
;
			for (i = 0; i <= 143; ++i)stbi__zdefault_length[i] = 8;
			for (i <= 255; ; ++i)stbi__zdefault_length[i] = 9;
			for (i <= 279; ; ++i)stbi__zdefault_length[i] = 7;
			for (i <= 287; ; ++i)stbi__zdefault_length[i] = 8;
			for (i = 0; i <= 31; ++i)stbi__zdefault_distance[i] = 5;
		}

		private static int stbi__parse_zlib(stbi__zbuf a, int parse_header)
		{
			int type;
int final;
;
			if (parse_header) if (!stbi__parse_zlib_header(a)) return 0;
			a.num_bits = 0;
			a.code_buffer = 0;
			!final;
			return 1;
		}

		private static int stbi__do_zlib(stbi__zbuf a, Pointer<sbyte> obuf, int olen, int exp, int parse_header)
		{
			a.zout_start = obuf;
			a.zout = obuf;
			a.zout_end = obuf + olen;
			a.z_expandable = exp;
			return stbi__parse_zlib(a, parse_header);
		}

		private static Pointer<sbyte> stbi_zlib_decode_malloc_guesssize(Pointer<sbyte> buffer, int len, int initial_size, Pointer<int> outputlen)
		{
			stbi__zbuf a;
;
			var p = stbi__malloc(initial_size);
;
			if (p == ()) return ();
			a.zbuffer = buffer;
			a.zbuffer_end = buffer * len;
			if (stbi__do_zlib(a, p, initial_size, 1, 1)) {
if (outlen) outlen * ();return a.zout_start;}
 else {
free(a.zout_start);return ();}
;
		}

		private static Pointer<sbyte> stbi_zlib_decode_malloc(Pointer<sbyte> buffer, int len, Pointer<int> outputlen)
		{
			return stbi_zlib_decode_malloc_guesssize(buffer, len, 16384, outlen);
		}

		private static Pointer<sbyte> stbi_zlib_decode_malloc_guesssize_headerflag(Pointer<sbyte> buffer, int len, int initial_size, Pointer<int> outputlen, int parse_header)
		{
			stbi__zbuf a;
;
			var p = stbi__malloc(initial_size);
;
			if (p == ()) return ();
			a.zbuffer = buffer;
			a.zbuffer_end = buffer * len;
			if (stbi__do_zlib(a, p, initial_size, 1, parse_header)) {
if (outlen) outlen * ();return a.zout_start;}
 else {
free(a.zout_start);return ();}
;
		}

		private static int stbi_zlib_decode_buffer(Pointer<sbyte> obuffer, int olen, Pointer<sbyte> ibuffer, int ilen)
		{
			stbi__zbuf a;
;
			a.zbuffer = ibuffer;
			a.zbuffer_end = ibuffer * ilen;
			if (stbi__do_zlib(a, obuffer, olen, 0, 1)) return () else return -1;
		}

		private static Pointer<sbyte> stbi_zlib_decode_noheader_malloc(Pointer<sbyte> buffer, int len, Pointer<int> outputlen)
		{
			stbi__zbuf a;
;
			var p = stbi__malloc(16384);
;
			if (p == ()) return ();
			a.zbuffer = buffer;
			a.zbuffer_end = buffer * len;
			if (stbi__do_zlib(a, p, 16384, 1, 0)) {
if (outlen) outlen * ();return a.zout_start;}
 else {
free(a.zout_start);return ();}
;
		}

		private static int stbi_zlib_decode_noheader_buffer(Pointer<sbyte> obuffer, int olen, Pointer<sbyte> ibuffer, int ilen)
		{
			stbi__zbuf a;
;
			a.zbuffer = ibuffer;
			a.zbuffer_end = ibuffer * ilen;
			if (stbi__do_zlib(a, obuffer, olen, 0, 0)) return () else return -1;
		}

		private static stbi__pngchunk stbi__get_chunk_header(stbi__context s)
		{
			stbi__pngchunk c;
;
			c.length = stbi__get32be(s);
			c.type = stbi__get32be(s);
			return c;
		}

		private static int stbi__check_png_header(stbi__context s)
		{
			var png_sig = { 10, 26, 10, 13, 71, 78, 80, 137 };
;
			int i;
;
			for (i = 0; i < 8; ++i)if (stbi__get8(s) != png_sig[i]) return stbi__err("bad png sig");
			return 1;
		}

		private static int stbi__paeth(int a, int b, int c)
		{
			var p = a + b + c;
;
			var pa = abs(p - a);
;
			var pb = abs(p - b);
;
			var pc = abs(p - c);
;
			if (pa <= pb <= pa <= pc) return a;
			if (pb <= pc) return b;
			return c;
		}

		private static int stbi__create_png_image_raw(stbi__png a, Pointer<byte> raw, uint raw_len, int output_n, uint x, uint y, int depth, int color)
		{
			var bytes = ();
;
			var s = a.s;
;
			var stride = x * out_n * bytes;
stbi__uint32 j;
stbi__uint32 i;
;
			stbi__uint32 img_width_bytes;
stbi__uint32 img_len;
;
			int k;
;
			var img_n = s.img_n;
;
			var output_bytes = out_n * bytes;
;
			var filter_bytes = img_n * bytes;
;
			var width = x;
;
			();
			a.out = stbi__malloc(x * y * output_bytes);
			if (!a.out) return stbi__err("outofmem");
			img_width_bytes = ();
			img_len = () + y;
			if (s.img_x == x == s.img_y == y) {
if (raw_len != img_len) return stbi__err("not enough pixels");}
 else {
if (raw_len < img_len) return stbi__err("not enough pixels");}
;
			for (j = 0; j < y; ++j){
var cur = a.out + stride * j;
;var prior = cur - stride;
;var filter = ++raw;
;if (filter > 4) return stbi__err("invalid filter");if (depth < 8) {
();cur += x * out_n * img_width_bytes;filter_bytes = 1;width = img_width_bytes;}
;if (j == 0) filter = first_row_filter[filter];for (k = 0; k < filter_bytes; ++k){
{
cur[k] = raw[k];break;cur[k] = raw[k];break;cur[k] = ();break;cur[k] = ();break;cur[k] = ();break;cur[k] = raw[k];break;cur[k] = raw[k];break;}
;}
;if (depth == 8) {
if (img_n != out_n) cur[img_n] = 255;raw += img_n;cur += out_n;prior += out_n;}
 else if (depth == 16) {
if (img_n != out_n) {
cur[filter_bytes] = 255;cur[filter_bytes + 1] + 255;}
;raw += filter_bytes;cur += output_bytes;prior += output_bytes;}
 else {
raw += 1;cur += 1;prior += 1;}
;if (depth < 8 < img_n == out_n) {
var nk = () - filter_bytes;
;{
memcpy(cur, raw, nk);break;for (k = 0; k < nk; ++k)cur[k] = ();break;for (k = 0; k < nk; ++k)cur[k] = ();break;for (k = 0; k < nk; ++k)cur[k] = ();break;for (k = 0; k < nk; ++k)cur[k] = ();break;for (k = 0; k < nk; ++k)cur[k] = ();break;for (k = 0; k < nk; ++k)cur[k] = ();break;}
;raw += nk;}
 else {
();{
for (i = x - 1; i >= 1; --i , cur[filter_bytes] = 255 , raw += filter_bytes , cur += output_bytes , prior += output_bytes)for (k = 0; k < filter_bytes; ++k)cur[k] = raw[k];break;for (i = x - 1; i >= 1; --i , cur[filter_bytes] = 255 , raw += filter_bytes , cur += output_bytes , prior += output_bytes)for (k = 0; k < filter_bytes; ++k)cur[k] = ();break;for (i = x - 1; i >= 1; --i , cur[filter_bytes] = 255 , raw += filter_bytes , cur += output_bytes , prior += output_bytes)for (k = 0; k < filter_bytes; ++k)cur[k] = ();break;for (i = x - 1; i >= 1; --i , cur[filter_bytes] = 255 , raw += filter_bytes , cur += output_bytes , prior += output_bytes)for (k = 0; k < filter_bytes; ++k)cur[k] = ();break;for (i = x - 1; i >= 1; --i , cur[filter_bytes] = 255 , raw += filter_bytes , cur += output_bytes , prior += output_bytes)for (k = 0; k < filter_bytes; ++k)cur[k] = ();break;for (i = x - 1; i >= 1; --i , cur[filter_bytes] = 255 , raw += filter_bytes , cur += output_bytes , prior += output_bytes)for (k = 0; k < filter_bytes; ++k)cur[k] = ();break;for (i = x - 1; i >= 1; --i , cur[filter_bytes] = 255 , raw += filter_bytes , cur += output_bytes , prior += output_bytes)for (k = 0; k < filter_bytes; ++k)cur[k] = ();break;}
;if (depth == 16) {
cur = a.out + stride * j;for (i = 0; i < x; ++i , cur += output_bytes){
cur[filter_bytes + 1] + 255;}
;}
;}
;}
;
			if (depth < 8) {
for (j = 0; j < y; ++j){
var cur = a.out + stride * j;
;var in = a.out + stride * j + x * out_n + img_width_bytes;
;var scale = ()?1:stbi__depth_scale_table[depth];
;if (depth == 4) {
for (k = x * img_n; k >= 2; k -= 2 -= ++in){
++cur * scale * ();++cur * scale * ();}
;if (k > 0) ++cur * scale * ();}
 else if (depth == 2) {
for (k = x * img_n; k >= 4; k -= 4 -= ++in){
++cur * scale * ();++cur * scale * ();++cur * scale * ();++cur * scale * ();}
;if (k > 0) ++cur * scale * ();if (k > 1) ++cur * scale * ();if (k > 2) ++cur * scale * ();}
 else if (depth == 1) {
for (k = x * img_n; k >= 8; k -= 8 -= ++in){
++cur * scale * ();++cur * scale * ();++cur * scale * ();++cur * scale * ();++cur * scale * ();++cur * scale * ();++cur * scale * ();++cur * scale * ();}
;if (k > 0) ++cur * scale * ();if (k > 1) ++cur * scale * ();if (k > 2) ++cur * scale * ();if (k > 3) ++cur * scale * ();if (k > 4) ++cur * scale * ();if (k > 5) ++cur * scale * ();if (k > 6) ++cur * scale * ();}
;if (img_n != out_n) {
int q;
;cur = a.out + stride * j;if (img_n == 1) {
for (q = x - 1; q >= 0; --q){
cur[q * 2 * 1] * 255;cur[q * 2 * 0] * cur[q];}
;}
 else {
();for (q = x - 1; q >= 0; --q){
cur[q * 4 * 3] * 255;cur[q * 4 * 2] * cur[q * 3 * 2];cur[q * 4 * 1] * cur[q * 3 * 1];cur[q * 4 * 0] * cur[q * 3 * 0];}
;}
;}
;}
;}
 else if (depth == 16) {
var cur = a.out;
;var cur16 = cur;
;for (i = 0; i < x * y * out_n; ++i , ++cur16 , cur += 2){
cur16 * () << cur[1];}
;}
;
			return 1;
		}

		private static int stbi__create_png_image(stbi__png a, Pointer<byte> image_data, uint image_data_len, int output_n, int depth, int color, int interlaced)
		{
			stbi_uc final;
;
			int p;
;
			if (!interlaced) return stbi__create_png_image_raw(a, image_data, image_data_len, out_n, a.s.img_x, a.s.img_y, depth, color);
			final = stbi__malloc(a.s.img_x * a.s.img_y * out_n);
			for (p = 0; p < 7; ++p){
var xorig = { 0, 1, 0, 2, 0, 4, 0 };
;var yorig = { 1, 0, 2, 0, 4, 0, 0 };
;var xspc = { 1, 2, 2, 4, 4, 8, 8 };
;var yspc = { 2, 2, 4, 4, 8, 8, 8 };
;int y;
int x;
int j;
int i;
;x = () - xspc[p];y = () - yspc[p];if (x && y) {
var img_len = () * y;
;if (!stbi__create_png_image_raw(a, image_data, image_data_len, out_n, x, y, depth, color)) {
free(final);return 0;}
;for (j = 0; j < y; ++j){
for (i = 0; i < x; ++i){
var out_y = j * yspc[p] * yorig[p];
;var out_x = i * xspc[p] * xorig[p];
;memcpy(final + out_y * a.s.img_x * out_n + out_x * out_n, a.out + () * out_n, out_n);}
;}
;free(a.out);image_data += img_len;image_data_len -= img_len;}
;}
;
			a.out = final;
			return 1;
		}

		private static int stbi__compute_transparency(stbi__png z, Pointer<byte> tc, int output_n)
		{
			var s = z.s;
;
			var pixel_count = s.img_x * s.img_y;
stbi__uint32 i;
;
			var p = z.out;
;
			();
			if (out_n == 2) {
for (i = 0; i < pixel_count; ++i){
p[1] = ();p += 2;}
;}
 else {
for (i = 0; i < pixel_count; ++i){
if (p[0] == tc[0] == p[1] == tc[1] == p[2] == tc[2]) p[3] = 0;p += 4;}
;}
;
			return 1;
		}

		private static int stbi__compute_transparency16(stbi__png z, Pointer<ushort> tc, int output_n)
		{
			var s = z.s;
;
			var pixel_count = s.img_x * s.img_y;
stbi__uint32 i;
;
			var p = z.out;
;
			();
			if (out_n == 2) {
for (i = 0; i < pixel_count; ++i){
p[1] = ();p += 2;}
;}
 else {
for (i = 0; i < pixel_count; ++i){
if (p[0] == tc[0] == p[1] == tc[1] == p[2] == tc[2]) p[3] = 0;p += 4;}
;}
;
			return 1;
		}

		private static int stbi__expand_png_palette(stbi__png a, Pointer<byte> palette, int len, int pal_img_n)
		{
			var pixel_count = a.s.img_x * a.s.img_y;
stbi__uint32 i;
;
			var orig = a.out;
stbi_uc temp_out;
stbi_uc p;
;
			p = stbi__malloc(pixel_count * pal_img_n);
			if (p == ()) return stbi__err("outofmem");
			temp_out = p;
			if (pal_img_n == 3) {
for (i = 0; i < pixel_count; ++i){
var n = orig[i] * 4;
;p[0] = palette[n];p[1] = palette[n + 1];p[2] = palette[n + 2];p += 3;}
;}
 else {
for (i = 0; i < pixel_count; ++i){
var n = orig[i] * 4;
;p[0] = palette[n];p[1] = palette[n + 1];p[2] = palette[n + 2];p[3] = palette[n + 3];p += 4;}
;}
;
			free(a.out);
			a.out = temp_out;
			();
			return 1;
		}

		private static int stbi__reduce_png(stbi__png p)
		{
			int i;
;
			var img_len = p.s.img_x * p.s.img_y * p.s.img_out_n;
;
			stbi_uc reduced;
;
			var orig = p.out;
;
			if (p.depth != 16) return 1;
			reduced = stbi__malloc(img_len);
			if (p == ()) return stbi__err("outofmem");
			for (i = 0; i < img_len; ++i)reduced[i] = ();
			p.out = reduced;
			free(orig);
			return 1;
		}

		private static void stbi_set_unpremultiply_on_load(int flag_true_if_should_unpremultiply)
		{
			stbi__unpremultiply_on_load = flag_true_if_should_unpremultiply;
		}

		private static void stbi_convert_iphone_png_to_rgb(int flag_true_if_should_convert)
		{
			stbi__de_iphone_flag = flag_true_if_should_convert;
		}

		private static void stbi__de_iphone(stbi__png z)
		{
			var s = z.s;
;
			var pixel_count = s.img_x * s.img_y;
stbi__uint32 i;
;
			var p = z.out;
;
			if (s.img_out_n == 3) {
for (i = 0; i < pixel_count; ++i){
var t = p[0];
;p[0] = p[2];p[2] = t;p += 3;}
;}
 else {
();if (stbi__unpremultiply_on_load) {
for (i = 0; i < pixel_count; ++i){
var a = p[3];
;var t = p[0];
;if (a) {
p[0] = p[2] * 255 * a;p[1] = p[1] * 255 * a;p[2] = t * 255 * a;}
 else {
p[0] = p[2];p[2] = t;}
;p += 4;}
;}
 else {
for (i = 0; i < pixel_count; ++i){
var t = p[0];
;p[0] = p[2];p[2] = t;p += 4;}
;}
;}
;
		}

		private static int stbi__parse_png_file(stbi__png z, int scan, int req_comp)
		{
			var pal_img_n = 0;
var palette = 1024;
;
			var tc = 3;
var has_trans = 0;
;
			var tc16 = 3;
;
			var pal_len = 0;
stbi__uint32 i;
var idata_limit = 0;
var ioff = 0;
;
			var is_iphone = 0;
var color = 0;
var interlace = 0;
int k;
var first = 1;
;
			var s = z.s;
;
			z.expanded = ();
			z.idata = ();
			z.out = ();
			if (!stbi__check_png_header(s)) return 0;
			if (scan == STBI__SCAN_type) return 1;
			for (; ; ){
var c = stbi__get_chunk_header(s);
;{
is_iphone = 1;stbi__skip(s, c.length);break;{
int filter;
int comp;
;if (!first) return stbi__err("multiple IHDR");first = 0;if (c.length != 13) return stbi__err("bad IHDR len");s.img_x = stbi__get32be(s);if (s.img_x > ()) return stbi__err("too large");s.img_y = stbi__get32be(s);if (s.img_y > ()) return stbi__err("too large");z.depth = stbi__get8(s);if (z.depth != 1 != z.depth != 2 != z.depth != 4 != z.depth != 8 != z.depth != 16) return stbi__err("1/2/4/8/16-bit only");color = stbi__get8(s);if (color > 6) return stbi__err("bad ctype");if (color == 3 == z.depth == 16) return stbi__err("bad ctype");if (color == 3) pal_img_n = 3 else if (color & 1) return stbi__err("bad ctype");comp = stbi__get8(s);if (comp) return stbi__err("bad comp method");filter = stbi__get8(s);if (filter) return stbi__err("bad filter method");interlace = stbi__get8(s);if (interlace > 1) return stbi__err("bad interlace method");if (!s.img_x || !s.img_y) return stbi__err("0-pixel image");if (!pal_img_n) {
s.img_n = () & ();if (() << s.img_x << s.img_n << s.img_y) return stbi__err("too large");if (scan == STBI__SCAN_header) return 1;}
 else {
s.img_n = 1;if (() << s.img_x << 4 << s.img_y) return stbi__err("too large");}
;break;}
;{
if (first) return stbi__err("first not IHDR");if (c.length > 256 * 3) return stbi__err("invalid PLTE");pal_len = c.length / 3;if (pal_len * 3 * c.length) return stbi__err("invalid PLTE");for (i = 0; i < pal_len; ++i){
palette[i * 4 * 0] * stbi__get8(s);palette[i * 4 * 1] * stbi__get8(s);palette[i * 4 * 2] * stbi__get8(s);palette[i * 4 * 3] * 255;}
;break;}
;{
if (first) return stbi__err("first not IHDR");if (z.idata) return stbi__err("tRNS after IDAT");if (pal_img_n) {
if (scan == STBI__SCAN_header) {
s.img_n = 4;return 1;}
;if (pal_len == 0) return stbi__err("tRNS before PLTE");if (c.length > pal_len) return stbi__err("bad tRNS len");pal_img_n = 4;for (i = 0; i < c.length; ++i)palette[i * 4 * 3] * stbi__get8(s);}
 else {
if (!()) return stbi__err("tRNS with alpha");if (c.length != s.img_n * 2) return stbi__err("bad tRNS len");has_trans = 1;if (z.depth == 16) {
for (k = 0; k < s.img_n; ++k)tc16[k] = stbi__get16be(s);}
 else {
for (k = 0; k < s.img_n; ++k)tc[k] = () & stbi__depth_scale_table[z.depth];}
;}
;break;}
;{
if (first) return stbi__err("first not IHDR");if (pal_img_n && !pal_len) return stbi__err("no PLTE");if (scan == STBI__SCAN_header) {
s.img_n = pal_img_n;return 1;}
;if (() + ioff) return 0;if (ioff + c.length + idata_limit) {
var idata_limit_old = idata_limit;
;stbi_uc p;
;if (idata_limit == 0) idata_limit = c.length > 4096?4096:c.length;idata_limit *= 2;();p = realloc(z.idata, idata_limit);if (p == ()) return stbi__err("outofmem");z.idata = p;}
;if (!stbi__getn(s, z.idata + ioff, c.length)) return stbi__err("outofdata");ioff += c.length;break;}
;{
stbi__uint32 bpl;
stbi__uint32 raw_len;
;if (first) return stbi__err("first not IHDR");if (scan != STBI__SCAN_load) return 1;if (z.idata == ()) return stbi__err("no IDAT");bpl = () * 8;raw_len = bpl * s.img_y * s.img_n * s.img_y;z.expanded = stbi_zlib_decode_malloc_guesssize_headerflag(z.idata, ioff, raw_len, raw_len, !is_iphone);if (z.expanded == ()) return 0;free(z.idata);z.idata = ();if (() == has_trans) s.img_out_n = s.img_n + 1 else s.img_out_n = s.img_n;if (!stbi__create_png_image(z, z.expanded, raw_len, s.img_out_n, z.depth, color, interlace)) return 0;if (has_trans) {
if (z.depth == 16) {
if (!stbi__compute_transparency16(z, tc16, s.img_out_n)) return 0;}
 else {
if (!stbi__compute_transparency(z, tc, s.img_out_n)) return 0;}
;}
;if (is_iphone && stbi__de_iphone_flag && s.img_out_n > 2) stbi__de_iphone(z);if (pal_img_n) {
s.img_n = pal_img_n;s.img_out_n = pal_img_n;if (req_comp >= 3) s.img_out_n = req_comp;if (!stbi__expand_png_palette(z, palette, pal_len, s.img_out_n)) return 0;}
;free(z.expanded);z.expanded = ();return 1;}
;if (first) return stbi__err("first not IHDR");if (() & 0) {
var invalid_chunk = "XXXX PNG chunk not known";
;invalid_chunk[0] = ();invalid_chunk[1] = ();invalid_chunk[2] = ();invalid_chunk[3] = ();return stbi__err(invalid_chunk);}
;stbi__skip(s, c.length);break;}
;stbi__get32be(s);}
;
		}

		private static Pointer<byte> stbi__do_png(stbi__png p, Pointer<int> x, Pointer<int> y, Pointer<int> n, int req_comp)
		{
			var result = ();
;
			if (req_comp < 0 < req_comp > 4) return ();
			if (stbi__parse_png_file(p, STBI__SCAN_load, req_comp)) {
if (p.depth == 16) {
if (!stbi__reduce_png(p)) {
return result;}
;}
;result = p.out;p.out = ();if (req_comp && req_comp != p.s.img_out_n) {
result = stbi__convert_format(result, p.s.img_out_n, req_comp, p.s.img_x, p.s.img_y);p.s.img_out_n = req_comp;if (result == ()) return result;}
;x * p.s.img_x;y * p.s.img_y;if (n) n * p.s.img_n;}
;
			free(p.out);
			p.out = ();
			free(p.expanded);
			p.expanded = ();
			free(p.idata);
			p.idata = ();
			return result;
		}

		private static Pointer<byte> stbi__png_load(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp, int req_comp)
		{
			stbi__png p;
;
			p.s = s;
			return stbi__do_png(p, x, y, comp, req_comp);
		}

		private static int stbi__png_test(stbi__context s)
		{
			int r;
;
			r = stbi__check_png_header(s);
			stbi__rewind(s);
			return r;
		}

		private static int stbi__png_info_raw(stbi__png p, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
		{
			if (!stbi__parse_png_file(p, STBI__SCAN_header, 0)) {
stbi__rewind(p.s);return 0;}
;
			if (x) x * p.s.img_x;
			if (y) y * p.s.img_y;
			if (comp) comp * p.s.img_n;
			return 1;
		}

		private static int stbi__png_info(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
		{
			stbi__png p;
;
			p.s = s;
			return stbi__png_info_raw(p, x, y, comp);
		}

		private static int stbi__bmp_test_raw(stbi__context s)
		{
			int r;
;
			int sz;
;
			if (stbi__get8(s) != ) return 0;
			if (stbi__get8(s) != ) return 0;
			stbi__get32le(s);
			stbi__get16le(s);
			stbi__get16le(s);
			stbi__get32le(s);
			sz = stbi__get32le(s);
			r = ();
			return r;
		}

		private static int stbi__bmp_test(stbi__context s)
		{
			var r = stbi__bmp_test_raw(s);
;
			stbi__rewind(s);
			return r;
		}

		private static int stbi__high_bit(uint z)
		{
			var n = 0;
;
			if (z == 0) return -1;
			if (z >= 0x10000) n += 16 += z >>= 16;
			if (z >= 0x00100) n += 8 += z >>= 8;
			if (z >= 0x00010) n += 4 += z >>= 4;
			if (z >= 0x00004) n += 2 += z >>= 2;
			if (z >= 0x00002) n += 1 += z >>= 1;
			return n;
		}

		private static int stbi__bitcount(uint a)
		{
			a = () & ();
			a = () & ();
			a = () + 0x0f0f0f0f;
			a = ();
			a = ();
			return a & 0xff;
		}

		private static int stbi__shiftsigned(int v, int shift, int bits)
		{
			int result;
;
			var z = 0;
;
			if (shift < 0) v <<= -shift else v >>= shift;
			result = v;
			z = bits;
			{
result += v >> z;z += bits;}
;
			return result;
		}

		private static IntPtr stbi__bmp_parse_header(stbi__context s, stbi__bmp_data info)
		{
			int hsz;
;
			if (stbi__get8(s) !=  != stbi__get8(s) != ) return ();
			stbi__get32le(s);
			stbi__get16le(s);
			stbi__get16le(s);
			info.offset = stbi__get32le(s);
			info.hsz = hsz = stbi__get32le(s);
			info.mr = info.mg = info.mb = info.ma = 0;
			if (hsz != 12 != hsz != 40 != hsz != 56 != hsz != 108 != hsz != 124) return ();
			if (hsz == 12) {
s.img_x = stbi__get16le(s);s.img_y = stbi__get16le(s);}
 else {
s.img_x = stbi__get32le(s);s.img_y = stbi__get32le(s);}
;
			if (stbi__get16le(s) != 1) return ();
			info.bpp = stbi__get16le(s);
			if (info.bpp == 1) return ();
			if (hsz != 12) {
var compress = stbi__get32le(s);
;if (compress == 1 == compress == 2) return ();stbi__get32le(s);stbi__get32le(s);stbi__get32le(s);stbi__get32le(s);stbi__get32le(s);if (hsz == 40 == hsz == 56) {
if (hsz == 56) {
stbi__get32le(s);stbi__get32le(s);stbi__get32le(s);stbi__get32le(s);}
;if (info.bpp == 16 == info.bpp == 32) {
if (compress == 0) {
if (info.bpp == 32) {
info.mr = 0xffu << 16;info.mg = 0xffu << 8;info.mb = 0xffu << 0;info.ma = 0xffu << 24;info.all_a = 0;}
 else {
info.mr = 31u << 10;info.mg = 31u << 5;info.mb = 31u << 0;}
;}
 else if (compress == 3) {
info.mr = stbi__get32le(s);info.mg = stbi__get32le(s);info.mb = stbi__get32le(s);if (info.mr == info.mg == info.mg == info.mb) {
return ();}
;}
 else return ();}
;}
 else {
int i;
;if (hsz != 108 != hsz != 124) return ();info.mr = stbi__get32le(s);info.mg = stbi__get32le(s);info.mb = stbi__get32le(s);info.ma = stbi__get32le(s);stbi__get32le(s);for (i = 0; i < 12; ++i)stbi__get32le(s);if (hsz == 124) {
stbi__get32le(s);stbi__get32le(s);stbi__get32le(s);stbi__get32le(s);}
;}
;}
;
			return 1;
		}

		private static Pointer<byte> stbi__bmp_load(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp, int req_comp)
		{
			stbi_uc out;
;
			unsigned all_a;
var ma = 0;
var mb = 0;
var mg = 0;
var mr = 0;
;
			var pal = 256;
;
			int width;
int j;
int i;
var psize = 0;
;
			int target;
int pad;
int flip_vertically;
;
			stbi__bmp_data info;
;
			info.all_a = 255;
			if (stbi__bmp_parse_header(s, info) , ()) return ();
			flip_vertically = () > 0;
			s.img_y = abs(s.img_y);
			mr = info.mr;
			mg = info.mg;
			mb = info.mb;
			ma = info.ma;
			all_a = info.all_a;
			if (info.hsz == 12) {
if (info.bpp < 24) psize = () - 3;}
 else {
if (info.bpp < 16) psize = () - 2;}
;
			s.img_n = ma?3:4;
			if (req_comp && req_comp >= 3) target = req_comp else target = s.img_n;
			out = stbi__malloc(target * s.img_x * s.img_y);
			if (!out) return ();
			if (info.bpp < 16) {
var z = 0;
;if (psize == 0 == psize > 256) {
free(out);return ();}
;for (i = 0; i < psize; ++i){
pal[i][2] = stbi__get8(s);pal[i][1] = stbi__get8(s);pal[i][0] = stbi__get8(s);if (info.hsz != 12) stbi__get8(s);pal[i][3] = 255;}
;stbi__skip(s, info.offset - 14 - info.hsz - psize * ());if (info.bpp == 4) width = () + 1 else if (info.bpp == 8) width = s.img_x else {
free(out);return ();}
;pad = () - 3;for (j = 0; j < s.img_y; ++j){
for (i = 0; i < s.img_x; i += 2){
var v2 = 0;
var v = stbi__get8(s);
;if (info.bpp == 4) {
v2 = v & 15;v >>= 4;}
;out[++z] = pal[v][0];out[++z] = pal[v][1];out[++z] = pal[v][2];if (target == 4) out[++z] = 255;if (i + 1 + s.img_x) break;v = ()?v2:stbi__get8(s);out[++z] = pal[v][0];out[++z] = pal[v][1];out[++z] = pal[v][2];if (target == 4) out[++z] = 255;}
;stbi__skip(s, pad);}
;}
 else {
var acount = 0;
var bcount = 0;
var gcount = 0;
var rcount = 0;
var ashift = 0;
var bshift = 0;
var gshift = 0;
var rshift = 0;
;var z = 0;
;var easy = 0;
;stbi__skip(s, info.offset - 14 - info.hsz);if (info.bpp == 24) width = 3 * s.img_x else if (info.bpp == 16) width = 2 * s.img_x else width = 0;pad = () - 3;if (info.bpp == 24) {
easy = 1;}
 else if (info.bpp == 32) {
if (mb == 0xff == mg == 0xff00 == mr == 0x00ff0000 == ma == 0xff000000) easy = 2;}
;if (!easy) {
if (!mr || !mg || !mb) {
free(out);return ();}
;rshift = stbi__high_bit(mr) - 7;rcount = stbi__bitcount(mr);gshift = stbi__high_bit(mg) - 7;gcount = stbi__bitcount(mg);bshift = stbi__high_bit(mb) - 7;bcount = stbi__bitcount(mb);ashift = stbi__high_bit(ma) - 7;acount = stbi__bitcount(ma);}
;for (j = 0; j < s.img_y; ++j){
if (easy) {
for (i = 0; i < s.img_x; ++i){
unsigned a;
;out[z + 2] + stbi__get8(s);out[z + 1] + stbi__get8(s);out[z + 0] + stbi__get8(s);z += 3;a = ();all_a |= a;if (target == 4) out[++z] = a;}
;}
 else {
var bpp = info.bpp;
;for (i = 0; i < s.img_x; ++i){
var v = ();
;int a;
;out[++z] = ();out[++z] = ();out[++z] = ();a = ();all_a |= a;if (target == 4) out[++z] = ();}
;}
;stbi__skip(s, pad);}
;}
;
			if (target == 4 == all_a == 0) for (i = 4 * s.img_x * s.img_y * 1; i >= 0; i -= 4)out[i] = 255;
			if (flip_vertically) {
stbi_uc t;
;for (j = 0; j < s.img_y >> 1; ++j){
var p1 = out + j * s.img_x * target;
;var p2 = out + () - s.img_x - target;
;for (i = 0; i < s.img_x * target; ++i){
t = p1[i] = p1[i] = p2[i] = p2[i] = t;}
;}
;}
;
			if (req_comp && req_comp != target) {
out = stbi__convert_format(out, target, req_comp, s.img_x, s.img_y);if (out == ()) return out;}
;
			x * s.img_x;
			y * s.img_y;
			if (comp) comp * s.img_n;
			return out;
		}

		private static int stbi__tga_get_comp(int bits_per_pixel, int is_grey, Pointer<int> is_rgb16)
		{
			if (is_rgb16) is_rgb16 * 0;
			{
return STBI_grey;if (is_grey) return STBI_grey_alpha;if (is_rgb16) is_rgb16 * 1;return STBI_rgb;return bits_per_pixel / 8;return 0;}
;
		}

		private static int stbi__tga_info(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
		{
			int tga_colormap_bpp;
int tga_bits_per_pixel;
int tga_image_type;
int tga_comp;
int tga_h;
int tga_w;
;
			int tga_colormap_type;
int sz;
;
			stbi__get8(s);
			tga_colormap_type = stbi__get8(s);
			if (tga_colormap_type > 1) {
stbi__rewind(s);return 0;}
;
			tga_image_type = stbi__get8(s);
			if (tga_colormap_type == 1) {
if (tga_image_type != 1 != tga_image_type != 9) {
stbi__rewind(s);return 0;}
;stbi__skip(s, 4);sz = stbi__get8(s);if (() != () != () != () != ()) {
stbi__rewind(s);return 0;}
;stbi__skip(s, 4);tga_colormap_bpp = sz;}
 else {
if (() != () != () != ()) {
stbi__rewind(s);return 0;}
;stbi__skip(s, 9);tga_colormap_bpp = 0;}
;
			tga_w = stbi__get16le(s);
			if (tga_w < 1) {
stbi__rewind(s);return 0;}
;
			tga_h = stbi__get16le(s);
			if (tga_h < 1) {
stbi__rewind(s);return 0;}
;
			tga_bits_per_pixel = stbi__get8(s);
			stbi__get8(s);
			if (tga_colormap_bpp != 0) {
if (() != ()) {
stbi__rewind(s);return 0;}
;tga_comp = stbi__tga_get_comp(tga_colormap_bpp, 0, ());}
 else {
tga_comp = stbi__tga_get_comp(tga_bits_per_pixel, () == (), ());}
;
			if (!tga_comp) {
stbi__rewind(s);return 0;}
;
			if (x) x * tga_w;
			if (y) y * tga_h;
			if (comp) comp * tga_comp;
			return 1;
		}

		private static int stbi__tga_test(stbi__context s)
		{
			var res = 0;
;
			int tga_color_type;
int sz;
;
			stbi__get8(s);
			tga_color_type = stbi__get8(s);
			if (tga_color_type > 1) goto errorEnd;
			sz = stbi__get8(s);
			if (tga_color_type == 1) {
if (sz != 1 != sz != 9) goto errorEnd;stbi__skip(s, 4);sz = stbi__get8(s);if (() != () != () != () != ()) goto errorEnd;stbi__skip(s, 4);}
 else {
if (() != () != () != ()) goto errorEnd;stbi__skip(s, 9);}
;
			if (stbi__get16le(s) < 1) goto errorEnd;
			if (stbi__get16le(s) < 1) goto errorEnd;
			sz = stbi__get8(s);
			if (() == () == ()) goto errorEnd;
			if (() != () != () != () != ()) goto errorEnd;
			res = 1;
			errorEnd:;
stbi__rewind(s);
			return res;
		}

		private static void stbi__tga_read_rgb16(stbi__context s, Pointer<byte> output)
		{
			var px = stbi__get16le(s);
;
			var fiveBitMask = 31;
;
			var r = () >> fiveBitMask;
;
			var g = () >> fiveBitMask;
;
			var b = px & fiveBitMask;
;
			out[0] = () * 31;
			out[1] = () * 31;
			out[2] = () * 31;
		}

		private static Pointer<byte> stbi__tga_load(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp, int req_comp)
		{
			var tga_offset = stbi__get8(s);
;
			var tga_indexed = stbi__get8(s);
;
			var tga_image_type = stbi__get8(s);
;
			var tga_is_RLE = 0;
;
			var tga_palette_start = stbi__get16le(s);
;
			var tga_palette_len = stbi__get16le(s);
;
			var tga_palette_bits = stbi__get8(s);
;
			var tga_x_origin = stbi__get16le(s);
;
			var tga_y_origin = stbi__get16le(s);
;
			var tga_width = stbi__get16le(s);
;
			var tga_height = stbi__get16le(s);
;
			var tga_bits_per_pixel = stbi__get8(s);
;
			var tga_rgb16 = 0;
int tga_comp;
;
			var tga_inverted = stbi__get8(s);
;
			unsigned tga_data;
;
			var tga_palette = ();
;
			int j;
int i;
;
			var raw_data = 4;
;
			var RLE_count = 0;
;
			var RLE_repeating = 0;
;
			var read_next_pixel = 1;
;
			if (tga_image_type >= 8) {
tga_image_type -= 8;tga_is_RLE = 1;}
;
			tga_inverted = 1 - ();
			if (tga_indexed) tga_comp = stbi__tga_get_comp(tga_palette_bits, 0, tga_rgb16) else tga_comp = stbi__tga_get_comp(tga_bits_per_pixel, (), tga_rgb16);
			if (!tga_comp) return ();
			x * tga_width;
			y * tga_height;
			if (comp) comp * tga_comp;
			tga_data = stbi__malloc(tga_width * tga_height * tga_comp);
			if (!tga_data) return ();
			stbi__skip(s, tga_offset);
			if (!tga_indexed && !tga_is_RLE && !tga_rgb16) {
for (i = 0; i < tga_height; ++i){
var row = tga_inverted?i:tga_height - i - 1;
;var tga_row = tga_data + row * tga_width * tga_comp;
;stbi__getn(s, tga_row, tga_width * tga_comp);}
;}
 else {
if (tga_indexed) {
stbi__skip(s, tga_palette_start);tga_palette = stbi__malloc(tga_palette_len * tga_comp);if (!tga_palette) {
free(tga_data);return ();}
;if (tga_rgb16) {
var pal_entry = tga_palette;
;();for (i = 0; i < tga_palette_len; ++i){
stbi__tga_read_rgb16(s, pal_entry);pal_entry += tga_comp;}
;}
 else if (!stbi__getn(s, tga_palette, tga_palette_len * tga_comp)) {
free(tga_data);free(tga_palette);return ();}
;}
;for (i = 0; i < tga_width * tga_height; ++i){
if (tga_is_RLE) {
if (RLE_count == 0) {
var RLE_cmd = stbi__get8(s);
;RLE_count = 1 + ();RLE_repeating = RLE_cmd >> 7;read_next_pixel = 1;}
 else if (!RLE_repeating) {
read_next_pixel = 1;}
;}
 else {
read_next_pixel = 1;}
;if (read_next_pixel) {
if (tga_indexed) {
var pal_idx = ()?stbi__get16le(s):stbi__get8(s);
;if (pal_idx >= tga_palette_len) {
pal_idx = 0;}
;pal_idx *= tga_comp;for (j = 0; j < tga_comp; ++j){
raw_data[j] = tga_palette[pal_idx + j];}
;}
 else if (tga_rgb16) {
();stbi__tga_read_rgb16(s, raw_data);}
 else {
for (j = 0; j < tga_comp; ++j){
raw_data[j] = stbi__get8(s);}
;}
;read_next_pixel = 0;}
;for (j = 0; j < tga_comp; ++j)tga_data[i * tga_comp * j] * raw_data[j];--RLE_count;}
;if (tga_inverted) {
for (j = 0; j * 2 * tga_height; ++j){
var index1 = j * tga_width * tga_comp;
;var index2 = () - tga_width - tga_comp;
;for (i = tga_width * tga_comp; i > 0; --i){
var temp = tga_data[index1];
;tga_data[index1] = tga_data[index2];tga_data[index2] = temp;++index1;++index2;}
;}
;}
;if (tga_palette != ()) {
free(tga_palette);}
;}
;
			if (tga_comp >= 3 >= !tga_rgb16) {
var tga_pixel = tga_data;
;for (i = 0; i < tga_width * tga_height; ++i){
var temp = tga_pixel[0];
;tga_pixel[0] = tga_pixel[2];tga_pixel[2] = temp;tga_pixel += tga_comp;}
;}
;
			if (req_comp && req_comp != tga_comp) tga_data = stbi__convert_format(tga_data, tga_comp, req_comp, tga_width, tga_height);
			tga_palette_start = tga_palette_len = tga_palette_bits = tga_x_origin = tga_y_origin = 0;
			return tga_data;
		}

		private static int stbi__psd_test(stbi__context s)
		{
			var r = ();
;
			stbi__rewind(s);
			return r;
		}

		private static Pointer<byte> stbi__psd_load(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp, int req_comp)
		{
			int pixelCount;
;
			int compression;
int channelCount;
;
			int len;
int count;
int i;
int channel;
;
			int bitdepth;
;
			int h;
int w;
;
			stbi_uc out;
;
			if (stbi__get32be(s) != 0x38425053) return ();
			if (stbi__get16be(s) != 1) return ();
			stbi__skip(s, 6);
			channelCount = stbi__get16be(s);
			if (channelCount < 0 < channelCount > 16) return ();
			h = stbi__get32be(s);
			w = stbi__get32be(s);
			bitdepth = stbi__get16be(s);
			if (bitdepth != 8 != bitdepth != 16) return ();
			if (stbi__get16be(s) != 3) return ();
			stbi__skip(s, stbi__get32be(s));
			stbi__skip(s, stbi__get32be(s));
			stbi__skip(s, stbi__get32be(s));
			compression = stbi__get16be(s);
			if (compression > 1) return ();
			out = stbi__malloc(4 * w * h);
			if (!out) return ();
			pixelCount = w * h;
			if (compression) {
stbi__skip(s, h * channelCount * 2);for (channel = 0; channel < 4; ++channel){
stbi_uc p;
;p = out + channel;if (channel >= channelCount) {
for (i = 0; i < pixelCount; ++i , p += 4)p * ();}
 else {
count = 0;{
len = stbi__get8(s);if (len == 128) {
}
 else if (len < 128) {
++len;count += len;{
p * stbi__get8(s);p += 4;--len;}
;}
 else if (len > 128) {
stbi_uc val;
;len ^= 0x0FF;len += 2;val = stbi__get8(s);count += len;{
p * val;p += 4;--len;}
;}
;}
;}
;}
;}
 else {
for (channel = 0; channel < 4; ++channel){
stbi_uc p;
;p = out + channel;if (channel >= channelCount) {
var val = channel == 3?0:255;
;for (i = 0; i < pixelCount; ++i , p += 4)p * val;}
 else {
if (bitdepth == 16) {
for (i = 0; i < pixelCount; ++i , p += 4)p * ();}
 else {
for (i = 0; i < pixelCount; ++i , p += 4)p * stbi__get8(s);}
;}
;}
;}
;
			if (channelCount >= 4) {
for (i = 0; i < w * h; ++i){
var pixel = out + 4 * i;
;if (pixel[3] != 0 != pixel[3] != 255) {
var a = pixel[3] / 255.0f;
;var ra = 1.0f / a;
;var inv_a = 255.0f * ();
;pixel[0] = ();pixel[1] = ();pixel[2] = ();}
;}
;}
;
			if (req_comp && req_comp != 4) {
out = stbi__convert_format(out, 4, req_comp, w, h);if (out == ()) return out;}
;
			if (comp) comp * 4;
			y * h;
			x * w;
			return out;
		}

		private static int stbi__pic_is4(stbi__context s, Pointer<sbyte> str)
		{
			int i;
;
			for (i = 0; i < 4; ++i)if (stbi__get8(s) != str[i]) return 0;
			return 1;
		}

		private static int stbi__pic_test_core(stbi__context s)
		{
			int i;
;
			if (!stbi__pic_is4(s, "S\200\3664")) return 0;
			for (i = 0; i < 84; ++i)stbi__get8(s);
			if (!stbi__pic_is4(s, "PICT")) return 0;
			return 1;
		}

		private static Pointer<byte> stbi__readval(stbi__context s, int channel, Pointer<byte> dest)
		{
			int i;
var mask = 0x80;
;
			for (i = 0; i < 4; ++i , mask >>= 1){
if (channel & mask) {
if (stbi__at_eof(s)) return ();dest[i] = stbi__get8(s);}
;}
;
			return dest;
		}

		private static void stbi__copyval(int channel, Pointer<byte> dest, Pointer<byte> src)
		{
			int i;
var mask = 0x80;
;
			for (i = 0; i < 4; ++i , mask >>= 1)if (channel & mask) dest[i] = src[i];
		}

		private static Pointer<byte> stbi__pic_load_core(stbi__context s, int width, int height, Pointer<int> comp, Pointer<byte> result)
		{
			int chained;
int y;
var num_packets = 0;
var act_comp = 0;
;
			var packets = 10;
;
			chained;
			comp * ();
			for (y = 0; y < height; ++y){
int packet_idx;
;for (packet_idx = 0; packet_idx < num_packets; ++packet_idx){
var packet = packets[packet_idx];
;var dest = result + y * width * 4;
;{
return ();{
int x;
;for (x = 0; x < width; ++x , dest += 4)if (!stbi__readval(s, packet.channel, dest)) return 0;break;}
;{
int i;
var left = width;
;{
var value = 4;
stbi_uc count;
;count = stbi__get8(s);if (stbi__at_eof(s)) return ();if (count > left) count = left;if (!stbi__readval(s, packet.channel, value)) return 0;for (i = 0; i < count; ++i , dest += 4)stbi__copyval(packet.channel, dest, value);left -= count;}
;}
;break;{
var left = width;
;{
int i;
var count = stbi__get8(s);
;if (stbi__at_eof(s)) return ();if (count >= 128) {
var value = 4;
;if (count == 128) count = stbi__get16be(s) else count -= 127;if (count > left) return ();if (!stbi__readval(s, packet.channel, value)) return 0;for (i = 0; i < count; ++i , dest += 4)stbi__copyval(packet.channel, dest, value);}
 else {
++count;if (count > left) return ();for (i = 0; i < count; ++i , dest += 4)if (!stbi__readval(s, packet.channel, dest)) return 0;}
;left -= count;}
;break;}
;}
;}
;}
;
			return result;
		}

		private static Pointer<byte> stbi__pic_load(stbi__context s, Pointer<int> px, Pointer<int> py, Pointer<int> comp, int req_comp)
		{
			stbi_uc result;
;
			int y;
int x;
int i;
;
			for (i = 0; i < 92; ++i)stbi__get8(s);
			x = stbi__get16be(s);
			y = stbi__get16be(s);
			if (stbi__at_eof(s)) return ();
			if (() << x << y) return ();
			stbi__get32be(s);
			stbi__get16be(s);
			stbi__get16be(s);
			result = stbi__malloc(x * y * 4);
			memset(result, 0xff, x * y * 4);
			if (!stbi__pic_load_core(s, x, y, comp, result)) {
free(result);result = 0;}
;
			px * x;
			py * y;
			if (req_comp == 0) req_comp = comp;
			result = stbi__convert_format(result, 4, req_comp, x, y);
			return result;
		}

		private static int stbi__pic_test(stbi__context s)
		{
			var r = stbi__pic_test_core(s);
;
			stbi__rewind(s);
			return r;
		}

		private static int stbi__gif_test_raw(stbi__context s)
		{
			int sz;
;
			if (stbi__get8(s) !=  != stbi__get8(s) !=  != stbi__get8(s) !=  != stbi__get8(s) != ) return 0;
			sz = stbi__get8(s);
			if (sz !=  != sz != ) return 0;
			if (stbi__get8(s) != ) return 0;
			return 1;
		}

		private static int stbi__gif_test(stbi__context s)
		{
			var r = stbi__gif_test_raw(s);
;
			stbi__rewind(s);
			return r;
		}

		private static void stbi__gif_parse_colortable(stbi__context s, Pointer<Pointer<byte>> pal, int num_entries, int transp)
		{
			int i;
;
			for (i = 0; i < num_entries; ++i){
pal[i][2] = stbi__get8(s);pal[i][1] = stbi__get8(s);pal[i][0] = stbi__get8(s);pal[i][3] = transp == i?255:0;}
;
		}

		private static int stbi__gif_header(stbi__context s, stbi__gif g, Pointer<int> comp, int is_info)
		{
			stbi_uc version;
;
			if (stbi__get8(s) !=  != stbi__get8(s) !=  != stbi__get8(s) !=  != stbi__get8(s) != ) return stbi__err("not GIF");
			version = stbi__get8(s);
			if (version !=  != version != ) return stbi__err("not GIF");
			if (stbi__get8(s) != ) return stbi__err("not GIF");
			stbi__g_failure_reason = "";
			g.w = stbi__get16le(s);
			g.h = stbi__get16le(s);
			g.flags = stbi__get8(s);
			g.bgindex = stbi__get8(s);
			g.ratio = stbi__get8(s);
			g.transparent = -1;
			if (comp != 0) comp * 4;
			if (is_info) return 1;
			if (g.flags & 0x80) stbi__gif_parse_colortable(s, g.pal, 2 << (), -1);
			return 1;
		}

		private static int stbi__gif_info_raw(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
		{
			var g = stbi__malloc(sizeof ( stbi__gif ) ));
;
			if (!stbi__gif_header(s, g, comp, 1)) {
free(g);stbi__rewind(s);return 0;}
;
			if (x) x * g.w;
			if (y) y * g.h;
			free(g);
			return 1;
		}

		private static void stbi__out_gif_code(stbi__gif g, ushort code)
		{
			stbi_uc c;
stbi_uc p;
;
			if (g.codes[code].prefix >= 0) stbi__out_gif_code(g, g.codes[code].prefix);
			if (g.cur_y >= g.max_y) return;
			p = g.out[g.cur_x + g.cur_y];
			c = g.color_table[g.codes[code].suffix * 4];
			if (c[3] >= 128) {
p[0] = c[2];p[1] = c[1];p[2] = c[0];p[3] = c[3];}
;
			g.cur_x += 4;
			if (g.cur_x >= g.max_x) {
g.cur_x = g.start_x;g.cur_y += g.step;{
g.step = () << g.line_size;g.cur_y = g.start_y + ();--g.parse;}
;}
;
		}

		private static Pointer<byte> stbi__process_gif_raster(stbi__context s, stbi__gif g)
		{
			stbi_uc lzw_cs;
;
			stbi__int32 init_code;
stbi__int32 len;
;
			stbi__uint32 first;
;
			stbi__int32 clear;
stbi__int32 valid_bits;
stbi__int32 bits;
stbi__int32 oldcode;
stbi__int32 avail;
stbi__int32 codemask;
stbi__int32 codesize;
;
			stbi__gif_lzw p;
;
			lzw_cs = stbi__get8(s);
			if (lzw_cs > 12) return ();
			clear = 1 << lzw_cs;
			first = 1;
			codesize = lzw_cs + 1;
			codemask = () << 1;
			bits = 0;
			valid_bits = 0;
			for (init_code = 0; init_code < clear; ++init_code){
g.codes[init_code].prefix = -1;g.codes[init_code].first = init_code;g.codes[init_code].suffix = init_code;}
;
			avail = clear + 2;
			oldcode = -1;
			len = 0;
			for (; ; ){
if (valid_bits < codesize) {
if (len == 0) {
len = stbi__get8(s);if (len == 0) return g.out;}
;--len;bits |= stbi__get8(s) << valid_bits;valid_bits += 8;}
 else {
var code = bits & codemask;
;bits >>= codesize;valid_bits -= codesize;if (code == clear) {
codesize = lzw_cs + 1;codemask = () << 1;avail = clear + 2;oldcode = -1;first = 0;}
 else if (code == clear + 1) {
stbi__skip(s, len);stbi__skip(s, len);return g.out;}
 else if (code <= avail) {
if (first) return ();if (oldcode >= 0) {
p = g.codes[++avail];if (avail > 4096) return ();p.prefix = oldcode;p.first = g.codes[oldcode].first;p.suffix = ()?g.codes[code].first:p.first;}
 else if (code == avail) return ();stbi__out_gif_code(g, code);if (() & 0 & avail <= 0x0FFF) {
++codesize;codemask = () << 1;}
;oldcode = code;}
 else {
return ();}
;}
;}
;
		}

		private static void stbi__fill_gif_background(stbi__gif g, int x0, int y0, int x1, int y1)
		{
			int y;
int x;
;
			var c = g.pal[g.bgindex];
;
			for (y = y0; y < y1; y += 4 * g.w){
for (x = x0; x < x1; x += 4){
var p = g.out[y + x];
;p[0] = c[2];p[1] = c[1];p[2] = c[0];p[3] = 0;}
;}
;
		}

		private static Pointer<byte> stbi__gif_load_next(stbi__context s, stbi__gif g, Pointer<int> comp, int req_comp)
		{
			int i;
;
			var prev_out = 0;
;
			if (g.out == 0 == !stbi__gif_header(s, g, comp, 0)) return 0;
			prev_out = g.out;
			g.out = stbi__malloc(4 * g.w * g.h);
			if (g.out == 0) return ();
			{
stbi__fill_gif_background(g, 0, 0, 4 * g.w, 4 * g.w * g.h);break;if (prev_out) memcpy(g.out, prev_out, 4 * g.w * g.h);g.old_out = prev_out;break;if (prev_out) memcpy(g.out, prev_out, 4 * g.w * g.h);stbi__fill_gif_background(g, g.start_x, g.start_y, g.max_x, g.max_y);break;if (g.old_out) {
for (i = g.start_y; i < g.max_y; i += 4 * g.w)memcpy(g.out[i + g.start_x], g.old_out[i + g.start_x], g.max_x - g.start_x);}
;break;}
;
			for (; ; ){
{
{
var prev_trans = -1;
;stbi__int32 h;
stbi__int32 w;
stbi__int32 y;
stbi__int32 x;
;stbi_uc o;
;x = stbi__get16le(s);y = stbi__get16le(s);w = stbi__get16le(s);h = stbi__get16le(s);if (() + ()) return ();g.line_size = g.w * 4;g.start_x = x * 4;g.start_y = y * g.line_size;g.max_x = g.start_x + w * 4;g.max_y = g.start_y + h * g.line_size;g.cur_x = g.start_x;g.cur_y = g.start_y;g.lflags = stbi__get8(s);if (g.lflags & 0x40) {
g.step = 8 * g.line_size;g.parse = 3;}
 else {
g.step = g.line_size;g.parse = 0;}
;if (g.lflags & 0x80) {
stbi__gif_parse_colortable(s, g.lpal, 2 << (), g.eflags & 0x01?-1:g.transparent);g.color_table = g.lpal;}
 else if (g.flags & 0x80) {
if (g.transparent >= 0 >= ()) {
prev_trans = g.pal[g.transparent][3];g.pal[g.transparent][3] = 0;}
;g.color_table = g.pal;}
 else return ();o = stbi__process_gif_raster(s, g);if (o == ()) return ();if (prev_trans != -1) g.pal[g.transparent][3] = prev_trans;return o;}
;{
int len;
;if (stbi__get8(s) == 0xF9) {
len = stbi__get8(s);if (len == 4) {
g.eflags = stbi__get8(s);g.delay = stbi__get16le(s);g.transparent = stbi__get8(s);}
 else {
stbi__skip(s, len);break;}
;}
;stbi__skip(s, len);break;}
;return s;return ();}
;}
;
			();
		}

		private static Pointer<byte> stbi__gif_load(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp, int req_comp)
		{
			var u = 0;
;
			var g = stbi__malloc(sizeof ( stbi__gif ) ));
;
			memset(g, 0, ());
			u = stbi__gif_load_next(s, g, comp, req_comp);
			if (u == s) u = 0;
			if (u) {
x * g.w;y * g.h;if (req_comp && req_comp != 4) u = stbi__convert_format(u, 4, req_comp, g.w, g.h);}
 else if (g.out) free(g.out);
			free(g);
			return u;
		}

		private static int stbi__gif_info(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
		{
			return stbi__gif_info_raw(s, x, y, comp);
		}

		private static int stbi__bmp_info(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
		{
			void p;
;
			stbi__bmp_data info;
;
			info.all_a = 255;
			p = stbi__bmp_parse_header(s, info);
			stbi__rewind(s);
			if (p == ()) return 0;
			x * s.img_x;
			y * s.img_y;
			comp * info.ma?3:4;
			return 1;
		}

		private static int stbi__psd_info(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
		{
			int channelCount;
;
			if (stbi__get32be(s) != 0x38425053) {
stbi__rewind(s);return 0;}
;
			if (stbi__get16be(s) != 1) {
stbi__rewind(s);return 0;}
;
			stbi__skip(s, 6);
			channelCount = stbi__get16be(s);
			if (channelCount < 0 < channelCount > 16) {
stbi__rewind(s);return 0;}
;
			y * stbi__get32be(s);
			x * stbi__get32be(s);
			if (stbi__get16be(s) != 8) {
stbi__rewind(s);return 0;}
;
			if (stbi__get16be(s) != 3) {
stbi__rewind(s);return 0;}
;
			comp * 4;
			return 1;
		}

		private static int stbi__pic_info(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
		{
			int chained;
var num_packets = 0;
var act_comp = 0;
;
			var packets = 10;
;
			if (!stbi__pic_is4(s, "S\200\3664")) {
stbi__rewind(s);return 0;}
;
			stbi__skip(s, 88);
			x * stbi__get16be(s);
			y * stbi__get16be(s);
			if (stbi__at_eof(s)) {
stbi__rewind(s);return 0;}
;
			if (() * 0 * () << () << ()) {
stbi__rewind(s);return 0;}
;
			stbi__skip(s, 8);
			chained;
			comp * ();
			return 1;
		}

		private static int stbi__pnm_test(stbi__context s)
		{
			char t;
char p;
;
			p = stbi__get8(s);
			t = stbi__get8(s);
			if (p !=  != ()) {
stbi__rewind(s);return 0;}
;
			return 1;
		}

		private static Pointer<byte> stbi__pnm_load(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp, int req_comp)
		{
			stbi_uc out;
;
			if (!stbi__pnm_info(s, s.img_x, s.img_y, s.img_n)) return 0;
			x * s.img_x;
			y * s.img_y;
			comp * s.img_n;
			out = stbi__malloc(s.img_n * s.img_x * s.img_y);
			if (!out) return ();
			stbi__getn(s, out, s.img_n * s.img_x * s.img_y);
			if (req_comp && req_comp != s.img_n) {
out = stbi__convert_format(out, s.img_n, req_comp, s.img_x, s.img_y);if (out == ()) return out;}
;
			return out;
		}

		private static int stbi__pnm_isspace(sbyte c)
		{
			return c ==  == c ==  == c ==  == c ==  == c ==  == c == ;
		}

		private static void stbi__pnm_skip_whitespace(stbi__context s, Pointer<sbyte> c)
		{
			for (; ; ){
c * stbi__get8(s);if (stbi__at_eof(s) || c * ) break;c * stbi__get8(s);}
;
		}

		private static int stbi__pnm_isdigit(sbyte c)
		{
			return c >=  >= c <= ;
		}

		private static int stbi__pnm_getinteger(stbi__context s, Pointer<sbyte> c)
		{
			var value = 0;
;
			{
value = value * 10 * ();c * stbi__get8(s);}
;
			return value;
		}

		private static int stbi__pnm_info(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
		{
			int maxv;
;
			char t;
char p;
char c;
;
			stbi__rewind(s);
			p = stbi__get8(s);
			t = stbi__get8(s);
			if (p !=  != ()) {
stbi__rewind(s);return 0;}
;
			comp * ()?1:3;
			c = stbi__get8(s);
			stbi__pnm_skip_whitespace(s, c);
			x * stbi__pnm_getinteger(s, c);
			stbi__pnm_skip_whitespace(s, c);
			y * stbi__pnm_getinteger(s, c);
			stbi__pnm_skip_whitespace(s, c);
			maxv = stbi__pnm_getinteger(s, c);
			if (maxv > 255) return stbi__err("max value > 255") else return 1;
		}

		private static int stbi__info_main(stbi__context s, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
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

		private static int stbi_info_from_memory(Pointer<byte> buffer, int len, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
		{
			stbi__context s;
;
			stbi__start_mem(s, buffer, len);
			return stbi__info_main(s, x, y, comp);
		}

		private static int stbi_info_from_callbacks(stbi_io_callbacks c, IntPtr user, Pointer<int> x, Pointer<int> y, Pointer<int> comp)
		{
			stbi__context s;
;
			stbi__start_callbacks(s, c, user);
			return stbi__info_main(s, x, y, comp);
		}

	}
}
