using System.Linq;

namespace Reverie.CodeGeneration
{
    public class Context
    {
        public ICallingConvention CallingConvention { get; }
        public VariableAccounter VariableAccounter { get; }
        private readonly RegisterContainer Registers;

        public Context(ICallingConvention callingConvention)
        {
            CallingConvention = callingConvention;
            Registers = new RegisterContainer(callingConvention.GetRegisters());
            VariableAccounter = new VariableAccounter();
        }

        public Register Load(Variable variable, Assembly assembly)
        {
            VariableAccounter.AccountVariable(variable);

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
            VariableAccounter.AccountVariable(variable);

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
            VariableAccounter = ctx.VariableAccounter;
        }
    }
}