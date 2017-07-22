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
            throw new System.NotImplementedException();
        }

        public void Generate(Assembly asm, Context ctx)
        {
            asm.Add($"jmp {Label}");
        }
    }
}
