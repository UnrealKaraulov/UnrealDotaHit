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

namespace DotaHIT
{
    namespace Jass
    {
        using Types;
        using Operations;
        using Native.Types;
        using DotaHIT.Core;
        using DotaHIT.Core.Resources;
        using Native.Events;
        using Native.Constants;
        using System.Collections.Specialized;

        public class DHJass
        {            
            public static void Read(string filename)
            {
                List<string> lines = GetLines(filename);
                ReadCustom(lines);
            }
            public static void ReadEx(string filename)
            {
                List<string> lines = GetLinesEx(filename);
                ReadCustom(lines);
            }

            public static void ReadCustom(MemoryStream ms)
            {
                ReadCustom(ms, true);
            }
            public static void ReadCustom(MemoryStream ms, bool emptyLinesAreIrrelevant)
            {
                PrepareForCustomRead();

                List<string> lines = GetLines(ms, emptyLinesAreIrrelevant);
                ReadCustom(lines);
            }
            public static void ReadCustomEx(MemoryStream ms)
            {
                PrepareForCustomRead();

                List<string> lines = GetLinesEx(ms);
                ReadCustom(lines);
            }
            public static void ReadWar3(MemoryStream ms, bool clearGlobals)
            {
                List<string> lines = GetLines(ms, true);
                ReadWar3(lines, clearGlobals);
            }
            public static void ReadWar3Ex(MemoryStream ms, bool clearGlobals)
            {
                List<string> lines = GetLinesEx(ms);
                ReadWar3(lines, clearGlobals);
            }

            static void ReadCustom(List<string> lines)
            {
                GetGlobals(lines, true);
                GetFunctions(lines, false);
            }
            static void ReadWar3(List<string> lines, bool clearGlobals)
            {
                GetWar3Globals(lines, true, clearGlobals);
                GetWar3Functions(lines, false);
            }

            public static void CompleteWar3Init()
            {                
                playerunitevent.WakeUp();
                playerevent.WakeUp();
                playerstate.WakeUp();
                unitevent.WakeUp();
                unitstate.WakeUp();
                gamestate.WakeUp();
                itemtype.WakeUp();
                unittype.WakeUp();

                // this is for backpack feature. 6 inventory slots + 4 backpack slots
                DHJassExecutor.War3Globals["bj_MAX_INVENTORY"].Value = 10;

                player.Reset();
            }
            static void PrepareForCustomRead()
            {
                DHJassExecutor.Globals = new Dictionary<string, DHJassValue>(DHJassExecutor.War3Globals);
                DHJassExecutor.Functions = new Dictionary<string, DHJassFunction>(DHJassExecutor.War3Functions);

                foreach (KeyValuePair<string, DHJassFunction> kvp in Native.DbJassNativeKnowledge.NameFunctionPairs)
                    DHJassExecutor.Functions.Add(kvp.Key, kvp.Value); 
            }

            /// <summary>
            /// clears everything from the jass script engine
            /// </summary>
            public static void Clear()
            {
                DHJassExecutor.War3Globals.Clear();
                DHJassExecutor.War3Functions.Clear();

                Reset();                
            }
            /// <summary>
            /// clears all handles, variables, constants and cache
            /// that were created by the custom jass script
            /// </summary>
            public static void Reset()
            {
                DHJassHandleEngine.Reset();
                player.Reset();
                unit.Reset();
                gamecache.Reset();                
            }

            public static void Config()
            {
                DHJassValue returned;
                DHJassExecutor.TryCallFunction("config", out returned);
            }

            public static void Go()
            {
                DHJassValue returned;
                DHJassExecutor.TryCallFunction("main", out returned);
            }

            protected static List<string> GetLines(MemoryStream ms, bool emptyLinesAreIrrelevant)
            {
                StreamReader sr = new StreamReader(ms);
                ms.Position = 0;

                List<string> lineList = new List<string>();
                string line;

                Regex irrelevantLineSyntax = emptyLinesAreIrrelevant ? DHJassSyntax.noncode_syntax : DHJassSyntax.noncomment_syntax;

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (irrelevantLineSyntax.IsMatch(line) == false)
                        lineList.Add(line);
                }

                sr.Close();

                return lineList;
            }
            protected static List<string> GetLinesEx(MemoryStream ms)
            {
                StreamReader sr = new StreamReader(ms);
                ms.Position = 0;

                List<string> lineList = new List<string>();
                DHJassSyntax.native_types = new Dictionary<string, string>();

                string line;
                Match match;

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (DHJassSyntax.noncode_syntax.IsMatch(line) == false)
                        if ((match = DHJassSyntax.native_types_syntax.Match(line)).Success)
                        {
                            string typename = match.Groups["typename"].Value;
                            string basetype = match.Groups["basetype"].Value;

                            DHJassSyntax.native_types.Add(typename, basetype);
                        }
                        else
                            lineList.Add(line);
                }

                sr.Close();              

                return lineList;
            }
            protected static List<string> GetLines(string filename)
            {
                FileStream fs = new FileStream(filename, FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                List<string> lineList = new List<string>();                
                string line;                

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (DHJassSyntax.noncode_syntax.IsMatch(line) == false)                       
                        lineList.Add(line);
                }

                sr.Close();

                return lineList;
            }
            protected static List<string> GetLinesEx(string filename)
            {
                FileStream fs = new FileStream(filename, FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                List<string> lineList = new List<string>();
                DHJassSyntax.native_types = new Dictionary<string, string>();

                string line;
                Match match;

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (DHJassSyntax.noncode_syntax.IsMatch(line) == false)
                        if ((match = DHJassSyntax.native_types_syntax.Match(line)).Success)
                        {
                            string typename = match.Groups["typename"].Value;
                            string basetype = match.Groups["basetype"].Value;

                            DHJassSyntax.native_types.Add(typename, basetype);
                        }
                        else
                            lineList.Add(line);
                }

                sr.Close();                

                return lineList;
            }

            protected static void GetGlobals(List<string> lines, bool removeRead)
            {
                int i=0;
                int length = lines.Count;                

                for (i = 0; i < length; i++)
                    if (lines[i] == "globals")
                        break;

                if (i < length) // if globals were found                
                {
                    int j = i+1;
                    for (; j < length; j++)
                    {
                        string curren_line = lines[j];

                        if (curren_line == "endglobals")
                            break;
                        else
                        {

                            DHJassValue value = DbJassTypeValueKnowledge.CreateKnownValue(curren_line);
                            if (value != null && !String.IsNullOrEmpty(value.Name))
                                DHJassExecutor.Globals.Add(value.Name, value);
                        }
                    }

                    if (removeRead)
                        lines.RemoveRange(i, (j-i)+1);                    
                }                
            }
            protected static void GetFunctions(List<string> lines, bool removeRead)
            {
                int i = 0;
                int length;
                List<string> args;

                do
                {
                    length = lines.Count;
                    args = new List<string>(3);

                    for (; i < length; i++)
                        if (DHJassFunction.IsSyntaxMatchFast(lines[i], ref args))
                            break;

                    if (args.Count > 1) // if function declaration was found
                    {
                        List<string> function_lines = new List<string>();

                        int j = i + 1;
                        for (; j < length; j++)
                        {
                            string curren_line = lines[j];

                            if (curren_line == "endfunction")
                                break;
                            else
                                function_lines.Add(curren_line);
                        }

                        if (removeRead)
                        {
                            lines.RemoveRange(i, (j - i) + 1);
                            i = 0;
                        }
                        else
                            i = ++j;
                      
                        DHJassFunction function = new DHJassFunction(args, function_lines);
                        DHJassExecutor.Functions.Add(function.Name, function);
                    }
                }
                while (args.Count > 1);
            }

            protected static void GetWar3Globals(List<string> lines, bool removeRead, bool clearGlobals)
            {
                if (clearGlobals)
                    DHJassExecutor.Globals = new Dictionary<string, DHJassValue>();

                GetGlobals(lines, true);
                DHJassExecutor.War3Globals = DHJassExecutor.Globals;
            }
            protected static void GetWar3Functions(List<string> lines, bool removeRead)
            {
                int i = 0;
                int length;
                List<string> args;

                do
                {
                    length = lines.Count;                    
                    args = new List<string>(3);

                    for (; i < length; i++)
                        if (DHJassFunction.IsSyntaxMatchFast(lines[i], ref args))
                            break;                        

                    if (args.Count > 1) // if function declaration was found
                    {
                        List<string> function_lines = new List<string>();

                        int j = i + 1;
                        for (; j < length; j++)
                        {
                            string curren_line = lines[j];

                            if (curren_line == "endfunction")
                                break;
                            else
                                function_lines.Add(curren_line);
                        }

                        if (removeRead)
                        {
                            lines.RemoveRange(i, (j - i) + 1);
                            i = 0;
                        }
                        else                        
                            i = ++j;


                        DHJassFunction function = new DHJassFunction(args, function_lines);
                        DHJassExecutor.War3Functions.Add(function.Name, function);
                    }
                }
                while (args.Count > 1);
            }
        }
    }
}
