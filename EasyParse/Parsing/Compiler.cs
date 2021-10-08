using System;
using System.Collections.Generic;

namespace EasyParse.Parsing
{
    public class Compiler
    {
        internal Compiler(Parser parser, ISymbolCompiler symbolCompiler)
        {
            this.Parser = parser;
            this.SymbolCompiler = symbolCompiler;
        }

        private Parser Parser { get; }
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

        private CompilationResult<object> OnParsed(ParsingResult parsingResult) =>
            CompilationResult<object>.Success((object)parsingResult.Compile(this.SymbolCompiler));

        private CompilationResult<object> OnParsingFailed(ParsingResult parsingResult) =>
            CompilationResult<object>.Error(parsingResult.ErrorMessage);
    }
}
