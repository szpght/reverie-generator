using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public class SysVABICallingConvention : ICallingConvention
    {
        public Assembly LoadArguments(IList<Variable> arguments, Context ctx)
        {
            var asm = new Assembly();
            int i;
            for (i = 0; i < ArgumentRegisters.Count && i < arguments.Count; ++i)
            {
                var arg = arguments[i];
                ctx.LockFunctionArgument(arg, ArgumentRegisters[i], asm);
            }

            for (int j = arguments.Count - 1; j >= i; --j)
            {
                asm.Add(new Push(arguments[j]), ctx);
            }

            return asm;
        }

        public Assembly UnloadArguments(IList<Variable> arguments, Context ctx)
        {
            int stackArguments = arguments.Count - ArgumentRegisters.Count;
            return new Assembly($"add rsp, {8 * stackArguments}");
        }

        private readonly List<string> ArgumentRegisters = new List<string>
        {
            "rdi",
            "rsi",
            "rdx",
            "rcx",
            "r8",
            "r9",
        };
    }
}