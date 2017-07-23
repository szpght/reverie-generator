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

        public void Generate(Assembly asm, Context ctx)
        {
            var code = new CodeBlock();
            var beginning = Label.New(LabelType.Local);
            var body = new If(Predicate, code);

            code.Add(Code);
            code.Add(new Jmp(beginning));

            ctx.Invalidate();

            beginning.Generate(asm, ctx);
            body.Generate(asm, ctx);
        }
    }
}
