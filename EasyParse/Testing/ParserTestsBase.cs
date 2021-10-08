using System;
using EasyParse.Parsing;

namespace EasyParse.Testing
{
    public abstract class ParserTestsBase
    {
        protected abstract Parser CreateParser();

        protected ParsingResult Parsed(string input) =>
            this.CreateParser().Parse(input);

        protected ParsingResult Parsed(params string[] input) =>
            this.CreateParser().Parse(input);

        protected bool Recognized(string input) =>
            this.Parsed(input).IsSuccess;

        protected bool Recognized(params string[] lines) =>
            this.Parsed(lines).IsSuccess;

        protected object Compiled(ISymbolCompiler compiler, string input) =>
            this.Parsed(input).Compile(compiler);

        protected object Compiled(ISymbolCompiler compiler, params string[] lines) =>
            this.Parsed(lines).Compile(compiler);

        protected object CompiledLine(ISymbolCompiler compiler, string input) =>
            this.Parsed(input).Compile(compiler);

        protected T Compiled<T>(ISymbolCompiler compiler, params string[] lines) where T : class =>
            (T) this.Compiled(compiler, lines);

        protected T Compiled<T>(ISymbolCompiler compiler, Action<object> orElse, params string[] lines) where T : class => 
            this.Compiled<T>(this.Compiled(compiler, lines), orElse);

        protected T CompiledLine<T>(ISymbolCompiler compiler, Action<object> orElse, string input) where T : class =>
            this.Compiled<T>(this.Compiled(compiler, input), orElse);

        private T Compiled<T>(object result, Action<object> orElse) where T : class
        {
            if (result is T success) return success;
            orElse(result);
            throw new InvalidOperationException($"Could not compile {result?.GetType().Name ?? "<null>"} into {typeof(T).Name}.");
        }
    }
}