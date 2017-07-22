namespace Reverie.CodeGeneration
{
    public class Label : ICode
    {
        public string Name { get; }
        public LabelType Type { get; }
        public string Declaration => Name + ":";

        private static long Count = -1;

        public Label(string name, LabelType type = LabelType.Global)
        {
            Type = type;
            Name = name;
            if (type == LabelType.Local)
            {
                Name = "." + Name;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public void Generate(Assembly asm, Context ctx)
        {
            asm.Add(Declaration);
        }

        public static Label New(LabelType type)
        {
            Count += 1;
            return new Label("L" + Count, type);
        }
    }

    public enum LabelType
    {
        Local,
        Global,
    }
}