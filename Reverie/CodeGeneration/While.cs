﻿namespace Reverie.CodeGeneration
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
            throw new System.NotImplementedException();
        }

        public void Generate(Assembly asm, Context ctx)
        {
            var code = new CodeBlock();
            var beginning = Label.New(LabelType.Local);
            var body = new If(Predicate, code);

            code.Add(Code);
            code.Add(new Jmp(beginning));

            ctx.InvalidateRegisters();

            asm.Generate(beginning, ctx);
            asm.Generate(body, ctx);
        }
    }
}
