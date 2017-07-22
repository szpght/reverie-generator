using System.Collections.Generic;
using System.Linq;

namespace Reverie.CodeGeneration
{
    public interface IArchitecture
    {
        IList<RegisterInfo> GetRegisters();
    }

    public class NewContext
    {
        public IArchitecture Architecture { get; }
        public ICallingConvention CallingConvention { get; }
        private IList<RegisterInfo> Registers;

        public NewContext(IArchitecture architecture, ICallingConvention callingConvention)
        {
            Architecture = architecture;
            CallingConvention = callingConvention;
            Registers = architecture.GetRegisters();
        }

        public Register Load(Variable variable, Assembly assembly)
        {
            var alloc = Registers.SingleOrDefault(x => x.Variable == variable);
            if (alloc == null)
            {
                alloc = GetRegister();
                alloc.Variable = variable;
                var loadAssembly = variable.Load(alloc.Register);
                assembly.Add(loadAssembly);
            }
            ToEnd(alloc);
            return alloc.Register;
        }

        private RegisterInfo GetRegister()
        {
            return Registers
                .First(x => !x.Locked);
        }

        private void ToEnd(RegisterInfo info)
        {
            Registers.Remove(info);
            Registers.Add(info);
        }
    }

    public class RegisterInfo
    {
        public Register Register { get; }
        public bool Nonvolatile { get; }
        public bool Dirty { get; set; }
        public bool Locked { get; set; }

        public Variable Variable
        {
            get => variable_;
            set
            {
                variable_ = value;
                Dirty = true;
            }
        }


        private Variable variable_;

        public RegisterInfo(string register, bool nonvolatile)
        {
            Register = new Register(register);
            Nonvolatile = nonvolatile;
        }
    }

    public class Context
    {
        public ICallingConvention CallingConvention { get; }

        private List<RegisterVariablePair> Allocations = new List<RegisterVariablePair>();
        private List<RegisterVariablePair> LockedAllocations = new List<RegisterVariablePair>();
        private IList<string> FreeRegisters;
        private IList<string> FreeSavedRegisters;

        public Context(ICallingConvention callingConvention)
        {
            CallingConvention = callingConvention;
            FreeRegisters = CallingConvention.GetVolatileRegisters();
            FreeSavedRegisters = CallingConvention.GetNonvolatileRegisters();
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
            var storeAssembly = variable.Store(register);
            assembly.Add(storeAssembly);

            var varPair = Allocations.SingleOrDefault(x => x.Variable == variable);
            if (varPair != null)
            {
                FreeRegisters.Add(varPair.Register.Name);
                Allocations.Remove(varPair);
            }

            Allocations.RemoveAll(x => x.Register == register);
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
                name = pair.Register.Name;
            }
            return new Register(name);
        }

        public Context GetCopy()
        {
            return new Context(Allocations, FreeRegisters, FreeSavedRegisters);
        }

        public void Join(Context a, Context b)
        {
            JoinAllocations(a, b);
            JoinFreeSavedRegisters(a, b);
            JoinFreeRegisters(a, b);
        }

        public void InvalidateRegisters()
        {
            foreach (var pair in Allocations)
            {
                FreeRegisters.Add(pair.Register.Name);
            }
            Allocations.Clear();
        }

        public void LockFunctionArgument(Variable variable, string registerName, Assembly assembly)
        {
            var variableAllocation = Allocations.SingleOrDefault(x => x.Variable == variable);
            if (variableAllocation != null)
            {
                Allocations.Remove(variableAllocation);
            }

            var alloc = Allocations.SingleOrDefault(x => x.Register.Name == registerName);
            if (alloc != null)
            {
                Allocations.Remove(alloc);
            }
            FreeRegisters.Remove(registerName);
            FreeSavedRegisters.Remove(registerName);

            var register = new Register(registerName);
            var pair = new RegisterVariablePair(register, variable);
            LockedAllocations.Add(pair);
            var loadAsm = variable.Load(register);
            assembly.Add(loadAsm);
        }

        public void AfterFunctionCall()
        {
            InvalidateUnsavedRegisters();
            UnlockArguments();
        }

        public void InvalidateVariable(Variable variable)
        {
            var pair = Allocations.SingleOrDefault(x => x.Variable == variable);
            if (pair != null)
            {
                Allocations.Remove(pair);
                FreeRegisters.Add(pair.Register.Name);
            }
        }

        private void InvalidateUnsavedRegisters()
        {
            var unsaved = CallingConvention.GetVolatileRegisters();
            foreach (var reg in unsaved)
            {
                ReleaseRegister(reg);
            }
        }

        private void UnlockArguments()
        {
            foreach (var locked in LockedAllocations)
            {
                FreeRegisters.Add(locked.Register.Name);
            }
            LockedAllocations.Clear();
        }

        private void ReleaseRegister(string name)
        {
            var pair = Allocations.SingleOrDefault(x => x.Register.Name == name);
            if (pair != null)
            {
                Allocations.Remove(pair);
                FreeRegisters.Add(name);
            }
        }

        private void JoinFreeRegisters(Context a, Context b)
        {
            FreeRegisters = CallingConvention.GetVolatileRegisters()
                .Union(CallingConvention.GetNonvolatileRegisters())
                .Except(FreeSavedRegisters)
                .Except(Allocations.Select(x => x.Register.Name))
                .ToList();
        }

        private void JoinFreeSavedRegisters(Context a, Context b)
        {
            FreeSavedRegisters = a.FreeSavedRegisters.
                Intersect(b.FreeSavedRegisters)
                .ToList();
        }

        private void JoinAllocations(Context a, Context b)
        {
            var newAllocations = new List<RegisterVariablePair>();
            foreach (var pair in a.Allocations)
            {
                if (b.Allocations.Any(x => x.Register == pair.Register && x.Variable == pair.Variable))
                {
                    newAllocations.Add(pair);
                }
            }
            Allocations = newAllocations;
        }

        private Context(IList<RegisterVariablePair> allocations, IList<string> freeRegisters,
            IList<string> freeSavedRegisters)
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
    }
}