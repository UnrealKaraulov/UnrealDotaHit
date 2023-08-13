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
using DotaHIT.Jass.Types;
using DotaHIT.Jass.Operations;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Core;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Native.Events;
using DotaHIT.Jass.Native.Constants;
using System.Collections.Specialized;

namespace DotaHIT.Jass.Commands
{
    public abstract class DHJassCommand
    {
        public abstract DHJassValue GetResult();
        public virtual string GetDebugString()
        {
            return null;
        }
    }

    public enum AnyOperation
    {
        Less = 0,
        LessOrEqual = 1,
        Equal = 2,
        GreaterOrEqual = 3,
        Greater = 4,
        NotEqual = 5,
        Add,
        Subtract,
        Multiply,
        Divide,
        AND,
        OR,
        Not
    }

    public class DHJassGetValueCommand : DHJassCommand
    {
        protected DHJassCommand command = null;

        protected DHJassGetValueCommand() { }
        public DHJassGetValueCommand(string code)
        {
            if (!TryParseCode(code, out command))
                command = new DHJassPassValueCommand(new DHJassUnusedType());
        }

        public override DHJassValue GetResult()
        {
            return command.GetResult();
        }

        public bool TryParseCode(string code, out DHJassCommand command)
        {
            if (String.IsNullOrEmpty(code))
            {
                command = null;
                return false;
            }

            string name;
            string param;
            List<string> operands;
            List<string> operators;
            bool isDirectValue;
            object parsedValue;

            unsafe
            {                
                fixed (char* pStr = code)
                {
                    char* ptr = DHJassSyntax.removeWsRbRecursive(pStr, pStr + code.Length);
                    code = new string(ptr);

                    if (isDirectValue = DHJassSyntax.checkDirectValueSyntaxFast(*ptr))
                    {
                        foreach (DHJassValue parser in DbJassTypeValueKnowledge.TypeValuePairs.Values)
                            if (parser.TryParseDirect(code, out parsedValue))
                            {
                                DHJassValue value = parser.GetNew();
                                value.Value = parsedValue;
                                command = new DHJassPassValueCommand(value);
                                return true;
                            }
                    }
                    else
                        switch (code)
                        {
                            case "null":
                                command = new DHJassPassValueCommand(new DHJassUnusedType());
                                return true;

                            case "true":
                                command = new DHJassPassValueCommand(new DHJassBoolean(null, true));
                                return true;

                            case "false":
                                command = new DHJassPassValueCommand(new DHJassBoolean(null, false));
                                return true;
                        }

                    if (DHJassSyntax.checkLogicalExpressionsSyntaxFast(ptr, out operands, out operators))//code, out match))
                        return TryGetLogicalExpressionCommandFast(operands, operators, out command);
                    else
                        if (DHJassSyntax.checkRelationalExpressionsSyntaxFast(ptr, out operands, out operators))// out match))
                            return TryGetRelationalExpressionCommandFast(operands, operators, out command);//match, out command);
                        else
                            if (DHJassSyntax.checkArithmeticExpressionsSyntaxFast(ptr, out operands, out operators))// out match))
                                return TryGetArithmeticExpressionCommandFast(operands, operators, out command); //match, out command);                                        
                            else
                                if (!isDirectValue)
                                {
                                    if (DHJassSyntax.checkNegativeOperatorUsageSyntaxFast(ptr, out name))
                                        return TryGetNegativeOperatorCommand(name, out command);
                                    else
                                        if (DHJassSyntax.checkTypeVariableUsageSyntaxFast(ptr, out name))
                                            return TryGetVariableCommand(name, out command);
                                        else
                                            if (DHJassSyntax.checkArrayVariableUsageSyntaxFast(ptr, out name, out param))
                                                return TryGetArrayElementCommand(name, param, out command);
                                            else
                                                if (DHJassSyntax.checkFunctionUsageSyntaxFast(ptr, out name, out operands))// out match))
                                                    return TryGetCallFunctionCommandFast(name, operands, out command);
                                                else
                                                    if (DHJassSyntax.checkFunctionPointerUsageSyntaxFast(ptr, out name))
                                                        return TryGetFunctionPointerCommand(name, out command);
                                }
                }
            }

            command = null;
            return false;
        }

        public static bool TryGetNegativeOperatorCommand(string operand, out DHJassCommand command)
        {
            command = new DHJassOperationCommand(new DHJassGetValueOnDemandCommand(operand), AnyOperation.Not);
            return true;
        }
        public static bool TryGetVariableCommand(string name, out DHJassCommand variable)
        {
            variable = new DHJassPassVariableCommand(name);
            return true;
        }
        public static bool TryGetArrayElementCommand(string name, string indexCode, out DHJassCommand element)
        {
            element = new DHJassGetArrayElementCommand(name, indexCode);
            return true;
        }
        public static bool TryGetFunctionPointerCommand(string name, out DHJassCommand functionPointer)
        {
            DHJassFunction function;
            if (DHJassExecutor.Functions.TryGetValue(name, out function))
            {
                functionPointer = new DHJassPassValueCommand(new DHJassCode(null, function));
                return true;
            }

            functionPointer = new DHJassPassValueCommand(null);
            return false;
        }

        public static bool TryGetLogicalExpressionCommandFast(List<string> sOperands, List<string> operators, out DHJassCommand value)
        {
            List<DHJassCommand> operands = new List<DHJassCommand>(sOperands.Count);

            //////////////////////////////////
            //  collect operands
            //////////////////////////////////
            foreach (string ccOperand in sOperands)
            {
                //DHJassBoolean operand = new DHJassBoolean();
                //operand.SetValue(ccOperand.Value);
                DHJassCommand operand = new DHJassGetValueCommand(ccOperand);
                operands.Add(operand);
            }

            //////////////////////////////////////
            //  process expression
            //////////////////////////////////////

            if (operands.Count == 0)
            {
                value = null;
                return false;
            }

            DHJassCommand operand2;
            DHJassCommand result;

            result = operands[0];

            for (int i = 0; i < operators.Count; i++)
            {
                string Operator = operators[i];

                if (Operator == "and")
                {
                    operand2 = operands[i + 1];
                    result = new DHJassOperationCommand(result, operand2, AnyOperation.AND);
                }
                else
                    if (Operator == "or")
                    {
                        operand2 = operands[i + 1];
                        result = new DHJassOperationCommand(result, operand2, AnyOperation.OR);
                    }
                    else
                        continue;
            }

            value = result;
            return true;
        }
        public static bool TryGetArithmeticExpressionCommandFast(List<string> sOperands, List<string> operators, out DHJassCommand value)
        {
            List<DHJassCommand> operands = new List<DHJassCommand>(sOperands.Count);

            //////////////////////////////////
            //  collect operands
            //////////////////////////////////
            foreach (string ccOperand in sOperands)
            {
                DHJassCommand operand = new DHJassGetValueCommand(ccOperand);
                operands.Add(operand);
            }

            //////////////////////////////////////
            //  process expression
            //////////////////////////////////////

            if (operands.Count == 0)
            {
                value = null;
                return false;
            }

            DHJassCommand operand1;
            DHJassCommand operand2;
            DHJassCommand result;

            // process '*' and '/' first

            for (int i = 0; i < operators.Count; i++)
            {
                string Operator = operators[i];

                if (Operator == "*")
                {
                    operand1 = operands[i];
                    operand2 = operands[i + 1];
                    result = new DHJassOperationCommand(operand1, operand2, AnyOperation.Multiply);
                }
                else
                    if (Operator == "/")
                    {
                        operand1 = operands[i];
                        operand2 = operands[i + 1];
                        result = new DHJassOperationCommand(operand1, operand2, AnyOperation.Divide);
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
                    result = new DHJassOperationCommand(result, operand2, AnyOperation.Add);
                }
                else
                    if (Operator == "-")
                    {
                        operand2 = operands[i + 1];
                        result = new DHJassOperationCommand(result, operand2, AnyOperation.Subtract);
                    }
                    else
                        continue;
            }

            value = result;
            return true;
        }
        public static bool TryGetRelationalExpressionCommandFast(List<string> sOperands, List<string> operators, out DHJassCommand value)
        {
            List<DHJassCommand> operands = new List<DHJassCommand>(sOperands.Count);

            //////////////////////////////////
            //  collect operands
            //////////////////////////////////
            foreach (string ccOperand in sOperands)
            {
                //DHJassValue operand = DHJassValue.CreateValueFromCode(ccOperand.Value);
                DHJassCommand operand = new DHJassGetValueCommand(ccOperand);
                operands.Add(operand);
            }

            //////////////////////////////////////
            //  process expression
            //////////////////////////////////////

            if (operands.Count == 0)
            {
                value = null;
                return false;
            }

            //DHJassBoolean operand1;
            DHJassCommand operand2;
            DHJassCommand result;

            result = operands[0];

            for (int i = 0; i < operators.Count; i++)
            {
                string Operator = operators[i];
                operand2 = operands[i + 1];

                switch (Operator)
                {
                    case ">":
                        result = new DHJassOperationCommand(result, operand2, AnyOperation.Greater);
                        break;

                    case ">=":
                        result = new DHJassOperationCommand(result, operand2, AnyOperation.GreaterOrEqual);
                        break;

                    case "<":
                        result = new DHJassOperationCommand(result, operand2, AnyOperation.Less);
                        break;

                    case "<=":
                        result = new DHJassOperationCommand(result, operand2, AnyOperation.LessOrEqual);
                        break;

                    case "==":
                        result = new DHJassOperationCommand(result, operand2, AnyOperation.Equal);
                        break;

                    case "!=":
                        result = new DHJassOperationCommand(result, operand2, AnyOperation.NotEqual);
                        break;
                }
            }

            value = result;
            return true;
        }

        public static bool TryGetCallFunctionCommandFast(string name, List<string> args, out DHJassCommand functionCall)
        {
            functionCall = new DHJassCallFunctionCommand(name, args);
            return true;
        }
    }
    public class DHJassGetValueOnDemandCommand : DHJassGetValueCommand
    {
        string code;
        public DHJassGetValueOnDemandCommand(string code)
        {
            this.code = code;
        }

        public override DHJassValue GetResult()
        {
            if (command == null)
            {              
                if (!TryParseCode(code, out command))
                    command = new DHJassPassValueCommand(new DHJassUnusedType());
            }            

            return command.GetResult();
        }

        public override string GetDebugString()
        {
            return code;
        }
    }

    public class DHJassSetValueOnDemandCommand : DHJassCommand
    {
        DHJassCommand command = null;
        DHJassCommand index = null;
        DHJassCommand value = null;

#if DEBUG
        string _name_usage = null;
        string _valueToSet = null;
#endif

        public DHJassSetValueOnDemandCommand(string name_usage, string valueToSet)
        {
#if DEBUG
            _name_usage = name_usage;
            _valueToSet = valueToSet;
#endif

            TryParseCode(name_usage, valueToSet, out command);
        }

        public override DHJassValue GetResult()
        {
            if (command != null)
            {
                if (index == null)
                {
                    DHJassValue variable = command.GetResult();
                    variable.SetValue(value.GetResult());                   

                    if (DHJassExecutor.IsTracing && variable.Name.StartsWith(DHJassExecutor.TraceName))
                    {
                        string strValue = "";

                        if (variable is DHJassInt && variable.IntValue != 0)
                            strValue = (variable as DHJassInt).IDValue + "/";

                        strValue += variable.Value;

                        Console.WriteLine("Called Set " + variable.Name + "=" + strValue);
                    }
                }
                else
                {
                    DHJassArray array = command.GetResult() as DHJassArray;
                    DHJassValue jIndex = index.GetResult();
                    DHJassValue jValue = value.GetResult();                   

                    if (DHJassExecutor.IsTracing && array.Name.StartsWith(DHJassExecutor.TraceName))
                    {
                        string strValue = "";

                        if (jValue is DHJassInt && jValue.IntValue != 0)
                            strValue = (jValue as DHJassInt).IDValue + "/";

                        strValue += jValue.Value;

                        Console.WriteLine("Called Set " + array.Name + "[" + jIndex.IntValue + "]=" + strValue);
                    }

                    array.SetElementValue(jIndex.IntValue, jValue);
                }
            }

            return null;
        }

        public bool TryParseCode(string name_usage, string valueToSet, out DHJassCommand command)
        {
            string name;
            string param;

            if (DHJassSyntax.checkTypeVariableUsageSyntaxFast(name_usage, out name))
            {
                command = new DHJassPassVariableCommand(name);
                value = new DHJassGetValueOnDemandCommand(valueToSet);
                return true;
            }
            else
                if (DHJassSyntax.checkArrayVariableUsageSyntaxFast(name_usage, out name, out param))
                {
                    command = new DHJassPassVariableCommand(name);
                    index = new DHJassGetValueOnDemandCommand(param);
                    value = new DHJassGetValueOnDemandCommand(valueToSet);

                    return true;
                }

            command = null;
            return false;
        }

        #if DEBUG
        public override string GetDebugString()
        {
            return "set " + _name_usage + "=" + _valueToSet; 
        }
        #endif
    }

    public class DHJassPassValueCommand : DHJassCommand
    {
        DHJassValue value;
        public DHJassPassValueCommand(DHJassValue value)
        {
            this.value = value;
        }

        public override DHJassValue GetResult()
        {
            return value;
        }
    }

    public class DHJassPassValueCopyCommand : DHJassCommand
    {
        string name;
        DHJassValue variable = null;
        DHJassCommand value = null;

        public DHJassPassValueCopyCommand(DHJassValue value, string copyName, DHJassCommand copyValue)
        {
            this.variable = value;
            this.name = copyName;
            this.value = copyValue;
        }

        public override DHJassValue GetResult()
        {
            DHJassValue copy = variable.GetNew();
            copy.Name = name;            
            copy.SetValue(value.GetResult());

            return copy;
        }

        public override string GetDebugString()
        {
            return name;
        }
    }

    public class DHJassPassVariableCommand : DHJassCommand
    {
        string name;
        public DHJassPassVariableCommand(string name)
        {
            this.name = name;
        }

        public override DHJassValue GetResult()
        {
            DHJassValue variable;
            DHJassExecutor.TryGetVariable(name, out variable);
            return variable;
        }

        public override string GetDebugString()
        {
            return name;
        }
    }

    public class DHJassGetArrayElementCommand : DHJassCommand
    {
        string name;
        DHJassCommand index;
        public DHJassGetArrayElementCommand(string name, string indexCode)
        {
            this.name = name;
            this.index = new DHJassGetValueCommand(indexCode);
        }

        public override DHJassValue GetResult()
        {
            DHJassValue array;
            if (DHJassExecutor.TryGetVariable(name, out array) && array is DHJassArray)
            {
                int index = this.index.GetResult().IntValue;

                if (DHJassExecutor.IsTracing && name.StartsWith(DHJassExecutor.TraceName))
                {
                    string strValue = "";
                    DHJassValue jValue = (array as DHJassArray)[index];

                    if (jValue is DHJassInt && jValue.IntValue != 0)
                        strValue = (jValue as DHJassInt).IDValue + "/";

                    strValue += jValue.Value;

                    Console.WriteLine("Called Get " + name + "[" + index + "]==" + strValue);
                }

                if (DHJassExecutor.CatchArrayReference && !DHJassExecutor.CaughtReferences.Contains(name))
                    DHJassExecutor.CaughtReferences.Add(name);

                return (array as DHJassArray)[index];
            }

            return new DHJassUnusedType();
        }
    }

    public class DHJassCallFunctionCommand : DHJassCommand
    {
        DHJassFunction function;
        DHJassCommand[] args;

        public DHJassCallFunctionCommand(string code)
        {
            //Match match; 
            string name;
            List<string> args;
            if (DHJassSyntax.checkFunctionUsageSyntaxFast(code, out name, out args)) //out match))
                parse(name, args);//match);
            else
                function = null;
        }

        public DHJassCallFunctionCommand(string name, List<string> argList)
        {
            function = null;
            parse(name, argList);
        }

        void parse(string name, List<string> argList)
        {
            DHJassExecutor.Functions.TryGetValue(name, out this.function);
            if (function == null && DHJassExecutor.ShowMissingFunctions)
                DHJassExecutor.ReportMissingFunction(name);

            //CaptureCollection cc = match.Groups["arg"].Captures;
            args = new DHJassCommand[argList.Count];

            for (int i = 0; i < argList.Count; i++)
                args[i] = new DHJassGetValueOnDemandCommand(argList[i]);
        }

        public override DHJassValue GetResult()
        {
            if (function == null)
                return new DHJassUnusedType();
            else
                if (DHJassExecutor.IsTracing && function.Name.StartsWith(DHJassExecutor.TraceName))
                {
                    Console.Write("Called:" + function.Name);
                    Console.Write(" Parameters:");
                    string pars = "";
                    foreach (DHJassCommand cmd in args)
                        pars += "'" + cmd.GetDebugString() + "',";
                    pars = pars.TrimEnd(',');
                    Console.Write(pars);
                    Console.Write("\n");
                }                

            return function.Execute(args);
        }
    }

    public class DHJassOperationCommand : DHJassCommand
    {
        DHJassCommand a;
        DHJassCommand b;
        AnyOperation operation;

        public DHJassOperationCommand(DHJassCommand a, DHJassCommand b, AnyOperation operation)
        {
            this.a = a;
            this.b = b;
            this.operation = operation;
        }
        public DHJassOperationCommand(DHJassCommand a, AnyOperation operation)
        {
            this.a = a;
            this.operation = operation;
        }

        public override DHJassValue GetResult()
        {
            DHJassValue operand1 = a.GetResult();

            switch (operation)
            {
                case AnyOperation.AND:
                    if (operand1.BoolValue == false)
                        return new DHJassBoolean(null, false);
                    break;

                case AnyOperation.OR:
                    if (operand1.BoolValue == true)
                        return new DHJassBoolean(null, true);
                    break;
            }

            DHJassValue operand2 = (b != null) ? b.GetResult() : null;

            switch (operation)
            {
                case AnyOperation.Add:
                    return operand1.Add(operand2);

                case AnyOperation.Subtract:
                    return operand1.Subtract(operand2);

                case AnyOperation.Multiply:
                    return operand1.Multiply(operand2);

                case AnyOperation.Divide:
                    return operand1.Divide(operand2);

                case AnyOperation.AND:
                    return operand1.And(operand2);

                case AnyOperation.OR:
                    return operand1.Or(operand2);

                case AnyOperation.Less:
                    return operand1.Less(operand2);

                case AnyOperation.LessOrEqual:
                    return operand1.LessOrEqual(operand2);

                case AnyOperation.Greater:
                    return operand1.Greater(operand2);

                case AnyOperation.GreaterOrEqual:
                    return operand1.GreaterOrEqual(operand2);

                case AnyOperation.Equal:
                    return operand1.Equals(operand2);

                case AnyOperation.NotEqual:
                    return operand1.NotEquals(operand2);

                case AnyOperation.Not:
                    return operand1.Not();

                default:
                    return new DHJassUnusedType();
            }
        }
    }

    public class DHJassDeclarationOnDemandCommand : DHJassCommand
    {
        string code;
        DHJassCommand command = null;

        public DHJassDeclarationOnDemandCommand(string code)
        {
            this.code = code;
        }

        public override DHJassValue GetResult()
        {
            if (command == null)
            {
                if (!TryParseCode(code, out command))
                    command = new DHJassPassValueCommand(new DHJassUnusedType());
            }
            return command.GetResult();
        }

        public bool TryParseCode(string code, out DHJassCommand command)
        {
            if (code != "nothing")
            {
                string name = null;
                DHJassGetValueOnDemandCommand value = null;

                DHJassValue variable;
                List<string> args;

                if (DHJassValue.TryParseDeclaration(code, out variable, out args))
                {
                    name = args[0];
                    value = new DHJassGetValueOnDemandCommand(args.Count > 1 ? args[1] : string.Empty);
                }
                else
                {
                    variable = new DHJassUnusedType();
                    name = "error!";
                    value = null;
                }

                command = new DHJassPassValueCopyCommand(variable, name, value);
                return true;
            }

            command = null;
            return false;
        }
    }

    public class DHJassGetGameStateCommand : DHJassCommand
    {
        string stateName;

        public DHJassGetGameStateCommand(string stateName)
        {
            this.stateName = stateName;
        }

        public override DHJassValue GetResult()
        {
            switch (stateName)
            {
                case "GAME_STATE_TIME_OF_DAY":
                    break;
            }

            return new DHJassReal(null, 0);
        }
    }
}
