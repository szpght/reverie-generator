using Reverie.Generator.Enums;

namespace Reverie.Generator.Representation
{
    public class CString : Variable
    {
        public string Value { get; }
        public CString(string value)
            : base (VariableSize.Pointer, false)
        {
            Value = value;
        }

        public override string ToString() => Value;
    }
}