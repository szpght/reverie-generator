using System;
using System.Linq;
using Xunit;
using Reverie.CodeGeneration;

namespace Reverie.Tests
{
    public class VariableTests
    {
        [Fact]
        public void LoadWordStackToEaxHasOneLine()
        {
            var asm = LoadWordStackToEax();

            Assert.Equal(1, asm.Count);
        }

        [Fact]
        public void LoadWordStackToEaxCorrect()
        {
            var asm = LoadWordStackToEax();

            Assert.Equal("mov r0d, WORD [rsp + 4]", asm.Single());
        }

        private Assembly LoadWordStackToEax()
        {
            var variable = new Variable("rsp", 4, VariableSize.Word);
            var register = new Register("rax", VariableSize.Word);

            return variable.Load(register);
        }
    }
}