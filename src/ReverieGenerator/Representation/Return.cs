using Reverie.Generator.Interfaces;

namespace Reverie.Generator.Representation
{
    public class Return : ICode
    {
        public Variable Value { get; }

        public Return(Variable value)
        {
            Value = value;
        }

        public override string ToString() =>
            $"return {Value}";
    }
}