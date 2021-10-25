using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using EasyParse.Fluent;
using EasyParse.Parsing;
using System;
using EasyParse.Fluent.Symbols;
using EasyParse.Fluent.Rules;
using System.Collections.Immutable;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Native.Annotations;

namespace EasyParse.Native
{
    public abstract class ReflectionGrammar
    {
        protected abstract IEnumerable<Regex> IgnorePatterns { get; }

        private IEnumerable<RegexSymbol> IgnoreSymbols =>
            this.IgnorePatterns.Select(pattern => new RegexSymbol(pattern.ToString(), pattern, typeof(string), x => x));

        public Parser BuildParser() =>
            FluentGrammar.BuildParser(this.IgnoreSymbols, this.MapMethods().StartSymbol, this.GetProductions());

        public IEnumerable<string> ToGrammarFileContent() =>
            new GrammarToGrammarFileFormatter().Convert(this.IgnoreSymbols, this.MapMethods().StartSymbol, this.GetProductions());

        public Compiler<T> BuildCompiler<T>() =>
            this.BuildCompiler<T>(this.MapMethods().StartSymbolType);

        public Compiler<T> BuildCompiler<T>(Type startSymbolType) =>
            typeof(T).IsAssignableFrom(startSymbolType) ? this.BuildParser().ToCompiler<T>(this.Compiler)
            : Fail.CompilerGrammarTypesMismatch<Compiler<T>>(typeof(T), startSymbolType);

        private GrammarMethodsMap MapMethods() =>
            new GrammarMethodsMap(this.SelectAllMethods());

        private IEnumerable<MethodInfo> SelectAllMethods() =>
            this.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(method => method.DeclaringType == this.GetType())
                .Where(method => !method.IsSpecialName);

        private ISymbolCompiler Compiler => 
            new DynamicSymbolicCompiler(this.GetProductions());

        private IEnumerable<Production> GetProductions() =>
            this.GetProductions(this.MapMethods());

        private IEnumerable<Production> GetProductions(GrammarMethodsMap map) =>
            map.ProductionMethods
                .SelectMany(method => this.ToProductions(map, method))
                .Select((production, offset) => production.WithReference(RuleReference.CreateOrdinal(offset + 1)));

        private NonTerminalName ToNonTerminal(MethodInfo method) =>
            new NonTerminalName(method.Name);

        private IEnumerable<Production> ToProductions(GrammarMethodsMap map, MethodInfo method)
        {
            List<ImmutableList<Symbol>> bodies = new List<ImmutableList<Symbol>>() { ImmutableList<Symbol>.Empty };

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                bodies = this
                    .ToSymbols(map, parameter)
                    .SelectMany(symbol => bodies.Select(body => body.Add(symbol)))
                    .ToList();
            }

            return bodies.Select(body => this.ToProduction(method, body));
        }

        private IEnumerable<Symbol> ToSymbols(GrammarMethodsMap map, ParameterInfo parameter) =>
            this.ToSymbols(
                map, parameter,
                this.GetSymbolAttribute(map, parameter, parameter.GetCustomAttributes<SymbolAttribute>().ToArray()));

        private IEnumerable<Symbol> ToSymbols(
            GrammarMethodsMap map,
            ParameterInfo parameter, SymbolAttribute attribute) =>
            attribute is RegexAttribute regex ? new Symbol[] { RegexSymbol.Create<string>(regex.Name, regex.Expression, x => x) }
            : attribute is LiteralAttribute literal ? literal.Values.Select(value => (Symbol)new LiteralSymbol(value))
            : attribute is FromAttribute @from ? this.ToNonTerminalSymbols(map, @from.NonTerminals)
            : throw new InvalidOperationException($"Unsupported parameter attribute {attribute.GetType().Name}");

        private IEnumerable<Symbol> ToNonTerminalSymbols(
            GrammarMethodsMap map,
            IEnumerable<NonTerminalName> nonTerminals) =>
            this.ToRules(map, nonTerminals).Select(rule => new NonTerminalSymbol(rule));

        private IEnumerable<IRule> ToRules(
            GrammarMethodsMap map,
            IEnumerable<NonTerminalName> nonTerminals) =>
            nonTerminals.Select(nonTerminal => new RulePlaceholder(nonTerminal, map.TypeFor(nonTerminal)));

        private SymbolAttribute GetSymbolAttribute(GrammarMethodsMap map, ParameterInfo parameter, SymbolAttribute[] attributes) =>
            attributes.Length == 0 ? map.ToFromAttribute(parameter)
            : attributes.Length == 1 && attributes[0] is FromAttribute @from ? map.Valid(parameter, @from)
            : attributes.Length == 1 ? attributes[0]
            : throw new InvalidOperationException($"Parameter {parameter.Name} of method {parameter.Member.Name} has multiple symbol attributes defined");

        private Production ToProduction(MethodInfo method, ImmutableList<Symbol> body) =>
            new Production(ToNonTerminal(method), body, this.ToTransform(method));

        private Transform ToTransform(MethodInfo method) =>
            new Transform(method.ReturnType, this.ParameterTypes(method).ToArray(), this.ToTransformFunction(method));

        private IEnumerable<Type> ParameterTypes(MethodInfo method) =>
            method.GetParameters().Select(parameter => parameter.ParameterType);

        private Func<object[], object> ToTransformFunction(MethodInfo method) =>
            (object[] arguments) => method.Invoke(this, arguments);
        private MethodInfo AsNonTerminal(MethodInfo method) =>
            method.IsPublic ? method 
            : method.CustomAttributes.Any(attribute => typeof(NonTerminalAttribute).IsAssignableFrom(attribute.AttributeType)) ? method
            : throw new ArgumentException($"Method {method.Name} must be decorated with NonTerminalAttribute");
    }
}