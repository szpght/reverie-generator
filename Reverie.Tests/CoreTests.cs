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

            Assert.Single(asm);
        }

        [Fact]
        public void LoadWordStackToEaxCorrect()
        {
            var asm = LoadWordStackToEax();

            Assert.Equal("mov r0w, WORD [rsp + 4]", asm.Single());
        }

        [Fact]
        public void StoreDwordEbxToStackHasTwoLines()
        {
            var asm = StoreDwordEbxToStack();

            Assert.Equal(2, asm.Count);
        }

        [Fact]
        public void StoreDwordEbxToStackZeroesRegister()
        {
            var asm = StoreDwordEbxToStack();

            Assert.Equal("xor r3, r3", asm[0]);
        }

        [Fact]
        public void StoreDwordEbxToStackCorrect()
        {
            var asm = StoreDwordEbxToStack();

            Assert.Equal("mov DWORD [rsp + 4], r3d", asm[1]);
        }

        private Assembly LoadWordStackToEax()
        {
            var variable = new Variable("rsp", 4, VariableSize.Word);
            var register = new Register("rax", VariableSize.Word);

            return variable.Load(register);
        }

        private Assembly StoreDwordEbxToStack()
        {
            var variable = new Variable("rsp", 4, VariableSize.Dword);
            var register = new Register("rbx", VariableSize.Dword);

            return variable.Store(register);
        }
    }
}