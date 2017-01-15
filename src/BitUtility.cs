/*
    Copyright (C) 2016, Platter Inc.
    All right reserved.

    Under the terms of the GNU General Public License

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions
    are met:

        1. Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.

        2. Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.

        3. The names of its contributors may not be used to endorse or promote 
        products derived from this software without specific prior written 
        permission.

    PES Platform Converter is free software: you can redistribute
    it and/or modify it under the terms of the GNU General Public License 
    as published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    PES Platform Converter is distributed in the hope that it will
    be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
    of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    General Public License for more details.
 
    You should have received a copy of the GNU General Public License along
    with PES Platform Converter.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;

namespace PESPlatformConverter
{
    class BitUtility
    {
        /// <summary>
        /// Bit converting algorithm.
        /// </summary>
        /// <param name="box">Unique pattern recognition keys.</param>
        /// <param name="len">Lenght of bytes to be overwritten.</param>
        /// <param name="copyOfOffset">Current offset position.</param>
        /// <param name="code">Specifies conversion method.</param>
        internal static void bitConverter(int[] box, int len, long copyOfOffset, int code)
        {
            byte[] arr = new byte[len];
            var index = (code == 0) ? 0 : box.Length - 1;
            int head = 0, tail = head + box[index] - 1, used = 0;
            var temp = copyOfOffset;
            if (code == 1) { copyOfOffset += len - 1; }

            for (int i = 0; i < len; i++)
            {
                var s = "";

                while (s.Length < 8)
                {
                    var num = Convert.ToString(SaveDataConverter.allBytes[copyOfOffset], 2);
                    while (num.Length < 8) { num = '0' + num; }

                    var nextOffset = 1;
                    while (num.Length - 1 < tail)
                    {
                        string num2;
                        if (code == 0) num2 = Convert.ToString(SaveDataConverter.allBytes[copyOfOffset + nextOffset], 2);
                        else num2 = Convert.ToString(SaveDataConverter.allBytes[copyOfOffset - nextOffset], 2);

                        while (num2.Length < 8) { num2 = '0' + num2; }
                        num += num2;
                        nextOffset++;
                    }

                    var j = tail;
                    for (; (j >= head && s.Length < 8); j--) { s = num[j] + s; }

                    if (j >= head)
                    {
                        used = tail - j;
                        tail = j;
                    }
                    else
                    {
                        if (used > 0) head += box[index];
                        else head = tail % 8 + 1;

                        if (head > 7)
                        {
                            if (code == 0) copyOfOffset += (int)(head / 8);
                            else copyOfOffset -= (int)(head / 8);
                            head %= 8;
                        }

                        if (code == 0)
                        {
                            if (index < box.Length - 1) tail = head + box[++index] - 1;
                        }
                        else
                        {
                            if (index > 0) tail = head + box[--index] - 1;
                        }

                        used = 0;
                    }
                }

                if (code == 0) arr[i] = Convert.ToByte(s, 2);
                else arr[(len - 1) - i] = Convert.ToByte(s, 2);
            }

            for (int i = 0; i < len; i++)
            {
                SaveDataConverter.allBytes[temp + i] = arr[i];
            }
        }

        /// <summary>
        /// Writes 32-bits number to the array.
        /// </summary>
        /// <param name="x">The 32-bits number.</param>
        /// <param name="init">Initial offset position.</param>
        internal static void write_UInt32(UInt32 x, long init)
        {
            var count = 0;
            byte[] hex = BitConverter.GetBytes(x);
            for (long i = init; i < init + 4; i++) { SaveDataConverter.allBytes[i] = hex[count]; count++; }
        }

        /// <summary>
        /// Writes 16-bits number to the array.
        /// </summary>
        /// <param name="x">The 16-bits number.</param>
        /// <param name="init">Initial offset position.</param>
        internal static void write_UInt16(UInt16 x, long init)
        {
            var count = 0;
            byte[] hex = BitConverter.GetBytes(x);
            for (long i = init; i < init + 2; i++) { SaveDataConverter.allBytes[i] = hex[count]; count++; }
        }

        /// <summary>
        /// Reads 32-bits array.
        /// </summary>
        /// <param name="init">Initial offset position.</param>
        /// <returns>32-bits integer.</returns>
        internal static UInt32 read_UInt32(long init)
        {
            var count = 0; byte[] chunk = new byte[4];
            for (long i = init; i < init + 4; i++) { chunk[count] = SaveDataConverter.allBytes[i]; count++; }
            return BitConverter.ToUInt32(chunk, 0);
        }

        /// <summary>
        /// Reads 16-bits array.
        /// </summary>
        /// <param name="init">Initial offset position.</param>
        /// <returns>16-bits integer.</returns>
        internal static UInt16 read_UInt16(long init)
        {
            var count = 0; byte[] chunk = new byte[2];
            for (long i = init; i < init + 2; i++) { chunk[count] = SaveDataConverter.allBytes[i]; count++; }
            return BitConverter.ToUInt16(chunk, 0);
        }

        /// <summary>
        /// Reverse endianness byte order for 8-bits number.
        /// </summary>
        /// <param name="value">The 8-bits number.</param>
        /// <returns>Reversed byte order.</returns>
        internal static byte reverseEndianness8Bits(byte value)
        {
            return (byte)((value & 15) << 4 | (value & 240) >> 4);
        }

        /// <summary>
        /// Reverse endianness byte order for 16-bits number.
        /// </summary>
        /// <param name="value">The 16-bits number.</param>
        /// <returns>Reversed byte order.</returns>
        internal static UInt16 reverseEndianness16Bits(UInt16 value)
        {
            return (UInt16)((value & 0xFF) << 8 | (value & 0xFF00) >> 8);
        }

        /// <summary>
        /// Reverse endianness byte order for 32-bits number.
        /// </summary>
        /// <param name="value">The 32-bits number.</param>
        /// <returns>Reversed byte order.</returns>
        internal static UInt32 reverseEndianness32Bits(UInt32 value)
        {
            return (value & 0x000000FF) << 24 | (value & 0x0000FF00) << 8 | (value & 0x00FF0000) >> 8 | (value & 0xFF000000) >> 24;
        }

        /// <summary>
        /// Reverse endianness byte order for 64-bits number.
        /// </summary>
        /// <param name="value">The 64-bits number.</param>
        /// <returns>Reversed byte order.</returns>
        internal static UInt64 reverseEndianness64Bits(UInt64 value)
        {
            return (value & 0x00000000000000FF) << 56 | (value & 0x000000000000FF00) << 40 | (value & 0x0000000000FF0000) << 24 |
                (value & 0x00000000FF000000) << 8 | (value & 0x000000FF00000000) >> 8 | (value & 0x0000FF0000000000) >> 24 |
                (value & 0x00FF000000000000) >> 40 | (value & 0xFF00000000000000) >> 56;
        }

        /// <summary>
        /// Reverse bits order for 8-bits number.
        /// </summary>
        /// <param name="inv8">The 8-bits number.</param>
        /// <returns>Reversed bits order.</returns>
        internal static byte reverse8Bits(byte inv8)
        {
            byte count = 7;
            byte reverse_num = inv8;
            inv8 >>= 1;
            while (inv8 != 0)
            {
                reverse_num <<= 1;
                reverse_num = (byte)(reverse_num | inv8 & 1);
                inv8 >>= 1;
                count--;
            }
            reverse_num <<= count;
            return reverse_num;
        }

        /// <summary>
        /// Reverse bits order for 16-bits number.
        /// </summary>
        /// <param name="inv16">The 16-bits number.</param>
        /// <returns>Reversed bits order.</returns>
        internal static UInt16 reverse16Bits(UInt16 inv16)
        {
            byte count = 15;
            UInt16 reverse_num = inv16;
            inv16 >>= 1;
            while (inv16 != 0)
            {
                reverse_num <<= 1;
                reverse_num = (UInt16)(reverse_num | inv16 & 1);
                inv16 >>= 1;
                count--;
            }
            reverse_num <<= count;
            return reverse_num;
        }

        /// <summary>
        /// Rotate bits to the left.
        /// </summary>
        /// <param name="value">The 8-bits number.</param>
        /// <param name="shift">Number of shift to be performed.</param>
        /// <returns>Rotated bits.</returns>
        internal static byte rol(byte value, int shift)
        {
            return (byte)((value << shift) | (value >> (8 - shift)));
        }

        /// <summary>
        /// Rotate bits to the right.
        /// </summary>
        /// <param name="value">The 8-bits number.</param>
        /// <param name="shift">Number of shift to be performed.</param>
        /// <returns>Rotated bits.</returns>
        internal static byte ror(byte value, int shift)
        {
            return (byte)((value >> shift) | (value << (8 - shift)));
        }
    }
}
