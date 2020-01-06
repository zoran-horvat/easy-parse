using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasyParse.ParserGenerator;
using EasyParse.ParserGenerator.Models;
using EasyParse.Parsing;

namespace EasyParse.CommandLineTool.Commands
{
    class Emulate : GrammarCommand
    {
        public Emulate(FileInfo grammar) : base(grammar) { }

        protected override void Execute(FileInfo grammar) => 
            this.Execute(this.CreateParser(grammar));

        private void Execute(Parser parser)
        {
            Console.WriteLine("Enter lines of text to parse");
            Console.WriteLine("End with Ctrl+Z (end of file)");
            Console.WriteLine();

            this.Process(parser);
        }

        private void Process(Parser parser)
        {
            foreach (var result in this.Input().Select(parser.Parse))
                Console.WriteLine($"{result.ToDenseString()}{Environment.NewLine}");
        }
        
        private Parser CreateParser(FileInfo grammar) =>
            Parser.From(this.CreateParserDefinition(grammar));

        private ParserDefinition CreateParserDefinition(FileInfo grammar) =>
            new GrammarLoader()
                .From(grammar.FullName)
                .BuildParser();

        private IEnumerable<string> Input()
        {
            while (Console.ReadLine() is string input)
            {
                yield return input;
            }
        }
    }
}
