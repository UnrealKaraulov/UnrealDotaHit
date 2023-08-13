using System;
using System.Collections.Generic;
using System.Text;

namespace DotaHIT.Jass
{
    using Types;
    using Operations;
    using Native.Types;
    using DotaHIT.Core;
    using DotaHIT.Core.Resources;
    using Native.Events;
    using Native.Constants;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;

    public class DHJassSyntax
    {
        public static char[] codeline_separator = new char[] { '\r', '\n' };
        public static char[] enumeration_separator = new char[] { ',' };

        public static string get_native_type_definition_syntax()
        {
            return @"\A\s*type\s+(?<typename>\w+)\s+extends\s+(?<basetype>\w+)";
        }

        // @"\A\s*not\s*(?<operand>[^\s].*)\z"
        public unsafe static bool checkNegativeOperatorUsageSyntaxFast(char* ptr, out string name)
        {
            string notStr = "not"; // should be interned                

            name = null;

            skipWhiteSpacesFast(ref ptr);

            fixed (char* ptrNOT = notStr)
            {
                if (!checkStringsEqual(ptr, ptrNOT, notStr.Length)) return false;

                ptr += notStr.Length;

                skipWhiteSpacesFast(ref ptr);

                if (*ptr == 0) return false;

                name = new string(ptr);

                return true;
            }
        }

        // @"\A\s*function\s+(?<name>\w+)\z"
        public unsafe static bool checkFunctionPointerUsageSyntaxFast(char* ptr, out string name)
        {
            string functionStr = "function"; // should be interned
            char* pStart;

            name = null;

            skipWhiteSpacesFast(ref ptr);

            fixed (char* ptrFUNCTION = functionStr)
            {
                if (!checkStringsEqual(ptr, ptrFUNCTION, functionStr.Length)) return false;

                ptr += functionStr.Length;

                skipWhiteSpacesFast(ref ptr);

                if (!checkVariableNameSyntaxFast(*ptr)) return false;

                pStart = ptr;

                reachEndOfVariableNameSyntaxFast(ref ptr);

                if (*ptr != 0) return false;

                name = new string(pStart);

                return true;
            }
        }

        // @"\A(?!\w+\++|[0-9]+)(?<name>\w+)\z"
        public static bool checkTypeVariableUsageSyntaxFast(string code, out string name)
        {
            unsafe
            {
                fixed (char* pStr = code)
                {
                    char* ptr = pStr;

                    if (checkVariableNameSyntaxFast(*ptr))
                    {
                        ptr = reachEndOfVariableNameSyntaxFast(ptr);
                        if (*ptr == 0)
                        {
                            name = code;
                            return true;
                        }
                    }
                }
            }

            name = null;
            return false;
        }
        public unsafe static bool checkTypeVariableUsageSyntaxFast(char* code, out string name)
        {
            char* ptr = code;

            if (checkVariableNameSyntaxFast(*ptr))
            {
                ptr = reachEndOfVariableNameSyntaxFast(ptr);
                if (*ptr == 0)
                {
                    name = new string(code);
                    return true;
                }
            }

            name = null;
            return false;
        }

        public static bool checkArrayVariableUsageSyntaxFast(string code, out string name, out string index)
        {
            unsafe
            {
                fixed (char* pStr = code)
                {
                    char* ptr = pStr;
                    char* pIndex;

                    if (checkVariableNameSyntaxFast(*ptr))
                    {
                        ptr = reachEndOfVariableNameSyntaxFast(ptr);
                        if (*ptr == '[')
                        {
                            pIndex = ptr++;
                            if (reachClosingSquareBracketFast(ref ptr))
                            {
                                name = new string(pStr, 0, (int)(pIndex - pStr));
                                index = new string(pIndex, 1, (int)(ptr - pIndex - 2));

                                return true;
                            }
                        }
                    }
                }
            }

            name = null;
            index = null;
            return false;
        }
        public unsafe static bool checkArrayVariableUsageSyntaxFast(char* code, out string name, out string index)
        {
            char* ptr = code;
            char* pIndex;

            if (checkVariableNameSyntaxFast(*ptr))
            {
                ptr = reachEndOfVariableNameSyntaxFast(ptr);
                if (*ptr == '[')
                {
                    pIndex = ptr++;
                    if (reachClosingSquareBracketFast(ref ptr))
                    {
                        name = new string(code, 0, (int)(pIndex - code));
                        index = new string(pIndex, 1, (int)(ptr - pIndex - 2));

                        return true;
                    }
                }
            }

            name = null;
            index = null;
            return false;
        }

        public static bool checkFunctionUsageSyntaxFast(string code, out string name, out List<string> args)
        {
            unsafe
            {
                fixed (char* pStr = code)
                {
                    char* ptr = pStr;

                    if (checkVariableNameSyntaxFast(*ptr))
                        ptr = reachEndOfVariableNameSyntaxFast(ptr);

                    if (*ptr != '(')
                    {
                        name = null;
                        args = null;
                        return false;
                    }
                    else
                    {
                        name = new string(pStr, 0, (int)(ptr - pStr));
                        args = new List<string>();
                    }

                    char* pArgStart = ++ptr;

                    while (*ptr != 0)
                    {
                        switch (*ptr)
                        {
                            case '(':
                                ++ptr;
                                if (!reachClosingRoundBracketFast(ref ptr)) return false;
                                break;

                            case ')':
                                if (ptr != pArgStart)
                                    args.Add(new string(pArgStart, 0, (int)(ptr - pArgStart)));
                                return true;

                            case '[':
                                ++ptr;
                                if (!reachClosingSquareBracketFast(ref ptr)) return false;
                                break;

                            case ',':
                                args.Add(new string(pArgStart, 0, (int)(ptr - pArgStart)));
                                pArgStart = ++ptr;
                                break;

                            case '"':
                                ++ptr;
                                if (!reachDoubleQuoteFast(ref ptr)) return false;
                                break;

                            default:
                                ptr++;
                                break;
                        }
                    }
                }
            }

            return false;
        }
        public unsafe static bool checkFunctionUsageSyntaxFast(char* code, out string name, out List<string> args)
        {
            char* ptr = code;

            if (checkVariableNameSyntaxFast(*ptr))
                ptr = reachEndOfVariableNameSyntaxFast(ptr);

            if (*ptr != '(')
            {
                name = null;
                args = null;
                return false;
            }
            else
            {
                name = new string(code, 0, (int)(ptr - code));
                args = new List<string>();
            }

            char* pArgStart = ++ptr;

            while (*ptr != 0)
            {
                switch (*ptr)
                {
                    case '(':
                        ++ptr;
                        if (!reachClosingRoundBracketFast(ref ptr)) return false;
                        break;

                    case ')':
                        if (ptr != pArgStart)
                            args.Add(new string(pArgStart, 0, (int)(ptr - pArgStart)));
                        return true;

                    case '[':
                        ++ptr;
                        if (!reachClosingSquareBracketFast(ref ptr)) return false;
                        break;

                    case ',':
                        args.Add(new string(pArgStart, 0, (int)(ptr - pArgStart)));
                        pArgStart = ++ptr;
                        break;

                    case '"':
                        ++ptr;
                        if (!reachDoubleQuoteFast(ref ptr)) return false;
                        break;

                    default:
                        ptr++;
                        break;
                }
            }

            return false;
        }

        #region unsafe fast

        unsafe public static bool checkLogicalExpressionsSyntaxFast(char* ptr, out List<string> operands, out List<string> operators)
        {
            operands = new List<string>();
            operators = new List<string>();

            char* pStart = ptr;
            char* pEnd = ptr;
            string result;

            while (*ptr != 0)
            {
                switch (*ptr)
                {
                    case ' ':
                        pEnd = ptr;
                        skipWhiteSpacesFast(ref ptr);
                        continue;

                    case '\'':
                        ++ptr;
                        if (reachSingleQuoteFast(ref ptr)) pEnd = ptr;
                        else return false;
                        continue;

                    case '"':
                        ++ptr;
                        if (reachDoubleQuoteFast(ref ptr)) pEnd = ptr;
                        else return false;
                        continue;

                    case '[':
                        ++ptr;
                        if (reachClosingSquareBracketFast(ref ptr)) pEnd = ptr;
                        else return false;
                        continue;

                    case '(':
                        ++ptr;
                        if (reachClosingRoundBracketFast(ref ptr)) pEnd = ptr;
                        else return false;
                        continue;

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        reachEndOfNumericSyntaxFast(ref ptr);
                        pEnd = ptr;
                        continue;

                    case '!':
                    case '=':
                    case '<':
                    case '>':
                        reachEndOfRelationalOpetatorSyntaxFast(ref ptr);
                        pEnd = ptr;
                        continue;

                    case '+':
                    case '-':
                    case '*':
                        reachEndOfArithmeticOpetatorSyntaxFast(ref ptr);
                        pEnd = ptr;
                        continue;

                    case '/':
                        // 2f00 2f00 == "//"
                        if (*(int*)ptr == 0x002F002F)
                        {
                            if (operators.Count != 0)
                            {
                                operands.Add(new string(pStart));
                                return true;
                            }

                            return false;
                        }
                        goto case '*';

                    default:
                        if (!char.IsLetter(*ptr))
                        {
                            ptr++;
                            continue;
                        }

                        result = checkLogicalOperatorSyntaxFast(ref ptr);
                        if (result != null)
                        {
                            operands.Add(new string(pStart, 0, (int)(pEnd - pStart)));
                            operators.Add(result);

                            skipWhiteSpacesFast(ref ptr);
                            pStart = ptr;
                        }
                        else reachEndOfVariableNameSyntaxFast(ref ptr);
                        continue;
                }
            }

            if (operators.Count != 0)
            {
                operands.Add(new string(pStart));
                return true;
            }

            return false;
        }
        unsafe public static bool checkRelationalExpressionsSyntaxFast(char* ptr, out List<string> operands, out List<string> operators)
        {
            operands = new List<string>();
            operators = new List<string>();

            char* pStart = ptr;
            char* pEnd = ptr;
            string result;

            while (*ptr != 0)
            {
                switch (*ptr)
                {
                    case ' ':
                        pEnd = ptr;
                        skipWhiteSpacesFast(ref ptr);
                        continue;

                    case '\'':
                        ++ptr;
                        if (reachSingleQuoteFast(ref ptr)) pEnd = ptr;
                        else return false;
                        continue;

                    case '"':
                        ++ptr;
                        if (reachDoubleQuoteFast(ref ptr)) pEnd = ptr;
                        else return false;
                        continue;

                    case '[':
                        ++ptr;
                        if (reachClosingSquareBracketFast(ref ptr)) pEnd = ptr;
                        else return false;
                        continue;

                    case '(':
                        ++ptr;
                        if (reachClosingRoundBracketFast(ref ptr)) pEnd = ptr;
                        else return false;
                        continue;

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        reachEndOfNumericSyntaxFast(ref ptr);
                        pEnd = ptr;
                        continue;

                    case '!':
                    case '=':
                    case '<':
                    case '>':
                        result = checkRelationalOperatorSyntaxFast(ref ptr);
                        if (result != null)
                        {
                            operands.Add(new string(pStart, 0, (int)(pEnd - pStart)));
                            operators.Add(result);

                            skipWhiteSpacesFast(ref ptr);
                            pStart = ptr;
                            continue;
                        }
                        else return false;

                    case '+':
                    case '-':
                    case '*':
                        reachEndOfArithmeticOpetatorSyntaxFast(ref ptr);
                        pEnd = ptr;
                        continue;

                    case '/':
                        // 2f00 2f00 == "//"
                        if (*(int*)ptr == 0x002F002F)
                        {
                            if (operators.Count != 0)
                            {
                                operands.Add(new string(pStart));
                                return true;
                            }

                            return false;
                        }
                        goto case '*';

                    default:
                        if (!char.IsLetter(*ptr)) ptr++;
                        else
                            reachEndOfVariableNameSyntaxFast(ref ptr);
                        pEnd = ptr;
                        continue;
                }
            }

            if (operators.Count != 0)
            {
                operands.Add(new string(pStart));
                return true;
            }

            return false;
        }
        unsafe public static bool checkArithmeticExpressionsSyntaxFast(char* ptr, out List<string> operands, out List<string> operators)
        {
            operands = new List<string>();
            operators = new List<string>();

            char* pStart = ptr;
            char* pEnd = ptr;

            while (*ptr != 0)
            {
                switch (*ptr)
                {
                    case ' ':
                        pEnd = ptr;
                        skipWhiteSpacesFast(ref ptr);
                        continue;

                    case '\'':
                        ++ptr;
                        if (reachSingleQuoteFast(ref ptr)) pEnd = ptr;
                        else return false;
                        continue;

                    case '"':
                        ++ptr;
                        if (reachDoubleQuoteFast(ref ptr)) pEnd = ptr;
                        else return false;
                        continue;

                    case '[':
                        ++ptr;
                        if (reachClosingSquareBracketFast(ref ptr)) pEnd = ptr;
                        else return false;
                        continue;

                    case '(':
                        ++ptr;
                        if (reachClosingRoundBracketFast(ref ptr)) pEnd = ptr;
                        else return false;
                        continue;

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        reachEndOfNumericSyntaxFast(ref ptr);
                        pEnd = ptr;
                        continue;

                    case '!':
                    case '=':
                    case '<':
                    case '>':
                        reachEndOfRelationalOpetatorSyntaxFast(ref ptr);
                        pEnd = ptr;
                        continue;

                    case '+':
                    case '-':
                    case '*':
                        operands.Add(new string(pStart, 0, (int)(pEnd - pStart)));
                        operators.Add(new string(ptr, 0, 1));
                        ptr++;

                        skipWhiteSpacesFast(ref ptr);
                        pStart = ptr;
                        continue;

                    case '/':
                        // 2f00 2f00 == "//"
                        if (*(int*)ptr == 0x002F002F)
                        {
                            if (operators.Count != 0)
                            {
                                operands.Add(new string(pStart));
                                return true;
                            }

                            return false;
                        }
                        goto case '*';

                    default:
                        if (!char.IsLetter(*ptr)) ptr++;
                        else
                            reachEndOfVariableNameSyntaxFast(ref ptr);
                        pEnd = ptr;
                        continue;
                }
            }

            if (operators.Count != 0)
            {
                operands.Add(new string(pStart));
                return true;
            }

            return false;
        }

        unsafe public static bool checkArithmeticOperatorSyntaxFast(char c)
        {
            switch (c)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// checks for logical operator syntax. 
        /// on success the pointer is moved beyond the operator characters.
        /// on failure no changes are made
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns>returns the found operator, otherwise null</returns>
        unsafe public static string checkLogicalOperatorSyntaxFast(ref char* ptr)
        {
            // 'and ' == 0x00200064006E0061
            // 'and(' == 0x00280064006E0061
            // 'and"' == 0x00220064006E0061
            // 'and'' == 0x00270064006E0061

            // 'or '  == 0x00200072006F0000
            // 'or('  == 0x00280072006F0000
            // 'or"'  == 0x00220072006F0000
            // 'or''  == 0x00270072006F0000

            char* pStart = ptr;

            switch (*(Int64*)ptr)
            {
                case 0x00200064006E0061:
                case 0x00280064006E0061:
                case 0x00220064006E0061:
                case 0x00270064006E0061:
                    ptr += 3;
                    return new string(pStart, 0, 3);
            }

            switch ((*(Int64*)ptr) << 16)
            {
                case 0x00200072006F0000:
                case 0x00280072006F0000:
                case 0x00220072006F0000:
                case 0x00270072006F0000:
                    ptr += 2;
                    return new string(pStart, 0, 2);
            }

            return null;
        }

        /// <summary>
        /// checks for relational operator syntax. 
        /// on success the pointer is moved beyond the operator characters.
        /// on failure no changes are made
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns>returns the found operator, otherwise null</returns>
        unsafe public static string checkRelationalOperatorSyntaxFast(ref char* ptr)
        {
            // '!=' == 0x003D0021
            // '==' == 0x003D003D
            // '>=' == 0x003D003E
            // '<=' == 0x003D003C

            // '>'  == 0x003E0000
            // '<'  == 0x003C0000

            char* pStart = ptr;

            switch (*(Int32*)ptr)
            {
                case 0x003D0021:
                case 0x003D003D:
                case 0x003D003E:
                case 0x003D003C:
                    ptr += 2;
                    return new string(pStart, 0, 2);
            }

            switch ((*(Int32*)ptr) << 16)
            {
                case 0x003E0000:
                case 0x003C0000:
                    ptr += 1;
                    return new string(pStart, 0, 1);
            }

            return null;
        }
        unsafe public static bool checkRelationalOperatorSyntaxFast(char c)
        {
            return (c == '>' || c == '<' || c == '!' || c == '=');
        }

        unsafe public static void skipWhiteSpacesFast(ref char* ptr)
        {
            while (*ptr == ' ') ptr++;
        }
        unsafe public static void skipWhiteSpacesReverseFast(ref char* ptr)
        {
            while (*ptr == ' ') ptr--;
        }
        unsafe public static void skipNonWhiteSpacesFast(ref char* ptr)
        {
            while (*ptr != ' ' && *ptr != 0) ptr++;
        }
        /// <summary>
        /// skips non-white space characters and stops at whitespace or comma
        /// this function assumes that the current position is a non-whitespace character
        /// </summary>
        /// <param name="ptr"></param> 
        unsafe public static void skipNonWhiteSpacesEnumFast(ref char* ptr)
        {
            while (true)
                switch (*ptr)
                {
                    case ' ':
                    case ',':
                    case '\0':
                        return;

                    default:
                        ptr++;
                        break;
                }
        }

        unsafe public static bool checkVariableNameSyntaxFast(char c)
        {
            return (char.IsLetter(c) || c == '_');
        }
        unsafe public static bool checkVariableNameContentSyntaxFast(char c)
        {
            return (char.IsLetter(c) || char.IsDigit(c) || c == '_');
        }
        unsafe public static bool checkNumberSyntaxFast(char c)
        {
            return (char.IsLetter(c) || char.IsDigit(c) || c == '_');
        }
        unsafe public static char* reachEndOfVariableNameSyntaxFast(char* ptr)
        {
            while (checkVariableNameContentSyntaxFast(*(++ptr))) ;
            return ptr;
        }
        unsafe public static void reachEndOfVariableNameSyntaxFast(ref char* ptr)
        {
            while (checkVariableNameContentSyntaxFast(*(++ptr))) ;
        }
        unsafe public static char* reachEndOfVarArrNameSyntaxFast(char* ptr)
        {
            char* pEnd = reachEndOfVariableNameSyntaxFast(ptr);

            if (*pEnd == '[')
            {
                ++pEnd;
                if (!reachClosingSquareBracketFast(ref pEnd)) return null;
            }

            return pEnd;
        }

        unsafe public static void reachEndOfNumericSyntaxFast(ref char* ptr)
        {
            while (checkNumericSyntaxFast(*(++ptr))) ;
        }

        unsafe public static void reachEndOfRelationalOpetatorSyntaxFast(ref char* ptr)
        {
            while (checkRelationalOperatorSyntaxFast(*(++ptr))) ;
        }

        unsafe public static void reachEndOfArithmeticOpetatorSyntaxFast(ref char* ptr)
        {
            while (checkArithmeticOperatorSyntaxFast(*(++ptr))) ;
        }

        unsafe public static string checkVarArrFuncSyntaxFast(ref char* ptr)
        {
            char* pEnd = reachEndOfVariableNameSyntaxFast(ptr);

            string result;

            switch (*pEnd)
            {
                case '[': // if it's array syntax
                    ++pEnd;
                    if (reachClosingSquareBracketFast(ref pEnd))
                        result = new string(ptr, 0, (int)(pEnd - ptr));
                    else
                        result = null;
                    break;

                case '(': // if it's function syntax
                    ++pEnd;
                    if (reachClosingRoundBracketFast(ref pEnd))
                        result = new string(ptr, 0, (int)(pEnd - ptr));
                    else
                        result = null;
                    break;

                default: // if it's variable
                    result = new string(ptr, 0, (int)(pEnd - ptr));
                    break;

            }

            ptr = pEnd;
            return result;
        }

        unsafe public static bool reachClosingSquareBracketFast(ref char* ptr)
        {
            int brackets = 1;
            while (*ptr != 0)
            {
                switch (*ptr)
                {
                    case '[':
                        brackets++;
                        break;
                    case ']':
                        brackets--;
                        if (brackets == 0)
                        {
                            ptr++; // move beyond closing bracket
                            return true;
                        }
                        break;
                }

                ptr++;
            }

            return false;
        }
        unsafe public static bool reachClosingRoundBracketFast(ref char* ptr)
        {
            int brackets = 1;
            while (*ptr != 0)
            {
                switch (*ptr)
                {
                    case '(':
                        brackets++;
                        break;
                    case ')':
                        brackets--;
                        if (brackets == 0)
                        {
                            ptr++; // move beyond closing bracket
                            return true;
                        }
                        break;
                }

                ptr++;
            }

            return false;
        }
        unsafe public static int getQuoteCountFast(string s)
        {            
            int quotes = 0;

            fixed (char* pStart = s)
            {
                char* ptr = pStart;

                while (*ptr != 0)
                    if (*(ptr++) == '"')
                        quotes++;
            }

            return quotes;
        }

        unsafe public static bool checkNumericSyntaxFast(char c)
        {
            return (char.IsDigit(c) || c == '.');
        }
        unsafe public static string checkNumericSyntaxFast(ref char* ptr)
        {
            char* pStart = ptr;
            while (checkNumericSyntaxFast(*(++ptr))) ;
            return new string(pStart, 0, (int)(ptr - pStart));
        }

        unsafe public static bool checkStringSyntaxFast(char c)
        {
            return c == '"';
        }
        unsafe public static string checkStringSyntaxFast(ref char* ptr)
        {
            char* pEnd = ptr + 1;
            string result;

            if (reachDoubleQuoteFast(ref pEnd))
                result = new string(ptr, 0, (int)(pEnd - ptr));
            else
                result = null;

            ptr = pEnd;
            return result;
        }

        unsafe public static bool checkExpressionSyntaxFast(char c)
        {
            return c == '(';
        }
        unsafe public static string checkExpressionSyntaxFast(ref char* ptr)
        {
            char* pEnd = ++ptr;
            string result;

            if (reachClosingRoundBracketFast(ref pEnd))
                result = new string(ptr, 0, (int)(pEnd - 1 - ptr));
            else
                result = null;

            ptr = pEnd;
            return result;
        }

        unsafe public static bool checkCharsSyntaxFast(char c)
        {
            return (c == '\'');
        }
        unsafe public static string checkCharsSyntaxFast(ref char* ptr)
        {
            char* pEnd = ptr + 1;
            string result;

            if (reachSingleQuoteFast(ref pEnd))
                result = new string(ptr, 0, (int)(pEnd - ptr));
            else
                result = null;

            ptr = pEnd;
            return result;
        }

        unsafe public static bool reachSingleQuoteFast(ref char* ptr)
        {
            while (*ptr != 0) if (*(ptr++) == '\'') return true;

            return false;
        }
        unsafe public static bool reachDoubleQuoteFast(ref char* ptr)
        {
            while (*ptr != 0) if (*(ptr++) == '"') return true;

            return false;
        }

        unsafe public static bool checkDirectValueSyntaxFast(char c)
        {
            return (char.IsDigit(c) || c == '.' || c == '\'' || c == '\"' || c == '-');
        }

        unsafe public static bool checkBlankEnd(char* ptr)
        {
            while (*ptr != 0 && *ptr == ' ') ptr++;
            return *ptr == 0;
        }

        /// <summary>
        /// removes white spaces on both sides of string.            
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        unsafe public static char* removeWsRecursive(char* ptr, char* pEnd)
        {
            while (*ptr != 0)
                switch (*ptr)
                {
                    case ' ':
                        ptr++;
                        break;

                    default:
                        while (--pEnd > ptr && *pEnd == ' ') ;
                        *(pEnd + 1) = '\0';
                        return ptr;
                }

            return ptr;
        }
        /// <summary>
        /// removes white spaces in the beginning and round brackets (smart remove) on both sides of string.            
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        unsafe public static char* removeWsRb(char* ptr)
        {
            char* pStart;

            while (*ptr != 0)
                if (*ptr == '(')
                {
                    pStart = ptr++;
                    if (reachClosingRoundBracketFast(ref ptr) && checkBlankEnd(ptr))
                    {
                        *(ptr - 1) = '\0'; // set ')' to zero, so string will end there
                        return pStart + 1; // make string start after '('
                    }
                    else
                        return pStart;
                }
                else
                    if (*ptr == ' ')
                        ptr++;
                    else
                        return ptr;

            return ptr;
        }
        /// <summary>
        /// removes white spaces and round brackets (smart remove) on both sides of string.            
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        unsafe public static char* removeWsRbRecursive(char* ptr, char* pEnd)
        {
            char* pStart;

            while (*ptr != 0)
                switch (*ptr)
                {
                    case '(':
                        pStart = ptr++;
                        if (reachClosingRoundBracketFast(ref ptr) && checkBlankEnd(ptr))
                        {
                            *(ptr - 1) = '\0'; // set ')' to zero, so string will end there
                            return removeWsRbRecursive(pStart + 1, ptr - 1); // make string start after '('
                        }
                        else
                            return pStart;

                    case ' ':
                        ptr++;
                        break;

                    default:
                        while (--pEnd > ptr && *pEnd == ' ') ;
                        *(pEnd + 1) = '\0';
                        return ptr;
                }

            return ptr;
        }

        /// <summary>
        /// checks if specidied number of characters are equal in both strings.
        /// comparision starts from the last characters to first
        /// </summary>
        /// <param name="ptrA"></param>
        /// <param name="ptrB"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        unsafe public static bool checkStringsEqual(char* ptrA, char* ptrB, int length)
        {
            while (length-- > 0)
                if (*(ptrA + length) != *(ptrB + length))
                    return false;
            return true;
        }

        #endregion

        public static System.Collections.Comparer Comparer = System.Collections.Comparer.DefaultInvariant;
        public static Regex noncode_syntax = new Regex(@"\A(\W*//.*|\s*\z)");
        public static Regex noncomment_syntax = new Regex(@"\A(\W*//.*)");

        public static Dictionary<string, string> native_types;
        public static Regex native_types_syntax = new Regex(get_native_type_definition_syntax());
    }
}
