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
            var code = new CodeBlock();
            var beginning = Label.New(true);
            var body = new If(Predicate, code, null);

            code.Add(Code);
            code.Add(new Jmp(beginning));

            ctx.InvalidateRegisters();

            asm.Add(beginning, ctx);
            asm.Add(body, ctx);
            return asm;
        }
    }
}
