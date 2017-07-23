namespace Reverie.CodeGeneration
{
    public class If : ICode
    {
        public IPredicate Predicate { get; }
        public CodeBlock Code { get; }
        public CodeBlock Else { get; }

        private IPredicate LastPredicate;

        public If(IPredicate predicate, CodeBlock code, CodeBlock @else = null)
        {
            Predicate = predicate;
            Code = code;
            Else = @else ?? new CodeBlock();
        }

        public void Generate(Assembly asm, Context ctx)
        {
            (Predicate as Relation)?.NormalizeToOr();
            GenerateChecks(Predicate, ctx, asm);
            GenerateBody(ctx, asm);
        }

        private void GenerateBody(Context ctx, Assembly asm)
        {
            var elseCtx = ctx.Copy();

            if (LastPredicate.JumpToElse)
            {
                Code.Generate(asm, ctx);
                asm.Add($"jmp {Else.EndLabel}");
                Else.Generate(asm, elseCtx);
            }
            else
            {
                Else.Generate(asm, elseCtx);
                asm.Add($"jmp {Code.EndLabel}");
                Code.Generate(asm, ctx);
            }
            ctx.Join(elseCtx);
        }

        private void GenerateChecks(IPredicate predicate, Context ctx, Assembly asm)
        {
            var relation = predicate as Relation;
            if (relation != null)
            {
                GenerateChecks(relation.Left, ctx, asm);
                GenerateChecks(relation.Right, ctx, asm);
            }
            else
            {
                LastPredicate = predicate;
                predicate.Generate(asm, ctx);
                var labelToJump = predicate.JumpToElse ? Else.BeginLabel : Code.BeginLabel;
                asm.Add($"{predicate.Jump} {labelToJump}");
            }
        }
    }
}