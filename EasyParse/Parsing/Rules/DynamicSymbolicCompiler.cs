using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EasyParse.Parsing.Nodes.Errors;
using EasyParse.Parsing.Rules.Symbols;
using EasyParse.Text;

namespace EasyParse.Parsing.Rules
{
    class DynamicSymbolicCompiler : ISymbolCompiler
    {
        public DynamicSymbolicCompiler(IEnumerable<Production> productions)
        {
            this.Productions = productions.ToList();
            this.TerminalTransforms =
                RegexSymbols(this.Productions).ToDictionary(symbol => symbol.Name, symbol => symbol.Transform);
        }

        private IEnumerable<Production> Productions { get; }
        private Dictionary<string, Func<string, object>> TerminalTransforms { get; }

        private static IEnumerable<RegexSymbol> RegexSymbols(IEnumerable<Production> productions) =>
            productions
                .SelectMany(production => production.Body)
                .OfType<RegexSymbol>()
                .GroupBy(symbol => symbol.Name)
                .Select(group => group.First());

        public object CompileTerminal(string label, string value) => 
            this.TerminalTransforms.TryGetValue(label, out Func<string, object> transform) ? transform(value) 
            : (object)value;

        public object CompileNonTerminal(Location location, string label, object[] children) =>
            this.TransformsFor(label, this.TypesOf(children))
                .DefaultIfEmpty(() => this.CompileErrorTransform(location, label, children))
                .Select(transform => transform(children))
                .First();

        private IEnumerable<Func<object[], object>> TransformsFor(string label, Type[] children) =>
            this.Productions
                .Where(production => production.Head.Name == label)
                .Where(production => production.Transform.IsApplicableTo(children))
                .Select(production => production.Transform.Function);

        private Func<object[], object> CompileErrorTransform(Location location, string label, object[] children) =>
            children.OfType<CompileError>()
                .Select<CompileError, Func<object[], object>>(error => (object[] _) => (object)error)
                .DefaultIfEmpty(() => (object[] arguments) => (object)new CompileError(location, label, arguments))
                .First();
                
                

        private Type[] TypesOf(object[] values) =>
            values.Select(value => value?.GetType() ?? typeof(object)).ToArray();
    }
}
