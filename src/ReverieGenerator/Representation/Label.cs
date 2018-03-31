using System.Threading;

namespace Reverie.Generator.Representation
{
    public class Label
    {
        public string Name { get; }

        private static long _number = 1;

        public static Label Local(string name = null)
        {
            if (name == null)
            {
                name = GetNextNumber().ToString();
            }

            return new Label($".L{name}");
        }

        public static Label Global(string name = null)
        {
            if (name == null)
            {
                name = GetNextNumber().ToString();
            }

            return new Label($"L{name}");
        }

        private Label(string name)
        {
            Name = name;
        }

        private static long GetNextNumber() =>
            Interlocked.Increment(ref _number);

        public override string ToString() => Name;
    }
}