namespace Reverie.CodeGeneration
{
    public class Register
    {
        public string FullName { get; }
        public string Name => ToString();
        public VariableSize Size { get; }

        public Register(string register, VariableSize size)
        {
            FullName = register;
            Size = size;
        }

        public Register WithSize(VariableSize size)
        {
            return new Register(FullName, size);
        }

        public override string ToString()
        {
            return FullName + Size.RegisterSuffix();
        }

        public bool TheSameAs(Register register)
        {
            return FullName == register.FullName;
        }
    }
}