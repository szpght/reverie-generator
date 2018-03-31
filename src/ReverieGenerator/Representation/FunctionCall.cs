using System.Collections.Generic;
using Reverie.Generator.Interfaces;

namespace Reverie.Generator.Representation
{
    public class FunctionCall : ICode
    {
        public string Name { get; }
        public IList<Variable> Arguments { get; }
        public Variable Result { get; }

        public FunctionCall(string name, Variable result, params Variable[] arguments)
        {
            Name = name;
            Result = result;
            Arguments = arguments;
        }

        public override string ToString() =>
            $"call {Name}, {Arguments.Count} args, result: {(Result == null ? "no" : Result.Label.Name)}";
    }
}