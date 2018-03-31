using System;

namespace Reverie.CodeGeneration
{
    public class ConstantInteger : Variable
    {
        public override bool Sign { get; }
        public override VariableSize Size => VariableSize.Qword;
        public string Content { get; }

        public ConstantInteger(long value)
        {
            Sign = true;
            Content = value.ToString();
        }

        public ConstantInteger(ulong value)
        {
            Sign = false;
            Content = value.ToString();
        }

        public override void Load(Register register, Assembly assembly)
        {
            assembly.Add($"mov {register}, {Content}");
        }

        public override void Store(Register register, Assembly assembly)
        {
            throw new Exception("Cannot store to constant value");
        }

        public override string ToString()
        {
            return Content;
        }
    }
}
