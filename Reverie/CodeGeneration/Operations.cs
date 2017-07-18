﻿using System;

namespace Reverie.CodeGeneration
{
    public interface ICode
    {
        Assembly Generate(Context ctx);
    }

    public abstract class BinaryOp : ICode
    {
        protected Variable A { get; }
        protected Variable B { get; }
        protected Variable Out { get; }

        protected virtual bool HasOutput => true;

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
        protected override bool HasOutput => false;

        public Cmp(Variable a, Variable b, Variable output) : base(a, b, output) { }

        protected override Assembly GenerateOperation(Register a, Register b)
        {
            return new Assembly($"cmp {a.FullName}, {b.FullName}");
        }
    }

    public class Test : BinaryOp
    {
        protected override bool HasOutput => false;

        public Test(Variable a, Variable b, Variable output) : base(a, b, output) { }

        protected override Assembly GenerateOperation(Register a, Register b)
        {
            return new Assembly($"test {a.FullName}, {b.FullName}");
        }
    }

    public interface IPredicate
    {
        bool Negated { get; set; }
    }

    public enum RelationType
    {
        And,
        Or,
    }

    public class Relation : IPredicate
    {
        public IPredicate Left { get; set; }
        public IPredicate Right { get; set; }
        public bool Negated { get; set; }
        public RelationType Type { get; set; }
    }

    public class Lol : IPredicate
    {
        public bool Negated { get; set; }
    }

    public static class PredicateConverter
    {
        public static void Convert(IPredicate predicate)
        {
            var relation = predicate as Relation;
            if (relation == null)
            {
                return;
            }
            if (relation.Type == RelationType.And)
            {
                relation.Type = RelationType.Or;
                if (relation.Left != null)
                {
                    relation.Left.Negated = !relation.Left.Negated;
                }
                if (relation.Right != null)
                {
                    relation.Right.Negated = !relation.Right.Negated;
                }
            }
            Convert(relation.Left);
            Convert(relation.Right);
        }
    }
}
