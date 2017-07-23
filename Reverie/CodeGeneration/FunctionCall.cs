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
            asm.Add(cc.LoadArguments(Arguments, ctx));
            asm.Add($"call {Function}");
            asm.Add(cc.UnloadArguments(Arguments, ctx));
            ctx.InvalidateVolatileRegisters();
            if (Result != null)
            {
                var resultRegister = new Register("rax");
                ctx.Store(resultRegister, Result, asm);
            }
        }
    }
}
