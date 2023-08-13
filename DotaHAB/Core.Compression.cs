using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing.Imaging;

namespace DotaHIT.Core.Compression
{
    public class GZipArchive : IEnumerable<KeyValuePair<string, byte[]>>
    {
        static int HeaderID = 'D' | ('H' << 8) | ('G' << 16) | ('A' << 24);

        protected int version;
        protected Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

        public GZipArchive()
        {
            version = 0;
        }        

        IEnumerator<KeyValuePair<string, byte[]>> IEnumerable<KeyValuePair<string, byte[]>>.GetEnumerator()
        {
            return files.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return files.GetEnumerator();
        }

        public void AddFile(string name, byte[] bytes)
        {
            files[name] = bytes;
        }

        public void AddHpc(string name, HabPropertiesCollection hpc)
        {
            MemoryStream ms = new MemoryStream();
            hpc.SaveToStream(ms);
            files[name] = ms.ToArray();
        }
        public void AddHpc(string name, HabPropertiesCollection hpc, string requiredPropName, string[] skipPropValues, bool keepParams, params string[] propNames)
        {
            MemoryStream ms = new MemoryStream();
            hpc.SaveToStream(ms, requiredPropName, skipPropValues, keepParams, propNames);
            files[name] = ms.ToArray();
        }
        public HabPropertiesCollection GetHpc(string name)
        {
            byte[] bytes;
            if (files.TryGetValue(name, out bytes))
                return new HabPropertiesCollection(new MemoryStream(bytes, 0, bytes.Length, false, true));

            return new HabPropertiesCollection();
        }

        public void AddHps(string name, HabProperties hps)
        {
            MemoryStream ms = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(hps.Count);

                foreach (KeyValuePair<string, object> kvp in hps)
                {
                    bw.Write(kvp.Key + "");
                    bw.Write(kvp.Value + "");
                }
            }

            files[name] = ms.ToArray();
        }
        public HabProperties GetHps(string name)
        {
            HabProperties hps = new HabProperties();

            byte[] bytes;
            if (files.TryGetValue(name, out bytes))
            {
                using (BinaryReader br = new BinaryReader(new MemoryStream(bytes)))
                {
                    int count = br.ReadInt32();
                    while (count-- > 0)
                    {
                        hps.Add(br.ReadString(), br.ReadString());
                    }
                }
            }

            return hps;
        }

        public void AddImage(string name, Bitmap image)
        {
            MemoryStream ms = new MemoryStream();            
            image.Save(ms, ImageFormat.Bmp);//image.RawFormat);
            ms.Close();

            files[name] = ms.ToArray();
        }
        public Bitmap GetImage(string name)
        {
            byte[] bytes;
            if (files.TryGetValue(name, out bytes))
                return (Bitmap)Bitmap.FromStream(new MemoryStream(bytes), true);

            return null;
        }

        public void AddStringDictionary(string name, StringDictionary dc)
        {
            MemoryStream ms = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(dc.Count);

                foreach (DictionaryEntry de in dc)
                {
                    bw.Write(de.Key + "");
                    bw.Write(de.Value + "");
                }
            }

            files[name] = ms.ToArray();
        }
        public void AddGenericStringDictionary(string name, Dictionary<string, string> dc)
        {
            MemoryStream ms = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(dc.Count);

                foreach (KeyValuePair<string, string> kvp in dc)
                {
                    bw.Write(kvp.Key + "");
                    bw.Write(kvp.Value + "");
                }
            }

            files[name] = ms.ToArray();
        }
        public StringDictionary GetStringDictionary(string name)
        {
            StringDictionary dc = new StringDictionary();

            byte[] bytes;
            if (files.TryGetValue(name, out bytes))
            {
                using (BinaryReader br = new BinaryReader(new MemoryStream(bytes)))
                {
                    int count = br.ReadInt32();
                    while (count-- > 0)
                    {
                        dc.Add(br.ReadString(), br.ReadString());
                    }
                }
            }

            return dc;
        }
        public Dictionary<string, string> GetGenericStringDictionary(string name)
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();

            byte[] bytes;
            if (files.TryGetValue(name, out bytes))
            {
                using (BinaryReader br = new BinaryReader(new MemoryStream(bytes)))
                {
                    int count = br.ReadInt32();
                    while (count-- > 0)
                    {
                        dc.Add(br.ReadString(), br.ReadString());
                    }
                }
            }

            return dc;
        }

        public byte[] this[string filename]
        {
            get
            {
                byte[] bytes;
                files.TryGetValue(filename, out bytes);
                return bytes;
            }
            set
            {
                files[filename] = value;
            }
        }

        public void SaveToFile(string filename, int version)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);            

            BinaryWriter ubw = new BinaryWriter(fs);// uncompressed binary writer 

            this.version = version;

            ubw.Write(HeaderID);
            ubw.Write(version);
            ubw.Write(files.Count);

            int sizePosition = (int)ubw.BaseStream.Position;
            int uncompressedSize = 0;
            ubw.Write(uncompressedSize); // will be overwritten later            

            MemoryStream ms = new MemoryStream();
            using (BinaryWriter mbw = new BinaryWriter(ms))
            {
                foreach (KeyValuePair<string, byte[]> kvp in files)
                {
                    mbw.Write(kvp.Key);
                    mbw.Write(kvp.Value.Length);
                    mbw.Write(kvp.Value);                    
                }

                uncompressedSize = (int)ms.Length;                
            }

            DHCOMPRESSOR.WriteGzipCompressed(fs, ms.GetBuffer(), uncompressedSize);

            ubw.BaseStream.Position = sizePosition;
            ubw.Write(uncompressedSize);

            ubw.Close();
        }

        public int ReadFromFile(string filename)
        {
            files.Clear();

            byte[] buffer = null;
            int numberOfFiles;

            FileStream infile = File.OpenRead(filename);
            using (BinaryReader ubr = new BinaryReader(infile))
            {

                if (ubr.ReadInt32() != HeaderID)
                {
                    version = -1;
                    ubr.Close();
                    return version;
                }
                version = ubr.ReadInt32();
                numberOfFiles = ubr.ReadInt32();
                int decompressedSize = ubr.ReadInt32();

                buffer = DHCOMPRESSOR.ReadGzipDecompressed(infile, decompressedSize);                
            }

            using(BinaryReader br = new BinaryReader(new MemoryStream(buffer)))
            {
                while (numberOfFiles-- > 0)
                {
                    string key = br.ReadString();
                    int size = br.ReadInt32();
                    byte[] value = br.ReadBytes(size);

                    files.Add(key, value);
                }
            }

            return version;
        }

        public int Version
        {
            get { return version; }            
        }

        public static GZipArchive FromFile(string filename)
        {
            GZipArchive archive = new GZipArchive();
            archive.ReadFromFile(filename);
            return archive;
        }        
    }

    public class DHCOMPRESSOR
    {
        public static byte[] GzipCompress(byte[] array)
        {
            MemoryStream stream = new MemoryStream(array.Length);
            GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress);
            gZipStream.Write(array, 0, array.Length);
            return stream.ToArray();
        }

        public static int WriteGzipCompressed(Stream stream, byte[] array, int length)
        {
            int position = (int)stream.Position;

            GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress, true);
            gZipStream.Write(array, 0, length);             
            gZipStream.Close();

            // return bytes written count
            return (int)stream.Position - position;
        }        

        public static byte[] ReadGzipDecompressed(Stream stream, int decompressedSize)
        {
            // create buffer for decompressed data
            byte[] buffer = new byte[decompressedSize];                     

            // now decompress it
            GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress, true);

            int bytesRead = 0;            
            do
            {
                bytesRead += gZipStream.Read(buffer, bytesRead, decompressedSize);
            }
            while (bytesRead > 0 && bytesRead < decompressedSize);

            gZipStream.Close();

            return buffer;
        }

        public static byte[] ReadGzipDecompressed(Stream stream)
        {
            // create buffer for decompressed data
            int bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];

            // stream to hold entire decompressed data
            MemoryStream ms = new MemoryStream();

            // now decompress it
            GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress, true);

            int bytesRead = 0;
            do
            {
                bytesRead += gZipStream.Read(buffer, 0, bufferSize);
                ms.Write(buffer, 0, bytesRead);
            }
            while (bytesRead > 0);

            gZipStream.Close();

            return ms.ToArray();
        }
    } 
}
