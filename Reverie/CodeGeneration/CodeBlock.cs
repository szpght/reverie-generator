using System;
using System.Collections.Generic;
using Microsoft.Build.Tasks;

namespace Reverie.CodeGeneration
{
    public class CodeBlock : ICode
    {
        public IList<ICode> Code { get; set; }
        public Variable Result { get; set; }
        public Label BeginLabel { get; set; }
        public Label EndLabel { get; set; }

        public CodeBlock(IList<ICode> code, Variable result)
        {
            Code = code;
            Result = result;
            BeginLabel = Label.New(true);
            EndLabel = Label.New(true);
        }

        public Assembly Generate(Context ctx)
        {
            var asm = new Assembly();
            asm.Add(BeginLabel.Declaration);
            foreach (var code in Code)
            {
                asm.Add(code.Generate(ctx));
            }
            asm.Add(EndLabel.Declaration);
            return asm;
        }
    }
}
