namespace Reverie.CodeGeneration
{
    public class Register
    {
        public string Name { get; }

        public Register(string register)
        {
            Name = register;
        }

        public string WithSize(VariableSize size)
        {
            return Name + size.RegisterSuffix();
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is Register register)
            {
                return register.Name == Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(Register a, Register b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Name == b.Name;
        }

        public static bool operator !=(Register a, Register b)
        {
            return !(a == b);
        }
    }
}