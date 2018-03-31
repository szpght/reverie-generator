namespace Reverie.CodeGeneration
{
    public interface ICode
    {
        void Generate(Assembly asm, Context ctx);
    }
}