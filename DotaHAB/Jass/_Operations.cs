using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.CodeDom;
using System.Reflection;
using DotaHIT.Jass.Operations;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Core;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Native.Events;
using DotaHIT.Jass.Native.Constants;
using System.Collections.Specialized;

namespace DotaHIT.Jass.Operations
{
    using DotaHIT.Jass.Types;
    using DotaHIT.Jass.Commands;

    public class DbJassTypeOperationKnowledge
    {
        public static Dictionary<Type, DHJassOperation> TypeOperationPairs = new Dictionary<Type, DHJassOperation>();

        static DbJassTypeOperationKnowledge()
        {
            TypeOperationPairs = CollectTypeOperationPairs();
        }
        public static void WakeUp() { }

        public static Dictionary<Type, DHJassOperation> CollectTypeOperationPairs()
        {
            Module m = Assembly.GetExecutingAssembly().GetModules(false)[0];

            Type[] types = m.FindTypes(new TypeFilter(SearchCriteria), "DotaHIT.Jass.Operations");

            Dictionary<Type, DHJassOperation> typeOperationPairs = new Dictionary<Type, DHJassOperation>();

            for (int i = 0; i < types.Length; i++)
            {
                try
                {
                    DHJassOperation value = (DHJassOperation)types[i].InvokeMember(null,
                                BindingFlags.Public | BindingFlags.DeclaredOnly |
                                BindingFlags.Instance | BindingFlags.CreateInstance,
                                null, null, null);

                    typeOperationPairs.Add(types[i], value);
                }
                catch
                {
                }
            }

            return typeOperationPairs;
        }
        private static bool SearchCriteria(Type t, object filter)
        {
            if ((t.Namespace == (string)filter) && t.IsPublic
                && t.IsSubclassOf(typeof(DHJassOperation))
                && !t.IsAbstract)
                return true;
            return false;
        }

        public static DHJassOperation CreateKnownOperation(List<string> lines, ref int line)
        {
            string currentLine = lines[line];
            if (!string.IsNullOrEmpty(currentLine))
            {
                unsafe
                {   
                    fixed (char* ptr = currentLine)
                    {
                        List<string> args = new List<string>(2);

                        foreach (DHJassOperation operation in TypeOperationPairs.Values)
                            if (operation.IsSyntaxMatchFast(ptr, currentLine.Length, ref args))
                                return operation.GetNew(lines, ref line, args);
                    }
                }
            }

            return null;
        }
    }

    public interface IBodyEndSyntaxHolder
    {
        bool CheckBodyEndSyntax(string code);
    }

    public abstract class DHJassOperation
    {
        public abstract DHJassOperation ProceedToNext(ref DHJassValue returnValue);
        public virtual void SetNext(DHJassOperation next) { }

        public abstract DHJassOperation GetLast();

        public static bool TryCreate(List<string> lines, ref int line, out DHJassOperation EntryPoint, out DHJassOperation ExitPoint)
        {
            DHJassOperation Operation = DbJassTypeOperationKnowledge.CreateKnownOperation(lines, ref line);
            if (Operation == null)
                for (line++; line < lines.Count; line++)
                {
                    Operation = DbJassTypeOperationKnowledge.CreateKnownOperation(lines, ref line);
                    if (Operation != null)
                    {
                        EntryPoint = Operation;
                        ExitPoint = Operation.GetLast();
                        return true;
                    }
                }
            else
            {
                EntryPoint = Operation;
                ExitPoint = Operation.GetLast();
                return true;
            }

            EntryPoint = null;
            ExitPoint = null;
            return false;
        }

        unsafe public virtual bool IsSyntaxMatchFast(char* code, int length, ref List<string> args)
        {
            return false;
        }
        public virtual bool IsSyntaxMatch(string code, out List<string> args)
        {
            args = new List<string>(2);
            unsafe
            {
                fixed (char* ptr = code)
                {
                    return IsSyntaxMatchFast(ptr, code.Length, ref args);
                }
            }
        }

        public abstract DHJassOperation GetNew(List<string> lines, ref int line, List<string> args);
    }
    public abstract class DHJassSimpleOperation : DHJassOperation
    {
        public DHJassOperation Next = null;

        public DHJassSimpleOperation() { }

        public override void SetNext(DHJassOperation next)
        {
            Next = next;
        }

        public override DHJassOperation GetLast()
        {
            return this;
        }
    }
    public class DHJassEmptyOperation : DHJassSimpleOperation
    {
        public override DHJassOperation ProceedToNext(ref DHJassValue returnValue)
        {
            return Next;
        }

        public override DHJassOperation GetNew(List<string> lines, ref int line, List<string> args)
        {
            return new DHJassEmptyOperation();
        }
    }

    public abstract class DHJassNonOperation : DHJassOperation
    {
        public override DHJassOperation ProceedToNext(ref DHJassValue returnValue)
        {
            return null;
        }
        public override void SetNext(DHJassOperation next)
        {
        }
        public override DHJassOperation GetLast()
        {
            return null;
        }
    }

    public class DHJassComments : DHJassNonOperation
    {
        // @"\A\W*//.*"
        unsafe public override bool IsSyntaxMatchFast(char* ptr, int length, ref List<string> args)
        {
            while (*ptr != 0)
            {
                if (char.IsDigit(*ptr) || char.IsLetter(*ptr)) return false;

                // 2f00 2f00 == "//"
                if (*(int*)ptr == 0x002F002F) return true;

                ptr++;
            }

            return false;
        }

        public override DHJassOperation GetNew(List<string> lines, ref int line, List<string> args)
        {
            return null;
        }
    }
    public class DHJassLocalDeclaration : DHJassNonOperation
    {
        //Match match = null;                
        DHJassDeclarationOnDemandCommand declaration;

        public DHJassLocalDeclaration() { }
        public DHJassLocalDeclaration(List<string> args)
        {
            //this.match = match;                    
            declaration = new DHJassDeclarationOnDemandCommand(args[0]);
        }

        public DHJassValue CreateLocal()
        {
            return declaration.GetResult();//DHJassValue.InitProperValue(match.Groups["declaration"].Value);
        }

        // @"\A\s*local\s+(?<declaration>.*)"
        unsafe public override bool IsSyntaxMatchFast(char* ptr, int length, ref List<string> args)
        {
            string localStr = "local"; // should be interned

            fixed (char* ptrB = localStr)
            {
                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // "local" keyword should be here
                if (!DHJassSyntax.checkStringsEqual(ptr, ptrB, localStr.Length)) return false;

                // skip the "local" bytes
                ptr += localStr.Length;

                // make sure the "local" is followed by whitespace
                if (*ptr != ' ') return false;

                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // add argument
                args.Add(new string(ptr));
                return true;
            }
        }

        public override DHJassOperation GetNew(List<string> lines, ref int line, List<string> args)
        {
            DHJassFunction function = DHJassCompiler.Functions.Peek();
            if (function != null)
                function.LocalDeclarations.Add(new DHJassLocalDeclaration(args));
            return null;
        }
    }

    public class DHJassReturnOperation : DHJassOperation
    {
        //string returnValueCode = null;
        DHJassGetValueOnDemandCommand returnCommand = null;

        public DHJassReturnOperation() { }
        public DHJassReturnOperation(List<string> lines, ref int line, List<string> args)
        {
            //returnValueCode = match.Groups["value"].Value;
            returnCommand = new DHJassGetValueOnDemandCommand(args[0]);
        }

        public override DHJassOperation ProceedToNext(ref DHJassValue returnValue)
        {
            /*if (DHJassExecutor.ShowReturnValues)
                Console.WriteLine("Returning:" + returnValueCode);
            returnValue = DHJassValue.CreateValueFromCode(this.returnValueCode);*/           

            returnValue = returnCommand.GetResult();
            return null;
        }
        public override void SetNext(DHJassOperation next)
        {
        }

        public override DHJassOperation GetLast()
        {
            return this;
        }

        // @"\A\s*return(\s*(?<value>.*)|(?<value>))"
        unsafe public override bool IsSyntaxMatchFast(char* ptr, int length, ref List<string> args)
        {
            string returnStr = "return"; // should be interned

            fixed (char* ptrB = returnStr)
            {
                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // "return" keyword should be here
                if (!DHJassSyntax.checkStringsEqual(ptr, ptrB, returnStr.Length)) return false;

                // skip the "return`" bytes
                ptr += returnStr.Length;

                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // add argument
                args.Add(new string(ptr));
                return true;
            }
        }

        public override DHJassOperation GetNew(List<string> lines, ref int line, List<string> args)
        {
            return new DHJassReturnOperation(lines, ref line, args);
        }
    }
    public class DHJassIfElseOperation : DHJassOperation, IBodyEndSyntaxHolder
    {
        public DHJassOperation Then = null;
        public DHJassOperation Else = null;
        public DHJassEmptyOperation EndPoint = null;
        //string Condition = "";
        DHJassGetValueOnDemandCommand Condition = null;

        public DHJassIfElseOperation() { }
        public DHJassIfElseOperation(List<string> lines, ref int line, List<string> args)
            : this(lines, ref line, args, new DHJassEmptyOperation()) { }

        public DHJassIfElseOperation(List<string> lines, ref int line, List<string> args, DHJassEmptyOperation endPoint)
        {
            DHJassOperation StatementEndPoint;

            Condition = new DHJassGetValueOnDemandCommand(args[0]);
            line++;
            if (DHJassFunction.TryCreateBody(lines, ref line, this, out Then, out StatementEndPoint))
            {
                this.EndPoint = endPoint;

                StatementEndPoint.SetNext(EndPoint);

                string currentLine = lines[line].Trim();

                switch (currentLine)
                {
                    case "else":
                        line++;
                        if (DHJassFunction.TryCreateBody(lines, ref line, this, out Else, out StatementEndPoint))
                            StatementEndPoint.SetNext(EndPoint);
                        break;

                    case "endif":
                        Else = EndPoint;
                        break;

                    default:
                        if (this.IsSyntaxMatch(currentLine, out args))
                        {
                            Else = new DHJassIfElseOperation(lines, ref line, args, endPoint);
                            //Else.GetLast().SetNext(EndPoint);
                        }
                        break;
                }
            }
        }

        public override DHJassOperation ProceedToNext(ref DHJassValue returnValue)
        {
            //DHJassBoolean BoolCondition = new DHJassBoolean(Condition);
            if (Condition.GetResult().BoolValue)//(bool)BoolCondition.Value)
            {
                #if DEBUG
                if (DHJassExecutor.LogExecution)
                {
                    Console.WriteLine("if " + Condition.GetDebugString() + " ->then");
                }
                #endif
                return Then;
            }
            else
            {
                #if DEBUG
                if (DHJassExecutor.LogExecution)
                {
                    Console.WriteLine("if " + Condition.GetDebugString() + " ->else");
                }
                #endif
                return Else;
            }
        }
        public override DHJassOperation GetLast()
        {
            return EndPoint;
        }

        // @"\A\s*(if|elseif)\s*(?<condition>.*[^\s]{1})\s*then"
        unsafe public override bool IsSyntaxMatchFast(char* code, int length, ref List<string> args)
        {
            string ifStr = "if"; // should be interned
            string elseifStr = "elseif";
            string thenStr = "then";

            char* ptr = code;
            fixed (char* ptrIF = ifStr, ptrELSEIF = elseifStr, ptrTHEN = thenStr)
            {
                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // "if" or "elseif" keyword should be here
                if (DHJassSyntax.checkStringsEqual(ptr, ptrIF, ifStr.Length))
                    ptr += ifStr.Length; // skip the "if" bytes
                else
                    if (DHJassSyntax.checkStringsEqual(ptr, ptrELSEIF, elseifStr.Length))
                        ptr += elseifStr.Length; // skip the "elseif" bytes
                    else
                        return false;

                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // go to the end of this string where "then" string should be
                char* pEnd = (code + length) - thenStr.Length;

                // check for "then" string
                if (!DHJassSyntax.checkStringsEqual(pEnd, ptrTHEN, thenStr.Length)) return false;

                // skip whitespaces from the end
                --pEnd;
                DHJassSyntax.skipWhiteSpacesReverseFast(ref pEnd);

                // make sure the condition is not empty
                if (ptr > pEnd) return false;

                // add argument
                args.Add(new string(ptr, 0, (int)(pEnd - ptr) + 1));
                return true;
            }
        }

        //@"\A\s*(else|elseif\s*.+|endif)"
        bool IBodyEndSyntaxHolder.CheckBodyEndSyntax(string code)
        {
            unsafe
            {
                fixed (char* pCode = code)
                {
                    char* tmpPtr = pCode;

                    // skip whitespaces
                    DHJassSyntax.skipWhiteSpacesFast(ref tmpPtr);

                    Int64* ptr = (Int64*)tmpPtr;

                    // 'else' == 0x00650073006C0065
                    // 'endi' == 0x00690064006E0065

                    switch (*ptr)
                    {
                        case 0x00650073006C0065: // 'else'
                            ptr++;
                            switch (*(Int16*)ptr)
                            {
                                case 0x0000: // '\0'
                                case 0x0020: // ' '
                                    return true;
                            }

                            // check for 'elseif'

                            // '\0if ' == 0x0020006600690000
                            // '\0if(' == 0x0028006600690000
                            // '\0if\0'== 0x0000006600690000

                            switch ((*ptr) << 16)
                            {
                                case 0x0020006600690000:
                                case 0x0028006600690000:
                                case 0x0000006600690000:
                                    return true;
                            }
                            return false;

                        case 0x00690064006E0065: // 'endi'
                            ptr++;

                            // 'f ' == 0x00200066
                            // 'f(' == 0x00280066
                            // 'f\0'== 0x00000066

                            switch (*(Int32*)ptr)
                            {
                                case 0x00200066:
                                case 0x00280066:
                                case 0x00000066:
                                    return true;
                            }
                            return false;
                    }

                    return false;
                }
            }
        }

        public override DHJassOperation GetNew(List<string> lines, ref int line, List<string> args)
        {
            return new DHJassIfElseOperation(lines, ref line, args);
        }
    }
    public class DHJassLoopOperation : DHJassOperation, IBodyEndSyntaxHolder
    {
        public DHJassOperation EntryPoint = null;
        public DHJassEmptyOperation EndPoint = new DHJassEmptyOperation();

        public DHJassLoopOperation() { }
        public DHJassLoopOperation(List<string> lines, ref int line, List<string> args)
        {
            DHJassCompiler.Loops.Push(this);

            DHJassOperation LoopEndPoint;
            line++;
            if (DHJassFunction.TryCreateBody(lines, ref line, this, out EntryPoint, out LoopEndPoint))
                LoopEndPoint.SetNext(EntryPoint);

            DHJassCompiler.Loops.Pop();
        }

        public override DHJassOperation ProceedToNext(ref DHJassValue returnValue)
        {
            #if DEBUG
            if (DHJassExecutor.LogExecution)
            {
                Console.WriteLine("loop");
            }
            #endif

            return EntryPoint.ProceedToNext(ref returnValue);
        }
        public override DHJassOperation GetLast()
        {
            return EndPoint;
        }

        // @"\A\s*loop"                
        unsafe public override bool IsSyntaxMatchFast(char* ptr, int length, ref List<string> args)
        {
            // skip whitespaces
            DHJassSyntax.skipWhiteSpacesFast(ref ptr);

            // 'loop' == 0x0070006F006F006C
            return (*(Int64*)ptr == 0x0070006F006F006C);
        }

        // @"\A\s*endloop"
        bool IBodyEndSyntaxHolder.CheckBodyEndSyntax(string code)
        {
            unsafe
            {
                fixed (char* pCode = code)
                {
                    char* tmpPtr = pCode;

                    // skip whitespaces
                    DHJassSyntax.skipWhiteSpacesFast(ref tmpPtr);

                    Int64* ptr = (Int64*)tmpPtr;

                    // 'endl' == 0x006C0064006E0065
                    // '\0oop'== 0x0070006F006F0000

                    return (*ptr == 0x006C0064006E0065) &&
                        ((*(++ptr)) << 16) == 0x0070006F006F0000;
                }
            }
        }

        public override DHJassOperation GetNew(List<string> lines, ref int line, List<string> args)
        {
            return new DHJassLoopOperation(lines, ref line, args);
        }
    }
    public class DHJassExitLoopWhenOperation : DHJassSimpleOperation
    {
        DHJassOperation LoopEndPoint = null;
        DHJassGetValueOnDemandCommand Condition = null;

        public DHJassExitLoopWhenOperation() { }
        public DHJassExitLoopWhenOperation(List<string> lines, ref int line, List<string> args)
        {
            Condition = new DHJassGetValueOnDemandCommand(args[0]);
            if (DHJassCompiler.Loops.Count != 0)
            {
                DHJassLoopOperation Loop = DHJassCompiler.Loops.Peek();
                LoopEndPoint = Loop.EndPoint;
            }
        }

        public override DHJassOperation ProceedToNext(ref DHJassValue returnValue)
        {
            if (Condition.GetResult().BoolValue)
            {
                #if DEBUG
                if (DHJassExecutor.LogExecution)
                {
                    Console.WriteLine("exitloopwhen "+ Condition.GetDebugString() + "" + " (exit)");
                }
                #endif
                return LoopEndPoint;
            }
            else
            {
                #if DEBUG
                if (DHJassExecutor.LogExecution)
                {
                    Console.WriteLine("exitloopwhen " + Condition.GetDebugString() + " (continue)");
                }
                #endif
                return Next;
            }
        }

        // @"\A\s*exitwhen\s*(?<condition>.*)"
        unsafe public override bool IsSyntaxMatchFast(char* ptr, int length, ref List<string> args)
        {
            string exitwhenStr = "exitwhen"; // should be interned

            fixed (char* ptrB = exitwhenStr)
            {
                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // "exitwhen" keyword should be here
                if (!DHJassSyntax.checkStringsEqual(ptr, ptrB, exitwhenStr.Length)) return false;

                // skip the "exitwhen`" bytes
                ptr += exitwhenStr.Length;

                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // add argument
                args.Add(new string(ptr));
                return true;
            }
        }

        public override DHJassOperation GetNew(List<string> lines, ref int line, List<string> args)
        {
            return new DHJassExitLoopWhenOperation(lines, ref line, args);
        }
    }
    public class DHJassSetOperation : DHJassSimpleOperation
    {
        //Match match = null;
        DHJassSetValueOnDemandCommand command;

        public DHJassSetOperation() { }
        public DHJassSetOperation(List<string> lines, ref int line, List<string> args)
        {
            command = new DHJassSetValueOnDemandCommand(args[0], args[1]);
            //this.match = match;
        }

        public override DHJassOperation ProceedToNext(ref DHJassValue returnValue)
        {
            //DHJassExecutor.TrySetValue(match);
            //string name_usage = nameValueMatch.Groups["name"].Value;
            //string value = nameValueMatch.Groups["value"].Value; 

            #if DEBUG
            if (DHJassExecutor.LogExecution)
            {
                Console.WriteLine(command.GetDebugString());
            }
            #endif  
       
            command.GetResult();

            return this.Next;
        }

        // @"\A\s*set\s+(?<name>[^=]+[^\s]|[^=]+)\s*=\s*(?<value>.+)"
        unsafe public override bool IsSyntaxMatchFast(char* ptr, int length, ref List<string> args)
        {
            string setStr = "set"; // should be interned

            fixed (char* ptrB = setStr)
            {
                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // "set" keyword should be here
                if (!DHJassSyntax.checkStringsEqual(ptr, ptrB, setStr.Length)) return false;

                // skip the "set`" bytes
                ptr += setStr.Length;

                // there should be white space after "set"
                if (*ptr != ' ') return false;

                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // move to the end of variable name
                char* pVariableEnd = DHJassSyntax.reachEndOfVarArrNameSyntaxFast(ptr);
                if (pVariableEnd == null) return false;

                // memorize starting position of variable's name
                char* pVariableStart = ptr;

                ptr = pVariableEnd;

                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // make sure the '=' is present
                if (*ptr != '=') return false;
                ++ptr; // move beyond '='

                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // add variable name argument
                args.Add(new string(pVariableStart, 0, (int)(pVariableEnd - pVariableStart)));

                // add variable value argument
                args.Add(new string(ptr));
                return true;
            }
        }

        public override DHJassOperation GetNew(List<string> lines, ref int line, List<string> args)
        {
            return new DHJassSetOperation(lines, ref line, args);
        }
    }
    public class DHJassCallOperation : DHJassSimpleOperation
    {
        //Match match = null;
        DHJassCallFunctionCommand command;

        public DHJassCallOperation() { }
        public DHJassCallOperation(List<string> lines, ref int line, List<string> args)
        {
            // concat this line with next if it contains a string with newline character
            // the determining factor is odd number of quotes

            int quotes = -1;
            string result = "";
            string currentLine = args[0];
            do
            {
                if (quotes != -1)
                {
                    currentLine = lines[++line];
                    result += Environment.NewLine + currentLine;
                }
                else
                {
                    quotes = 0;
                    result += currentLine;
                }

                quotes += DHJassSyntax.getQuoteCountFast(currentLine);                
            }
            while (quotes % 2 != 0);            

            command = new DHJassCallFunctionCommand(result);            
        }

        public override DHJassOperation ProceedToNext(ref DHJassValue returnValue)
        {
            //DHJassValue returned;
            //DHJassExecutor.TryCallFunction(match, out returned);
            //Console.WriteLine("+: GetResult");
            command.GetResult();
            //Console.WriteLine("-: GetResult");
            return this.Next;
        }

        // @"\A\s*call\s+(?<name>.+)"
        unsafe public override bool IsSyntaxMatchFast(char* ptr, int length, ref List<string> args)
        {
            string callStr = "call"; // should be interned

            fixed (char* ptrB = callStr)
            {
                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // "call" keyword should be here
                if (!DHJassSyntax.checkStringsEqual(ptr, ptrB, callStr.Length)) return false;

                // skip the "call`" bytes
                ptr += callStr.Length;

                // make sure the "call" is followed by whitespace
                if (*ptr != ' ') return false;

                // skip whitespaces
                DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                // add argument
                args.Add(new string(ptr));
                return true;
            }
        }

        public override DHJassOperation GetNew(List<string> lines, ref int line, List<string> args)
        {
            return new DHJassCallOperation(lines, ref line, args);
        }
    }
}
