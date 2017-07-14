﻿using System;
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
        public VariableSize Size { get; set; }

        public Variable(string baseLabel, long offset, VariableSize size)
        {
            Base = new Label(baseLabel);
            Offset = offset;
            Size = size;
        }

        public Assembly Load(Register register)
        {
            var asm = new Assembly();
            if (Size != VariableSize.Qword)
            {
                asm.Add($"xor {register.NormalizedName}, {register.NormalizedName}");
            }
            asm.Add($"mov {register}, {Size.Asm()} [{Base} + {Offset}]");
            return asm;
        }

        public Assembly Store(Register register)
        {
            return new Assembly($"mov {Size.Asm()} [{Base} + {Offset}], {register}");
        }
    }

    public class RegisterAllocator
    {
        public Dictionary<Variable, Register> Allocations { get; set; }
            = new Dictionary<Variable, Register>();

        private static HashSet<string> FreeRegisters =
            new HashSet<string>()
            {
                "r15",
                "r14",
                "r13",
                "r12"
            };

        public Register Allocate(Variable variable)
        {
            if (Allocations.ContainsKey(variable))
            {
                return Allocations[variable];
            }
            var registerName = FreeRegisters.First();
            FreeRegisters.Remove(registerName);
            var register = new Register(registerName, variable.Size);
            Allocations[variable] = register;
            return register;
        }

        public void SetAllocation(Register register, Variable variable)
        {
            Allocations[variable] = register;
        }
    }

    public class Add
    {
        private RegisterAllocator allocator_;
        private Variable a_;
        private Variable b_;
        private Variable output_;

        public Add(RegisterAllocator allocator, Variable a, Variable b, Variable output)
        {
            allocator_ = allocator;
            a_ = a;
            b_ = b;
            output_ = output;
        }

        public Assembly Generate()
        {
            var regA = allocator_.Allocate(a_);
            var regB = allocator_.Allocate(b_);
            var asm = new Assembly();
            asm.Add(a_.Load(regA));
            asm.Add(b_.Load(regB));
            asm.Add($"add {regA.Name}, {regB.Name}");
            regA.Size = output_.Size;
            asm.Add(output_.Store(regA));
            allocator_.SetAllocation(regA, output_);
            return asm;
        }
    }
}