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
            BeginLabel = Label.New(LabelType.Local);
            EndLabel = Label.New(LabelType.Local);
        }

        public void Generate(Assembly asm, Context ctx)
        {
            asm.Add(BeginLabel.Declaration);
            foreach (var code in Code)
            {
                code.Generate(asm, ctx);
            }
            asm.Add(EndLabel.Declaration);
        }

        public void Add(ICode item)
        {
            Code.Add(item);
        }
    }
}
