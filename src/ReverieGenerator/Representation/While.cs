using Reverie.Generator.Interfaces;

namespace Reverie.Generator.Representation
{
    public class While : ICode
    {
        public Relation Condition { get; }
        public CodeBlock Body { get; }

        public While(Relation condition, CodeBlock body)
        {
            Condition = condition;
            Body = body;
        }
    }
}