using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using EasyParse.Fluent;
using EasyParse.Parsing;
using System;
using EasyParse.Fluent.Symbols;
using EasyParse.Fluent.Rules;

namespace EasyParse.Native
{
    public abstract class ReflectionGrammar
    {
        protected abstract IEnumerable<Regex> IgnorePatterns { get; }

        private IEnumerable<RegexSymbol> IgnoreSymbols =>
            this.IgnorePatterns.Select(pattern => new RegexSymbol(pattern.ToString(), pattern, typeof(string), x => x));

        public Parser BuildParser() => null;

        public IEnumerable<string> ToGrammarFileContent() =>
            new GrammarToGrammarFileFormatter().Convert(this.IgnoreSymbols, this.StartSymbol, Enumerable.Empty<Production>());

        private NonTerminalName StartSymbol =>
            this.SelectMethods<StartAttribute>()
                .DefaultIfEmpty<MethodInfo>(() => throw new InvalidOperationException("Start symbol not defined on grammar"))
                .Select(method => new NonTerminalName(method.Name))
                .First();

        private IEnumerable<MethodInfo> SelectMethods<TAttribute>() where TAttribute : Attribute => 
            this.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(method => method.CustomAttributes.Any(attribute => typeof(TAttribute).IsAssignableFrom(attribute.AttributeType)));
    }
}