﻿using System.Linq;

namespace EasyParse.Fluent.Rules
{
    public interface IProductionBuilder
    {
        IPendingMapping Match(Symbol first, params Symbol[] others);
        
        IRule Match<T>(Symbol first, params Symbol[] others) => 
            Match(first, others).ToIdentity<T>();

        IRule MatchOne<T>(Symbol first, params Symbol[] others) =>
            others.Aggregate(this.Match<T>(first), (rule, symbol) => rule.Match<T>(symbol));
    }
}