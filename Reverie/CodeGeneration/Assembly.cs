using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public class Assembly : List<string>
    {
        public Assembly()
        {
        }

        public Assembly(string line)
        {
            Add(line);
        }

        public void Add(Assembly assembly)
        {
            AddRange(assembly);
        }

        public void Generate(ICode code, Context ctx)
        {
            AddRange(code.Generate(ctx));
        }

        public override string ToString()
        {
            return string.Join("\n", this);
        }
    }
}