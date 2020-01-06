using System;
using System.IO;
using EasyParse.ParserGenerator;
using EasyParse.ParserGenerator.Models;
using EasyParse.Parsing;

namespace EasyParse.CommandLineTool.Commands
{
    class Emulate : GrammarCommand
    {
        public Emulate(FileInfo grammar) : base(grammar) { }

        protected override void Execute(FileInfo grammar)
        {
            Console.WriteLine($"Emulating with {this.CreateParser(grammar)}");
        }

        private Parser CreateParser(FileInfo grammar) =>
            Parser.From(this.CreateParserDefinition(grammar));

        private ParserDefinition CreateParserDefinition(FileInfo grammar) =>
            new GrammarLoader()
                .From(grammar.FullName)
                .BuildParser();
    }
}
