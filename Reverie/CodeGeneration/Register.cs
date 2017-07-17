using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public class Register
    {
        public string FullName { get; }
        public string Name => ToString();
        public string NormalizedName => RegisterNames.ContainsKey(FullName) ? RegisterNames[FullName] : FullName;
        public VariableSize Size { get; }

        public Register(string register, VariableSize size)
        {
            FullName = register;
            Size = size;
        }

        public Register WithSize(VariableSize size)
        {
            return new Register(FullName, size);
        }

        public override string ToString()
        {
            return NormalizedName + Size.RegisterSuffix();
        }

        public bool TheSameAs(Register register)
        {
            return NormalizedName == register.NormalizedName;
        }

        private static readonly Dictionary<string, string> RegisterNames = new Dictionary<string, string>()
        {
            {"rax", "r0"},
            {"rcx", "r1"},
            {"rdx", "r2"},
            {"rbx", "r3"},
            {"rsp", "r4"},
            {"rbp", "r5"},
            {"rsi", "r6"},
            {"rdi", "r7"},
        };
    }
}