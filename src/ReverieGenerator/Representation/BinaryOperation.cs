using Reverie.Generator.Enums;
using Reverie.Generator.Interfaces;

namespace Reverie.Generator.Representation
{
    public class BinaryOperation : ICode
    {
        public BinaryOperationType Type { get; }
        public Variable First { get; }
        public Variable Second { get; }
        public Variable Result { get; }

        public static BinaryOperation Add(Variable argA, Variable argB, Variable result)
            => new BinaryOperation(BinaryOperationType.Add, argA, argB, result);

        public static BinaryOperation Subtract(Variable argA, Variable argB, Variable result)
            => new BinaryOperation(BinaryOperationType.Subtract, argA, argB, result);

        public static BinaryOperation ShiftLeft(Variable argA, Variable argB, Variable result)
            => new BinaryOperation(BinaryOperationType.ShiftLeft, argA, argB, result);

        public static BinaryOperation ShiftRight(Variable argA, Variable argB, Variable result)
            => new BinaryOperation(BinaryOperationType.ShiftRight, argA, argB, result);

        public static BinaryOperation And(Variable argA, Variable argB, Variable result)
            => new BinaryOperation(BinaryOperationType.And, argA, argB, result);

        public static BinaryOperation Or(Variable argA, Variable argB, Variable result)
            => new BinaryOperation(BinaryOperationType.Or, argA, argB, result);

        public static BinaryOperation Xor(Variable argA, Variable argB, Variable result)
            => new BinaryOperation(BinaryOperationType.Xor, argA, argB, result);

        public static BinaryOperation Multiply(Variable argA, Variable argB, Variable result)
            => new BinaryOperation(BinaryOperationType.Mul, argA, argB, result);

        public static BinaryOperation Divide(Variable argA, Variable argB, Variable result)
            => new BinaryOperation(BinaryOperationType.Div, argA, argB, result);

        public static BinaryOperation Modulo(Variable argA, Variable argB, Variable result)
            => new BinaryOperation(BinaryOperationType.Mod, argA, argB, result);

        public BinaryOperation(BinaryOperationType type, Variable first, Variable second, Variable result)
        {
            Type = type;
            First = first;
            Second = second;
            Result = result;
        }

        public override string ToString() =>
            $"{Type}: ({First}) => ({Second}) => ({Result})";
    }
}