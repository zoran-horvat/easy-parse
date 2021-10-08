using System;

namespace EasyParse.Parsing.Rules
{
    public interface IProductionBuilder<T>
    {
        IPendingMapping<T> Literal(string value);
        IPendingMapping<T> Regex(string name, string pattern);
        IPendingMapping<T> Symbol(Func<IRule<T>> factory);
    }
}