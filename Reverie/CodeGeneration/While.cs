namespace Reverie.CodeGeneration
{
    public class While : ICode
    {
        public IPredicate Predicate { get; }
        public CodeBlock Code { get; }

        public While(IPredicate predicate, CodeBlock code)
        {
            Predicate = predicate;
            Code = code;
        }

        public Assembly Generate(Context ctx)
        {
            var asm = new Assembly();
            var beginning = Label.New(true);
            // TODO do not modify Code
            Code.Code.Add(new Jmp(beginning));
            asm.Add(beginning, ctx);
            var check = new If(Predicate, Code, null);
            ctx.InvalidateRegisters();
            asm.Add(check, ctx);
            return asm;
        }
    }
}
