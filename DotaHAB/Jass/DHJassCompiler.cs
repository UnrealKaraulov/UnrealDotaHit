using System;
using System.Collections.Generic;
using System.Text;

namespace DotaHIT.Jass
{
    using Types;
    using Operations;

    public class DHJassCompiler
    {
        public static Stack<DHJassFunction> Functions = new Stack<DHJassFunction>();
        public static Stack<DHJassLoopOperation> Loops = new Stack<DHJassLoopOperation>();
    }
}
