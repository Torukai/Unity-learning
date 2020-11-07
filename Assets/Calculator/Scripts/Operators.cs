using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace Operators
{

    public interface IOperator
    {
        uint Weight { get; }

    }
    public interface IExecutableOperator : IOperator
    {
        double CalculateOperator(double a, double b);

    }
    public interface IFunction : IOperator
    {
        bool isNegative { get; set; }
    }
    public interface IOneArgFunction : IFunction
    {
        double CalculateFunction(double a);
    }

    public interface IConstant : IFunction
    {
        double Value { get; }
    }

    public class Default : IOperator
    {
        public uint Weight => 0;
    }

    public class Pi : IConstant
    {
        public bool isNegative { get; set; } = false;
        public uint Weight => 4;
        public double Value => 3.14159265358979323;
    }
    public class EulersNumber : IConstant
    {
        public bool isNegative { get; set; } = false;
        public uint Weight => 4;
        public double Value => (double)Math.E;
    }

    public class Addition : IExecutableOperator
    {
        public uint Weight => 2;

        public double CalculateOperator(double a, double b)
        {
            return a + b;
        }
    }
    public class Subtraction : IExecutableOperator
    {
        public uint Weight => 2;

        public double CalculateOperator(double a, double b)
        {
            return a - b;
        }
    }
    public class Multiplication : IExecutableOperator
    {
        public uint Weight => 3;

        public double CalculateOperator(double a, double b)
        {
            return a * b;
        }
    }
    public class Division : IExecutableOperator
    {
        public uint Weight => 3;

        public double CalculateOperator(double a, double b)
        {
            return a / b;
        }
    }
    public class Modulo : IExecutableOperator
    {
        public uint Weight => 3;

        public double CalculateOperator(double a, double b)
        {
            return a % b;
        }
    }
    public class Power : IExecutableOperator
    {
        public uint Weight => 4;

        public double CalculateOperator(double a, double b)
        {
            return (double)Math.Pow((double)a, (double)b);
        }
    }

    public class Sinus : IOneArgFunction
    {
        public uint Weight => 5;

        public bool isNegative { get; set; } = false;

        public double CalculateFunction(double a)
        {
            return (double)Math.Sin((double)a);
        }
    }
    public class Cosinus : IOneArgFunction
    {
        public uint Weight => 5;
        public bool isNegative { get; set; } = false;
        public double CalculateFunction(double a)
        {
            return (double)Math.Cos((double)a);
        }
    }

    public class Tangent : IOneArgFunction
    {
        public uint Weight => 5;
        public bool isNegative { get; set; } = false;
        public double CalculateFunction(double a)
        {
            return (double)Math.Tan((double)a);
        }
    }

    public class Cotangent : IOneArgFunction
    {
        public uint Weight => 5;
        public bool isNegative { get; set; } = false;
        public double CalculateFunction(double a)
        {
            return 1 / (double)Math.Tan((double)a);
        }
    }
    public class DegToRad : IOneArgFunction
    {
        public uint Weight => 5;
        public bool isNegative { get; set; } = false;
        public double CalculateFunction(double a)
        {
            return a * (3.14159265358979323 / 180);
        }
    }
    public class RadToDeg : IOneArgFunction
    {
        public uint Weight => 5;
        public bool isNegative { get; set; } = false;
        public double CalculateFunction(double a)
        {
            return a * (180 / 3.14159265358979323);
        }
    }

    public class LeftBracket : IOperator
    {
        public uint Weight => 1;
    }
    public class RightBracket : IOperator
    {
        public uint Weight => 6;
    }
    public static class OperatorFactory
    {

        public static void Test()
        {
            var opTypes = Assembly.GetExecutingAssembly().DefinedTypes.Where(x => typeof(IOperator).IsAssignableFrom(x));
            var dic = opTypes.Select(x => (IOperator)Activator.CreateInstance(x)).ToDictionary(x => x.Weight, x => x);
        }

        public static T Create<T>() where T : IOperator, new()
        {
            return new T();
        }
    }

    public static class OperatorParser
    {
        public static IOperator ToOperator(this string s, int pos)
        {
            switch (s[pos])
            {
                case '+':
                    return OperatorFactory.Create<Addition>();
                case '-':
                    return OperatorFactory.Create<Subtraction>();
                case '*':
                case 'x':
                    return OperatorFactory.Create<Multiplication>();
                case '/':
                case ':':
                case '÷':
                    return OperatorFactory.Create<Division>();
                case '%':
                    return OperatorFactory.Create<Modulo>();
                case '^':
                    return OperatorFactory.Create<Power>();
                case '[':
                case '{':
                case '(':
                    return OperatorFactory.Create<LeftBracket>();
                case ']':
                case '}':
                case ')':
                    return OperatorFactory.Create<RightBracket>();
                default:
                    return OperatorFactory.Create<Default>();
            }
        }
        public static bool ComparePriority(this IOperator operatorToCompare, IOperator comparingOperator)
        {
            if (operatorToCompare.GetType() != typeof(Power))
            {
                if (operatorToCompare.Weight <= comparingOperator.Weight)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (operatorToCompare.Weight < comparingOperator.Weight)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static IOperator ToFunction(this string s)
        {
            switch (s)
            {
                case "sin":
                case "sinus":
                    return OperatorFactory.Create<Sinus>();
                case "cos":
                case "cosinus":
                    return OperatorFactory.Create<Cosinus>();
                case "tangent":
                case "tan":
                case "tg":
                    return OperatorFactory.Create<Tangent>();
                case "cotangent":
                case "ctg":
                case "cot":
                    return OperatorFactory.Create<Cotangent>();
                case "PI":
                case "pi":
                    return OperatorFactory.Create<Pi>();
                case "E":
                case "e":
                    return OperatorFactory.Create<EulersNumber>();
                case "rad":
                    return OperatorFactory.Create<DegToRad>();
                case "deg":
                    return OperatorFactory.Create<RadToDeg>();
                default:
                    return OperatorFactory.Create<Default>();
            }
        }
    }
}