using System;
using System.Collections.Generic;
using EasyParse.Fluent.Rules;

namespace EasyParse.Fluent
{
    public interface IRule : IProductionBuilder
    {
        NonTerminalName Head { get; }
        Type Type { get; }
        IEnumerable<Production> Productions { get; }
        IEnumerable<Production> Expand();
    }
}