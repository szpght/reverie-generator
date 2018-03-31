using Reverie.Generator.Enums;

namespace Reverie.Generator.Representation
{
    public class Constant : Variable
    {
        public ulong Value { get; }

        public Constant(VariableSize size, ulong value)
            : base(size, false)
        {
            Value = value;
        }

        public Constant(VariableSize size, long value)
            : base(size, true)
        {
            Value = (ulong)value;
        }

        public override string ToString() => $"constant {Value}";
    }
}