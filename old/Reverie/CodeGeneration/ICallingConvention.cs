﻿using System.Collections.Generic;

namespace Reverie.CodeGeneration
{
    public interface ICallingConvention
    {
        void LoadArguments(IList<Variable> arguments, Assembly asm, Context ctx);
        void UnloadArguments(IList<Variable> arguments, Assembly asm, Context ctx);
        void StoreResult(Variable result, Assembly asm, Context ctx);
        void SetArgumentVariables(IList<StackVariable> variables);
        IList<RegisterInfo> GetRegisters();
        IReadOnlyList<Register> GetArgumentRegisters();
    }
}
