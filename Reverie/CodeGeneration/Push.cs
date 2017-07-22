namespace Reverie.CodeGeneration
{
    public class Push : ICode
    {
        public Variable Variable { get; }

        public Push(Variable variable)
        {
            Variable = variable;
        }

        public Assembly Generate(Context ctx)
        {
            var asm = new Assembly();
            var register = ctx.Load(Variable, asm);
            asm.Add($"push {register}");
            return asm;
        }
    }
}