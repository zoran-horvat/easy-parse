using System;
using System.Linq;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing.Rules
{
    public interface IProductionBuilder
    {
        IPendingMapping Match(params Symbol[] symbols);
        IPendingMapping Literal(string value);
        IPendingMapping Regex(string name, string pattern) => Regex<string>(name, pattern, x => x);
        IPendingMapping Regex<T>(string name, string pattern, Func<string, T> transform);
        IPendingMapping Symbol(Func<IRule> factory);
    }
}