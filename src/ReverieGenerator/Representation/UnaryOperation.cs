using Reverie.Generator.Enums;
using Reverie.Generator.Interfaces;

namespace Reverie.Generator.Representation
{
    public class UnaryOperation : ICode
    {
        public UnaryOperationType Type { get; }
        public Variable First { get; }
        public Variable Result { get; }

        public static UnaryOperation Negate(Variable argument, Variable result) =>
            new UnaryOperation(UnaryOperationType.Neg, argument, result);

        public static UnaryOperation Not(Variable argument, Variable result) =>
            new UnaryOperation(UnaryOperationType.Not, argument, result);

        public UnaryOperation(UnaryOperationType type, Variable first, Variable result)
        {
            Type = type;
            First = first;
            Result = result;
        }
    }
}