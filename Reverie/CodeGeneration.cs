using System;
using System.Collections.Generic;
using System.Linq;

namespace Reverie.CodeGeneration
{
    public enum VariableSize
    {
        Byte = 1,
        Word = 2,
        Dword = 4,
        Qword = 8
    }

    public static class VariableSizeExtensions
    {
        public static string Asm(this VariableSize size)
        {
            switch (size)
            {
                case VariableSize.Byte:
                    return "BYTE";
                case VariableSize.Dword:
                    return "DWORD";
                case VariableSize.Qword:
                    return "QWORD";
                case VariableSize.Word:
                    return "WORD";
            }
            throw new Exception();
        }

        public static string RegisterSuffix(this VariableSize size)
        {
            switch (size)
            {
                case VariableSize.Byte:
                    return "b";
                case VariableSize.Word:
                    return "w";
                case VariableSize.Dword:
                    return "d";
                default:
                    return "";
            }
        }
    }

    public class Assembly : List<string>
    {
        public Assembly()
        {
        }

        public Assembly(string line)
        {
            Add(line);
        }

        public void Add(Assembly assembly)
        {
            AddRange(assembly);
        }

        public override string ToString()
        {
            return string.Join("\n", this);
        }
    }

    public class Register
    {
        public string Name { get; set; }
        public string NormalizedName => RegisterNames.ContainsKey(Name) ? RegisterNames[Name] : Name;
        public VariableSize Size { get; set; }

        public Register(string register, VariableSize size)
        {
            Name = register;
            Size = size;
        }

        public override string ToString()
        {
            return NormalizedName + Size.RegisterSuffix();
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

    public interface IOperand
    {
        Assembly Load(Register register);
        Assembly Store(Register register);
    }

    public class Label
    {
        public string Name { get; set; }

        public Label(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Variable : IOperand
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
            if (Size == VariableSize.Qword || Size == VariableSize.Dword && !Sign)
            {
                movInstruction = "mov";
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
            return new Assembly($"{movInstruction} {register}, {Size.Asm()} [{Base} + {Offset}]");
        }

        public Assembly Store(Register register)
        {
            return new Assembly($"mov {Size.Asm()} [{Base} + {Offset}], {register}");
        }
    }

    public class Context
    {
        private static HashSet<string> FreeRegisters =
            new HashSet<string>()
            {
                "r10",
                "r11",
                "rbx",
                "rax"
            };

        public Register Load(Variable variable, Assembly assembly)
        {
            var register = AllocateRegister(variable, assembly);
            var loadAsm = variable.Load(register);
            assembly.Add(loadAsm);
            return register;
        }

        public void SetAllocation(Register register, Variable variable, Assembly assembly)
        {
            register.Size = variable.Size;
            var storeAsm = variable.Store(register);
            assembly.Add(storeAsm);
        }

        private Register AllocateRegister(Variable variable, Assembly assembly)
        {
            var registerName = FreeRegisters.First();
            FreeRegisters.Remove(registerName);
            return new Register(registerName, variable.Size);
        }
    }

    public class Add
    {
        private Variable a_;
        private Variable b_;
        private Variable output_;

        public Add(Variable a, Variable b, Variable output)
        {
            a_ = a;
            b_ = b;
            output_ = output;
        }

        public Assembly Generate(Context ctx)
        {
            var asm = new Assembly();
            var regA = ctx.Load(a_, asm);
            var regB = ctx.Load(b_, asm);
            asm.Add($"add {regA.Name}, {regB.Name}");
            ctx.SetAllocation(regA, output_, asm);
            return asm;
        }
    }
}