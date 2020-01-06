using System;
using System.IO;
using EasyParse.ParserGenerator;
using EasyParse.ParserGenerator.Models;

namespace EasyParse.CommandLineTool.Commands
{
    class Construct : Command
    {
        private FileInfo Grammar { get; }

        public Construct(FileInfo grammar)
        {
            this.Grammar = grammar;
        }

        public override void Execute() =>
            Console.WriteLine(this.CreateParser());

        private ParserDefinition CreateParser() =>
            new GrammarLoader()
                .From(this.Grammar.FullName)
                .BuildParser();
    }
}
