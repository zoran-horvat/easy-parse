namespace EasyParse.Parsing.Rules
{
    public interface IPendingProductionEnd : IProductionBuilder
    {
        IRule End();
    }
}