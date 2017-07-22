using System;

namespace Reverie.CodeGeneration
{
    public abstract class BinaryOp : ICode
    {
        public Variable A { get; }
        public Variable B { get; }
        public Variable Out { get; }

        public virtual bool HasOutput => true;

        protected BinaryOp(Variable a, Variable b, Variable output)
        {
            if (!HasOutput && output != null)
            {
                throw new ArgumentException("This operation does not produce an output value", nameof(output));
            }
            A = a;
            B = b;
            Out = output;
        }

        protected abstract Assembly GenerateOperation(Register a, Register b);

        public void Generate(Assembly asm, Context ctx)
        {
            var regA = ctx.Load(A, asm);
            var regB = ctx.Load(B, asm);
            asm.Add(GenerateOperation(regA, regB));
            if (Out != null)
            {
                ctx.Store(regA, Out, asm);
            }
        }
    }
}