namespace Reverie.CodeGeneration
{
    public class Division : IBinaryOp
    {
        public Variable A { get; }
        public Variable B { get; }
        public Variable Out { get; }
        public Variable Modulo { get; }

        public Division(Variable a, Variable b, Variable @out, Variable modulo)
        {
            A = a;
            B = b;
            Out = @out;
            Modulo = modulo;
        }

        public void Generate(Assembly asm, Context ctx)
        {
            var sign = A.Sign || B.Sign;
            var instruction = sign ? "idiv" : "div";
            var div = new Register("rax");
            var mod = new Register("rdx");
            ctx.LoadToRegister(A, div, asm);
            ctx.Lock(div);
            ctx.Lock(mod);
            var op = ctx.Load(B, asm);
            asm.Add(sign ? "cdq" : "xor rdx, rdx");
            asm.Add($"{instruction} {op}");
            if (Out != null)
            {
                ctx.Store(div, Out, asm);
            }
            if (Modulo != null)
            {
                ctx.Store(mod, Modulo, asm);
            }
            ctx.Unlock(div);
            ctx.Unlock(mod);
        }
    }
}
