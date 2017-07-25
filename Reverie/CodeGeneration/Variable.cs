namespace Reverie.CodeGeneration
{
    public abstract class Variable
    {
        public bool Sign { get; set; }
        public VariableSize Size { get; set; }
        public abstract void Load(Register register, Assembly assembly);
        public abstract void Store(Register register, Assembly assembly);
        public abstract override string ToString();
    }
}
