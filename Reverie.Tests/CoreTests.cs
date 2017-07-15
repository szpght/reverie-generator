using System;
using Xunit;
using Reverie.CodeGeneration;

namespace Reverie.Tests
{
    public class OperationsTests
    {
        [Fact]
        void TestCannotHaveOuptut()
        {
            var variable = new Variable("", 0, VariableSize.Byte);

            Assert.Throws<ArgumentException>(() => new Test(variable, variable, variable));
        }

        [Fact]
        void CmpCannotHaveOuptut()
        {
            var variable = new Variable("", 0, VariableSize.Byte);

            Assert.Throws<ArgumentException>(() => new Cmp(variable, variable, variable));
        }
    }
}