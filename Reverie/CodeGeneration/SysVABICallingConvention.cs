using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public class SysVAbiCallingConvention : ICallingConvention
    {
        public IList<RegisterInfo> GetRegisters()
        {
            return new List<RegisterInfo>
            {
                new RegisterInfo("rax", false),
                new RegisterInfo("rbx", true),
                new RegisterInfo("rcx", false),
                new RegisterInfo("rdx", false),
                new RegisterInfo("rdi", false),
                new RegisterInfo("rsi", false),
                new RegisterInfo("r8", false),
                new RegisterInfo("r9", false),
                new RegisterInfo("r10", false),
                new RegisterInfo("r11", false),
                new RegisterInfo("r12", true),
                new RegisterInfo("r13", true),
                new RegisterInfo("r14", true),
                new RegisterInfo("r15", true),
            };
        }

        public Assembly LoadArguments(IList<Variable> arguments, Context ctx)
        {
            var asm = new Assembly();
            int i;
            for (i = 0; i < ArgumentRegisters.Count && i < arguments.Count; ++i)
            {
                var arg = arguments[i];
                var reg = ArgumentRegisters[i];
                ctx.LoadToRegister(arg, reg, asm);
                ctx.Lock(reg);
            }

            for (int j = arguments.Count - 1; j >= i; --j)
            {
                var push = new Push(arguments[j]);
                push.Generate(asm, ctx);
            }

            // number of float arguments in variadic function
            asm.Add("xor rax, rax");
            return asm;
        }

        public Assembly UnloadArguments(IList<Variable> arguments, Context ctx)
        {
            int stackArguments = arguments.Count - ArgumentRegisters.Count;
            return new Assembly($"add rsp, {8 * stackArguments}");
        }

        private readonly List<Register> ArgumentRegisters = new List<Register>
        {
            new Register("rdi"),
            new Register("rsi"),
            new Register("rdx"),
            new Register("rcx"),
            new Register("r8"),
            new Register("r9"),
        };
    }
}