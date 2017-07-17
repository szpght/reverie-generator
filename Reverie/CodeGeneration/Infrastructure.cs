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

    public class Label
    {
        public string Name { get; }

        public Label(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

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
            string registerName = register.FullName;
            if (Size == VariableSize.Qword || Size == VariableSize.Dword && !Sign)
            {
                movInstruction = "mov";
                registerName = register.Name;
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
            return new Assembly($"{movInstruction} {registerName}, {Size.Asm()} [{Base} + {Offset}]");
        }

        public Assembly Store(Register register)
        {
            return new Assembly($"mov {Size.Asm()} [{Base} + {Offset}], {register}");
        }
    }

    public class Context
    {
        private readonly List<RegisterVariablePair> Allocations = new List<RegisterVariablePair>();
        private readonly HashSet<string> FreeRegisters;

        public Context()
        {
            FreeRegisters = FreshFreeRegisters();
        }

        public Register Load(Variable variable, Assembly assembly)
        {
            var pair = Allocations.SingleOrDefault(x => x.Variable == variable);
            if (pair != null)
            {
                Allocations.Remove(pair);
                Allocations.Add(pair);
                return pair.Register;
            }
            var register = GetFreeRegister(variable.Size);
            var loadAssembly = variable.Load(register);
            assembly.Add(loadAssembly);
            pair = new RegisterVariablePair(register, variable);
            Allocations.Add(pair);
            return register;
        }

        public void Store(Register register, Variable variable, Assembly assembly)
        {
            register = register.WithSize(variable.Size);
            var storeAssembly = variable.Store(register);
            assembly.Add(storeAssembly);

            var varPair = Allocations.SingleOrDefault(x => x.Variable == variable);
            if (varPair != null)
            {
                FreeRegisters.Add(varPair.Register.NormalizedName);
                Allocations.Remove(varPair);
            }

            Allocations.RemoveAll(x => x.Register.TheSameAs(register));
            var pair = new RegisterVariablePair(register, variable);
            Allocations.Add(pair);
        }

        private Register GetFreeRegister(VariableSize size)
        {
            string name;
            if (FreeRegisters.Any())
            {
                name = FreeRegisters.First();
                FreeRegisters.Remove(name);
            }
            else
            {
                var pair = Allocations.First();
                Allocations.Remove(pair);
                name = pair.Register.NormalizedName;
            }
            return new Register(name, size);
        }

        private class RegisterVariablePair
        {
            public Register Register { get; }
            public Variable Variable { get; }

            public RegisterVariablePair(Register register, Variable variable)
            {
                Register = register;
                Variable = variable;
            }
        }

        private HashSet<string> FreshFreeRegisters()
        {
            return new HashSet<string>()
            {
                "r10",
                "r11",
                "rbx",
                "rax"
            };
        }
    }
}
