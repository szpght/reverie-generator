namespace Reverie.CodeGeneration
{
    public class Jmp : ICode
    {
        public Label Label { get; }

        public Jmp(Label label)
        {
            Label = label;
        }

        public Assembly Generate(Context ctx)
        {
            return new Assembly($"jmp {Label}");
        }
    }
}
