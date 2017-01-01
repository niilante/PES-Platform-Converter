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
    class SaveDataConverter
    {
        internal static byte[] allBytes = { };
        private static int[] key;

        /// <summary>
        /// Check specified file type.
        /// </summary>
        /// <returns>True if the specified file is Option File</returns>
        public static bool isSaveData()
        {
            byte[] temp = new byte[4];
            for (int i = 0; i < 4; i++) { temp[i] = allBytes[i]; }
            long OptionFileID = BitConverter.ToUInt32(temp, 0);

            return (OptionFileID == 14);
        }

        /// <summary>
        /// Convert specified option file from XBOX to PC format
        /// </summary>
        /// <returns>Array of bytes on PC format</returns>
        public static byte[] xboxToPc()
        {
            changePlatform(0);

            return allBytes;
        }

        /// <summary>
        /// Convert specified option file from PC to XBOX format
        /// </summary>
        /// <returns>Array of bytes on XBOX format</returns>
        public static byte[] pcToXbox()
        {
            changePlatform(1);

            return allBytes;
        }

        /// <summary>
        /// Convert option file to desired platform
        /// </summary>
        /// <param name="code">0 = XBOX to PC || 1 = PC to XBOX || 2 = PS3 to XBOX || 3 = XBOX to PS3</param>
        private static void changePlatform(int code)
        {
            long offset = (code < 2) ? 4 : 4987676;

            if (code < 2)
            {
                #region header
                for (int i = 0; i < 22; i++)
                {
                    if (i == 7 || (i > 10 && i < 19)) { offset += 4; continue; }
                    BitUtility.write_UInt32(BitUtility.reverseEndianness32Bits(BitUtility.read_UInt32(offset)), offset);
                    offset += 4;
                }

                for (int i = 0; i < 14; i++)
                {
                    BitUtility.write_UInt16(BitUtility.reverseEndianness16Bits(BitUtility.read_UInt16(offset)), offset);
                    offset += 2;
                }
                #endregion header

                #region player stats
                for (int player = 0; player < 21000; player++)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        BitUtility.write_UInt32(BitUtility.reverseEndianness32Bits(BitUtility.read_UInt32(offset)), offset);
                        offset += 4;
                    }

                    offset += 2;

                    BitUtility.write_UInt16(BitUtility.reverseEndianness16Bits(BitUtility.read_UInt16(offset)), offset);
                    offset += 6;

                    key = new int[98] { 7, 7, 7, 7, 4, 7, 7, 7, 7, 3, 1, 7, 7, 7, 7, 3, 1, 7, 7, 7,
                    7, 3, 1, 6, 5, 5, 7, 7, 2, 7, 3, 3, 7, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
                    2, 2, 2, 2, 7, 1, 1, 1, 7, 7, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

                    BitUtility.bitConverter(key, 35, offset, code);
                    offset += 100;

                    #region player app
                    BitUtility.write_UInt32(BitUtility.reverseEndianness32Bits(BitUtility.read_UInt32(offset)), offset);
                    offset += 4;

                    key = new int[7] { 1, 1, 1, 1, 14, 10, 4 };

                    BitUtility.bitConverter(key, 4, offset, code);
                    offset += 4;

                    BitUtility.write_UInt32(BitUtility.reverseEndianness32Bits(BitUtility.read_UInt32(offset)), offset);
                    offset += 4;

                    key = new int[28] { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 2, 3, 3, 2, 2, 2, 2, 1, 1, 1, 3, 4 };

                    BitUtility.bitConverter(key, 11, offset, code);
                    offset += 11;

                    for (int i = 0; i < 22; i++) { allBytes[offset] = BitUtility.reverseEndianness8Bits(allBytes[offset]); offset++; }

                    for (int i = 0; i < 3; i++)
                    {
                        if (code == 0) allBytes[offset] = BitUtility.rol(allBytes[offset], 3);
                        else allBytes[offset] = BitUtility.ror(allBytes[offset], 3);
                        offset++;
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        if (i != 2) allBytes[offset] = BitUtility.reverseEndianness8Bits(allBytes[offset]);
                        else if (i == 2)
                        {
                            if (code == 0) allBytes[offset] = BitUtility.ror(allBytes[offset], 3);
                            else allBytes[offset] = BitUtility.rol(allBytes[offset], 3);
                        }

                        offset++;
                    }

                    if (code == 0) allBytes[offset] = BitUtility.ror(allBytes[offset], 2);
                    else allBytes[offset] = BitUtility.rol(allBytes[offset], 2);
                    offset++;

                    key = new int[5] { 3, 3, 6, 6, 6 };

                    BitUtility.bitConverter(key, 3, offset, code);
                    offset += 3;

                    for (int i = 0; i < 4; i++)
                    {
                        if (i == 0) allBytes[offset] = BitUtility.reverseEndianness8Bits(allBytes[offset]);
                        else if (i != 0)
                        {
                            if (code == 0) allBytes[offset] = BitUtility.ror(allBytes[offset], 2);
                            else allBytes[offset] = BitUtility.rol(allBytes[offset], 2);
                        }

                        offset++;
                    }

                    key = new int[7] { 3, 3, 2, 1, 3, 6, 6 };

                    BitUtility.bitConverter(key, 3, offset, code);
                    offset += 3;

                    if (code == 0) allBytes[offset] = BitUtility.ror(allBytes[offset], 2);
                    else allBytes[offset] = BitUtility.rol(allBytes[offset], 2);
                    offset++;

                    key = new int[9] { 4, 8, 4, 3, 3, 3, 3, 2, 2 };

                    BitUtility.bitConverter(key, 4, offset, code);
                    offset += 8;
                    #endregion player app

                }
                #endregion player stats

                #region team
                for (int team = 0; team < 650; team++)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        BitUtility.write_UInt32(BitUtility.reverseEndianness32Bits(BitUtility.read_UInt32(offset)), offset);
                        offset += 4;
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        BitUtility.write_UInt16(BitUtility.reverseEndianness16Bits(BitUtility.read_UInt16(offset)), offset);
                        offset += 2;
                    }

                    key = new int[13] { 6, 6, 4, 6, 6, 6, 6, 4, 4, 4, 4, 4, 4 };

                    BitUtility.bitConverter(key, 8, offset, code);
                    offset += 8;

                    // POSSIBLE BUG HERE! NEED MORE RESEARCH
                    for (int i = 0; i < 2; i++)
                    {
                        allBytes[offset] = BitUtility.reverse8Bits(allBytes[offset]);
                        offset++;
                    }

                    for (int i = 0; i < 31; i++)
                    {
                        BitUtility.write_UInt32(BitUtility.reverseEndianness32Bits(BitUtility.read_UInt32(offset)), offset);
                        offset += 4;
                    }

                    offset += 328;
                }
                #endregion team

                #region manager
                for (int manager = 0; manager < 850; manager++)
                {
                    BitUtility.write_UInt32(BitUtility.reverseEndianness32Bits(BitUtility.read_UInt32(offset)), offset);
                    offset += 4;

                    for (int i = 0; i < 2; i++)
                    {
                        BitUtility.write_UInt16(BitUtility.reverseEndianness16Bits(BitUtility.read_UInt16(offset)), offset);
                        offset += 2;
                    }

                    // POSSIBLE BUG HERE! NEED MORE RESEARCH
                    key = new int[2] { 4, 4 };

                    BitUtility.bitConverter(key, 1, offset, code);
                    offset += 80;
                }
                #endregion manager

                #region competition
                for (int compe = 0; compe < 50; compe++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        BitUtility.write_UInt16(BitUtility.reverseEndianness16Bits(BitUtility.read_UInt16(offset)), offset);
                        offset += 2;
                    }

                    offset += 4;

                    // POSSIBLE BUG HERE! NEED MORE RESEARCH
                    key = new int[10] { 6, 6, 4, 5, 6, 1, 1, 1, 1, 1 };
                    BitUtility.bitConverter(key, 4, offset, code);
                    offset += 4;

                    offset += 110;
                }
                #endregion competition

                #region stadium
                for (int stadium = 0; stadium < 40; stadium++)
                {
                    BitUtility.write_UInt16(BitUtility.reverseEndianness16Bits(BitUtility.read_UInt16(offset)), offset);
                    offset += 128;
                }
                #endregion stadium

                #region uni color
                for (int uni = 0; uni < 2500; uni++)
                {
                    BitUtility.write_UInt32(BitUtility.reverseEndianness32Bits(BitUtility.read_UInt32(offset)), offset);
                    offset += 88;
                }
                #endregion uni color

                #region player assignment
                for (int assign = 0; assign < 650; assign++)
                {
                    for (int i = 0; i < 33; i++)
                    {
                        BitUtility.write_UInt32(BitUtility.reverseEndianness32Bits(BitUtility.read_UInt32(offset)), offset);
                        offset += 4;
                    }
                    offset += 32;
                }
                #endregion player assignment

                #region competition structure
                for (int compeStruct = 0; compeStruct < 722; compeStruct++)
                {
                    BitUtility.write_UInt32(BitUtility.reverseEndianness32Bits(BitUtility.read_UInt32(offset)), offset);
                    offset += 4;
                }
                #endregion competition structure

                // POSSIBLE BUG HERE! NEED MORE RESEARCH
                #region team tactic
                for (int tactic = 0; tactic < 650; tactic++)
                {
                    BitUtility.write_UInt32(BitUtility.reverseEndianness32Bits(BitUtility.read_UInt32(offset)), offset);
                    offset += 628;
                }
                #endregion team tactic
            }
        }
    }
}