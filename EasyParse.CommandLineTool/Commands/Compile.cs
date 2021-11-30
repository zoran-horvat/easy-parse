using System;
using System.IO;
using EasyParse.ParserGenerator;
using EasyParse.ParserGenerator.Models;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Parsing;

namespace EasyParse.CommandLineTool.Commands
{
    class Compile : GrammarCommand
    {
        private static FileInfo DestinationFile(FileInfo grammar) =>
            new(Path.Combine(Directory.GetCurrentDirectory(), DestinationFileName(grammar)));

        private static string DestinationFileName(FileInfo grammar) =>
            Path.ChangeExtension(grammar.Name, ".xml");

        public Compile(FileInfo grammar) : base(grammar) { }

        protected override void Execute(FileInfo grammar)
        {
            CreateDestinationFile(grammar);
            Console.WriteLine($"Created parser definition in {DestinationFile(grammar).FullName}");
        }

        private static void CreateDestinationFile(FileInfo grammarSource)
        {
            CompilationResult<Grammar> grammar = new GrammarLoader().TryLoadFrom(grammarSource.FullName);
            if (grammar.IsSuccess)
                CreateDestinationFile(grammarSource, grammar.Result.BuildParser());
            else
                throw new Exception($"Error loading grammar: {grammar.ErrorMessage}");
        }

        private static void CreateDestinationFile(FileInfo grammar, ParserDefinition definition) =>
            definition.ToXml().Save(DestinationFile(grammar).FullName);
    }
}