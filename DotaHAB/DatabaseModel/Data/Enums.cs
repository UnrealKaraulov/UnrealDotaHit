using System;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Collections.Specialized;

namespace DotaHIT.DatabaseModel.Data
{
    public enum DbValueType
    {
        Unknown = 0,
        Number,
        String,
        Image,
        Struct,
        Array
    }
}


