using System;
using System.Collections.Generic;
using System.Text;
using DotaHIT.Core;
using DotaHIT.Core.Resources;

namespace Deerchao.War3Share.W3gParser
{
    public struct ParseSettings
    {        
        public bool EmulateInventory;        
        public ReplayPreviewEventHandler Preview;
        public ReplayPreviewEventHandler MapPreview;
    }
}
