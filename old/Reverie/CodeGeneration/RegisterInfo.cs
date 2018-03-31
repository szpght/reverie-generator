namespace Reverie.CodeGeneration
{
    public class RegisterInfo
    {
        public Register Register { get; }
        public bool Nonvolatile { get; }
        public bool Dirty { get; set; }
        public bool Locked { get; set; }
        public bool Empty => variable_ == null;

        public Variable Variable
        {
            get => variable_;
            set
            {
                variable_ = value;
                if (value != null)
                {
                    Dirty = true;
                }
            }
        }

        private Variable variable_;

        public RegisterInfo(string register, bool nonvolatile)
        {
            Register = new Register(register);
            Nonvolatile = nonvolatile;
        }

        public RegisterInfo(RegisterInfo info)
        {
            Register = info.Register;
            Nonvolatile = info.Nonvolatile;
            Dirty = info.Dirty;
            Locked = info.Locked;
        }

        public override string ToString()
        {
            string variable;
            if (Variable == null)
            {
                variable = "empty";
            }
            else
            {
                variable = Variable.ToString();
            }
            string vol = Nonvolatile ? "v" : "V";
            string locked = Locked ? "Q" : "q";
            string dirty = Dirty ? "D" : "d";
            variable = $"{variable} {vol} {dirty} {locked}";
            return $"{Register} -> {variable}";
        }
    }
}