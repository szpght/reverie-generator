using System.Collections.Generic;
using Reverie.Generator.Interfaces;

namespace Reverie.Generator.Representation
{
    public class Function
    {
        public string Name { get; }
        public IList<ICode> Instructions { get; }

        public Function(string name) : this(name, new List<ICode>())
        {
        }

        public Function(string name, IList<ICode> instructions)
        {
            Name = name;
            Instructions = instructions;
        }

        public override string ToString() =>
            $"{Name}: function";
    }
}