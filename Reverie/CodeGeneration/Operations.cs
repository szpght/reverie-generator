using System;

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
                relation.Left.Negated = !relation.Left.Negated;
                relation.Right.Negated = !relation.Right.Negated;
            }
            Convert(relation.Left);
            Convert(relation.Right);
        }
    }

    public class Greater : BinaryOp, IPredicate
    {
        public bool Negated { get; set; }

        public Greater(Variable a, Variable b) : base(a, b, null) { }

        public string Instruction()
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

        protected override Assembly GenerateOperation(Register a, Register b)
        {
            return new Assembly($"cmp {a.FullName}, {b.FullName}");
        }
    }

    public class If : ICode
    {
        public IPredicate Predicate { get; set; }
        public CodeBlock Code { get; set; }
        public CodeBlock Else { get; set; }

        public Assembly Generate(Context ctx)
        {
            var relation = Predicate as Relation;
            if (relation != null)
            {
                G
            }
            
        }
    }
}
