using System.Collections.Generic;
using Reverie.Generator.Interfaces;

namespace Reverie.Generator.Representation
{
    public class CodeBlock : ICode
    {
        public Label Start { get; }
        public Label End { get; }
        public List<ICode> Code { get; }

        public CodeBlock(Label start, Label end)
        {
            Start = start;
            End = end;
            Code = new List<ICode>();
        }
    }
}