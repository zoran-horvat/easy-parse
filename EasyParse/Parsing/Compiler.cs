using System;
using System.Collections.Generic;

namespace EasyParse.Parsing
{
    public class Compiler<T>
    {
        internal Compiler(Parser parser, ISymbolCompiler symbolCompiler)
        {
            this.Parser = parser;
            this.SymbolCompiler = symbolCompiler;
        }

        private Parser Parser { get; }
        private ISymbolCompiler SymbolCompiler { get; }

        public CompilationResult<T> Compile(string line) =>
            this.Compile(this.Parser.Parse(line));

        public CompilationResult<T> Compile(IEnumerable<string> lines) =>
            this.Compile(this.Parser.Parse(lines));

        private CompilationResult<T> Compile(ParsingResult parsingResult)
        {
            try
            {
                return parsingResult.IsSuccess ? this.OnParsed(parsingResult)
                    : this.OnParsingFailed(parsingResult);
            }
            catch (Exception ex)
            {
                return CompilationResult<T>.Error(ex.Message);
            }
        }

        private CompilationResult<T> OnParsed(ParsingResult parsingResult) =>
            CompilationResult<T>.Success((T)parsingResult.Compile(this.SymbolCompiler));

        private CompilationResult<T> OnParsingFailed(ParsingResult parsingResult) =>
            CompilationResult<T>.Error(parsingResult.ErrorMessage);
    }
}
