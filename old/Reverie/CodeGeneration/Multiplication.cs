namespace Reverie.CodeGeneration
{
    public class Multiplication : IBinaryOp
    {
        public Variable A { get; }
        public Variable B { get; }
        public Variable Out { get; }

        public Multiplication(Variable a, Variable b, Variable @out)
        {
            A = a;
            B = b;
            Out = @out;
        }

        public void Generate(Assembly asm, Context ctx)
        {
            if (A.Sign || B.Sign)
            {
                GenerateSigned(asm, ctx);
            }
            else
            {
                GenerateUnsigned(asm, ctx);
            }
        }

        private void GenerateUnsigned(Assembly asm, Context ctx)
        {
            var opA = new Register("rax");
            ctx.LoadToRegister(A, opA, asm);
            ctx.Lock(opA);
            var opB = ctx.Load(B, asm);
            ctx.Unlock(opA);
            asm.Add($"mul {opB}");
            ctx.Store(opA, Out, asm);
        }

        private void GenerateSigned(Assembly asm, Context ctx)
        {
            var opA = ctx.Load(A, asm);
            var opB = ctx.Load(B, asm);
            asm.Add($"imul {opA}, {opB}");
            ctx.Store(opA, Out, asm);
        }
    }
}