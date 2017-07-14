using System;
using System.Collections.Generic;

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

        public void AddLine(string line)
        {
            Add(line);
        }

        public override string ToString()
        {
            return string.Join("\n", this);
        }
    }

    public class Register
    {
        public string Name { get; set; }
        public string NormalizedName => RegisterNames[Name];
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
        public VariableSize Size { get; set; }

        public Variable(string baseLabel, long offset, VariableSize size)
        {
            Base = new Label(baseLabel);
            Offset = offset;
            Size = size;
        }

        public Assembly Load(Register register)
        {
            var asm = new Assembly($"mov {register}, {Size.Asm()} [{Base} + {Offset}]");
            return asm;
        }

        public Assembly Store(Register register)
        {
            var asm = new Assembly($"xor {register.NormalizedName}, {register.NormalizedName}");
            asm.AddLine($"mov {Size.Asm()} [{Base} + {Offset}], {register}");
            return asm;
        }
    }
}