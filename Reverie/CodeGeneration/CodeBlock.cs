using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public class CodeBlock
    {
        public IList<ICode> Code { get; set; }
        public Variable Result { get; set; }

    }
}
