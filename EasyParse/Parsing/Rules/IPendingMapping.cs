namespace EasyParse.Parsing.Rules
{
    public interface IPendingMapping<T> : IProductionBuilder<T>
    {
        IRule<T> End();
    }
}