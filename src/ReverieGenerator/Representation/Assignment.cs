using System;
using Reverie.Generator.Interfaces;

namespace Reverie.Generator.Representation
{
    public class Assignment : ICode
    {
        public Variable Destination { get; }
        public Variable Source { get; }

        public Assignment(Variable destination, Variable source)
        {
            if (destination is Constant)
            {
                throw new Exception("cannot assign to constant");
            }

            Destination = destination;
            Source = source;
        }

        public override string ToString() =>
            $"{Destination} <- {Source}";
    }
}