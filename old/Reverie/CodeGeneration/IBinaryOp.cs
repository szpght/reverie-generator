namespace Reverie.CodeGeneration
{
    public interface IBinaryOp : ICode
    {
        Variable A { get; }
        Variable B { get; }
        Variable Out { get; }
    }
}