using System.Reflection;
using System;
using System.Collections.Generic;
using EasyParse.Parsing.Nodes.Errors;

namespace EasyParse.Parsing
{
    public class Compiler<T>
    {
        internal Compiler(Parser parser, ISymbolCompiler symbolCompiler)
        {
            this.Parser = parser;
            this.SymbolCompiler = symbolCompiler;
        }

        public Parser Parser { get; }
        private ISymbolCompiler SymbolCompiler { get; }

        public CompilationResult<T> Compile(string line) =>
            this.Compile(this.Parser.Parse(line));

        public CompilationResult<T> Compile(IEnumerable<string> lines) =>
            this.Compile(this.Parser.Parse(lines));

        private CompilationResult<T> Compile(ParsingResult parsingResult)
        {
            try
            {
                return parsingResult.IsSuccess ? this.OnParsed(parsingResult) : this.OnParsingFailed(parsingResult);
            }
            catch (TargetInvocationException invocationException)
            {
                return CompilationResult<T>.Error(invocationException.InnerException.Message);
            }
            catch (Exception ex)
            {
                return CompilationResult<T>.Error(ex.Message);
            }
        }

        private CompilationResult<T> OnParsed(ParsingResult parsingResult)
        {
            try
            {
                return this.OnCompiled((object)parsingResult.Compile(this.SymbolCompiler));
            }
            catch (TargetInvocationException invocationException)
            {
                return CompilationResult<T>.Error(invocationException.InnerException.ToString());
            }
            catch (Exception ex)
            {
                return CompilationResult<T>.Error(ex.Message);
            }
        }

        private CompilationResult<T> OnCompiled(object result) =>
            result is CompileError error ? CompilationResult<T>.Error($"Error at {error.Location}: {error.Message}")
            : result is null ? CompilationResult<T>.Error("Compiler returned null")
            : result is T compiled ? CompilationResult<T>.Success(compiled)
            : CompilationResult<T>.Error(
                $"Compiler returned object of type {result.GetType().Name} when expecting {typeof(T).Name}");

        private CompilationResult<T> OnParsingFailed(ParsingResult parsingResult) =>
            CompilationResult<T>.Error(parsingResult.ErrorMessage);
    }
}
