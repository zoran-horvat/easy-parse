using System.IO;
using EasyParse.ParserGenerator.GrammarCompiler;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Parsing;

namespace EasyParse.ParserGenerator
{
    public class GrammarLoader
    {
        private Compiler<Grammar> GrammarCompiler { get; } =
            new GrammarGrammar().BuildCompiler<Grammar>();

        public Grammar LoadFrom(string filePath) =>
            this.TryLoadFrom(filePath).Result;

        public CompilationResult<Grammar> TryLoadFrom(string filePath) =>
            this.GrammarCompiler.Compile(File.ReadAllLines(filePath));
    }
}
