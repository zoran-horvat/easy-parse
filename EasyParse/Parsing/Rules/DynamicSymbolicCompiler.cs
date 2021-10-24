using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Parsing.Nodes.Errors;
using EasyParse.Parsing.Rules.Symbols;
using EasyParse.Text;

namespace EasyParse.Parsing.Rules
{
    class DynamicSymbolicCompiler : ISymbolCompiler
    {
        public DynamicSymbolicCompiler(IEnumerable<Production> productions)
        {
            List<Production> productionsList = productions.ToList();
            this.ProductionsList = WithValidTypes(productionsList);
            this.Productions = 
                WithValidTypes(productionsList).ToDictionary(production => production.Reference, production => production);
            this.TerminalTransforms =
                RegexSymbols(this.ProductionsList).ToDictionary(symbol => symbol.Name, symbol => symbol.Transform);
        }

        private IEnumerable<Production> ProductionsList { get; }
        private Dictionary<RuleReference, Production> Productions { get; }
        private Dictionary<string, Func<string, object>> TerminalTransforms { get; }

        private static List<Production> WithValidTypes(List<Production> productions)
        {
            Dictionary<NonTerminalName, Type> nonTerminalTypes = NonTerminalTypes(productions);

            productions.SelectMany(production => UnassignableTypes(production, nonTerminalTypes))
                .ToList()
                .ForEach(tuple => throw new ArgumentException(
                    $"Cannot pass {tuple.argument.Name} as {tuple.transformParameter.Name} in rule {tuple.production}"));

            return productions;
        }

        private static Dictionary<NonTerminalName, Type> NonTerminalTypes(IEnumerable<Production> productions) =>
            productions.Select(production => (symbol: production.Head, type: production.ReturnType))
                .Distinct()
                .ToDictionary(tuple => tuple.symbol, tuple => tuple.type);

        private static IEnumerable<(Production production, Type argument, Type transformParameter)> UnassignableTypes(
            Production production, Dictionary<NonTerminalName, Type> nonTerminalTypes) =>
            ComponentTypes(production, nonTerminalTypes)
                .Zip(production.Transform.ArgumentTypes, (value, transform) => (production, value, transform))
                .Where(tuple => !tuple.transform.IsAssignableFrom(tuple.value));

        private static IEnumerable<Type> ComponentTypes(
            Production production, Dictionary<NonTerminalName, Type> nonTerminalTypes) =>
            production.Body.Select(symbol => TypeOf(symbol, nonTerminalTypes));

        private static Type TypeOf(Symbol symbol, Dictionary<NonTerminalName, Type> nonTerminalTypes) =>
            symbol is NonTerminalSymbol nonTerminal ? nonTerminalTypes[nonTerminal.Head] 
            : symbol.Type;

        private static IEnumerable<RegexSymbol> RegexSymbols(IEnumerable<Production> productions) =>
            productions
                .SelectMany(production => production.Body)
                .OfType<RegexSymbol>()
                .GroupBy(symbol => symbol.Name)
                .Select(group => group.First());

        public object CompileTerminal(string label, string value) => 
            this.TerminalTransforms.TryGetValue(label, out Func<string, object> transform) ? transform(value) 
            : (object)value;

        public object CompileNonTerminal(Location location, string label, RuleReference production, object[] children) => 
            this.TryFindProduction(production)
                .Select(production => production.Transform.Function(children))
                .DefaultIfEmpty(() => this.CompileErrorTransform(location, label, children))
                .First();

        private IEnumerable<Production> TryFindProduction(RuleReference productionReference) =>
            this.Productions.TryGetValue(productionReference, out Production production) ? new[] { production }
            : Enumerable.Empty<Production>();

        private Func<object[], object> CompileErrorTransform(Location location, string label, object[] children) =>
            children.OfType<CompileError>()
                .Select<CompileError, Func<object[], object>>(error => (object[] _) => (object)error)
                .DefaultIfEmpty(() => (object[] arguments) => (object)new CompileError(location, label, arguments))
                .First();

        private Type[] TypesOf(object[] values) =>
            values.Select(value => value?.GetType() ?? typeof(object)).ToArray();
    }
}
