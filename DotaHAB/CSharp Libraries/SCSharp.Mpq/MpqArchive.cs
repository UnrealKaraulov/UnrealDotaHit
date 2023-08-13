//
// MpqArchive.cs
//
// Authors:
//		Foole (fooleau@gmail.com)
//
// (C) 2006 Foole (fooleau@gmail.com)
// Based on code from StormLib by Ladislav Zezula
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MpqReader
{
	public class MpqArchive : IDisposable
	{
		private Stream mStream;

		private MpqHeader mHeader;
		private long mHeaderOffset;
		private int mBlockSize;
		//private MpqHash[] mHashes;
        private Dictionary<UInt64, MpqHash> dcHashes;
		private MpqBlock[] mBlocks;
        private bool isValid;
		
		private static uint[] sStormBuffer;
		
		static MpqArchive()
		{
			sStormBuffer = BuildStormBuffer();
		}

		public MpqArchive(string Filename)
		{
			mStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			Init();
		}
		
		public MpqArchive(Stream SourceStream)
		{
			mStream = SourceStream;
			Init();
		}

		public void Dispose ()
		{
			if (mStream != null)
				mStream.Close ();
		}

        public bool IsValid { 
            get { return isValid; } 
        }

		private void Init()
		{
            if (LocateMpqHeader() == false)
            {
                Console.WriteLine("Unable to find MPQ header");
                mStream.Close();
                return;
            }

			BinaryReader br = new BinaryReader(mStream);

			mBlockSize = 0x200 << mHeader.BlockSize;

			// Load hash table
			mStream.Seek(mHeader.HashTablePos, SeekOrigin.Begin);
			byte[] hashdata = br.ReadBytes((int)(mHeader.HashTableSize * MpqHash.Size));
			DecryptTable(hashdata, "(hash table)");

			BinaryReader br2 = new BinaryReader(new MemoryStream(hashdata));
			//mHashes = new MpqHash[mHeader.HashTableSize];
            dcHashes = new Dictionary<UInt64, MpqHash>((int)mHeader.HashTableSize);
            
            for (int i = 0; i < mHeader.HashTableSize; i++)
            {
                //mHashes[i] = new MpqHash(br2);
                MpqHash mpqHash = new MpqHash(br2);

                // 63   32 31    0
                // |     | |     |
                // [name2] [name1]

                if (mpqHash.IsValid)                                    
                    //dcHashes.Add((((UInt64)mpqHash.Name2) << 32) | mpqHash.Name1, mpqHash);
                    dcHashes[(((UInt64)mpqHash.Name2) << 32) | mpqHash.Name1] = mpqHash;
            }            

			// Load block table
			mStream.Seek(mHeader.BlockTablePos, SeekOrigin.Begin);
			byte[] blockdata = br.ReadBytes((int)(mHeader.BlockTableSize * MpqBlock.Size));
			DecryptTable(blockdata, "(block table)");

            // workaround for badly read archives
            if (blockdata.Length != (mHeader.BlockTableSize * MpqBlock.Size))
                mHeader.BlockTableSize = (uint)(blockdata.Length / MpqBlock.Size);

			br2 = new BinaryReader(new MemoryStream(blockdata));
			mBlocks = new MpqBlock[mHeader.BlockTableSize];

			for (int i = 0; i < mHeader.BlockTableSize; i++)
				mBlocks[i] = new MpqBlock(br2, (uint)mHeaderOffset);

            isValid = true;
		}
		
		private bool LocateMpqHeader()
		{
			BinaryReader br = new BinaryReader(mStream);

			// In .mpq files the header will be at the start of the file
			// In .exe files, it will be at a multiple of 0x200
			for (long i = 0; i < mStream.Length - MpqHeader.Size; i += 0x200)
			{
				mStream.Seek(i, SeekOrigin.Begin);
				mHeader = new MpqHeader(br);

				if (mHeader.ID == MpqHeader.MpqId)
				{
					mHeaderOffset = i;
					mHeader.HashTablePos += (uint)mHeaderOffset;
					mHeader.BlockTablePos += (uint)mHeaderOffset;
					if (mHeader.DataOffset == 0x6d9e4b86)
					{
						// then this is a protected archive
						mHeader.DataOffset = (uint)(MpqHeader.Size + i);
					}
					return true;
				}
			}
			return false;
		}
		
		public MpqStream OpenFile(string Filename)
		{
			MpqHash hash;
			MpqBlock block;

			hash = GetHashEntry(Filename);
			
			if (!hash.IsValid) 
				throw new FileNotFoundException("File not found: " + Filename);
			
			block = mBlocks[hash.BlockIndex];

			return new MpqStream(this, block);
		}
		
		public bool FileExists(string Filename)
		{
			MpqHash hash = GetHashEntry(Filename);
			return (hash.IsValid);
		}        

        public uint GetFileSize(string Filename)
        {
            MpqHash hash = GetHashEntry(Filename);
            if (hash.IsValid)
            {
                MpqBlock block = mBlocks[hash.BlockIndex];
                return block.FileSize;
            }

            return 0xFFFFFFFF;
        }

        public uint GetFileSize(MpqHash Filehash)
        {
            if (Filehash.IsValid)
            {
                MpqBlock block = mBlocks[Filehash.BlockIndex];
                return block.FileSize;
            }

            return 0xFFFFFFFF;
        }
		
		internal Stream BaseStream
		{ get { return mStream; } }
		
		internal int BlockSize
		{ get { return mBlockSize; } }

		public MpqHash GetHashEntry(string Filename)
		{
			//uint index = HashString(Filename, 0);
			//index  &= mHeader.HashTableSize - 1;
			uint name1 = HashString(Filename, 0x100);
			uint name2 = HashString(Filename, 0x200);

            // 63   32 31    0
            // |     | |     |
            // [name2] [name1]

            MpqHash hash;
            if (dcHashes.TryGetValue((((UInt64)name2) << 32) | name1, out hash))
                return hash;
            

            /*for (uint i = index; ; )
            {
                MpqHash hash = mHashes[i];
                if (hash.Name1 == name1 && hash.Name2 == name2) return hash;

                if (++i >= mHashes.Length) i = 0;
                if (i == index) break;
            }*/

            return MpqHash.InvalidHash();
		}

		internal static uint HashString(string Input, int Offset)
		{
			uint seed1 = 0x7fed7fed;
			uint seed2 = 0xeeeeeeee;
			
			foreach(char c in Input)
			{
				int val = (int)char.ToUpper(c);
				seed1 = sStormBuffer[Offset + val] ^ (seed1 + seed2);
				seed2 = (uint)val + seed1 + seed2 + (seed2 << 5) + 3;
			}
			return seed1;
		}
		
		// Used for Hash Tables and Block Tables
		internal static void DecryptTable(byte[] Data, string Key)
		{
			DecryptBlock(Data, HashString(Key, 0x300));
		}

		internal static void DecryptBlock(byte[] Data, uint Seed1)
		{
			uint seed2 = 0xeeeeeeee;

			// NB: If the block is not an even multiple of 4,
			// the remainder is not encrypted
			for (int i = 0; i < Data.Length - 3; i += 4)
			{
				seed2 += sStormBuffer[0x400 + (Seed1 & 0xff)];

				uint result = BitConverter.ToUInt32(Data, i);

				result ^= (Seed1 + seed2);

				Seed1 = ((~Seed1 << 21) + 0x11111111) | (Seed1 >> 11);
				seed2 = result + seed2 + (seed2 << 5) + 3;

				if (BitConverter.IsLittleEndian) {
					Data[i + 0] = ((byte)(result & 0xff));
					Data[i + 1] = ((byte)((result >> 8) & 0xff));
					Data[i + 2] = ((byte)((result >> 16) & 0xff));
					Data[i + 3] = ((byte)((result >> 24) & 0xff));
				}
				else {
					Data[i + 3] = ((byte)(result & 0xff));
					Data[i + 2] = ((byte)((result >> 8) & 0xff));
					Data[i + 1] = ((byte)((result >> 16) & 0xff));
					Data[i + 0] = ((byte)((result >> 24) & 0xff));
				}
			}
		}
		
		internal static void DecryptBlock(uint[] Data, uint Seed1)
		{
			uint seed2 = 0xeeeeeeee;

			for (int i = 0; i < Data.Length; i++)
			{
				seed2 += sStormBuffer[0x400 + (Seed1 & 0xff)];
				uint result = Data[i];
				result ^= Seed1 + seed2;
				
				Seed1 = ((~Seed1 << 21) + 0x11111111) | (Seed1 >> 11);
				seed2 = result + seed2 + (seed2 << 5) + 3;
				Data[i] = result;
			}
		}

		// This function calculates the encryption key based on
		// some assumptions we can make about the headers for encrypted files
		internal static uint DetectFileSeed(uint[] Data, uint Decrypted)
		{
			uint value0 = Data[0];
			uint value1 = Data[1];
			uint temp = (value0 ^ Decrypted) - 0xeeeeeeee;
			
			for (int i = 0; i < 0x100; i++)
			{
				uint seed1 = temp - sStormBuffer[0x400 + i];
				uint seed2 = 0xeeeeeeee + sStormBuffer[0x400 + (seed1 & 0xff)];
				uint result = value0 ^ (seed1 + seed2);

				if (result != Decrypted)
				    continue;

				uint saveseed1 = seed1;
				
				// Test this result against the 2nd value
				seed1 = ((~seed1 << 21) + 0x11111111) | (seed1 >> 11);
				seed2 = result + seed2 + (seed2 << 5) + 3;
				
				seed2 += sStormBuffer[0x400 + (seed1 & 0xff)];
				result = value1 ^ (seed1 + seed2);

                if ((result & 0xfff00000) == 0) // DANAT: changed from 0xfffc0000 to 0xfff00000
					return saveseed1;
			}
			return 0;
		}

        internal static uint DetectFileSeed(uint Value0, uint Value1, uint Decrypted1, uint Decrypted2)
        {
            uint temp = (Value0 ^ Decrypted1) - 0xeeeeeeee;

            for (int i = 0; i < 0x100; i++)
            {
                uint seed1 = temp - sStormBuffer[0x400 + i];
                uint seed2 = 0xeeeeeeee + sStormBuffer[0x400 + (seed1 & 0xff)];
                uint result = Value0 ^ (seed1 + seed2);

                if (result != Decrypted1)
                    continue;

                uint saveseed1 = seed1;

                // Test this result against the 2nd value
                seed1 = ((~seed1 << 21) + 0x11111111) | (seed1 >> 11);
                seed2 = result + seed2 + (seed2 << 5) + 3;

                seed2 += sStormBuffer[0x400 + (seed1 & 0xff)];
                result = Value1 ^ (seed1 + seed2);

                if (result == Decrypted2) return saveseed1;
            }
            return 0;
        }

        private static uint[] BuildStormBuffer()
		{
			uint seed = 0x100001;
			
			uint[] result = new uint[0x500];
			
			for(uint index1 = 0; index1 < 0x100; index1++)
			{
				uint index2 = index1;
				for(int i = 0; i < 5; i++, index2 += 0x100)
				{
					seed = (seed * 125 + 3) % 0x2aaaab;
					uint temp = (seed & 0xffff) << 16;
					seed = (seed * 125 + 3) % 0x2aaaab;

					result[index2]  = temp | (seed & 0xffff);
				}
			}

			return result;
		}
	    
        // OW
        #region File Info Support
	    
	    public class FileInfo
	    {
            public string Name;
            public long CompressedSize;
            public long UncompressedSize;
            public MpqFileFlags Flags;
        }

	    string _ExternalListFile = null;

        /// <summary>
        /// Gets or sets the external list file. This setting overrides any list file contained in the archive
        /// </summary>
        /// <value>The external list file.</value>
	    public string ExternalListFile
	    {
            get
            {
                return _ExternalListFile;
            }
	        
	        set
	        {
	            _ExternalListFile = value;
	        }
	    }
	    
        protected FileInfo[] _Files;

        /// <summary>
        /// Returns a collection of file infos for the archive. The archive must contain "ListFileName" (default "(listfile)") for this to work.
        /// </summary>
        /// <value>The files.</value>
        public FileInfo[] Files
        {
            get
            {
                if (this._Files == null)
                {
                    try
                    {
                        Stream stm = null;

                        // Open list file stream - either external or internal
                        if (ExternalListFile != null && ExternalListFile != "")
                            stm = new FileStream(ExternalListFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                        else
                            stm = OpenFile("(listfile)");

                        using (stm)
                        {
                            using (StreamReader reader = new StreamReader(stm))
                            {
                                string data;
                                List<FileInfo> files = new List<FileInfo>();

                                while ((data = reader.ReadLine()) != null)
                                {
                                    MpqHash hash = GetHashEntry(data);
                                    if (!hash.IsValid) continue;
                                    MpqBlock block = mBlocks[hash.BlockIndex];

                                    // initialize and add new FileInfo
                                    FileInfo fi = new FileInfo();
                                    fi.Name = data;
                                    fi.Flags = block.Flags;
                                    fi.UncompressedSize = block.FileSize;
                                    fi.CompressedSize = block.CompressedSize;

                                    files.Add(fi);
                                }

                                this._Files = files.ToArray();
                            }
                        }
                    }

                    catch (FileNotFoundException)
                    {
                        throw new NotSupportedException("Error: the archive contains no listfile");
                    }
                }

                return this._Files;
            }
        #endregion    // File Info Support
        }	    
	}
}
