namespace Reverie.CodeGeneration
{
    public class Jmp : ICode
    {
        public Label Label { get; }

        public Jmp(Label label)
        {
            Label = label;
        }

        public void Generate(Assembly asm, Context ctx)
        {
            asm.Add($"jmp {Label}");
        }
    }
}
