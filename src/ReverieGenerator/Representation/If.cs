using Reverie.Generator.Interfaces;

namespace Reverie.Generator.Representation
{
    public class If : ICode
    {
        public Relation Condition { get; }
        public CodeBlock Yes { get; }
        public CodeBlock No { get; }

        public If(Relation condition, CodeBlock yes)
        {
            Condition = condition;
            Yes = yes;
        }

        public If(Relation condition, CodeBlock yes, CodeBlock no)
            : this(condition, yes)
        {
            No = no;
        }
    }
}