using System;
using System.IO;
using EasyParse.ParserGenerator;

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
            new GrammarLoader()
                .From(grammar.FullName)
                .BuildParser()
                .ToXml()
                .Save(this.DestinationFile(grammar).FullName);
    }
}