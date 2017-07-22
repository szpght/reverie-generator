using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public class CodeBlock : ICode
    {
        public IList<ICode> Code { get; }
        public Variable Result { get; }
        public Label BeginLabel { get; }
        public Label EndLabel { get; }

        public CodeBlock() : this(new List<ICode>(), null)
        {
        }

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
                asm.Generate(code, ctx);
            }
            asm.Add(EndLabel.Declaration);
            return asm;
        }

        public void Add(ICode item)
        {
            Code.Add(item);
        }
    }
}
