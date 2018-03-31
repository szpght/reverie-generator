using Reverie.Generator.Enums;
using Reverie.Generator.Interfaces;
using Xunit;
using Reverie.Generator.Representation;

namespace ReverieGenerator.Tests
{
    public class SampleCode
    {
        [Fact]
        public void SampleCodeDoesNotThrow()
        {
            var a = new Constant(VariableSize.Word, 5);
            var b = new Variable(VariableSize.Word, true);
            var c = new Variable(VariableSize.Word, true);

            var instructions = new ICode[]
            {
                new Assignment(b, new Constant(VariableSize.Word, 7)),
                BinaryOperation.Add(a, b, c),
                new FunctionCall(
                    "printf",
                    null,
                    new CString("%d + %d = %d\n"),
                    a,
                    b,
                    c
                ),
                new Return(new Constant(VariableSize.Word, 123))
            };

            var function = new Function("add", instructions);
        }
    }
}
