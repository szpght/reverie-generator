using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Reverie.CodeGeneration
{
    public class NewContext
    {
        public ICallingConvention CallingConvention { get; }
        private readonly RegisterContainer Registers;

        public NewContext(ICallingConvention callingConvention)
        {
            CallingConvention = callingConvention;
            Registers = new RegisterContainer(callingConvention.GetRegisters());
        }

        public Register Load(Variable variable, Assembly assembly)
        {
            var info = Registers.GetVariableInfo(variable);
            if (info == null)
            {
                info = Registers.GetFreeRegisterInfo();
                info.Variable = variable;
                variable.Load(info.Register, assembly);
            }
            Registers.UseRegister(info.Register);
            return info.Register;
        }

        public void Store(Variable variable, Register register, Assembly assembly)
        {
            Registers.InvalidateVariable(variable);
            var info = Registers.GetRegisterInfo(register);
            info.Variable = variable;
            variable.Store(register, assembly);
        }

        public NewContext Copy()
        {
            return new NewContext(this);
        }

        public void Join(NewContext src)
        {
            foreach (var reg in Registers)
            {
                var srcReg = src.Registers.GetRegisterInfo(reg.Register);
                if (srcReg.Dirty)
                    reg.Dirty = true;
                if (srcReg.Variable != reg.Variable)
                    reg.Variable = null;
            }
        }

        private NewContext(NewContext ctx)
        {
            CallingConvention = ctx.CallingConvention;
            var registers = ctx.Registers.Registers
                .Select(x => new RegisterInfo(x))
                .ToList();
            Registers = new RegisterContainer(registers);
        }
    }

    public class RegisterContainer : IEnumerable<RegisterInfo>
    {
        public IList<RegisterInfo> Registers { get; }

        public RegisterContainer(IList<RegisterInfo> registers)
        {
            Registers = registers;
        }

        public RegisterInfo GetVariableInfo(Variable variable)
        {
            return Registers.SingleOrDefault(x => x.Variable == variable);
        }

        public RegisterInfo GetRegisterInfo(Register register)
        {
            return Registers.SingleOrDefault(x => x.Register == register);
        }

        public RegisterInfo GetFreeRegisterInfo()
        {
            return Registers.FirstOrDefault(x => x.Empty) ?? Registers.First(x => !x.Locked);
        }

        public void UseRegister(Register register)
        {
            var info = GetRegisterInfo(register);
            Registers.Remove(info);
            Registers.Add(info);
        }

        public void LockRegister(Register register)
        {
            var info = GetRegisterInfo(register);
            info.Locked = true;
        }

        public void InvalidateVariable(Variable variable)
        {
            var info = Registers
                .SingleOrDefault(x => x.Variable == variable);
            if (info != null)
            {
                info.Variable = null;
            }
        }

        public void InvalidateVolatileRegisters()
        {
            var regs = Registers
                .Where(x => !x.Nonvolatile);
            foreach (var reg in regs)
            {
                reg.Variable = null;
                reg.Locked = false;
            }
        }

        public void InvalidateRegisters()
        {
            foreach (var info in Registers)
            {
                info.Variable = null;
                info.Locked = false;
            }
        }

        public IEnumerator<RegisterInfo> GetEnumerator()
        {
            return Registers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class RegisterInfo
    {
        public Register Register { get; }
        public bool Nonvolatile { get; }
        public bool Dirty { get; set; }
        public bool Locked { get; set; }
        public bool Empty => variable_ == null;

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

        public RegisterInfo(RegisterInfo info)
        {
            Register = info.Register;
            Nonvolatile = info.Nonvolatile;
            Dirty = info.Dirty;
            Locked = info.Locked;
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

        private void UnlockArguments()
        {
            foreach (var locked in LockedAllocations)
            {
                FreeRegisters.Add(locked.Register.Name);
            }
            LockedAllocations.Clear();
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