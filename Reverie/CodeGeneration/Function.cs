using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public class Function : ICode
    {
        public string Name { get; set; }
        public CodeBlock Code { get; set; }
        public List<StackVariable> Arguments { get; set; }
        public Variable ReturnedValue { get; set; }

        private IList<CString> Strings;
        private IList<StackVariable> StackVariables;

        public Function(string name)
        {
            Name = name;
            Code = new CodeBlock();
            Arguments = new List<StackVariable>();
        }

        public void Generate(Assembly asm, Context ctx)
        {
            var storeVariablesAssembly = new Assembly();
            DetectVariables(ctx);
            var space = GenerateVariablesAndReturnStackSpace(storeVariablesAssembly, ctx);
            GeneratePrologue(asm, space);
            asm.Add(storeVariablesAssembly);
            Code.Generate(asm, ctx);
            GenerateEpilogue(asm, ctx);
            GenerateRodata(asm, ctx);
        }

        private void GeneratePrologue(Assembly asm, int spaceOnStack)
        {
            asm.Add("SECTION .text");
            var label = new Label(Name);
            asm.Add($"global {label}");
            label.Generate(asm, null);
            asm.Add("push rbp");
            asm.Add("mov rbp, rsp");
            asm.Add($"sub rsp, {spaceOnStack}");
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
            foreach (var s in Strings)
            {
                s.GenerateRepresentation(asm);
            }
        }

        private int GenerateVariablesAndReturnStackSpace(Assembly asm, Context ctx)
        {
            var cc = ctx.CallingConvention;
            var argRegisters = cc.GetArgumentRegisters();

            int variableCount = 0;

            // alokacja miejsca argumentom w rejestrach
            for (int i = 0; i < argRegisters.Count && i < Arguments.Count; ++i)
            {
                var register = argRegisters[i];
                var variable = Arguments[i];
                if (!StackVariables.Contains(variable))
                {
                    continue;
                }
                StackVariables.Remove(variable);
                variable.Base = new Label("rbp");
                variable.Offset = -8 * (variableCount + 1);
                variableCount++;
                ctx.Store(register, variable, asm);
            }

            // alokacja argumentow na stosie
            for (int i = argRegisters.Count; i < Arguments.Count; ++i)
            {
                var variable = Arguments[i];
                StackVariables.Remove(variable);
                int indexOnStack = i - argRegisters.Count;
                variable.Base = new Label("rbp");
                variable.Offset = 16 + indexOnStack * 8;
            }

            // alokacja pozostalych zmiennych
            foreach (var variable in StackVariables)
            {
                variable.Base = new Label("rbp");
                variable.Offset = -8 * (variableCount + 1);
                variableCount++;
            }

            return variableCount * 8;
        }

        private void DetectVariables(Context ctx)
        {
            var tempCtx = new Context(ctx.CallingConvention);
            var asm = new Assembly();
            Code.Generate(asm, tempCtx);
            var accounter = tempCtx.VariableAccounter;
            Strings = accounter.GetStrings();
            StackVariables = accounter.GetStackVariables();
        }
    }
}
