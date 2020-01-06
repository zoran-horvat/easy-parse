using System;
using System.IO;
using EasyParse.ParserGenerator;
using EasyParse.ParserGenerator.Models;
using EasyParse.Parsing;

namespace EasyParse.CommandLineTool.Commands
{
    class Compile : GrammarCommand
    {
        private FileInfo DestinationFile(FileInfo grammar) =>
            new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), this.DestinationFileName(grammar)));

        private string DestinationFileName(FileInfo grammar) =>
            Path.ChangeExtension(grammar.Name, ".xml");

        public Compile(FileInfo grammar) : base(grammar) { }

        protected override void Execute(FileInfo grammar)
        {
            this.CreateDestinationFile(grammar);
            Console.WriteLine($"Created parser definition in {this.DestinationFile(grammar).FullName}");
        }

        private void CreateDestinationFile(FileInfo grammar) =>
            this.CreateDestinationFile(grammar, new GrammarLoader().From(grammar.FullName).BuildParser());

        private void CreateDestinationFile(FileInfo grammar, ParserDefinition definition) =>
            this.CreateDestinationFile(grammar, definition, Parser.From(definition));

        private void CreateDestinationFile(FileInfo grammar, ParserDefinition definition, Parser parser) =>
            definition.ToXml().Save(this.DestinationFile(grammar).FullName);
    }
}