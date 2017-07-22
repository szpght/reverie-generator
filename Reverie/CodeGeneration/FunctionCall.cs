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

        public Assembly Generate(Context ctx)
        {
            var asm = new Assembly();
            var cc = ctx.CallingConvention;
            asm.Add(cc.LoadArguments(Arguments, ctx));
            asm.Add($"call {Function}");
            asm.Add(cc.UnloadArguments(Arguments, ctx));
            if (Result != null)
            {
                ctx.InvalidateVariable(Result);
                var resultRegister = new Register("rax");
                asm.Add(Result.Store(resultRegister));
            }
            ctx.AfterFunctionCall();
            return asm;
        }
    }
}
