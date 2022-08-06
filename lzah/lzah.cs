﻿/*
 This is the just C# implementation of de_lzah.c with a little CRC check function
All source code is copyright here.
https://github.com/mistydemeo/quickbms/blob/master/compression/de_lzah.c

converted by dhlrunner
*/

namespace lzah
{
	unsafe internal class lzah
	{
		private static ushort[] CrcTable = {
		0x0000,0x8005,0x800F,0x000A,0x801B,0x001E,0x0014,0x8011,0x8033,0x0036,0x003C,0x8039,0x0028,0x802D,0x8027,0x0022,
0x8063,0x0066,0x006C,0x8069,0x0078,0x807D,0x8077,0x0072,0x0050,0x8055,0x805F,0x005A,0x804B,0x004E,0x0044,0x8041,
0x80C3,0x00C6,0x00CC,0x80C9,0x00D8,0x80DD,0x80D7,0x00D2,0x00F0,0x80F5,0x80FF,0x00FA,0x80EB,0x00EE,0x00E4,0x80E1,
0x00A0,0x80A5,0x80AF,0x00AA,0x80BB,0x00BE,0x00B4,0x80B1,0x8093,0x0096,0x009C,0x8099,0x0088,0x808D,0x8087,0x0082,
0x8183,0x0186,0x018C,0x8189,0x0198,0x819D,0x8197,0x0192,0x01B0,0x81B5,0x81BF,0x01BA,0x81AB,0x01AE,0x01A4,0x81A1,
0x01E0,0x81E5,0x81EF,0x01EA,0x81FB,0x01FE,0x01F4,0x81F1,0x81D3,0x01D6,0x01DC,0x81D9,0x01C8,0x81CD,0x81C7,0x01C2,
0x0140,0x8145,0x814F,0x014A,0x815B,0x015E,0x0154,0x8151,0x8173,0x0176,0x017C,0x8179,0x0168,0x816D,0x8167,0x0162,
0x8123,0x0126,0x012C,0x8129,0x0138,0x813D,0x8137,0x0132,0x0110,0x8115,0x811F,0x011A,0x810B,0x010E,0x0104,0x8101,
0x8303,0x0306,0x030C,0x8309,0x0318,0x831D,0x8317,0x0312,0x0330,0x8335,0x833F,0x033A,0x832B,0x032E,0x0324,0x8321,
0x0360,0x8365,0x836F,0x036A,0x837B,0x037E,0x0374,0x8371,0x8353,0x0356,0x035C,0x8359,0x0348,0x834D,0x8347,0x0342,
0x03C0,0x83C5,0x83CF,0x03CA,0x83DB,0x03DE,0x03D4,0x83D1,0x83F3,0x03F6,0x03FC,0x83F9,0x03E8,0x83ED,0x83E7,0x03E2,
0x83A3,0x03A6,0x03AC,0x83A9,0x03B8,0x83BD,0x83B7,0x03B2,0x0390,0x8395,0x839F,0x039A,0x838B,0x038E,0x0384,0x8381,
0x0280,0x8285,0x828F,0x028A,0x829B,0x029E,0x0294,0x8291,0x82B3,0x02B6,0x02BC,0x82B9,0x02A8,0x82AD,0x82A7,0x02A2,
0x82E3,0x02E6,0x02EC,0x82E9,0x02F8,0x82FD,0x82F7,0x02F2,0x02D0,0x82D5,0x82DF,0x02DA,0x82CB,0x02CE,0x02C4,0x82C1,
0x8243,0x0246,0x024C,0x8249,0x0258,0x825D,0x8257,0x0252,0x0270,0x8275,0x827F,0x027A,0x826B,0x026E,0x0264,0x8261,
0x0220,0x8225,0x822F,0x022A,0x823B,0x023E,0x0234,0x8231,0x8213,0x0216,0x021C,0x8219,0x0208,0x820D,0x8207,0x0202 };
		private int BYTEMASK = 0x000000ff;
		private ushort[] HuffCode = {
	0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000,
	0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000,
	0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000,
	0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000,
	0x040, 0x040, 0x040, 0x040, 0x040, 0x040, 0x040, 0x040,
	0x040, 0x040, 0x040, 0x040, 0x040, 0x040, 0x040, 0x040,
	0x080, 0x080, 0x080, 0x080, 0x080, 0x080, 0x080, 0x080,
	0x080, 0x080, 0x080, 0x080, 0x080, 0x080, 0x080, 0x080,
	0x0c0, 0x0c0, 0x0c0, 0x0c0, 0x0c0, 0x0c0, 0x0c0, 0x0c0,
	0x0c0, 0x0c0, 0x0c0, 0x0c0, 0x0c0, 0x0c0, 0x0c0, 0x0c0,
	0x100, 0x100, 0x100, 0x100, 0x100, 0x100, 0x100, 0x100,
	0x140, 0x140, 0x140, 0x140, 0x140, 0x140, 0x140, 0x140,
	0x180, 0x180, 0x180, 0x180, 0x180, 0x180, 0x180, 0x180,
	0x1c0, 0x1c0, 0x1c0, 0x1c0, 0x1c0, 0x1c0, 0x1c0, 0x1c0,
	0x200, 0x200, 0x200, 0x200, 0x200, 0x200, 0x200, 0x200,
	0x240, 0x240, 0x240, 0x240, 0x240, 0x240, 0x240, 0x240,
	0x280, 0x280, 0x280, 0x280, 0x280, 0x280, 0x280, 0x280,
	0x2c0, 0x2c0, 0x2c0, 0x2c0, 0x2c0, 0x2c0, 0x2c0, 0x2c0,
	0x300, 0x300, 0x300, 0x300, 0x340, 0x340, 0x340, 0x340,
	0x380, 0x380, 0x380, 0x380, 0x3c0, 0x3c0, 0x3c0, 0x3c0,
	0x400, 0x400, 0x400, 0x400, 0x440, 0x440, 0x440, 0x440,
	0x480, 0x480, 0x480, 0x480, 0x4c0, 0x4c0, 0x4c0, 0x4c0,
	0x500, 0x500, 0x500, 0x500, 0x540, 0x540, 0x540, 0x540,
	0x580, 0x580, 0x580, 0x580, 0x5c0, 0x5c0, 0x5c0, 0x5c0,
	0x600, 0x600, 0x640, 0x640, 0x680, 0x680, 0x6c0, 0x6c0,
	0x700, 0x700, 0x740, 0x740, 0x780, 0x780, 0x7c0, 0x7c0,
	0x800, 0x800, 0x840, 0x840, 0x880, 0x880, 0x8c0, 0x8c0,
	0x900, 0x900, 0x940, 0x940, 0x980, 0x980, 0x9c0, 0x9c0,
	0xa00, 0xa00, 0xa40, 0xa40, 0xa80, 0xa80, 0xac0, 0xac0,
	0xb00, 0xb00, 0xb40, 0xb40, 0xb80, 0xb80, 0xbc0, 0xbc0,
	0xc00, 0xc40, 0xc80, 0xcc0, 0xd00, 0xd40, 0xd80, 0xdc0,
	0xe00, 0xe40, 0xe80, 0xec0, 0xf00, 0xf40, 0xf80, 0xfc0 };
		private short[] HuffLength = {
	3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
	3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
	4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
	4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
	4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
	5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
	5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
	5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
	5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
	6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
	6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
	6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
	7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
	7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
	7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
	8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8 };
		private byte[] lzah_buf = new byte[4096];
		private int lzah_bufptr;
		private int lzah_bitsavail;
		private int lzah_bits;
		private int[] Frequ = new int[1000];
		private int[] ForwTree = new int[1000];
		private int[] BackTree = new int[1000];
		private byte* out_ptr = null;
		private byte* in_ptr = null;
		private byte* in_ptrl = null;
		private int N = 314;
		private int T = 0;
		public lzah()
		{
			T = (2 * N - 1);
		}
		public byte[] CalculateCRC16(byte[] data)
		{
			ushort crc = 0x0000;

			//byte[] data = GetBytesFromHexString(dtc);

			for (var pos = 0; pos < data.Length; pos++)
			{
				crc ^= data[pos];
				for (var i = 8; i != 0; i--)
				{
					if ((crc & 0x0001) != 0)
					{
						crc >>= 1;
						crc ^= 0xA001;
					}
					else
						crc >>= 1;
				}
			}
			return BitConverter.GetBytes(crc);
		}
		public byte[] Decompress(byte[] compdata, int decompDatasize)
		{
			byte[] decompdata = new byte[decompDatasize];
			fixed (byte* compptr = compdata)
			fixed (byte* decompptr = decompdata)
				de_lzah(compptr, compdata.Length, decompptr, (int)decompDatasize);

			return decompdata;
		}

		private byte lzah_getbyte()
		{
			if (in_ptr >= in_ptrl) return 255;
			return (*in_ptr++);
		}
		public int de_lzah(byte* inbuff, int insz, byte* outbuff, int obytes)
		{
			int i, i1, j, ch, byte_, offs, skip;

			out_ptr = outbuff;
			in_ptr = inbuff;
			in_ptrl = inbuff + insz;

			lzah_inithuf();
			lzah_bitsavail = 0;
			for (i = 0; i < 4036; i++)
			{
				lzah_buf[i] = (byte)' ';
			}
			lzah_bufptr = 4036;
			while (obytes != 0)
			{
				ch = ForwTree[T - 1];
				while (ch < T)
				{
					lzah_getbit();
					if ((lzah_bits & 0x80) > 0)
					{
						ch = ch + 1;
					}
					ch = ForwTree[ch];
				}
				ch -= T;
				if (Frequ[T - 1] >= 0x8000)
				{
					lzah_reorder();
				}

				i = BackTree[ch + T];
				do
				{
					j = ++Frequ[i];
					i1 = i + 1;
					if (Frequ[i1] < j)
					{
						while (Frequ[++i1] < j) ;
						i1--;
						Frequ[i] = Frequ[i1];
						Frequ[i1] = j;

						j = ForwTree[i];
						BackTree[j] = i1;
						if (j < T)
						{
							BackTree[j + 1] = i1;
						}
						ForwTree[i] = ForwTree[i1];
						ForwTree[i1] = j;
						j = ForwTree[i];
						BackTree[j] = i;
						if (j < T)
						{
							BackTree[j + 1] = i;
						}
						i = i1;
					}
					i = BackTree[i];
				} while (i != 0);

				if (ch < 256)
				{
					lzah_outchar((byte)ch);
					obytes--;
				}
				else
				{
					if (lzah_bitsavail != 0)
					{
						byte_ = (lzah_bits << 1) & BYTEMASK;
						lzah_bits = lzah_getbyte() & BYTEMASK;
						byte_ |= (lzah_bits >> lzah_bitsavail);
						lzah_bits = lzah_bits << (7 - lzah_bitsavail);
					}
					else
					{
						byte_ = lzah_getbyte() & BYTEMASK;
					}
					offs = HuffCode[byte_];
					skip = HuffLength[byte_] - 2;
					while (skip-- != 0)
					{
						byte_ = byte_ + byte_;
						lzah_getbit();
						if ((lzah_bits & 0x80) > 0)
						{
							byte_++;
						}
					}
					offs |= (byte_ & 0x3f);
					offs = ((lzah_bufptr - offs - 1) & 0xfff);
					ch = ch - 253;
					while (ch-- > 0)
					{
						lzah_outchar(lzah_buf[offs++ & 0xfff]);
						obytes--;
						if (obytes == 0)
						{
							break;
						}
					}
				}
			}
			return ((int)(out_ptr - outbuff));
		}

		private void lzah_inithuf()
		{

			int i, j;

			for (i = 0; i < N; i++)
			{
				Frequ[i] = 1;
				ForwTree[i] = i + T;
				BackTree[i + T] = i;
			}
			for (i = 0, j = N; j < T; i += 2, j++)
			{
				Frequ[j] = Frequ[i] + Frequ[i + 1];
				ForwTree[j] = i;
				BackTree[i] = j;
				BackTree[i + 1] = j;
			}
			Frequ[T] = 0xffff;
			BackTree[T - 1] = 0;
		}

		private void lzah_reorder()
		{
			int i, j, k, l;

			j = 0;
			for (i = 0; i < T; i++)
			{
				if (ForwTree[i] >= T)
				{
					Frequ[j] = ((Frequ[i] + 1) >> 1);
					ForwTree[j] = ForwTree[i];
					j++;
				}
			}
			for (i = 0, j = N; i < T; i += 2, j++)
			{
				k = i + 1;
				l = Frequ[i] + Frequ[k];
				Frequ[j] = l;
				k = j - 1;
				while (l < Frequ[k])
				{
					k--;
				}
				k = k + 1;
				fixed (int* ptr = Frequ)
				{
					lzah_move(ptr + k, ptr + k + 1, j - k);
				}

				Frequ[k] = l;

				fixed (int* ptr = ForwTree)
				{
					lzah_move(ptr + k, ptr + k + 1, j - k);
				}


				ForwTree[k] = i;
			}
			for (i = 0; i < T; i++)
			{
				k = ForwTree[i];
				if (k >= T)
				{
					BackTree[k] = i;
				}
				else
				{
					BackTree[k] = i;
					BackTree[k + 1] = i;
				}
			}
		}
		private void lzah_move(int* p, int* q, int n)
		{
			if (p > q)
			{
				while (n-- > 0)
				{
					*q++ = *p++;
				}
			}
			else
			{
				p += n;
				q += n;
				while (n-- > 0)
				{
					*--q = *--p;
				}
			}
		}
		private void lzah_getbit()
		{
			if (lzah_bitsavail != 0)
			{
				lzah_bits = lzah_bits + lzah_bits;
				lzah_bitsavail--;
			}
			else
			{
				lzah_bits = lzah_getbyte() & BYTEMASK;
				lzah_bitsavail = 7;
			}
		}
		private void lzah_outchar(byte ch)
		{
			*out_ptr++ = ch;
			lzah_buf[lzah_bufptr++] = ch;
			lzah_bufptr &= 0xfff;
		}
	}
}