using System;
using System.Collections.Generic;
using EasyParse.Parsing.Formatting;
using EasyParse.Parsing.Nodes.Errors;

namespace EasyParse.Parsing
{
    public class Compiler
    {
        internal Compiler(Parser parser, ISymbolCompiler symbolCompiler)
        {
            this.Parser = parser;
            this.SymbolCompiler = symbolCompiler;
        }

        public Parser Parser { get; }
        private ISymbolCompiler SymbolCompiler { get; }

        public CompilationResult<object> Compile(string line) =>
            this.Compile(this.Parser.Parse(line));

        public CompilationResult<object> Compile(IEnumerable<string> lines) =>
            this.Compile(this.Parser.Parse(lines));

        private CompilationResult<object> Compile(ParsingResult parsingResult)
        {
            try
            {
                return parsingResult.IsSuccess ? this.OnParsed(parsingResult)
                    : this.OnParsingFailed(parsingResult);
            }
            catch (Exception ex)
            {
                return CompilationResult<object>.Error(ex.Message);
            }
        }

        private CompilationResult<object> OnParsed(ParsingResult parsingResult)
        {
            try
            {
                return this.OnCompiled((object)parsingResult.Compile(this.SymbolCompiler));
            }
            catch (Exception ex)
            {
                return CompilationResult<object>.Error(ex.Message);
            }
        }

        private CompilationResult<object> OnCompiled(object result) =>
            result is CompileError error ? CompilationResult<object>.Error($"Error at {error.Location}: {error.Message}")
            : CompilationResult<object>.Success(result);

        private CompilationResult<object> OnParsingFailed(ParsingResult parsingResult) =>
            CompilationResult<object>.Error(parsingResult.ErrorMessage);
    }
}
