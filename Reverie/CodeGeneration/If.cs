namespace Reverie.CodeGeneration
{
    public class If : ICode
    {
        public IPredicate Predicate { get; }
        public CodeBlock Code { get; }
        public CodeBlock Else { get; }
        public Label ElseLabel => Else == null ? Code.EndLabel : Else.EndLabel;

        private IPredicate LastPredicate;

        public If(IPredicate predicate, CodeBlock code, CodeBlock @else)
        {
            Predicate = predicate;
            Code = code;
            Else = @else;
        }

        public Assembly Generate(Context ctx)
        {
            var asm = new Assembly();
            (Predicate as Relation)?.NormalizeToOr();
            GenerateChecks(Predicate, ctx, asm);
            GenerateBody(ctx, asm);
            return asm;
        }

        private void GenerateBody(Context ctx, Assembly asm)
        {
            var elseCtx = ctx.GetCopy();

            if (LastPredicate.JumpToElse)
            {
                asm.Add(Code, ctx);
                if (Else != null)
                {
                    asm.Add($"jmp {Else.EndLabel}");
                    asm.Add(Else, elseCtx);
                }
            }
            else
            {
                if (Else != null)
                {
                    asm.Add(Else, elseCtx);
                    asm.Add($"jmp {Code.EndLabel}");
                }
                asm.Add(Code, ctx);
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
                output.Add(predicate, ctx);
                var labelToJump = predicate.JumpToElse ? ElseLabel : Code.BeginLabel;
                output.Add($"{predicate.Jump} {labelToJump}");
            }
        }
    }
}