using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public interface ICallingConvention
    {
        void LoadArguments(IList<Variable> arguments, Assembly asm, Context ctx);
        void UnloadArguments(IList<Variable> arguments, Assembly asm, Context ctx);
        IList<RegisterInfo> GetRegisters();
    }
}
