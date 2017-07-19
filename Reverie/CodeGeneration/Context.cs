using System.Collections.Generic;
using System.Linq;

namespace Reverie.CodeGeneration
{
    public class Context
    {
        private readonly List<RegisterVariablePair> Allocations = new List<RegisterVariablePair>();
        private readonly List<string> FreeRegisters;
        private readonly List<string> FreeSavedRegisters;

        public Context()
        {
            FreeRegisters = FreshFreeRegisters();
            FreeSavedRegisters = FreshSavedRegisters();
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
                FreeRegisters.Add(varPair.Register.FullName);
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
                name = FreeRegisters.Last();
                FreeRegisters.Remove(name);
            }
            else if (FreeSavedRegisters.Any())
            {
                name = FreeSavedRegisters.Last();
                FreeSavedRegisters.Remove(name);
            }
            else
            {
                var pair = Allocations.First();
                Allocations.Remove(pair);
                name = pair.Register.FullName;
            }
            return new Register(name, size);
        }

        public Context GetCopy()
        {
            return new Context(Allocations, FreeRegisters, FreeSavedRegisters);
        }

        private Context(List<RegisterVariablePair> allocations, List<string> freeRegisters,
            List<string> freeSavedRegisters)
        {
            Allocations = new List<RegisterVariablePair>(allocations);
            FreeRegisters = new List<string>(freeRegisters);
            FreeSavedRegisters = new List<string>(freeSavedRegisters);
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

        private List<string> FreshFreeRegisters()
        {
            return new List<string>()
            {
                "rax",
                "rcd",
                "rdx",
                "rsi",
                "rdi",
                "r8",
                "r9",
                "r10",
                "r11",
            };
        }

        private List<string> FreshSavedRegisters()
        {
            return new List<string>()
            {
                "rbx",
                "r12",
                "r13",
                "r14",
                "r15",
            };
        }
    }
}