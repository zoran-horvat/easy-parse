using System;

namespace EasyParse.Parsing.Rules
{
    public interface IProductionBuilder
    {
        IPendingProductionEnd Literal(string value);
        IPendingProductionEnd Regex(string name, string pattern);
        IPendingProductionEnd Symbol(Func<IRule> factory);
    }
}