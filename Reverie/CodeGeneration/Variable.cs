namespace Reverie.CodeGeneration
{
    public class Variable
    {
        public Label Base { get; set; }
        public long Offset { get; set; }
        public bool Sign { get; set; }
        public VariableSize Size { get; set; }

        public Variable(string baseLabel, long offset, VariableSize size, bool sign = false)
        {
            Base = new Label(baseLabel);
            Offset = offset;
            Size = size;
            Sign = sign;
        }

        public Assembly Load(Register register)
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
            return new Assembly($"{movInstruction} {registerName}, {ToString()}");
        }

        public Assembly Store(Register register)
        {
            return new Assembly($"mov {ToString()}, {register.WithSize(Size)}");
        }

        public override string ToString()
        {
            return $"{Size.Asm()} [{Base}{Offset: + #; - #;''}]";
        }
    }
}