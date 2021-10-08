using System;

namespace EasyParse.Parsing.Rules
{
    public interface IProductionBuilder
    {
        IPendingMapping Literal(string value);
        IPendingMapping Regex(string name, string pattern);
        IPendingMapping Symbol(Func<IRule> factory);
    }
}