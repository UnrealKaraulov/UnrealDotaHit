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

    public class DHJassExecutor
    {
        public static Random randomizer = new Random();

        public static bool ShowMissingFunctions = true;
        public static bool ShowMissingVariables = true;
        public static bool ShowReturnValues = true;
        public static bool ShowUnknownEvents = false;
        public static bool ShowTriggerActions = false;
        public static bool CatchArrayReference = false;
        public static bool LogExecution = false;
        public static bool IsTracing = false;
        public static string TraceName = "NC2";

        static int expressionDepth = 0;
        static bool traceFound = false;

        public static void traceIntoExpression()
        {
            if (IsTracing)
            {
                if (expressionDepth == 0) traceFound = false;
                expressionDepth++;
            }
        }
        public static bool traceOutofExpression()
        {
            if (IsTracing)
            {
                expressionDepth--;
                if (expressionDepth == 0)
                    return traceFound;
            }
            return false;
        }

        internal static Dictionary<string, DHJassValue> War3Globals = new Dictionary<string, DHJassValue>();
        internal static Dictionary<string, DHJassFunction> War3Functions = new Dictionary<string, DHJassFunction>();

        public static Dictionary<string, DHJassValue> Globals = new Dictionary<string, DHJassValue>();
        public static Stack<Dictionary<string, DHJassValue>> Stack = new Stack<Dictionary<string, DHJassValue>>();
        public static Dictionary<string, DHJassFunction> Functions = Native.DbJassNativeKnowledge.NameFunctionPairs;

        public static Stack<Dictionary<string, object>> TriggerStack = new Stack<Dictionary<string, object>>();
        public static Stack<player> ForceStack = new Stack<player>();
        public static Stack<unit> GroupStack = new Stack<unit>();
        public static Stack<item> ItemStack = new Stack<item>();

        public static List<string> CaughtReferences = new List<string>();

        public static bool TryGetValue<T>(string code, out DHJassValue value) where T : DHJassValue, new()
        {
            if (String.IsNullOrEmpty(code))
            {
                value = null;
                return false;
            }

            unsafe
            {
                fixed (char* pStr = code)
                {
                    char* ptr = DHJassSyntax.removeWsRbRecursive(pStr, pStr + code.Length);
                    code = new string(ptr);

                    if (code == "null")
                    {
                        value = new DHJassUnusedType();
                        return true;
                    }

                    DHJassValue parser;
                    if (DbJassTypeValueKnowledge.TypeValuePairs.TryGetValue(typeof(T), out parser))
                    {
                        object parsedValue;
                        if (parser.TryParseDirect(code, out parsedValue))
                        {
                            value = new T();
                            value.Value = parsedValue;
                            return true;
                        }
                    }

                    string name;
                    string param;
                    List<string> operands;
                    List<string> operators;

                    if (DHJassSyntax.checkLogicalExpressionsSyntaxFast(ptr, out operands, out operators))
                        return TryGetLogicalExpressionValueFast(operands, operators, out value);
                    else
                        if (DHJassSyntax.checkRelationalExpressionsSyntaxFast(ptr, out operands, out operators))//, out match))
                            return TryGetRelationalExpressionValueFast(operands, operators, out value);//match, out value);
                        else
                            if (DHJassSyntax.checkArithmeticExpressionsSyntaxFast(ptr, out operands, out operators))//, out match))
                                return TryGetArithmeticExpressionValueFast<T>(operands, operators, out value);//match, out value);
                            else
                                if (DHJassSyntax.checkTypeVariableUsageSyntaxFast(code, out name))
                                    return TryGetVariable(name, out value);
                                else
                                    if (DHJassSyntax.checkArrayVariableUsageSyntaxFast(ptr, out name, out param))
                                    {
                                        DHJassValue array;
                                        if (TryGetVariable(name, out array) && array is DHJassArray)
                                        {
                                            int index;
                                            if (DHJassInt.TryParse(param, out index))
                                            {
                                                value = (array as DHJassArray)[index];
                                                return true;
                                            }
                                        }
                                    }
                                    else
                                        if (DHJassSyntax.checkFunctionUsageSyntaxFast(ptr, out name, out operands))
                                            return TryCallFunctionFast(name, operands, out value);
                                        else
                                            if (DHJassSyntax.checkFunctionPointerUsageSyntaxFast(ptr, out name))
                                                return TryGetFunctionPointer(name, out value);
                }
            }

            value = null;
            return false;
        }
        public static bool TryGetValue(string code, out DHJassValue value)
        {
            if (String.IsNullOrEmpty(code))
            {
                value = null;
                return false;
            }

            string name;
            string param;
            List<string> operands;
            List<string> operators;
            object parsedValue;

            unsafe
            {
                fixed (char* pStr = code)
                {
                    char* ptr = DHJassSyntax.removeWsRbRecursive(pStr, pStr + code.Length);
                    code = new string(ptr);

                    if (DHJassSyntax.checkDirectValueSyntaxFast(*pStr))
                    {
                        foreach (DHJassValue parser in DbJassTypeValueKnowledge.TypeValuePairs.Values)
                            if (parser.TryParseDirect(code, out parsedValue))
                            {
                                value = parser.GetNew();
                                value.Value = parsedValue;
                                return true;
                            }
                    }
                    else
                        switch (code)
                        {
                            case "null":
                                value = new DHJassUnusedType();
                                return true;

                            case "true":
                                value = new DHJassBoolean(null, true);
                                return true;

                            case "false":
                                value = new DHJassBoolean(null, false);
                                return true;
                        }

                    if (DHJassSyntax.checkLogicalExpressionsSyntaxFast(ptr, out operands, out operators))
                        return TryGetLogicalExpressionValueFast(operands, operators, out value);
                    else
                        if (DHJassSyntax.checkRelationalExpressionsSyntaxFast(ptr, out operands, out operators))//, out match))
                            return TryGetRelationalExpressionValueFast(operands, operators, out value);//match, out value);
                        else
                            if (DHJassSyntax.checkArithmeticExpressionsSyntaxFast(ptr, out operands, out operators))//, out match))
                                return TryGetArithmeticExpressionValueFast(operands, operators, out value);//match, out value);
                            else
                                if (DHJassSyntax.checkTypeVariableUsageSyntaxFast(code, out name))
                                    return TryGetVariable(name, out value);
                                else
                                    if (DHJassSyntax.checkArrayVariableUsageSyntaxFast(ptr, out name, out param))
                                    {
                                        DHJassValue array;
                                        if (TryGetVariable(name, out array) && array is DHJassArray)
                                        {
                                            int index;
                                            if (DHJassInt.TryParse(param, out index))
                                            {
                                                value = (array as DHJassArray)[index];
                                                return true;
                                            }
                                        }
                                    }
                                    else
                                        if (DHJassSyntax.checkFunctionUsageSyntaxFast(ptr, out name, out operands))// out match))
                                            return TryCallFunctionFast(name, operands, out value);// match, out value);
                                        else
                                            if (DHJassSyntax.checkFunctionPointerUsageSyntaxFast(ptr, out name))
                                                return TryGetFunctionPointer(name, out value);
                }
            }

            value = null;
            return false;
        }

        public static bool TryGetLogicalExpressionValueFast(List<string> sOperands, List<string> sOperators, out DHJassValue value)
        {
            if (IsTracing)
            {
                if (expressionDepth == 0) traceFound = false;
                expressionDepth++;
            }

            List<DHJassBoolean> operands = new List<DHJassBoolean>(sOperands.Count);
            List<string> operators = new List<string>(sOperators.Count);

            //////////////////////////////////
            //  collect operands
            //////////////////////////////////
            foreach (string ccOperand in sOperands)
            {
                DHJassBoolean operand = new DHJassBoolean();
                operand.SetValue(ccOperand);
                operands.Add(operand);
            }

            ////////////////////////////////////
            //  collect operators
            ////////////////////////////////////
            foreach (string ccOperator in sOperators)
                operators.Add(ccOperator);

            //////////////////////////////////////
            //  process expression
            //////////////////////////////////////

            if (operands.Count == 0)
            {
                value = null;
                return false;
            }

            DHJassBoolean operand2;
            DHJassBoolean result;

            result = operands[0];

            for (int i = 0; i < operators.Count; i++)
            {
                string Operator = operators[i];

                if (Operator == "and")
                {
                    operand2 = operands[i + 1];
                    result = result.And(operand2);
                }
                else
                    if (Operator == "or")
                    {
                        operand2 = operands[i + 1];
                        result = result.Or(operand2);
                    }
                    else
                        continue;
            }

            if (IsTracing)
            {
                expressionDepth--;
                //if (expressionDepth == 0 && traceFound)
                //  Console.WriteLine("Found reference in:" + match.Value);
            }

            value = result;
            return true;
        }

        public static bool TryGetRelationalExpressionValueFast(List<string> sOperands, List<string> sOperators, out DHJassValue value)
        {
            if (IsTracing)
            {
                if (expressionDepth == 0) traceFound = false;
                expressionDepth++;
            }

            List<DHJassValue> operands = new List<DHJassValue>(sOperands.Count);
            List<string> operators = new List<string>(sOperators.Count);

            //////////////////////////////////
            //  collect operands
            //////////////////////////////////
            foreach (string ccOperand in sOperands)
            {
                DHJassValue operand = DHJassValue.CreateValueFromCode(ccOperand);
                operands.Add(operand);
            }

            ////////////////////////////////////
            //  collect operators
            ////////////////////////////////////
            foreach (string ccOperator in sOperators)
                operators.Add(ccOperator);

            //////////////////////////////////////
            //  process expression
            //////////////////////////////////////

            if (operands.Count == 0)
            {
                value = null;
                return false;
            }

            //DHJassBoolean operand1;
            DHJassValue operand2;
            DHJassValue result;

            result = operands[0];

            for (int i = 0; i < operators.Count; i++)
            {
                string Operator = operators[i];
                operand2 = operands[i + 1];

                switch (Operator)
                {
                    case ">":
                        result = result.Greater(operand2);
                        break;

                    case ">=":
                        result = result.GreaterOrEqual(operand2);
                        break;

                    case "<":
                        result = result.Less(operand2);
                        break;

                    case "<=":
                        result = result.LessOrEqual(operand2);
                        break;

                    case "==":
                        result = result.Equals(operand2);
                        break;

                    case "!=":
                        result = result.NotEquals(operand2);
                        break;
                }
            }

            if (IsTracing)
            {
                expressionDepth--;
                //if (expressionDepth == 0 && traceFound)
                //Console.WriteLine("Found reference in:" + match.Value);
            }

            value = result;
            return true;
        }

        public static bool TryGetArithmeticExpressionValueFast<T>(List<string> sOperands, List<string> sOperators, out DHJassValue value) where T : DHJassValue, new()
        {
            if (IsTracing)
            {
                if (expressionDepth == 0) traceFound = false;
                expressionDepth++;
            }

            List<T> operands = new List<T>();
            List<string> operators = new List<string>();

            //////////////////////////////////
            //  collect operands
            //////////////////////////////////
            foreach (string ccOperand in sOperands)
            {
                T operand = new T();
                operand.SetValue(ccOperand);
                operands.Add(operand);
            }

            ////////////////////////////////////
            //  collect operators
            ////////////////////////////////////
            foreach (string ccOperator in sOperators)
                operators.Add(ccOperator);

            //////////////////////////////////////
            //  process expression
            //////////////////////////////////////

            if (operands.Count == 0)
            {
                value = null;
                return false;
            }

            T operand1;
            T operand2;
            T result;

            // process '*' and '/' first

            for (int i = 0; i < operators.Count; i++)
            {
                string Operator = operators[i];

                if (Operator == "*")
                {
                    operand1 = operands[i];
                    operand2 = operands[i + 1];
                    result = operand1.Multiply(operand2) as T;
                }
                else
                    if (Operator == "/")
                    {
                        operand1 = operands[i];
                        operand2 = operands[i + 1];
                        result = operand1.Divide(operand2) as T;
                    }
                    else
                        continue;

                operands[i] = result;
                operands.RemoveAt(i + 1);
                operators.RemoveAt(i);
                i--;
            }

            // now '+' and '-'

            result = operands[0];

            for (int i = 0; i < operators.Count; i++)
            {
                string Operator = operators[i];

                if (Operator == "+")
                {
                    operand2 = operands[i + 1];
                    result = result.Add(operand2) as T;
                }
                else
                    if (Operator == "-")
                    {
                        operand2 = operands[i + 1];
                        result = result.Subtract(operand2) as T;
                    }
                    else
                        continue;
            }

            if (IsTracing)
            {
                expressionDepth--;
                //if (expressionDepth == 0 && traceFound)
                //Console.WriteLine("Found reference in:" + match.Value);
            }

            value = result;
            return true;
        }
        public static bool TryGetArithmeticExpressionValueFast(List<string> sOperands, List<string> sOperators, out DHJassValue value)
        {
            if (IsTracing)
            {
                if (expressionDepth == 0) traceFound = false;
                expressionDepth++;
            }

            List<DHJassValue> operands = new List<DHJassValue>();
            List<string> operators = new List<string>();

            //////////////////////////////////
            //  collect operands
            //////////////////////////////////
            foreach (string ccOperand in sOperands)
            {
                DHJassValue operand = DHJassValue.CreateValueFromCode(ccOperand);
                operands.Add(operand);
            }

            ////////////////////////////////////
            //  collect operators
            ////////////////////////////////////
            foreach (string ccOperator in sOperators)
                operators.Add(ccOperator);

            //////////////////////////////////////
            //  process expression
            //////////////////////////////////////

            if (operands.Count == 0)
            {
                value = null;
                return false;
            }

            DHJassValue operand1;
            DHJassValue operand2;
            DHJassValue result;

            // process '*' and '/' first

            for (int i = 0; i < operators.Count; i++)
            {
                string Operator = operators[i];

                if (Operator == "*")
                {
                    operand1 = operands[i];
                    operand2 = operands[i + 1];
                    result = operand1.Multiply(operand2);
                }
                else
                    if (Operator == "/")
                    {
                        operand1 = operands[i];
                        operand2 = operands[i + 1];
                        result = operand1.Divide(operand2);
                    }
                    else
                        continue;

                operands[i] = result;
                operands.RemoveAt(i + 1);
                operators.RemoveAt(i);
                i--;
            }

            // now '+' and '-'

            result = operands[0];

            for (int i = 0; i < operators.Count; i++)
            {
                string Operator = operators[i];

                if (Operator == "+")
                {
                    operand2 = operands[i + 1];
                    result = result.Add(operand2);
                }
                else
                    if (Operator == "-")
                    {
                        operand2 = operands[i + 1];
                        result = result.Subtract(operand2);
                    }
                    else
                        continue;
            }

            if (IsTracing)
            {
                expressionDepth--;
                //if (expressionDepth == 0 && traceFound)
                //Console.WriteLine("Found reference in:" + match.Value);
            }

            value = result;
            return true;
        }

        public static bool TryCallFunction(string name, out DHJassValue returned)
        {
            DHJassFunction function;
            if (Functions.TryGetValue(name, out function))
            {
                if (IsTracing && name.StartsWith(TraceName))
                    Console.WriteLine("Called:" + name);

                returned = function.Execute();
                return true;
            }
            else if (ShowMissingFunctions)
                ReportMissingFunction(name);

            returned = new DHJassUnusedType();
            return false;
        }
        public static bool TryCallFunctionFast(string name, List<string> args, out DHJassValue returned)
        {
            DHJassFunction function;
            if (Functions.TryGetValue(name, out function))
            {
                returned = function.Execute(args);

                if (IsTracing && name.StartsWith(TraceName))
                    Console.WriteLine("Called:" + name + "  returned:" + returned.StringValue);

                return true;
            }
            else if (ShowMissingFunctions)
                ReportMissingFunction(name);

            returned = new DHJassUnusedType();
            return false;
        }

        public static bool TryGetVariable(string name, out DHJassValue variable)
        {
            bool OK = false;
            if (Stack.Count != 0 && Stack.Peek().TryGetValue(name, out variable))
                OK = true;
            else
                if (Globals.TryGetValue(name, out variable))
                    OK = true;
                else
                {
                    variable = new DHJassUnusedType();
                    if (ShowMissingVariables) Console.WriteLine("Variable:'" + name + "' not found");
                    OK = false;
                }

            if (OK && IsTracing && TraceName == name && (expressionDepth != 0))
                traceFound = true;

            return OK;
        }

        public static bool TryGetFunctionPointer(string name, out DHJassValue variable)
        {
            DHJassFunction function;
            if (Functions.TryGetValue(name, out function))
            {
                variable = new DHJassCode(null, function);
                return true;
            }

            variable = null;
            return false;
        }

        static Dictionary<string, string> MissingFunctions = new Dictionary<string, string>();
        public static void ReportMissingFunction(string name)
        {
            if (!MissingFunctions.ContainsKey(name))
            {
                Console.WriteLine("Function:'" + name + "' not found");
                MissingFunctions.Add(name, name);
            }
        }
    }
}
