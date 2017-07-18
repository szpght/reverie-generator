namespace Reverie.CodeGeneration
{
    public class Label
    {
        public string Name { get; }
        public bool Local { get; }
        public string Declaration => Name + ":";

        private static ulong Count = 1;

        public Label(string name) : this(name, false) {}

        public Label(string name, bool local)
        {
            Local = local;
            Name = name;
            if (local)
            {
                Name = "." + Name;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public static Label New(bool local)
        {
            var label = new Label("L" + Count);
            Count += 1;
            return label;
        }
    }
}