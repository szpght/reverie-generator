using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public class Function : ICode
    {
        public string Name { get; set; }
        public CodeBlock Code { get; set; }
        public List<Variable> Variables { get; set; }
        public List<Variable> Arguments { get; set; }
        public Variable ReturnedValue { get; set; }

        public Function(string name)
        {
            Name = name;
            Code = new CodeBlock();
            Variables = new List<Variable>();
            Arguments = new List<Variable>();
        }

        public void Generate(Assembly asm, Context ctx)
        {
            GeneratePrologue(asm);
            Code.Generate(asm, ctx);
            GenerateEpilogue(asm, ctx);
            GenerateRodata(asm, ctx);
        }

        private void GeneratePrologue(Assembly asm)
        {
            asm.Add("SECTION .text");
            var label = new Label(Name);
            asm.Add($"global {label}");
            label.Generate(asm, null);
            asm.Add("push rbp");
            asm.Add("mov rbp, rsp");
            asm.Add($"sub rsp, {GetStackSpaceSize()}");
        }

        private void GenerateEpilogue(Assembly asm, Context ctx)
        {
            if (ReturnedValue != null)
            {
                ctx.LoadToRegister(ReturnedValue, new Register("rax"), asm);
            }
            asm.Add("mov rsp, rbp");
            asm.Add("pop rbp");
            asm.Add("ret");
        }

        private void GenerateRodata(Assembly asm, Context ctx)
        {
            asm.Add("SECTION .rodata");
            foreach (var s in ctx.Strings)
            {
                s.GenerateRepresentation(asm);
            }
        }

        private int GetStackSpaceSize()
        {
            var count = Variables.Count;

            // ensure stack alignement on 16 byte boundary
            if (count % 2 == 0)
            {
                count += 1;
            }
            return count * 8;
        }
    }
}
