using System;
using System.IO;
using EasyParse.ParserGenerator;
using EasyParse.ParserGenerator.Models;
using EasyParse.Parsing;

namespace EasyParse.CommandLineTool.Commands
{
    class Construct : GrammarCommand
    {
        public Construct(FileInfo grammar) : base(grammar) { }

        protected override void Execute(FileInfo grammar) =>
            Console.WriteLine(this.CreateParser(grammar).definition);

        private (ParserDefinition definition, Parser parser) CreateParser(FileInfo grammar) =>
            this.CreateParser(this.CreateParserDefinition(grammar));

        private (ParserDefinition, Parser) CreateParser(ParserDefinition definition) =>
            (definition, Parser.From(definition));

        private ParserDefinition CreateParserDefinition(FileInfo grammar) =>
            new GrammarLoader()
                .LoadFrom(grammar.FullName)
                .BuildParser();
    }
}
