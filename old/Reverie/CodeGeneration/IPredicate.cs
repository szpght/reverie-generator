namespace Reverie.CodeGeneration
{
    public interface IPredicate : ICode
    {
        bool Negated { get; set; }
        bool JumpToElse { get; set; }
        string Jump { get; }
    }
}