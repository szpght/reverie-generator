using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Reverie.CodeGeneration
{
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
}