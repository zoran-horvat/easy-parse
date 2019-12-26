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

        private Parser CreateParser() =>
            Parser.FromXmlResource(this.XmlDefinitionAssembly, this.XmlDefinitionResourceName, this.Lexer);
    }
}
