using System;

namespace Reverie.CodeGeneration
{
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
}