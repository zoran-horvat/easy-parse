namespace EasyParse.Parsing.Rules
{
    public interface IPendingMapping : IProductionBuilder
    {
        IRule End();
    }
}