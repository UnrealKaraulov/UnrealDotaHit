using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace SoundPlayerEx
{
    public class SoundPlayerAsync
    {
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern bool PlaySound(byte[] ptrToSound,
           System.UIntPtr hmod, uint fdwSound);

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern bool PlaySound(IntPtr ptrToSound,
           System.UIntPtr hmod, uint fdwSound);

        static private GCHandle? gcHandle = null;
        private static byte[] bytesToPlay = null;
        private static byte[] BytesToPlay
        {
            get { return bytesToPlay; }
            set
            {
                FreeHandle();
                bytesToPlay = value;
            }
        }

        public SoundPlayerAsync() { }

        public void PlaySound(System.IO.Stream stream)
        {
            PlaySound(stream, SoundFlags.SND_MEMORY |
                              SoundFlags.SND_ASYNC);
        }

        public void PlaySound(MemoryStream ms)
        {
            PlaySound(ms, SoundFlags.SND_MEMORY |
                              SoundFlags.SND_ASYNC);
        }

        public void PlaySound(System.IO.Stream stream,
                                     SoundFlags flags)
        {
            LoadStream(stream);            

            if (BytesToPlay != null)
            {
                gcHandle = GCHandle.Alloc(BytesToPlay,
                                         GCHandleType.Pinned);
                PlaySound(gcHandle.Value.AddrOfPinnedObject(),
                                     (UIntPtr)0, (uint)flags);
            }
            else
            {
                PlaySound((byte[])null, (UIntPtr)0, (uint)flags);
            }
        }

        public void PlaySound(MemoryStream ms,
                                     SoundFlags flags)
        {
            LoadStream(ms);           

            if (BytesToPlay != null)
            {
                gcHandle = GCHandle.Alloc(BytesToPlay,
                                         GCHandleType.Pinned);
                PlaySound(gcHandle.Value.AddrOfPinnedObject(),
                                     (UIntPtr)0, (uint)flags);
            }
            else
            {
                PlaySound((byte[])null, (UIntPtr)0, (uint)flags);
            }
        }

        private static void LoadStream(System.IO.Stream stream)
        {
            if (stream != null)
            {
                byte[] bytesToPlay = new byte[stream.Length];
                stream.Read(bytesToPlay, 0, (int)stream.Length);
                BytesToPlay = bytesToPlay;
            }
            else
            {
                BytesToPlay = null;
            }
        }

        private static void LoadStream(MemoryStream ms)
        {
            if (ms != null)
            {
                BytesToPlay = ms.GetBuffer();
            }
            else
            {
                BytesToPlay = null;
            }
        }

        private static void FreeHandle()
        {
            if (gcHandle != null)
            {
                //PlaySound((byte[])null, (UIntPtr)0, (uint)0);
                gcHandle.Value.Free();
                gcHandle = null;
            }
        }

        ~SoundPlayerAsync()
        {
            FreeHandle();
        }
    }

    [Flags]
    public enum SoundFlags : int
    {
        SND_SYNC = 0x0000,            // play synchronously (default)
        SND_ASYNC = 0x0001,        // play asynchronously
        SND_NODEFAULT = 0x0002,        // silence (!default) if sound not found
        SND_MEMORY = 0x0004,        // pszSound points to a memory file
        SND_LOOP = 0x0008,            // loop the sound until next sndPlaySound
        SND_NOSTOP = 0x0010,        // don't stop any currently playing sound
        SND_NOWAIT = 0x00002000,        // don't wait if the driver is busy
        SND_ALIAS = 0x00010000,        // name is a registry alias
        SND_ALIAS_ID = 0x00110000,        // alias is a predefined id
        SND_FILENAME = 0x00020000,        // name is file name
    }            
}
