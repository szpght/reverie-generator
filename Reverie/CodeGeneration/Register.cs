namespace Reverie.CodeGeneration
{
    public class Register
    {
        public string FullName { get; }

        public Register(string register)
        {
            FullName = register;
        }

        public string WithSize(VariableSize size)
        {
            return FullName + size.RegisterSuffix();
        }

        public override string ToString()
        {
            return FullName;
        }

        public override bool Equals(object obj)
        {
            if (obj is Register register)
            {
                return register.FullName == FullName;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
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

            return a.FullName == b.FullName;
        }

        public static bool operator !=(Register a, Register b)
        {
            return !(a == b);
        }
    }
}