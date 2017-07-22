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
            throw new System.NotImplementedException();
        }

        public void Generate(Assembly asm, Context ctx)
        {
            var register = ctx.Load(Variable, asm);
            asm.Add($"push {register}");
        }
    }
}