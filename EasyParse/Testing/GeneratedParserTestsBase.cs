using System;
using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.Parsing;

namespace EasyParse.Testing
{
    public abstract class GeneratedParserTestsBase : ParserTestsBase
    {
        protected abstract Assembly XmlDefinitionAssembly { get; }
        protected abstract string XmlDefinitionResourceName { get; }
        protected virtual Func<Lexer, Lexer> LexicalRules { get; } = lexer => lexer;

        protected override Parser CreateParser() =>
            Parser.FromXmlResource(this.XmlDefinitionAssembly, this.XmlDefinitionResourceName, this.LexicalRules);
    }
}
