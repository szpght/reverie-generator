using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Reverie.CodeGeneration
{
    public class Context
    {
        public ICallingConvention CallingConvention { get; }
        private readonly RegisterContainer Registers;

        public Context(ICallingConvention callingConvention)
        {
            CallingConvention = callingConvention;
            Registers = new RegisterContainer(callingConvention.GetRegisters());
        }

        public Register Load(Variable variable, Assembly assembly)
        {
            var info = Registers.GetUsableVariableInfo(variable);
            if (info == null)
            {
                info = Registers.GetFreeRegisterInfo();
                info.Variable = variable;
                variable.Load(info.Register, assembly);
            }
            Registers.UseRegister(info.Register);
            return info.Register;
        }

        public void LoadToRegister(Variable variable, Register register, Assembly assembly)
        {
            variable.Load(register, assembly);
            var info = Registers.GetRegisterInfo(register);
            info.Variable = variable;
            Registers.UseRegister(register);
        }

        public void Store(Register register, Variable variable, Assembly assembly)
        {
            Registers.InvalidateVariable(variable);
            var info = Registers.GetRegisterInfo(register);
            info.Variable = variable;
            variable.Store(register, assembly);
        }

        public void Lock(Register register)
        {
            var info = Registers.GetRegisterInfo(register);
            info.Locked = true;
        }

        public void Unlock(Register register)
        {
            var info = Registers.GetRegisterInfo(register);
            info.Locked = false;
        }

        public void Invalidate()
        {
            Registers.InvalidateRegisters();
        }

        public void InvalidateVolatileRegisters()
        {
            Registers.InvalidateVolatileRegisters();
        }

        public Context Copy()
        {
            return new Context(this);
        }

        public void Join(Context src)
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

        private Context(Context ctx)
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

        public RegisterInfo GetUsableVariableInfo(Variable variable)
        {
            return Registers.SingleOrDefault(x => x.Variable == variable && !x.Locked);
        }

        public RegisterInfo GetRegisterInfo(Register register)
        {
            return Registers.SingleOrDefault(x => x.Register == register);
        }

        public RegisterInfo GetFreeRegisterInfo()
        {
            return Registers.FirstOrDefault(x => x.Empty && !x.Nonvolatile)
                ?? Registers.FirstOrDefault(x => x.Empty && x.Dirty)
                ?? Registers.FirstOrDefault(x => x.Empty)
                ?? Registers.First(x => !x.Locked);
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
            var infos = Registers
                .Where(x => x.Variable == variable);
            foreach (var info in infos)
            {
                info.Variable = null;
                info.Locked = false;
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
                if (value != null)
                {
                    Dirty = true;
                }
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

        public override string ToString()
        {
            string variable;
            if (Variable == null)
            {
                variable = "empty";
            }
            else
            {
                variable = Variable.ToString();
            }
            string vol = Nonvolatile ? "v" : "V";
            string locked = Locked ? "Q" : "q";
            string dirty = Dirty ? "D" : "d";
            variable = $"{variable} {vol} {dirty} {locked}";
            return $"{Register} -> {variable}";
        }
    }
}