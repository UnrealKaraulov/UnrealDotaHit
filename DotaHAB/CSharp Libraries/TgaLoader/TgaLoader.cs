using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Tga
{
    /// <summary>
    /// Source: http://www.gpwiki.org/index.php/LoadTGACpp
    /// </summary>
    public class TgaLoader
    {
        short iWidth,iHeight,iBPP;
        int lImageSize;
        byte bEnc;
        byte[] pImage, pPalette, pData;
        bool isFlipped;

        public TgaLoader()
        {
        }

        public bool Load(string filename)
        {
            return Load(File.ReadAllBytes(filename));
        }

        public bool Load(MemoryStream stream)
        {
            return Load(stream.GetBuffer());
        }

        public bool Load(byte[] data)
        {              
            int ulSize;            
             
            // Clear out any existing image and palette
            pImage = null;
            pPalette = null;
 
            // Read the file into memory
            pData = data;
            ulSize = pData.Length;

            // Process the header
            ReadHeader();

            switch (bEnc)
            {
                case 1: // Raw Indexed
                    {
                        // Check filesize against header values
                        if ((lImageSize + 18 + pData[0] + 768) > ulSize)
                            throw new Exception("IMG_ERR_BAD_FORMAT");

                        // Double check image type field
                        if (pData[1] != 1)
                            throw new Exception("IMG_ERR_BAD_FORMAT");

                        // Load image data
                        LoadRawData();

                        // Load palette
                        LoadTgaPalette();
                        break;
                    }

                case 2: // Raw RGB
                    {
                        // Check filesize against header values
                        if ((lImageSize + 18 + pData[0]) > ulSize)
                            throw new Exception("IMG_ERR_BAD_FORMAT");

                        // Double check image type field
                        if (pData[1] != 0)
                            throw new Exception("IMG_ERR_BAD_FORMAT");

                        // Load image data
                        LoadRawData();

                        //BGRtoRGB(); // Convert to RGB
                        break;
                    }

                case 9: // RLE Indexed
                    {
                        // Double check image type field
                        if (pData[1] != 1)
                            throw new Exception("IMG_ERR_BAD_FORMAT");

                        // Load image data
                        LoadTgaRLEData();

                        // Load palette
                        LoadTgaPalette();
                        break;
                    }

                case 10: // RLE RGB
                    {
                        // Double check image type field
                        if (pData[1] != 0)
                            throw new Exception("IMG_ERR_BAD_FORMAT");

                        // Load image data
                        LoadTgaRLEData();

                        //BGRtoRGB(); // Convert to RGB
                        break;
                    }

                default:
                    throw new Exception("IMG_ERR_UNSUPPORTED");
            }

            // Check flip bit
            if ((pData[17] & 0x20) == 0)
            {
                isFlipped = true;
                //FlipImg();
            }
 
            // Release file memory
            pData = null;  
 
            return true;
        }

        public Bitmap ToBitmap()
        {
            Bitmap bmp = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0,0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* bmpMemPtr = (byte*)(void*)bmpData.Scan0.ToPointer();
                fixed (byte* imagePtr = pImage, palettePtr = pPalette)
                {                       
                    byte* imgPtr = imagePtr;
                    byte* bmpPtr = bmpMemPtr;
                    byte* palPtr;                    

                    if (iBPP == 8)
                    {                        
                        for (int y = iHeight-1; y >= 0; y--)
                        {
                            // if image is upside-down
                            if (isFlipped)
                                bmpPtr = bmpMemPtr + bmpData.Stride * y;

                            for (int x = 0; x < iWidth; x++)
                            {
                                // get color index stored in image data
                                byte colorIndex = *(imgPtr++);

                                // get pointer to color values in palette
                                palPtr = palettePtr + (colorIndex * 3);

                                *(bmpPtr++) = *(palPtr++);
                                *(bmpPtr++) = *(palPtr++);
                                *(bmpPtr++) = *(palPtr);
                            }
                        }
                    }

                    if (iBPP == 24)
                    {
                        int* imgPixelPtr = (int*)imgPtr;
                        int* bmpPixelPtr = (int*)bmpPtr;

                        for (int y = iHeight - 1; y >= 0; y--)
                        {
                            // if image is upside-down
                            if (isFlipped)
                                bmpPixelPtr = (int*)(bmpMemPtr + bmpData.Stride * y);

                            for (int x = 0; x < (bmpData.Stride / 4); x++)
                                *(bmpPixelPtr++) = *(imgPixelPtr++);                            
                        }
                    }
                }
            }

            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public int BPP
        {
            get
            {
                return iBPP;
            }
        }

        public int Width
        {
            get
            {
                return iWidth;
            }
        }

        public int Height
        {
            get
            {
                return iHeight;
            }
        }

        public int ImageSize
        {
            get
            {
                return lImageSize;
            }
        }
                
        /// <summary>
        /// Return a pointer to image data
        /// </summary>
        public byte[] Image
        {
            get
            {
                return pImage;
            }
        }
        
        /// <summary>
        /// Return a pointer to VGA palette
        /// </summary>
        public byte[] Palette
        {
            get
            {
                return pPalette;
            }
        }        

        // Internal workers
        void ReadHeader()
        {
            short ColMapStart, ColMapLen;
            short x1, y1, x2, y2;

            if (pData == null)
                throw new Exception("IMG_ERR_NO_FILE");

            if (pData[1] > 1)    // 0 (RGB) and 1 (Indexed) are the only types we know about
                throw new Exception("IMG_ERR_UNSUPPORTED");

            bEnc = pData[2];     // Encoding flag  1 = Raw indexed image
            //                2 = Raw RGB
            //                3 = Raw greyscale
            //                9 = RLE indexed
            //               10 = RLE RGB
            //               11 = RLE greyscale
            //               32 & 33 Other compression, indexed

            if (bEnc > 11)       // We don't want 32 or 33                
                throw new Exception("IMG_ERR_UNSUPPORTED");


            // Get palette info
            unsafe
            {
                fixed (byte* bpData = pData)
                {
                    ColMapStart = *(short*)(bpData + 3);
                    ColMapLen = *(short*)(bpData + 5);

                    // Reject indexed images if not a VGA palette (256 entries with 24 bits per entry)
                    if (pData[1] == 1) // Indexed
                    {
                        if (ColMapStart != 0 || ColMapLen != 256 || pData[7] != 24)
                            throw new Exception("IMG_ERR_UNSUPPORTED");
                    }

                    // Get image window and produce width & height values
                    x1 = *(short*)(bpData + 8);
                    y1 = *(short*)(bpData + 10);
                    x2 = *(short*)(bpData + 12);
                    y2 = *(short*)(bpData + 14);                    

                }
            }

            iWidth = (short)(x2 - x1);
            iHeight = (short)(y2 - y1);

            if (iWidth < 1 || iHeight < 1)                
                throw new Exception("IMG_ERR_BAD_FORMAT");

            // Bits per Pixel
            iBPP = pData[16];

            // Check flip / interleave byte
            if (pData[17] > 32) // Interleaved data
                throw new Exception("IMG_ERR_UNSUPPORTED");

            // Calculate image size
            lImageSize = (iWidth * iHeight * (iBPP / 8));            
        }

        void LoadRawData()
        {
            int iOffset;
            pImage = new byte[lImageSize];   
 
            iOffset=pData[0]+18; // Add header to ident field size

            if (pData[1] == 1) // Indexed images
                iOffset += 768;  // Add palette offset

            Buffer.BlockCopy(pData, iOffset, pImage, 0, lImageSize);            
        }

        void LoadTgaRLEData()
        { 
            int iOffset,iPixelSize;            
            int Index=0;
            int bLength,bLoop;
 
            // Calculate offset to image data
            iOffset= pData[0]+18;

            // Add palette offset for indexed images
            if(pData[1]==1)
                iOffset+=768;

            // Get pixel size in bytes
            iPixelSize=iBPP/8;
 
            // Set our pointer to the beginning of the image data
            unsafe
            {
                byte *pCur;
                fixed(byte* ptr = pData)
                {
                    pCur = ptr + iOffset;

                    // Allocate space for the image data            
                    pImage = new byte[lImageSize];   

                    // Decode
                    while(Index<lImageSize)
                    {
                        if (((*pCur) & 0x80)!=0) // Run length chunk (High bit = 1)
                        {
                            bLength = *pCur - 127; // Get run length
                            pCur++;            // Move to pixel data  

                            // Repeat the next pixel bLength times
                            for (bLoop = 0; bLoop != bLength; ++bLoop, Index += iPixelSize)
                                Buffer.BlockCopy(pData, (int)(pCur - ptr), pImage, Index, iPixelSize);

                            pCur += iPixelSize; // Move to the next descriptor chunk
                        }
                        else // Raw chunk
                        {
                            bLength = *pCur + 1; // Get run length
                            pCur++;          // Move to pixel data

                            // Write the next bLength pixels directly
                            for (bLoop = 0; bLoop != bLength; ++bLoop, Index += iPixelSize, pCur += iPixelSize)
                                Buffer.BlockCopy(pData, (int)(pCur - ptr), pImage, Index, iPixelSize);
                        }
                    }
                }
            }    
        }

        void LoadTgaPalette()
        {
            //byte bTemp;
            //short iIndex,iPalPtr;
  
            // Delete old palette if present
            pPalette = null;

            // Create space for new palette
            pPalette = new byte[768];

            // VGA palette is the 768 bytes following the header
            Buffer.BlockCopy(pData, pData[0] + 18, pPalette, 0, 768);

            // Palette entries are BGR ordered so we have to convert to RGB
            /*for (iIndex = 0, iPalPtr = 0; iIndex != 256; ++iIndex, iPalPtr += 3)
            {
                bTemp = pPalette[iPalPtr];               // Get Blue value
                pPalette[iPalPtr] = pPalette[iPalPtr + 2]; // Copy Red to Blue
                pPalette[iPalPtr + 2] = bTemp;             // Replace Blue at the end
            }*/  
        }        

        void FlipImg()
        {
            /* unsigned char bTemp;
  unsigned char *pLine1, *pLine2;
  int iLineLen,iIndex;
 
  iLineLen=iWidth*(iBPP/8);
  pLine1=pImage;
  pLine2=&pImage[iLineLen * (iHeight - 1)];
 
   for( ;pLine1<pLine2;pLine2-=(iLineLen*2))
    {
     for(iIndex=0;iIndex!=iLineLen;pLine1++,pLine2++,iIndex++)
      {
       bTemp=*pLine1;
       *pLine1=*pLine2;
       *pLine2=bTemp;       
      }
    } */
        }

        public static Bitmap BitmapFromTga(string filename)
        {
            TgaLoader loader = new TgaLoader();
            loader.Load(filename);
            return loader.ToBitmap();
        }

        public static Bitmap BitmapFromTga(MemoryStream stream)
        {
            TgaLoader loader = new TgaLoader();
            loader.Load(stream);
            return loader.ToBitmap();
        }
    }    
}
