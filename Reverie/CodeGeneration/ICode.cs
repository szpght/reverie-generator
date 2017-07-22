namespace Reverie.CodeGeneration
{
    public interface ICode
    {
        Assembly Generate(Context ctx);
        void Generate(Assembly asm, Context ctx);
    }
}