using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public enum BasicBinaryOpType
    {
        Add,
        Subtract,
        ShiftLeft,
        ShiftRight,
        And,
        Or,
        Xor,
    }

    public class BasicBinaryOp : BinaryOp
    {
        private BasicBinaryOpType Type { get; }
        public string Instruction => Instructions[Type];

        private readonly Dictionary<BasicBinaryOpType, string> Instructions = new Dictionary<BasicBinaryOpType, string>
        {
            { BasicBinaryOpType.Add, "add" },
            { BasicBinaryOpType.And, "and" },
            { BasicBinaryOpType.Or, "or" },
            { BasicBinaryOpType.ShiftLeft, "shl" },
            { BasicBinaryOpType.ShiftRight, "shr" },
            { BasicBinaryOpType.Subtract, "sub" },
            { BasicBinaryOpType.Xor, "xor" },
        };

        public BasicBinaryOp(BasicBinaryOpType type, Variable a, Variable b, Variable output) : base(a, b, output)
        {
            Type = type;
        }

        protected override Assembly GenerateOperation(Register registerA, Register registerB)
        {
            return new Assembly($"{Instruction} {registerA}, {registerB}");
        }

        public static BasicBinaryOp Add(Variable a, Variable b, Variable output)
        {
            return new BasicBinaryOp(BasicBinaryOpType.Add, a, b, output);
        }

        public static BasicBinaryOp Subtract(Variable a, Variable b, Variable output)
        {
            return new BasicBinaryOp(BasicBinaryOpType.Subtract, a, b, output);
        }

        public static BasicBinaryOp ShiftLeft(Variable a, Variable b, Variable output)
        {
            return new BasicBinaryOp(BasicBinaryOpType.ShiftLeft, a, b, output);
        }

        public static BasicBinaryOp ShiftRight(Variable a, Variable b, Variable output)
        {
            return new BasicBinaryOp(BasicBinaryOpType.ShiftRight, a, b, output);
        }

        public static BasicBinaryOp And(Variable a, Variable b, Variable output)
        {
            return new BasicBinaryOp(BasicBinaryOpType.And, a, b, output);
        }

        public static BasicBinaryOp Or(Variable a, Variable b, Variable output)
        {
            return new BasicBinaryOp(BasicBinaryOpType.Or, a, b, output);
        }

        public static BasicBinaryOp Xor(Variable a, Variable b, Variable output)
        {
            return new BasicBinaryOp(BasicBinaryOpType.Or, a, b, output);
        }
    }

    public class Multiplication : IBinaryOp
    {
        public Variable A { get; }
        public Variable B { get; }
        public Variable Out { get; }

        public Multiplication(Variable a, Variable b, Variable @out)
        {
            A = a;
            B = b;
            Out = @out;
        }

        public void Generate(Assembly asm, Context ctx)
        {
            if (A.Sign || B.Sign)
            {
                GenerateSigned(asm, ctx);
            }
            else
            {
                GenerateUnsigned(asm, ctx);
            }
        }

        private void GenerateUnsigned(Assembly asm, Context ctx)
        {
            var opA = new Register("rax");
            ctx.LoadToRegister(A, opA, asm);
            ctx.Lock(opA);
            var opB = ctx.Load(B, asm);
            ctx.Unlock(opA);
            asm.Add($"mul {opB}");
            ctx.Store(opA, Out, asm);
        }

        private void GenerateSigned(Assembly asm, Context ctx)
        {
            var opA = ctx.Load(A, asm);
            var opB = ctx.Load(B, asm);
            asm.Add($"imul {opA}, {opB}");
            ctx.Store(opA, Out, asm);
        }
    }
}
