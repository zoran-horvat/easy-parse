using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.Parsing;

namespace EasyParse.Testing
{
    public abstract class ParserTestsBase
    {
        protected abstract Assembly XmlDefinitionAssembly { get; }
        protected abstract string XmlDefinitionResourceName { get; }
        protected abstract Lexer Lexer { get; }

        protected bool Recognized(string input) =>
            this.CreateParser().Parse(input).IsSuccess;

        protected bool Recognized(params string[] lines) =>
            this.CreateParser().Parse(lines).IsSuccess;

        protected object Compiled(string input, ICompiler compiler) =>
            this.CreateParser().Parse(input).Compile(compiler);

        private Parser CreateParser() =>
            Parser.FromXmlResource(this.XmlDefinitionAssembly, this.XmlDefinitionResourceName, this.Lexer);
    }
}
