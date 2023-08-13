using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace BlpLib
{    
    public class BlpLibWrapper
    {
        [DllImport("libblp.dll")]
        extern public static UInt32 LoadBLP(byte[] destBuf, byte[] srcBuf, out Int32 width, out Int32 height, out UInt32 type, out UInt32 subtype, bool convertToRGB);

        [DllImport("libblp.dll")]
        extern public static UInt32 LoadBLP(IntPtr destBuf, byte[] srcBuf, out Int32 width, out Int32 height, out UInt32 type, out UInt32 subtype, bool convertToRGB);
        
        static public Bitmap BlpToBitmap(MemoryStream ms, PixelFormat pf)
        {
            if (ms.Length == 0) return null;

            int width, height;
            uint type, subtype;

            byte[] srcBlp = ms.GetBuffer();

            //////////////////////////////
            // get required texture size
            //////////////////////////////

            uint textureSize = LoadBLP(IntPtr.Zero, srcBlp, out width, out height, out type, out subtype, false);

            IntPtr scan0 = Marshal.AllocHGlobal((int)textureSize);          

            LoadBLP(scan0, srcBlp, out width, out height, out type, out subtype, false);

            Bitmap bmp = new Bitmap(width, height, 
                (int)(textureSize / height), 
                pf == PixelFormat.DontCare ? PixelFormat.Format32bppRgb : pf,
                scan0);

            return bmp;       
        }
        static public Bitmap BlpToBitmap(MemoryStream ms)
        {
            return BlpToBitmap(ms, PixelFormat.DontCare);
        }        

        static protected byte[] CreateEmptyBitmap(int width, int height, ushort bpp, uint imageSize, out uint headerSize)
        {
            BITMAPINFO bi = new BITMAPINFO();
            bi.biSize = (uint)Marshal.SizeOf(bi);
            bi.biBitCount = bpp; // bit depth
            bi.biClrUsed = 0;
            bi.biClrImportant = 0;
            bi.biCompression = 0; // BI_RGB
            bi.biHeight = -height;
            bi.biWidth = width;            
            bi.biPlanes = 1;
            bi.biSizeImage = imageSize;            

            BITMAPFILEHEADER bfh = new BITMAPFILEHEADER();
            
            uint Fsize = (uint)Marshal.SizeOf(bfh) + (uint)Marshal.SizeOf(bi) + imageSize;            
                        
            bfh.bfType = 0x4D42; // 'B' and 'M' bytes for BM type
            bfh.bfSize = Fsize; // total bitmap file size
            bfh.bfOffBits = Fsize - imageSize; // bitmap header offset

            headerSize = bfh.bfOffBits;

            //////////////////////////////////////////////////////////
            // create memory buffer that will hold the complete bitmap
            //////////////////////////////////////////////////////////
            byte[] buffer = new byte[Fsize];

            unsafe
            {
                fixed (byte* bPtr = buffer)
                {
                    IntPtr bfhPtr = new IntPtr((void*)bPtr);
                    IntPtr bihPtr = new IntPtr((void*)(bPtr + Marshal.SizeOf(bfh))); 

                    ////////////////////////////////////////
                    //  write bitmap header to sream 
                    ////////////////////////////////////////
                    Marshal.StructureToPtr(bfh, bfhPtr, true);

                    ////////////////////////////////////////
                    //  write bitmap info header to sream 
                    ////////////////////////////////////////
                    Marshal.StructureToPtr(bi, bihPtr, true);
                }
            }           

            return buffer;
        }
    }

    [StructLayout(LayoutKind.Sequential,Pack=1)]
    struct BITMAPFILEHEADER
    {
        public ushort bfType;
        public uint bfSize;
        public ushort bfReserved1;
        public ushort bfReserved2;
        public uint bfOffBits;
    }   

    [StructLayout(LayoutKind.Sequential,Pack=1)]
    public struct BITMAPINFO
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;       
    }
}    

