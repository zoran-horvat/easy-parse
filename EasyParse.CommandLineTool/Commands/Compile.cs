using System;
using System.IO;
using EasyParse.ParserGenerator;

namespace EasyParse.CommandLineTool.Commands
{
    class Compile : Command
    {
        private FileInfo Grammar { get; }

        private FileInfo DestinationFile =>
            new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), this.DestinationFileName));

        private string DestinationFileName =>
            Path.ChangeExtension(Grammar.Name, ".xml");

        public Compile(FileInfo grammar)
        {
            Grammar = grammar;
        }

        public static Command Create(FileInfo grammar) =>
            grammar.Exists ? (Command)new Compile(grammar) : new FileNotFound(grammar);

        public override void Execute()
        {
            this.CreateDestinationFile();
            Console.WriteLine($"Created parser definition in {this.DestinationFile.FullName}");
        }

        private void CreateDestinationFile() =>
            new GrammarLoader()
                .From(this.Grammar.FullName)
                .BuildParser()
                .ToXml()
                .Save(this.DestinationFile.FullName);
    }
}