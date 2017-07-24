using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public class Function : ICode
    {
        public string Name { get; set; }
        public CodeBlock Code { get; set; }
        public IList<Variable> Variables { get; set; }
        public Variable ReturnedValue { get; set; }

        public Function(string name, CodeBlock code, IList<Variable> variables, Variable returnedValue)
        {
            Name = name;
            Code = code;
            Variables = variables;
            ReturnedValue = returnedValue;
        }

        public void Generate(Assembly asm, Context ctx)
        {
            GeneratePrologue(asm);
            Code.Generate(asm, ctx);
            GenerateEpilogue(asm, ctx);
        }

        private void GeneratePrologue(Assembly asm)
        {
            new Label(Name).Generate(asm, null);
            asm.Add("push rbp");
            asm.Add("mov rbp, rsp");
            asm.Add($"sub rsp, {GetStackSpaceSize()}");
        }

        private void GenerateEpilogue(Assembly asm, Context ctx)
        {
            ctx.LoadToRegister(ReturnedValue, new Register("rax"), asm);
            asm.Add("mov rsp, rbp");
            asm.Add("pop rbp");
            asm.Add("ret");
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
