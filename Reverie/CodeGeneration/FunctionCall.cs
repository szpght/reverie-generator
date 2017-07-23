using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public class FunctionCall : ICode
    {
        public Label Function { get; }
        public IList<Variable> Arguments { get; }
        public Variable Result { get; }
        public ICallingConvention CallingConvention { get; }

        public FunctionCall(Label function, Variable result = null,
            IList<Variable> args = null, ICallingConvention callingConvention = null)
        {
            Function = function;
            Arguments = args ?? new List<Variable>();
            Result = result;
            CallingConvention = callingConvention;
        }

        public void Generate(Assembly asm, Context ctx)
        {
            var cc = CallingConvention ?? ctx.CallingConvention;
            cc.LoadArguments(Arguments, asm, ctx);
            asm.Add($"call {Function}");
            cc.UnloadArguments(Arguments, asm, ctx);
            ctx.InvalidateVolatileRegisters();
            if (Result != null)
            {
                cc.StoreResult(Result, asm, ctx);
            }
        }
    }
}
