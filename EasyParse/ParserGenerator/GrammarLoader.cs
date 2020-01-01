using System.IO;
using EasyParse.ParserGenerator.GrammarCompiler;
using EasyParse.ParserGenerator.Models.Rules;

namespace EasyParse.ParserGenerator
{
    public class GrammarLoader
    {
        public Grammar From(string filePath) =>
            new GrammarParser().Parse(File.ReadAllLines(filePath));
    }
}
