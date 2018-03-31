using Reverie.Generator.Enums;

namespace Reverie.Generator.Representation
{
    public class Relation
    {
        public RelationType Type { get; }
        public Variable First { get; }
        public Variable Second { get; }
        public Variable Result { get; }

        public static Relation Equal(Variable argA, Variable argB, Variable result)
            => new Relation(RelationType.Equal, argA, argB, result);

        public static Relation Greater(Variable argA, Variable argB, Variable result)
            => new Relation(RelationType.Greater, argA, argB, result);

        public static Relation GreaterOrEqual(Variable argA, Variable argB, Variable result)
            => new Relation(RelationType.GreaterOrEqual, argA, argB, result);

        public static Relation And(Variable argA, Variable argB, Variable result)
            => new Relation(RelationType.And, argA, argB, result);

        public static Relation Or(Variable argA, Variable argB, Variable result)
            => new Relation(RelationType.Or, argA, argB, result);

        public Relation(RelationType type, Variable first, Variable second, Variable result)
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