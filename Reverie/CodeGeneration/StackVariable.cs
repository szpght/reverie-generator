namespace Reverie.CodeGeneration
{
    public class StackVariable : Variable
    {
        public override bool Sign { get; }
        public override VariableSize Size { get; }
        public Label Base { get; }
        public long Offset { get; }

        public StackVariable(string baseLabel, long offset, VariableSize size, bool sign = false)
        {
            Base = new Label(baseLabel);
            Offset = offset;
            Size = size;
            Sign = sign;
        }

        public override void Load(Register register, Assembly assembly)
        {
            string movInstruction = "mov";
            string registerName = register.Name;
            if (Size == VariableSize.Qword || Size == VariableSize.Dword && !Sign)
            {
                movInstruction = "mov";
                registerName = register.WithSize(Size);
            }
            else if (!Sign)
            {
                movInstruction = "movzx";
            }
            else if (Size == VariableSize.Byte && Sign || Size == VariableSize.Word && Sign)
            {
                movInstruction = "movsx";
            }
            else if (Size == VariableSize.Dword && Sign)
            {
                movInstruction = "movsxd";
            }
            assembly.Add($"{movInstruction} {registerName}, {ToString()}");
        }

        public override void Store(Register register, Assembly assembly)
        {
            assembly.Add($"mov {ToString()}, {register.WithSize(Size)}");
        }

        public override string ToString()
        {
            return $"{Size.Asm()} [{Base}{Offset: + #; - #;''}]";
        }
    }
}