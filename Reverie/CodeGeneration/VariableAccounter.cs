using System.Collections.Generic;
using System.Linq;

namespace Reverie.CodeGeneration
{
    public class VariableAccounter
    {
        public ISet<Variable> Variables { get; }
        public VariableAccounter()
        {
            Variables = new HashSet<Variable>();
        }

        public void AccountVariable(Variable variable)
        {
            Variables.Add(variable);
        }

        public List<CString> GetStrings()
        {
            return Variables
                .OfType<CString>()
                .ToList();
        }

        public List<StackVariable> GetStackVariables()
        {
            return Variables
                .OfType<StackVariable>()
                .ToList();
        }
    }
}
