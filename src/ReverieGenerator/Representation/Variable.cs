using Reverie.Generator.Enums;

namespace Reverie.Generator.Representation
{
    public class Variable
    {
        public VariableSize Size { get; }
        public bool Signed { get; }
        public Label Label { get; }

        public Variable(VariableSize size, bool signed)
        {
            Size = size;
            Signed = signed;
            Label = Label.Local();
        }

        public override string ToString() =>
            $"{Label.Name}: {(Signed ? "Signed" : "Unsigned")} {Size.ToString()}";
    }
}
