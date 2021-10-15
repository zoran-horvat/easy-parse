using System;
using System.Linq;

namespace EasyParse.Parsing.Rules
{
    public interface IProductionBuilder
    {
        IPendingMapping Match(Symbol first, params Symbol[] others);
        IRule Match<T>(Symbol first, params Symbol[] others) => 
            Match(first, others).ToIdentity<T>();
        IPendingMapping Literal(string value);
        IPendingMapping Regex(string name, string pattern) => Regex<string>(name, pattern, x => x);
        IPendingMapping Regex<T>(string name, string pattern, Func<string, T> transform);
        IPendingMapping Symbol(Func<IRule> factory);
    }
}