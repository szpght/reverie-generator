using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public interface ICallingConvention
    {
        Assembly LoadArguments(IList<Variable> arguments, Context ctx);
        Assembly UnloadArguments(IList<Variable> arguments, Context ctx);
        IList<RegisterInfo> GetRegisters();
    }
}
