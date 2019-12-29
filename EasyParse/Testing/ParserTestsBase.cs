using System;
using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.Parsing;

namespace EasyParse.Testing
{
    public abstract class ParserTestsBase
    {
        protected abstract Assembly XmlDefinitionAssembly { get; }
        protected abstract string XmlDefinitionResourceName { get; }
        protected abstract Func<Lexer, Lexer> LexicalRules { get; }

        protected bool Recognized(string input) =>
            this.CreateParser().Parse(input).IsSuccess;

        protected bool Recognized(params string[] lines) =>
            this.CreateParser().Parse(lines).IsSuccess;

        protected object Compiled(ICompiler compiler, string input) =>
            this.CreateParser().Parse(input).Compile(compiler);

        protected object Compiled(ICompiler compiler, params string[] lines) =>
            this.CreateParser().Parse(lines).Compile(compiler);

        private Parser CreateParser() =>
            Parser.FromXmlResource(this.XmlDefinitionAssembly, this.XmlDefinitionResourceName, this.LexicalRules);
    }
}
