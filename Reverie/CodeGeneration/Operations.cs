using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace Reverie.CodeGeneration
{
    public interface ICode
    {
        Assembly Generate(Context ctx);
    }

    public interface IBinaryOp
    {
        Variable A { get; }
        Variable B { get; }
    }

    public abstract class BinaryOp : ICode, IBinaryOp
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

        public Assembly Generate(Context ctx)
        {
            var asm = new Assembly();
            var regA = ctx.Load(A, asm);
            var regB = ctx.Load(B, asm);
            asm.Add(GenerateOperation(regA, regB));
            if (Out != null)
            {
                ctx.Store(regA, Out, asm);
            }
            return asm;
        }
    }

    public class Add : BinaryOp
    {
        public Add(Variable a, Variable b, Variable output) : base(a, b, output) { }

        protected override Assembly GenerateOperation(Register a, Register b)
        {
            return new Assembly($"add {a.FullName}, {b.FullName}");
        }
    }

    public class Sub : BinaryOp
    {
        public Sub(Variable a, Variable b, Variable output) : base(a, b, output) { }

        protected override Assembly GenerateOperation(Register a, Register b)
        {
            return new Assembly($"sub {a.FullName}, {b.FullName}");
        }
    }

    public class ShiftLeft : BinaryOp
    {
        public ShiftLeft(Variable a, Variable b, Variable output) : base(a, b, output) { }

        protected override Assembly GenerateOperation(Register a, Register b)
        {
            return new Assembly($"shl {a.FullName}, {b.FullName}");
        }
    }

    public class ShiftRight : BinaryOp
    {
        public ShiftRight(Variable a, Variable b, Variable output) : base(a, b, output) { }

        protected override Assembly GenerateOperation(Register a, Register b)
        {
            return new Assembly($"shr {a.FullName}, {b.FullName}");
        }
    }

    public class And : BinaryOp
    {
        public And(Variable a, Variable b, Variable output) : base(a, b, output) { }

        protected override Assembly GenerateOperation(Register a, Register b)
        {
            return new Assembly($"and {a.FullName}, {b.FullName}");
        }
    }

    public class Or : BinaryOp
    {
        public Or(Variable a, Variable b, Variable output) : base(a, b, output) { }

        protected override Assembly GenerateOperation(Register a, Register b)
        {
            return new Assembly($"or {a.FullName}, {b.FullName}");
        }
    }

    public class Xor : BinaryOp
    {
        public Xor(Variable a, Variable b, Variable output) : base(a, b, output) { }

        protected override Assembly GenerateOperation(Register a, Register b)
        {
            return new Assembly($"xor {a.FullName}, {b.FullName}");
        }
    }

    public class Cmp : BinaryOp
    {
        public override bool HasOutput => false;

        public Cmp(Variable a, Variable b, Variable output) : base(a, b, output) { }

        protected override Assembly GenerateOperation(Register a, Register b)
        {
            return new Assembly($"cmp {a.FullName}, {b.FullName}");
        }
    }

    public class Test : BinaryOp
    {
        public override bool HasOutput => false;

        public Test(Variable a, Variable b, Variable output) : base(a, b, output) { }

        protected override Assembly GenerateOperation(Register a, Register b)
        {
            return new Assembly($"test {a.FullName}, {b.FullName}");
        }
    }

    public interface IPredicate : ICode
    {
        bool Negated { get; set; }
        bool JumpToElse { get; set; }
        string Jump { get; }
    }

    public enum RelationType
    {
        And,
        Or,
        Nor,
    }

    public class Relation : IPredicate
    {
        public IPredicate Left { get; }
        public IPredicate Right { get; }
        public bool Negated { get; set; }
        public bool JumpToElse { get; set; }
        public RelationType Type { get; private set; }
        public string Jump => null;

        private Relation LeftAsRelation => Left as Relation;
        private Relation RightAsRelation => Right as Relation;

        public Relation(IPredicate left, IPredicate right, RelationType type, bool negated = false)
        {
            Left = left;
            Right = right;
            Type = type;
            Negated = negated;
        }

        /* wstępny algorytm:
         * 1. przechodzenie drzewa post-order
         * 2. jeśli węzeł jest andem, to zmieniam go na nor i neguję dzieci
         *   2.1. Negując dziecko zmieniam nor na or i odwrotnie
         * 3. po przejściu całego drzewa mam tylko ory i nory
         * 4. przechodzę drzewo znowu i jeśli spotykam nora to neguję JumpToElse wszystkim wychodzącym z niego liściom
         */
        public void Convert()
        {
            var relations = new List<Relation>();
            MakePostOrderListOfRelations(relations);
            foreach (var relation in relations)
            {
                relation.ConvertRelationType();
            }
            foreach (var relation in relations.Where(x => x.Type == RelationType.Nor && !x.Negated|| x.Type == RelationType.Or && x.Negated))
            {
                relation.NegateJumpInLeaves();
            }
        }

        private void MakePostOrderListOfRelations(List<Relation> relations)
        {
            LeftAsRelation?.MakePostOrderListOfRelations(relations);
            RightAsRelation?.MakePostOrderListOfRelations(relations);
            relations.Add(this);
        }

        private void ConvertRelationType()
        {
            if (Type == RelationType.And)
            {
                Type = RelationType.Nor;
                if (LeftAsRelation != null)
                {
                    LeftAsRelation?.FlipType();
                    RightAsRelation?.FlipType();
                }
                else
                {
                    Left.Negated = !Left.Negated;
                    Right.Negated = !Right.Negated;
                }
            }
        }

        private void FlipType()
        {
            if (Type == RelationType.Or)
            {
                Type = RelationType.Nor;
            }
            else if (Type == RelationType.Nor)
            {
                Type = RelationType.Or;
            }
            else
            {
                throw new Exception("this is supposed to be Or or Nor");
            }
        }

        private void NegateJumpInLeaves()
        {
            LeftAsRelation?.NegateJumpInLeaves();
            RightAsRelation?.NegateJumpInLeaves();
            if (LeftAsRelation == null)
            {
                Left.JumpToElse = !Left.JumpToElse;
                Right.JumpToElse = !Right.JumpToElse;
            }
        }

        public Assembly Generate(Context ctx)
        {
            return new Assembly();
        }
    }

    public class Greater : BinaryOp, IPredicate
    {
        public bool Negated { get; set; }
        public bool JumpToElse { get; set; }

        public Greater(Variable a, Variable b) : base(a, b, null) { }

        public string Jump
        {
            get
            {
                var sign = A.Sign || B.Sign;
                if (!Negated && !sign)
                    return "ja";
                if (!Negated && sign)
                    return "jg";
                if (Negated && !sign)
                    return "jbe";
                if (Negated && sign)
                    return "jle";
                throw new Exception("runtime is broken");
            }
        }

        protected override Assembly GenerateOperation(Register a, Register b)
        {
            return new Assembly($"cmp {a.FullName}, {b.FullName}");
        }
    }

    public class If : ICode
    {
        public IPredicate Predicate { get; }
        public CodeBlock Code { get; }
        public CodeBlock Else { get; }

        private IPredicate LastPredicate;

        public If(IPredicate predicate, CodeBlock code, CodeBlock @else)
        {
            Predicate = predicate;
            Code = code;
            if (@else != null)
            {
                Else = @else;
            }
            else
            {
                Else = new CodeBlock();
            }
        }

        public Assembly Generate(Context ctx)
        {
            var asm = new Assembly();
            var relation = Predicate as Relation;
            relation?.Convert();
            TraverseAndGenerate(Predicate, ctx, asm);

            var elseCtx = ctx.GetCopy();

            if (LastPredicate.JumpToElse)
            {
                asm.Add(Code.Generate(ctx));
                asm.Add($"jmp {Else.EndLabel}");
                asm.Add(Else.Generate(elseCtx));
            }
            else
            {
                asm.Add(Else.Generate(elseCtx));
                asm.Add($"jmp {Code.EndLabel}");
                asm.Add(Code.Generate(ctx));
            }

            ctx.Join(ctx, elseCtx);
            return asm;
        }

        private void TraverseAndGenerate(IPredicate predicate, Context ctx, Assembly output)
        {
            var relation = predicate as Relation;
            if (relation != null)
            {
                TraverseAndGenerate(relation.Left, ctx, output);
                TraverseAndGenerate(relation.Right, ctx, output);
            }
            else
            {
                LastPredicate = predicate;
                output.Add(predicate.Generate(ctx));
                var labelToJump = predicate.JumpToElse ? Else.BeginLabel : Code.BeginLabel;
                output.Add($"{predicate.Jump} {labelToJump}");
            }
        }
    }
}
