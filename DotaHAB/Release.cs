using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;
using DotaHIT.Core.Resources;
using System.Threading;

namespace DotaHIT
{
    public class DHRELEASE
    {
        public static string CurrentVersion { get { return "NEW v1.05"; } }

       
        public static bool TryGetUpdate(bool showSplash, bool notifyOnLatest, out string sfxPackageName)
        {
            sfxPackageName = "";
            return false;
        }

        public static bool CreateBatchScript(string batchName, string sfxPackageName)
        {
            return false;
        }
    }
}
