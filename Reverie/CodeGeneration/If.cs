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
            var elseCtx = ctx.GetCopy();

            if (LastPredicate.JumpToElse)
            {
                asm.Generate(Code, ctx);
                asm.Add($"jmp {Else.EndLabel}");
                asm.Generate(Else, elseCtx);
            }
            else
            {
                asm.Generate(Else, elseCtx);
                asm.Add($"jmp {Code.EndLabel}");
                asm.Generate(Code, ctx);
            }
            ctx.Join(ctx, elseCtx);
        }

        private void GenerateChecks(IPredicate predicate, Context ctx, Assembly output)
        {
            var relation = predicate as Relation;
            if (relation != null)
            {
                GenerateChecks(relation.Left, ctx, output);
                GenerateChecks(relation.Right, ctx, output);
            }
            else
            {
                LastPredicate = predicate;
                output.Generate(predicate, ctx);
                var labelToJump = predicate.JumpToElse ? Else.BeginLabel : Code.BeginLabel;
                output.Add($"{predicate.Jump} {labelToJump}");
            }
        }
    }
}