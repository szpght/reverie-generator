using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public class FunctionCall : ICode
    {
        public Label Function { get; }
        public IList<Variable> Arguments { get; set; }
        public Variable Result { get; set; }

        public FunctionCall(Label function) : this(function, new List<Variable>())
        {
        }

        public FunctionCall(Label function, IList<Variable> args)
        {
            Function = function;
            Arguments = args;
        }

        public void Generate(Assembly asm, Context ctx)
        {
            var cc = ctx.CallingConvention;
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
