using System;
using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.Parsing;

namespace EasyParse.Testing
{
    public abstract class ParserTestsBase
    {
        protected abstract Assembly XmlDefinitionAssembly { get; }
        protected abstract string XmlDefinitionResourceName { get; }
        protected abstract Func<Lexer, Lexer> LexicalRules { get; }

        protected bool Recognized(string input) =>
            this.CreateParser().Parse(input).IsSuccess;

        protected bool Recognized(params string[] lines) =>
            this.CreateParser().Parse(lines).IsSuccess;

        protected object Compiled(ICompiler compiler, string input) =>
            this.CreateParser().Parse(input).Compile(compiler);

        protected object Compiled(ICompiler compiler, params string[] lines) =>
            this.CreateParser().Parse(lines).Compile(compiler);

        protected T Compiled<T>(ICompiler compiler, params string[] lines) where T : class =>
            (T) this.Compiled(compiler, lines);

        protected T Compiled<T>(ICompiler compiler, Action<object> orElse, params string[] lines) where T : class
        {
            object result = this.Compiled(compiler, lines);
            if (result is T success) return success;
            orElse(result);
            throw new InvalidOperationException($"Could not compile {result?.GetType().Name ?? "<null>"} into {typeof(T).Name}.");
        }
            

        private Parser CreateParser() =>
            Parser.FromXmlResource(this.XmlDefinitionAssembly, this.XmlDefinitionResourceName, this.LexicalRules);
    }
}
