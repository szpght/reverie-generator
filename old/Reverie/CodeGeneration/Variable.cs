namespace Reverie.CodeGeneration
{
    public abstract class Variable
    {
        public abstract bool Sign { get; }
        public abstract VariableSize Size { get; }
        public abstract void Load(Register register, Assembly assembly);
        public abstract void Store(Register register, Assembly assembly);
        public abstract override string ToString();
    }
}
