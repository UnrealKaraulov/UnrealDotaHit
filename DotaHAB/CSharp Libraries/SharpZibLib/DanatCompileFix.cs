using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.SharpZipLib.Zip.Compression
{
    public class Deflater
    {
        #region Deflater Documentation
        /*
		* The Deflater can do the following state transitions:
		*
		* (1) -> INIT_STATE   ----> INIT_FINISHING_STATE ---.
		*        /  | (2)      (5)                          |
		*       /   v          (5)                          |
		*   (3)| SETDICT_STATE ---> SETDICT_FINISHING_STATE |(3)
		*       \   | (3)                 |        ,--------'
		*        |  |                     | (3)   /
		*        v  v          (5)        v      v
		* (1) -> BUSY_STATE   ----> FINISHING_STATE
		*                                | (6)
		*                                v
		*                           FINISHED_STATE
		*    \_____________________________________/
		*                    | (7)
		*                    v
		*               CLOSED_STATE
		*
		* (1) If we should produce a header we start in INIT_STATE, otherwise
		*     we start in BUSY_STATE.
		* (2) A dictionary may be set only when we are in INIT_STATE, then
		*     we change the state as indicated.
		* (3) Whether a dictionary is set or not, on the first call of deflate
		*     we change to BUSY_STATE.
		* (4) -- intentionally left blank -- :)
		* (5) FINISHING_STATE is entered, when flush() is called to indicate that
		*     there is no more INPUT.  There are also states indicating, that
		*     the header wasn't written yet.
		* (6) FINISHED_STATE is entered, when everything has been flushed to the
		*     internal pending output buffer.
		* (7) At any time (7)
		*
		*/
        #endregion
        #region Public Constants
        /// <summary>
        /// The best and slowest compression level.  This tries to find very
        /// long and distant string repetitions.
        /// </summary>
        public const int BEST_COMPRESSION = 9;

        /// <summary>
        /// The worst but fastest compression level.
        /// </summary>
        public const int BEST_SPEED = 1;

        /// <summary>
        /// The default compression level.
        /// </summary>
        public const int DEFAULT_COMPRESSION = -1;

        /// <summary>
        /// This level won't compress at all but output uncompressed blocks.
        /// </summary>
        public const int NO_COMPRESSION = 0;

        /// <summary>
        /// The compression method.  This is the only method supported so far.
        /// There is no need to use this constant at all.
        /// </summary>
        public const int DEFLATED = 8;
        #endregion
    }

    /// <summary>
    /// This is the DeflaterHuffman class.
    /// 
    /// This class is <i>not</i> thread safe.  This is inherent in the API, due
    /// to the split of Deflate and SetInput.
    /// 
    /// author of the original java version : Jochen Hoenicke
    /// </summary>
    public class DeflaterHuffman
    {        
        static readonly byte[] bit4Reverse = {
			0,
			8,
			4,
			12,
			2,
			10,
			6,
			14,
			1,
			9,
			5,
			13,
			3,
			11,
			7,
			15
		};                                 

        /// <summary>
        /// Reverse the bits of a 16 bit value.
        /// </summary>
        /// <param name="toReverse">Value to reverse bits</param>
        /// <returns>Value with bits reversed</returns>
        public static short BitReverse(int toReverse)
        {
            return (short)(bit4Reverse[toReverse & 0xF] << 12 |
                            bit4Reverse[(toReverse >> 4) & 0xF] << 8 |
                            bit4Reverse[(toReverse >> 8) & 0xF] << 4 |
                            bit4Reverse[toReverse >> 12]);
        }
    }
}
